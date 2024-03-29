using BookkeeperAPI.Constants;
using BookkeeperAPI.Entity;
using BookkeeperAPI.Model;

namespace BookkeeperAPI.Repository.Interface
{
    public interface ITransactionRepository
    {
        public Task<List<TransactionView>> GetTransactionsAsync();

        public Task<int> GetTransactionCountAsync(Guid userId, ExpenseCategory? category, string? name, DateTime? from, DateTime? to, TransactionType? type);

        public Task<Transaction?> GetTransactionByIdAsync(Guid transactionId);

        public Task SaveTransactionAsync(Transaction transaction);

        public Task SaveChangesAsync();

        public Task DeleteTransactionAsync(Transaction trabsaction);
    }
}
