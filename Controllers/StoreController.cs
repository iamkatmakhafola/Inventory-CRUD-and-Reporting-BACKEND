using Assignment3_Backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Assignment3_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly IRepository _repository;
        public StoreController(IRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [Route("AllProducts")]
        public async Task<ActionResult> AllProducts() 
        {
            try
            {
                var results = await _repository.GetProducts();

                dynamic products = results.Select(p => new
                {
                    p.ProductId,
                    p.Price,
                    ProductTypeName = p.ProductType.Name,
                    BrandName = p.Brand.Name,
                    p.Name,
                    p.Description,
                    p.DateCreated,
                    p.DateModified,
                    p.IsActive,
                    p.IsDeleted,
                    p.Image
                });

                return Ok(products);
            }
            catch (Exception)
            {

                return BadRequest("Server Error, Please try again");
            }
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("AddProduct")]
        public async Task<IActionResult> AddProduct([FromForm] IFormCollection formData)
        {
            try
            {
                var formCollection = await Request.ReadFormAsync();

                var uploadedFile = formCollection.Files.First();

                if (uploadedFile.Length > 0)
                {

                    using (var memoryStream = new MemoryStream())
                    {
                        uploadedFile.CopyTo(memoryStream);
                        var fileBytes = memoryStream.ToArray();
                        string base64Image = Convert.ToBase64String(fileBytes);

                        string priceString = formData["price"];
                        decimal parsedPrice = decimal.Parse(priceString.Replace(".", ","));

                        var newProduct = new Product
                        {
                            BrandId = Convert.ToInt32(formData["brand"])
                            ,
                            ProductTypeId = Convert.ToInt32(formData["producttype"])
                            ,
                            Name = formData["name"]
                            ,
                            Description = formData["description"]
                            ,
                            Image = base64Image
                            ,
                            Price = parsedPrice
                            ,
                            DateCreated = DateTime.Now
                        };


                        _repository.Add(newProduct);
                        await _repository.SaveChanges();
                    }

                    return Ok();
                }
                else
                {
                    return BadRequest("Error, Please try again");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpGet]
        [Route("Brands")]
        public async Task<ActionResult> Brands()
        {
            try
            {
                var results = await _repository.GetBrands();

                return Ok(results);
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error. Please contact support.");
            }
        }

        [HttpGet]
        [Route("ProductTypes")]
        public async Task<ActionResult> ProductTypes()
        {
            try
            {
                var results = await _repository.GetProductTypes();

                return Ok(results);
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error. Please contact support.");
            }
        }

        [HttpGet]
        [Route("GetProductCountsByBrand")]
        public async Task<IActionResult> GetProductCountsByBrand()
        {
            var products = await _repository.GetProducts();
            var productCountsByBrand = products.GroupBy(p => p.Brand)
                                               .Select(g => new { Brand = g.Key.Name, Count = g.Count() })
                                               .ToList();

            return Ok(productCountsByBrand);
        }

        [HttpGet]
        [Route("GetProductCountsByType")]
        public async Task<IActionResult> GetProductCountsByType()
        {
            var products = await _repository.GetProducts();
            var productCountsByType = products.GroupBy(p => p.ProductType)
                                              .Select(g => new { ProductType = g.Key.Name, Count = g.Count() })
                                              .ToList();

            return Ok(productCountsByType);
        }

        [HttpGet]
        [Route("GetActiveProductsReport")]
        public async Task<IActionResult> GetActiveProductsReport()
        {
            var products = await _repository.GetProducts();
            var activeProducts = products.Where(p => p.IsActive)
                                         .GroupBy(p => new { p.ProductType, p.Brand })
                                         .Select(g => new
                                         {
                                             ProductType = g.Key.ProductType.Name,
                                             Brand = g.Key.Brand.Name,
                                             Products = g.Select(p => new { p.Name, p.Description }).ToList()
                                         })
                                         .ToList();

            return Ok(activeProducts);
        }
    }
}
