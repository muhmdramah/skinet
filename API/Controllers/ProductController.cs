using API.Helpers.LinkGeneratorHelper;
using AutoMapper;
using Core.DTOs;
using Core.DTOs.Requests;
using Core.DTOs.Responses;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Reader;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/products")]
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

            var product = _mapper.Map<Product>(createProductRequest);

            await _unitOfWork.ProductsGeneric.AddAsync(product);
            await _unitOfWork.CompleteAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id, version = "1.0" }, new
            {
                Product = product,
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
        public async Task<ActionResult<IReadOnlyCollection<Product>>> GetProductsByBrand(string brandName)
        {
            var products = await _productRepository.GetProductsByBrandAsync(brandName);

            if (!products.Any())
                return NotFound("There's no Products in this brand right now... Try again later!");

            return Ok(products);
        }

        [HttpGet("types/{typeName}")]
        public async Task<ActionResult<IReadOnlyCollection<Product>>> GetProductsByType(string typeName)
        {
            var products = await _productRepository.GetProductsByTypeAsync(typeName);

            if (!products.Any())
                return NotFound("There's no Products in this type right now... Try again later!");

            return Ok(products);
        }

        [HttpGet("specific-brand-and-type")]
        public async Task<ActionResult<IReadOnlyCollection<Product>>> GetProductsByASpecificBrandAndType(string brand, string type)
        {
            var products = await _productRepository.GetProductsByASpecificBrandAndTypeAsync(brand, type);

            if (!products.Any())
                return NotFound("There's no Products with this brand and type right now... Try again later!");

            return Ok(products);
        }


        [HttpGet("sorted-by-price")]
        public async Task<ActionResult<IReadOnlyCollection<Product>>> GetProductsSortedByPrice
            ([FromQuery,
                SwaggerParameter("Sorting option: 'priceAsc' for ascending, 'priceDesc' for descending... " +
                    "No parameter? will sort by name ascending!", Required = false)] string? sort)
        {
            var products = await _productRepository.GetProductsSortedByPriceAsync(sort);

            if (!products.Any())
                return NotFound("There's no Products right now... Try again later!");

            return Ok(products);
        }

        [HttpOptions]
        public IActionResult RouteOptions()
        {
            Response.Headers.Append("Allow", "GET, POST, PUT, DELETE, OPTIONS");
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