namespace BookkeeperAPI.Service
{
    #region usings
    using BookkeeperAPI.Constants;
    using BookkeeperAPI.Data;
    using BookkeeperAPI.Entity;
    using BookkeeperAPI.Exceptions;
    using BookkeeperAPI.Model;
    using BookkeeperAPI.Repository.Interface;
    using BookkeeperAPI.Service.Interface;
    using Microsoft.EntityFrameworkCore;
    #endregion

    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<List<TransactionView>> GetTransactionsAsync()
        {
            List<TransactionView> data = await _transactionRepository.GetTransactionsAsync();
            return data;
        }

        public async Task<TransactionView> GetTransactionByIdAsync(Guid expenseId)
        {
            Transaction? result = await _transactionRepository.GetTransactionByIdAsync(expenseId);

            if (result != null)
            {
                return new TransactionView()
                {
                    Name = result.Name,
                    Id = result.Id,
                    Amount = result.Amount,
                    Category = result.Category.ToString(),
                    Date = result.Date,
                    Type = result.Type.ToString()
                };
            }

            throw new HttpOperationException(StatusCodes.Status404NotFound, "Invalid transaction id");
        }

        public async Task<TransactionView> CreateTransactionAsync(Guid userId, CreateTransactionRequest request)
        {
            // check transaction type and category, that is, if transaction type is debit then only category should be given otherwise it should be null
            if (request.Type == TransactionType.credit && request.Category != ExpenseCategory.none)
            {
                throw new HttpOperationException(400, "For transaction type credit, category should be none");
            }

            Transaction transaction = new Transaction();
            transaction.UserId = userId;
            transaction.Name = request.Name;
            transaction.Amount = request.Amount;
            transaction.Category = request.Category;
            transaction.Date = request.Date;
            transaction.Type = request.Type;


            await _transactionRepository.SaveTransactionAsync(transaction);

            TransactionView result = new TransactionView()
            {
                Id = transaction.Id,
                Name = transaction.Name,
                Category = transaction.Category.ToString(),
                Amount = transaction.Amount,
                Date = transaction.Date,
                Type = transaction.Type.ToString()
            };

            return result;
        }

        public async Task<TransactionView> UpdateTransactionAsync(Guid transactionId, UpdateTransactionRequest request)
        {
            Transaction? transaction = await _transactionRepository.GetTransactionByIdAsync(transactionId);

            if (transaction == null)
            {
                throw new HttpOperationException(StatusCodes.Status404NotFound, $"Expense with id '{transactionId}' does not exist");
            }


            if (
                (
                transaction.Type == TransactionType.credit 
                || 
                (request.Type != null && request.Type == TransactionType.credit)) // checks if transaction type is already credit or is being updated to credit
                && 
                (request.Category != null 
                ? request.Category != ExpenseCategory.none 
                : transaction.Category != ExpenseCategory.none) // checks if category is not being updated to none or it isn't already none
            )
            {
                throw new HttpOperationException(400, "For transaction type credit, category should be none");
            }

                transaction.Amount = request?.Amount ?? transaction.Amount;
            transaction.Category = request?.Category ?? transaction.Category;
            transaction.Name = request?.Name ?? transaction.Name;
            transaction.Date = request?.Date ?? transaction.Date;
            transaction.Type = request?.Type ?? transaction.Type;

            await _transactionRepository.SaveChangesAsync();

            TransactionView result = new TransactionView()
            {
                Id = transaction.Id,
                Name = transaction.Name,
                Category = transaction.Category.ToString(),
                Amount = transaction.Amount,
                Date = transaction.Date,
                Type = transaction.Type.ToString()
            };

            return result;
        }

        public async Task DeleteTransactionAsync(Guid transactionId)
        {
            Transaction? transaction = await _transactionRepository.GetTransactionByIdAsync(transactionId);

            if (transaction == null)
            {
                throw new HttpOperationException(StatusCodes.Status404NotFound, $"Expense with id '{transactionId}' does not exist");
            }

            await _transactionRepository.DeleteTransactionAsync(transaction);
        }
    }
}
