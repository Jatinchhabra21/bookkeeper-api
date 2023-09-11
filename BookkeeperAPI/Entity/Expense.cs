namespace BookkeeperAPI.Entity
{
    #region usings
    using System.Text.Json.Serialization;
    #endregion

    public class Expense
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public string Name { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ExpenseCategory Category { get; set; }

        public double Amount { get ; set; }


        public User User { get; set; }
    }
}
