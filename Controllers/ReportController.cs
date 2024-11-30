using Assignment3_Backend.Models;
using Assignment3_Backend.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Assignment3_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IRepository _repository;

        public ReportController(IRepository repository)
        {
            _repository = repository;
        }

        [HttpGet, Route("getReportData")]
        public async Task<IActionResult> GetReportData()
        {
            try
            {
                var brands = await _repository.GetBrands();
                var products = await _repository.GetProducts();
                var productTypes = await _repository.GetProductTypes();
                ReportDataViewModel reportData = new ReportDataViewModel();

                // Product by Count
                reportData.ProductCountByBrand = brands.Select(x => new ReportViewModel
                {
                    label = new List<string> { x.Name },
                    data = new List<int> { products.Count(y => y.BrandId == x.BrandId) }
                }).ToList();


                // Product count by product Type 
                reportData.ProductCountByProductType = productTypes.GroupBy(x => x.ProductTypeId).Select(y => new ReportViewModel
                {
                    label = new List<string> { y.FirstOrDefault()?.Name},
                    data = new List<int> { y.Count() }
                }).ToList();


                // Active Products Report
                foreach (var item in products)
                {
                    //Set Product Type
                    var productTypeInDb = productTypes.Where(x => x.ProductTypeId == item.ProductTypeId).FirstOrDefault();
                    ProductType productType = new ProductType();
                    productType.ProductTypeId = productTypeInDb.ProductTypeId;
                    productType.Name = productTypeInDb.Name;
                    productType.IsActive = productTypeInDb.IsActive;
                    productType.DateCreated = productTypeInDb.DateCreated;
                    productType.DateModified = productTypeInDb.DateModified;
                    productType.IsDeleted = productTypeInDb.IsDeleted;
                    productType.Description = productTypeInDb.Description;
                    item.ProductType = productType;


                    // Set Brand
                    var brandInDb = brands.Where(x => x.BrandId == item.BrandId).FirstOrDefault();
                    Brand brand = new Brand();
                    brand.BrandId = brandInDb.BrandId;
                    brand.DateModified = brandInDb.DateModified;
                    brand.IsActive = brandInDb.IsActive;
                    brand.IsDeleted = brandInDb.IsDeleted;
                    brand.Name = brandInDb.Name;
                    brand.Description = brandInDb.Description;
                    brand.DateCreated = brandInDb.DateCreated;
                    item.Brand = brand;

                }
                reportData.ActiveProductReport = products.Where(x => x.IsActive == true).Select(y => new ReportBrandProduct
                {
                    brand = y.Name,
                    products = products.Where(x => x.BrandId == y.BrandId).ToList()
                }).ToList();

                return Ok(reportData);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}