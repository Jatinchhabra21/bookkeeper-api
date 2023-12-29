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

        public async Task<PaginatedResult<TransactionView>> GetPaginatedTransactionsAsync(Guid userId, string domain, int pageNumber, int pageSize, ExpenseCategory? category, string? name, DateTime? from, DateTime? to, TransactionType? type)
        {
            string filterQuery = "";
            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>()
            {
                { "pageNumber", pageNumber },
                { "pageSize", pageSize }
            };

            if (category != null)
            {
                filterQuery += $"&category={category}";
                parameters.Add("category", category.ToString()!);
            }

            if (name != null)
            {
                filterQuery += $"&name={name}";
                parameters.Add("name", name);
            }

            if (from != null)
            {
                filterQuery += $"&from={from}";
                parameters.Add("from", from);
            }

            if (to != null)
            {
                filterQuery += $"&to={to}";
                parameters.Add("to", to);
            }

            if (type != null)
            {
                filterQuery += $"&type={type}";
                parameters.Add("type", type.ToString()!);
            }

            List<TransactionView> data = await _transactionRepository.GetTransactionsAsync(userId, pageNumber, pageSize, category, name, from, to, type);

            int totalCount = await _transactionRepository.GetTransactionCountAsync(userId, category, name, from, to, type);

            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            PaginatedResult<TransactionView> result = new PaginatedResult<TransactionView>()
            {
                PageCount = totalPages,
                PageNumber = pageNumber,
                PageSize = pageSize,
                FirstPage = domain + "?pageNumber=1&pageSize=" + pageSize + filterQuery,
                LastPage = domain + "?pageNumber=" + totalPages + "&pageSize=" + pageSize + filterQuery,
                TotalCount = totalCount,
                NextPage = pageNumber == totalPages ? null : (domain + "?pageNumber=" + (pageNumber + 1) + "&pageSize=" + pageSize + filterQuery),
                PreviousPage = pageNumber == 1 ? null : (domain + "?pageNumber=" + (pageNumber - 1) + "&pageSize=" + pageSize + filterQuery),
                Data = data,
            };

            return result;
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
            };

            return result;
        }

        public async Task<TransactionView> UpdateTransactionAsync(Guid transactionId, UpdateTransactionRequest request)
        {
            Transaction? _transaction = await _transactionRepository.GetTransactionByIdAsync(transactionId);

            if (_transaction == null)
            {
                throw new HttpOperationException(StatusCodes.Status404NotFound, $"Expense with id '{transactionId}' does not exist");
            }


            if (
                (
                _transaction.Type == TransactionType.credit 
                || 
                (request.Type != null && request.Type == TransactionType.credit)) // checks if transaction type is already credit or is being updated to credit
                && 
                (request.Category != null 
                ? request.Category != ExpenseCategory.none 
                : _transaction.Category != ExpenseCategory.none) // checks if category is not being updated to none or it isn't already none
            )
            {
                throw new HttpOperationException(400, "For transaction type credit, category should be none");
            }

                _transaction.Amount = request?.Amount ?? _transaction.Amount;
            _transaction.Category = request?.Category ?? _transaction.Category;
            _transaction.Name = request?.Name ?? _transaction.Name;
            _transaction.Date = request?.Date ?? _transaction.Date;

            await _transactionRepository.SaveChangesAsync();

            TransactionView result = new TransactionView()
            {
                Id = _transaction.Id,
                Name = _transaction.Name,
                Category = _transaction.Category.ToString(),
                Amount = _transaction.Amount,
                Date = _transaction.Date
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
