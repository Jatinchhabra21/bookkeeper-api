namespace BookkeeperAPI.Model
{
    public class TransactionView
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public double Amount { get; set; }
    }
}
