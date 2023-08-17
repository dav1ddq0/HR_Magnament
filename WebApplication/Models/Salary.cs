namespace HR_API.Models
{
    public class Salary
    {

        public Guid Id { get; set; }

        public User User { get; set; }

        public double CurrentBalance { get; set; }

        public DateTime LastBalanceChangeDate { get; set; } = DateTime.UtcNow;



    }
}
