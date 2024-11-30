using Assignment3_Backend.Models;

namespace Assignment3_Backend.ViewModels
{
    public class ReportBrandProduct
    {
        //
        public string brand { get; set; }

        public List<Product> products { get; set; } = new List<Product>();
    }
}
