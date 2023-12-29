namespace BookkeeperAPI.Service.Interface
{
    #region usings
    using BookkeeperAPI.Constants;
    using BookkeeperAPI.Model;
    #endregion

    public interface ITransactionService
    {
        public Task<PaginatedResult<TransactionView>> GetPaginatedTransactionsAsync(Guid userId, string domain, int pageNumber, int pageSize, ExpenseCategory? category, string? name, DateTime? from, DateTime? to, TransactionType? type);

        public Task<TransactionView> GetTransactionByIdAsync(Guid expenseId);

        public Task<TransactionView> CreateTransactionAsync(Guid userId, CreateTransactionRequest request);

        public Task<TransactionView> UpdateTransactionAsync(Guid expenseId, UpdateTransactionRequest request);

        public Task DeleteTransactionAsync(Guid expenseId);
    }
}
