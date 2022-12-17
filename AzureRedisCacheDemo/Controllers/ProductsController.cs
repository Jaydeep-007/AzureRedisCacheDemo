using AzureRedisCacheDemo.Models;
using AzureRedisCacheDemo.Repositories;
using AzureRedisCacheDemo.Repositories.AzureRedisCache;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AzureRedisCacheDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IRedisCache _redisCache;

        public ProductsController(IProductService productService, IRedisCache redisCache)
        {
            _productService = productService;
            _redisCache = redisCache;
        }

        /// <summary>
        /// Product List
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<ProductDetails>>> ProductListAsync()
        {
            var cacheData = _redisCache.GetCacheData<List<ProductDetails>>("product");
            if (cacheData != null)
            {
                return new List<ProductDetails>(cacheData);
            }

            var productList = await _productService.ProductListAsync();
            if(productList != null)
            {
                var expirationTime = DateTimeOffset.Now.AddMinutes(5.0);
                _redisCache.SetCacheData<List<ProductDetails>>("product", productList, expirationTime);
                return Ok(productList);
            }
            else
            {
                return NoContent();
            }
        }

        /// <summary>
        /// Get Product By Id
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        [HttpGet("{productId}")]
        public async Task<ActionResult<ProductDetails>> GetProductDetailsByIdAsync(int productId)
        {
            var cacheData = _redisCache.GetCacheData<List<ProductDetails>>("product");
            if (cacheData != null)
            {
                ProductDetails filteredData = cacheData.Where(x => x.Id == productId).FirstOrDefault();
                return new ActionResult<ProductDetails>(filteredData);
            }

            var productDetails = await _productService.GetProductDetailByIdAsync(productId);
            if(productDetails != null)
            {
                return Ok(productDetails);
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Add a new product
        /// </summary>
        /// <param name="productDetails"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddProductAsync(ProductDetails productDetails)
        {
            var isProductInserted = await _productService.AddProductAsync(productDetails);
            _redisCache.RemoveData("product");
            if (isProductInserted)
            {
                return Ok(isProductInserted);
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Update product details
        /// </summary>
        /// <param name="productDetails"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateProductAsync(ProductDetails productDetails)
        {
            var isProductUpdated = await _productService.UpdateProductAsync(productDetails);
            _redisCache.RemoveData("product");
            if (isProductUpdated)
            {
                return Ok(isProductUpdated);
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Delete product by id
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteProductAsync(int productId)
        {
            var isProductDeleted = await _productService.DeleteProductAsync(productId);
            _redisCache.RemoveData("product");
            if (isProductDeleted)
            {
                return Ok(isProductDeleted);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
