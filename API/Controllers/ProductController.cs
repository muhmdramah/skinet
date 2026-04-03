using AutoMapper;
using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Reader;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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

        [HttpGet("{productId:int}")]
        public async Task<IActionResult> GetProduct(int productId)
        {
            var products = await _unitOfWork.Products.GetByIdAsync(productId);

            if (products is null)
                return NotFound($"product with id {productId} was not found!");

            var dto = _mapper.Map<ProductDto>(products);

            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] ProductDto productDto) 
        {
            if(productDto == null)
                return BadRequest("Please enter a valid Product!");

            var product = _mapper.Map<Product>(productDto);

            await _unitOfWork.Products.AddAsync(product);
            _unitOfWork.Complete();

            return Ok($"Product with id: {product.Id} was created successfully!");
        }

        [HttpDelete("{productId:int}")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var currentProduct = await _unitOfWork.Products.GetByIdAsync(productId);

            if (currentProduct is null)
                return NotFound($"Product with id: {productId} is already not fount in the context!");

            _unitOfWork.Products.Delete(currentProduct);
            _unitOfWork.Complete();

            return Ok($"Product with id: {productId} deleted successfully!");
        }

        [HttpPut("{productId:int}")]
        public async Task<IActionResult> UpdateProduct(int productId, [FromBody] ProductDto productDto)
        {
            var currentProduct = await _unitOfWork.Products.GetByIdAsync(productId);

            if (currentProduct is null)
                return NotFound($"Product with id: {productId} was not fount in the context!");

            var product = _mapper.Map(productDto, currentProduct);

            _unitOfWork.Products.Update(currentProduct);
            _unitOfWork.Complete();
            return Ok($"Product with id: {productId} updated successfully!");
        }
    }
}
