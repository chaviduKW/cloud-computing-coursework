namespace SearchApi
{
    public class SalaryRecord
    {
        public decimal SalaryAmount { get; set; }

        public string CompanyName { get; set; } = string.Empty;

        public string Designation { get; set; } = string.Empty;

        public DateOnly SubmittedDate { get; set; }
    }
}
