namespace BookkeeperAPI.Model
{
    #region usings
    using BookkeeperAPI.Constants;
    #endregion

    public class UpdateTransactionRequest
    {
        public string? Name { get; set; }

        public ExpenseCategory? Category { get; set; }

        public double? Amount { get; set; }

        public DateTime? Date { get; set; }

        public TransactionType? Type { get; set; }
    }
}
