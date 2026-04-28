using API.Helpers.LinkGeneratorHelper;
using AutoMapper;
using Core.DTOs.Requests;
using Core.DTOs.Responses;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/products")]
    [Produces("application/json", "application/xml")]
    public class ProductController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Product> _genericRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly ILinkGeneratorHelper _linkGeneratorHelper;

        public ProductController(IUnitOfWork unitOfWork, IGenericRepository<Product> genericRepository,
            IProductRepository productRepository,
            IMapper mapper, ILinkGeneratorHelper linkGeneratorHelper)
        {
            _unitOfWork = unitOfWork;
            _genericRepository = genericRepository;
            _productRepository = productRepository;
            _mapper = mapper;
            _linkGeneratorHelper = linkGeneratorHelper;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyCollection<ProductResponse>>> GetProducts()
        {
            var products = await _unitOfWork.ProductsGeneric.GetAllAsync();

            if (!products.Any())
                return NotFound("There's no products right now... Try again later!");

            var productResponse = _mapper.Map<List<ProductResponse>>(products);

            return Ok(productResponse);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<IReadOnlyCollection<ProductResponse>>> GetAllPaged([FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            // Ensure page is always >= 1
            // Clamp pageSize between 1 and 100
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var products = await _genericRepository.GetPagedAsync(page, pageSize);

            if (!products.Any())
                return NotFound("There's no products right now... Try again later!");

            var productResponse = _mapper.Map<List<ProductResponse>>(products);

            return Ok(productResponse);
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public async Task<ActionResult<ProductResponse>> GetProduct(int id)
        {
            var products = await _unitOfWork.ProductsGeneric.GetByIdAsync(id);

            if (products is null)
                return NotFound($"product with id {id} was not found!");

            var productResponse = _mapper.Map<ProductResponse>(products);

            var response = new
            {
                productResponse,
                links = _linkGeneratorHelper.GenerateConfirmationUrl(id)
            };

            return Ok(response);
        }

        [HttpPost(Name = "Create")]
        public async Task<ActionResult<CreateProductRequest>> AddProduct([FromBody] CreateProductRequest createProductRequest)
        {
            if (createProductRequest == null)
                return BadRequest("Please enter a valid Product!");

            var exists = await _productRepository
                .IsExistByName(createProductRequest.ProductName.ToLower());

            if (exists is true)
                return BadRequest($"The product with name '{createProductRequest.ProductName}' is already exists... " +
                    $"try add another product with different name!");

            var product = _mapper.Map<Product>(createProductRequest);

            await _unitOfWork.ProductsGeneric.AddAsync(product);
            await _unitOfWork.CompleteAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id, version = "1.0" }, new
            {
                Product = createProductRequest,
                Operation = $"Product with id: {product.Id} was created successfully!"
            });
            //return Ok($"Product with id: {product.Id} was created successfully!");
        }

        [HttpDelete("{id:int}", Name = "Delete")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var currentProduct = await _unitOfWork.ProductsGeneric.GetByIdAsync(id);

            if (currentProduct is null)
                return NotFound($"Product with id: {id} is already not fount in the context!");

            _unitOfWork.ProductsGeneric.Delete(currentProduct);
            await _unitOfWork.CompleteAsync();

            return Ok($"Product with id: {id} deleted successfully!");
        }

        [HttpPut("{id:int}", Name = "Update")]
        public async Task<ActionResult> UpdateProduct(int id, [FromBody] UpdateProductRequest updateProductRequest)
        {
            var currentProduct = await _unitOfWork.ProductsGeneric.GetByIdAsync(id);

            if (currentProduct is null)
                return NotFound($"Product with id: {id} was not fount in the context!");

            var product = _mapper.Map(updateProductRequest, currentProduct);

            _unitOfWork.ProductsGeneric.Update(currentProduct);
            await _unitOfWork.CompleteAsync();

            return Ok($"Product with id: {id} updated successfully!");
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> UpdateProductPrice(int id, [FromBody] UpdateProductPriceRequest request)
        {
            var product = await _unitOfWork.ProductsGeneric.GetByIdAsync(id);

            if (product is null)
                return NotFound($"Product with id: {id} was not found!");

            //product.ProductPrice = request.ProductPrice;
            _mapper.Map(request, product);

            _unitOfWork.ProductsGeneric.Update(product);
            await _unitOfWork.CompleteAsync();

            return Ok(new
            {
                Message = $"Product price updated successfully!",
                ProductId = id,
                NewPrice = product.ProductPrice
            });
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyCollection<string>>> GetProductBrands()
        {
            var brands = await _productRepository.GetBrandsAsync();

            if (!brands.Any())
                return NotFound("There's no brands right now... Try again later!");

            return Ok(brands);
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyCollection<string>>> GetProductTypes()
        {
            var types = await _productRepository.GetTypesAsync();

            if (!types.Any())
                return NotFound("There's no types right now... Try again later!");

            return Ok(types);
        }

        [HttpGet("brands/{brandName}")]
        public async Task<ActionResult<IReadOnlyCollection<ProductResponse>>> GetProductsByBrand(string brandName)
        {
            var products = await _productRepository.GetProductsByBrandAsync(brandName);

            if (!products.Any())
                return NotFound("There's no Products in this brand right now... Try again later!");

            return Ok(products);
        }

        [HttpGet("types/{typeName}")]
        public async Task<ActionResult<IReadOnlyCollection<ProductResponse>>> GetProductsByType(string typeName)
        {
            var products = await _productRepository.GetProductsByTypeAsync(typeName);

            if (!products.Any())
                return NotFound("There's no Products in this type right now... Try again later!");

            return Ok(products);
        }

        [HttpGet("specific-brand-and-type")]
        public async Task<ActionResult<IReadOnlyCollection<ProductResponse>>> GetProductsByASpecificBrandAndType(string brand, string type)
        {
            var products = await _productRepository.GetProductsByASpecificBrandAndTypeAsync(brand, type);

            if (!products.Any())
                return NotFound("There's no Products with this brand and type right now... Try again later!");

            return Ok(products);
        }


        [HttpGet("sorted-by-price")]
        public async Task<ActionResult<IReadOnlyCollection<ProductResponse>>> GetProductsSortedByPrice
            ([FromQuery,
                SwaggerParameter("Sorting option: 'priceAsc' for ascending, 'priceDesc' for descending... " +
                    "No parameter? will sort by name ascending!", Required = false)] string? sort)
        {
            var products = await _productRepository.GetProductsSortedByPriceAsync(sort!);

            if (!products.Any())
                return NotFound("There's no Products right now... Try again later!");

            return Ok(products);
        }

        [HttpGet("csv")]
        public async Task<IActionResult> ExportProductsToCsv()
        {
            // retrieve all products from the database
            var products = await _genericRepository.GetAllAsync();

            // build the CSV content (name of columns)
            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("Product Id,Product Name,Product Description,Product Price,Product Brand," +
                "Product Type,Product Quantity In Stock, Product Picture Url");

            // append each product as a new line in the CSV
            foreach (var product in products)
                csvBuilder.AppendLine($"{product.Id},{product.ProductName},{product.ProductDescription}," +
                    $"{product.ProductPrice},{product.ProductBrand},{product.ProductType}," +
                    $"{product.ProductQuantityInStock},{product.ProductPictureUrl}");

            // convert the CSV content to a byte array and return it as a file download
            var fileBytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());

            // set the content type to "text/csv" and specify a filename for the downloaded file
            return File(fileBytes, "text/csv", "products.csv");
        }

        [HttpOptions]
        public IActionResult RouteOptions()
        {
            List<string> options = new List<string> { "GET", "POST", "PUT", "DELETE", "OPTIONS", "HEADERS" };

            foreach (var option in options)
                Response.Headers.Append("Allow", option);

            return NoContent();
        }

        [HttpHead]
        public async Task<IActionResult> HeadProducts()
        {
            var products = await _unitOfWork.ProductsGeneric.CountAsync();

            if (products > 0)
            {
                Response.Headers.Append("X-Total-Count", products.ToString());
                return NoContent();
            }

            return NotFound();
        }

        [HttpHead("{id:int}")]
        public async Task<IActionResult> HeadProduct(int id)
        {
            var product = await _unitOfWork.ProductsGeneric.GetByIdAsync(id);

            if (product is not null)
                return Ok();

            return NotFound();
        }
    }
}