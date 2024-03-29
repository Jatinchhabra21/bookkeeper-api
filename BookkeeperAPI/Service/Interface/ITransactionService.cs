namespace BookkeeperAPI.Service.Interface
{
    #region usings
    using BookkeeperAPI.Constants;
    using BookkeeperAPI.Model;
    #endregion

    public interface ITransactionService
    {
        public Task<List<TransactionView>> GetTransactionsAsync();

        public Task<TransactionView> GetTransactionByIdAsync(Guid expenseId);

        public Task<TransactionView> CreateTransactionAsync(Guid userId, CreateTransactionRequest request);

        public Task<TransactionView> UpdateTransactionAsync(Guid expenseId, UpdateTransactionRequest request);

        public Task DeleteTransactionAsync(Guid expenseId);
    }
}
