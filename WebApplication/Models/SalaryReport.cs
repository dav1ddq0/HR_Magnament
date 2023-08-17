using System.ComponentModel.DataAnnotations;

namespace HR_API.Models
{
    public class SalaryReport
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public double Balance { get; set; }

        public DateTime ReportDate { get; set; } = DateTime.UtcNow;

        public Salary Salary { get; set; }
    }
}
