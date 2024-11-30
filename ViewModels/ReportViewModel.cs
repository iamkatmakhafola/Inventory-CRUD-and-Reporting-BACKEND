namespace Assignment3_Backend.ViewModels
{
    public class ReportViewModel
    {
        //1st label linked to 1st data object
        public List<string> label { get; set; } = new List<string>();

        public List<int> data { get; set; } = new List<int>();
    }
}
