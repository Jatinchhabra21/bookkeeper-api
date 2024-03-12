namespace BookkeeperAPI.Model
{
    #region usings
    using Constants;
    using System.Text.Json.Serialization;
    #endregion
    
    public class TransactionView
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public double Amount { get; set; }

        public string Type { get; set; }
    }
}
