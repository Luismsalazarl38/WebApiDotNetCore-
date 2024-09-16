using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.DTOs;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserWishlistRepository _userWishlistRepository;

    public ProductsController(
        ILogger<ProductsController> logger,
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IUserWishlistRepository userWishlistRepository)
    {
        _logger = logger;
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _userWishlistRepository = userWishlistRepository;
    }

    // Get all categories
    [HttpGet("categories")]
    public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
    {
        var categories = await _categoryRepository.GetAllCategoriesAsync();
        if (!categories.Any())
        {
            _logger.LogWarning("No hay categorías disponibles.");
            return NotFound("No hay categorías disponibles.");
        }
        _logger.LogInformation("Se han recuperado {Count} categorías.", categories.Count());
        return Ok(categories);
    }

    // Create a new category
    [HttpPost("categories")]
    public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
    {
        if (createCategoryDto == null)
        {
            _logger.LogWarning("Se recibió una categoría nula.");
            return BadRequest("Categoría no válida.");
        }

        var newCategory = new Category
        {
            Name = createCategoryDto.Name
        };

        var addedCategory = await _categoryRepository.AddCategoryAsync(newCategory);
        _logger.LogInformation("Categoría '{CategoryName}' añadida con ID {CategoryId}.", addedCategory.Name, addedCategory.Id);

        var categoryDto = new CategoryDto
        {
            Id = addedCategory.Id,
            Name = addedCategory.Name
        };

        return CreatedAtAction(nameof(GetCategoryDetail), new { id = addedCategory.Id }, categoryDto);
    }

    // Get a category by ID
    [HttpGet("categories/{id}")]
    public async Task<ActionResult<CategoryDto>> GetCategoryDetail(int id)
    {
        var category = await _categoryRepository.GetCategoryByIdAsync(id);
        if (category == null)
        {
            _logger.LogWarning("Categoría con ID {CategoryId} no encontrada.", id);
            return NotFound("Categoría no encontrada.");
        }

        var categoryDto = new CategoryDto
        {
            Id = category.Id,
            Name = category.Name
        };

        _logger.LogInformation("Se ha recuperado el detalle de la categoría con ID {CategoryId}.", id);
        return Ok(categoryDto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
    {
        var products = await _productRepository.GetAllProductsAsync();
        if (!products.Any())
        {
            _logger.LogWarning("No hay productos disponibles.");
            return NotFound("No hay productos disponibles.");
        }

        var productDtos = products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            CategoryId = p.CategoryId // Usa CategoryId
        });

        _logger.LogInformation("Se han recuperado {Count} productos.", products.Count());
        return Ok(productDtos);
    }


    [HttpPost]
    public async Task<ActionResult<ProductDto>> AddProduct([FromBody] CreateProductDto createProductDto)
    {
        if (createProductDto == null)
        {
            _logger.LogWarning("Se recibió un producto nulo.");
            return BadRequest("Producto no válido.");
        }

        var category = await _categoryRepository.GetCategoryByIdAsync(createProductDto.CategoryId);
        if (category == null)
        {
            _logger.LogWarning("La categoría con ID {CategoryId} no existe.", createProductDto.CategoryId);
            return NotFound($"La categoría con ID {createProductDto.CategoryId} no existe.");
        }

        var newProduct = new Product
        {
            Name = createProductDto.Name,
            Description = createProductDto.Description,
            Price = createProductDto.Price,
            CategoryId = createProductDto.CategoryId
        };

        var addedProduct = await _productRepository.AddProductAsync(newProduct);
        _logger.LogInformation("Producto '{ProductName}' añadido con ID {ProductId} a la categoría '{CategoryName}'.", addedProduct.Name, addedProduct.Id, category.Name);

        var productDto = new ProductDto
        {
            Id = addedProduct.Id,
            Name = addedProduct.Name,
            Description = addedProduct.Description,
            Price = addedProduct.Price,
            CategoryId = addedProduct.CategoryId // Usa CategoryId
        };

        return CreatedAtAction(nameof(GetProductDetail), new { id = addedProduct.Id }, productDto);
    }
    [HttpGet("category/{categoryName}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategory(string categoryName)
    {
        var category = await _categoryRepository.GetCategoryByNameAsync(categoryName);
        if (category == null)
        {
            _logger.LogWarning("Categoría '{CategoryName}' no encontrada.", categoryName);
            return NotFound("Categoría no encontrada.");
        }

        var productsInCategory = await _productRepository.GetProductsByCategoryIdAsync(category.Id);
        if (!productsInCategory.Any())
        {
            _logger.LogWarning("No se encontraron productos en la categoría '{CategoryName}'.", categoryName);
            return NotFound($"No se encontraron productos en la categoría '{categoryName}'.");
        }

        var productDtos = productsInCategory.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            CategoryId = p.CategoryId // Usa CategoryId
        });

        _logger.LogInformation("Se han recuperado {Count} productos en la categoría '{CategoryName}'.", productsInCategory.Count(), categoryName);
        return Ok(productDtos);
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProductDetail(int id)
    {
        var product = await _productRepository.GetProductByIdAsync(id);
        if (product == null)
        {
            _logger.LogWarning("Producto con ID {ProductId} no encontrado.", id);
            return NotFound("Producto no encontrado.");
        }

        var productDto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            CategoryId = product.CategoryId // Usa CategoryId
        };

        _logger.LogInformation("Se ha recuperado el detalle del producto con ID {ProductId}.", id);
        return Ok(productDto);
    }

    [HttpPost("wishlist/{userId}/{productId}")]
    public async Task<ActionResult> AddToWishlist(int userId, int productId)
    {
        var product = await _productRepository.GetProductByIdAsync(productId);
        if (product == null)
        {
            _logger.LogWarning("Producto con ID {ProductId} no encontrado.", productId);
            return NotFound("Producto no encontrado.");
        }

        var wishlist = await _userWishlistRepository.GetUserWishlistAsync(userId);
        if (wishlist.Products.Any(p => p.Id == productId))
        {
            _logger.LogWarning("El producto con ID {ProductId} ya está en la lista de deseos.", productId);
            return BadRequest("El producto ya está en la lista de deseos.");
        }

        await _userWishlistRepository.AddProductToWishlistAsync(userId, product);
        _logger.LogInformation("Producto '{ProductName}' añadido a la lista de deseos del usuario con ID {UserId}.", product.Name, userId);
        return Ok($"Producto '{product.Name}' añadido a la lista de deseos.");
    }

    [HttpDelete("wishlist/{userId}/{productId}")]
    public async Task<ActionResult> RemoveFromWishlist(int userId, int productId)
    {
        var product = await _productRepository.GetProductByIdAsync(productId);
        if (product == null)
        {
            _logger.LogWarning("Producto con ID {ProductId} no encontrado.", productId);
            return NotFound("Producto no encontrado.");
        }

        await _userWishlistRepository.RemoveProductFromWishlistAsync(userId, productId);
        _logger.LogInformation("Producto '{ProductName}' eliminado de la lista de deseos del usuario con ID {UserId}.", product.Name, userId);
        return Ok($"Producto '{product.Name}' eliminado de la lista de deseos.");
    }

    [HttpGet("wishlist/{userId}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetUserWishlist(int userId)
    {
        var wishlist = await _userWishlistRepository.GetUserWishlistAsync(userId);
        if (wishlist == null || !wishlist.Products.Any())
        {
            _logger.LogWarning("No se encontraron productos en la lista de deseos del usuario con ID {UserId}.", userId);
            return NotFound("No se encontraron productos en la lista de deseos.");
        }

        var productDtos = wishlist.Products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            CategoryId = p.CategoryId // Usa CategoryId
        });

        _logger.LogInformation("Se han recuperado {Count} productos de la lista de deseos del usuario con ID {UserId}.", wishlist.Products.Count(), userId);
        return Ok(productDtos);
    }

}
