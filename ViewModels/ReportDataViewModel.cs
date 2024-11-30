namespace Assignment3_Backend.ViewModels
{
    public class ReportDataViewModel
    {
        //Returns everything || 

        public List<ReportViewModel> ProductCountByBrand { get; set; }

        public List<ReportViewModel> ProductCountByProductType { get; set; }

        public List<ReportBrandProduct> ActiveProductReport {  get; set; }

    }
}
