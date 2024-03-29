namespace BookkeeperAPI.Repository
{
    #region usings
    using Constants;
    using Data;
    using Entity;
    using Exceptions;
    using Model;
    using Interface;
    using Microsoft.EntityFrameworkCore;
    #endregion
    public class TransactionRepository : ITransactionRepository
    {
        private BookkeeperContext _context;

        public TransactionRepository(BookkeeperContext context)
        {
            _context = context;
        }

        public async Task<List<TransactionView>> GetTransactionsAsync()
        {
            List<TransactionView> data = await _context.Transaction 
                .Select(x => new TransactionView()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Amount = x.Amount,
                    Category = x.Category.ToString(),
                    Date = x.Date,
                    Type = x.Type.ToString()
                })
                .ToListAsync();

            return data;
        }

        public async Task<int> GetTransactionCountAsync(Guid userId, ExpenseCategory? category, string? name, DateTime? from, DateTime? to, TransactionType? type)
        {
            int totalCount = await _context.Transaction
                .Where(x => (
                    x.UserId.Equals(userId) &&
                    (x.Category == category || category == null) &&
                    (name == null || x.Name.Contains(name)) &&
                    (from == null || x.Date >= from) &&
                    (to == null || x.Date <= to) &&
                    (x.Type == type || type == null)
                    )
                )
                .CountAsync();

            return totalCount;
        }

        public async Task<Transaction?> GetTransactionByIdAsync(Guid transactionId)
        {
            Transaction? transaction = await _context.Transaction
                .Where(x => x.Id.Equals(transactionId))
                .FirstOrDefaultAsync();

            return transaction;
        }


        public async Task SaveTransactionAsync(Transaction transaction)
        {
            await _context.Transaction.AddAsync(transaction);

            int result = await _context.SaveChangesAsync();

            if (result == 0)
            {
                throw new HttpOperationException("Something went wrong");
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTransactionAsync(Transaction transaction)
        {
            _context.Transaction.Remove(transaction);
            await _context.SaveChangesAsync();
        }
    }
}
