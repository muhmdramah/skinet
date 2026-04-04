using API.Helpers.LinkGeneratorHelper;
using AutoMapper;
using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Tasks.Deployment.Bootstrapper;
using Microsoft.OpenApi.Reader;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/products")] 
    public class ProductController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILinkGeneratorHelper _linkGeneratorHelper;

        public ProductController(IUnitOfWork unitOfWork, IMapper mapper, ILinkGeneratorHelper linkGeneratorHelper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
           _linkGeneratorHelper = linkGeneratorHelper;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _unitOfWork.Products.GetAllAsync();

            if (!products.Any())
                return NotFound("There's no products right now... Try again later!");

            var dto = _mapper.Map<List<ProductDto>>(products);

            return Ok(dto);
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var products = await _unitOfWork.Products.GetByIdAsync(id);

            if (products is null)
                return NotFound($"product with id {id} was not found!");

            var dto = _mapper.Map<ProductDto>(products);

            var response = new
            {
                dto,
                links = _linkGeneratorHelper.GenerateConfirmationUrl(id)
            };

            return Ok(response);
        }

        [HttpPost(Name = "Create")]
        public async Task<IActionResult> AddProduct([FromBody] CreateProductDto createProductDto) 
        {
            if(createProductDto == null)
                return BadRequest("Please enter a valid Product!");

            var product = _mapper.Map<Core.Entities.Product>(createProductDto);

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.CompleteAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id, version = "1.0" }, product);
            //return Ok($"Product with id: {product.Id} was created successfully!");
        }

        [HttpDelete("{id:int}", Name = "Delete")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var currentProduct = await _unitOfWork.Products.GetByIdAsync(id);

            if (currentProduct is null)
                return NotFound($"Product with id: {id} is already not fount in the context!");

            _unitOfWork.Products.Delete(currentProduct);
            await _unitOfWork.CompleteAsync();

            return Ok($"Product with id: {id} deleted successfully!");
        }

        [HttpPut("{id:int}", Name = "Update")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto updareProductDto)
        {
            var currentProduct = await _unitOfWork.Products.GetByIdAsync(id);

            if (currentProduct is null)
                return NotFound($"Product with id: {id} was not fount in the context!");

            var product = _mapper.Map(updareProductDto, currentProduct);

            _unitOfWork.Products.Update(currentProduct);
            await _unitOfWork.CompleteAsync();

            return Ok($"Product with id: {id} updated successfully!");
        }
    }
}
