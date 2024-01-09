namespace BookkeeperAPI.Service
{
    #region usings
    using BookkeeperAPI.Entity;
    using BookkeeperAPI.Exceptions;
    using BookkeeperAPI.Model;
    using BookkeeperAPI.Repository.Interface;
    using BookkeeperAPI.Service.Interface;
    #endregion

    public class IncomeService : IIncomeService
    {
        private readonly IIncomeRepository _incomeRepository;
        public IncomeService(IIncomeRepository incomeRepository)
        {
            _incomeRepository = incomeRepository;
        }

        public async Task<IEnumerable<IncomeView>> GetAllIncomesAsync(Guid userId)
        {
            return await _incomeRepository.GetAllIncomesAsync(userId);
        }

        public async Task<IncomeView> AddIncomeAsync(Guid userId, AddIncomeRequest request)
        {
            bool doesExist = (await _incomeRepository.GetIncomeByNameAsync(userId, request.Name)) != null ;

            if (doesExist)
            {
                throw new HttpOperationException(400, $"Income with name \"{request.Name}\" already exist");
            }

            Income income = new Income()
            {
                Amount = request.Amount,
                Name = request.Name,
                Type = request.Type,
                UserId = userId
            };

            await _incomeRepository.AddIncomeAsync(income);

            return new IncomeView()
            {
                Type = income.Type.ToString(),
                Amount = income.Amount,
                Name = income.Name,
            };
        }

        public async Task<IncomeView> UpdateIncomeAsync(Guid userId, string name, UpdateIncomeRequest request)
        {
            Income? income = await _incomeRepository.GetIncomeByNameAsync(userId, name);

            if (income == null)
            {
                throw new HttpOperationException(400, $"Income with name \"{name}\" does not exist");
            }

            income.Name = request.Name ?? income.Name;
            income.Type = request.Type ?? income.Type;
            income.Amount = request.Amount ?? income.Amount;

            await _incomeRepository.SaveChangesAsync();

            return new IncomeView()
            {
                Amount = income.Amount,
                Type = income.Type.ToString(),
                Name = income.Name,
            };
        }

        public async Task RemoveIncomeAsync(Guid userId, string name)
        {
            Income? income = await _incomeRepository.GetIncomeByNameAsync(userId, name);

            if (income == null)
            {
                throw new HttpOperationException(400, $"Income with name \"{name}\" does not exist");
            }

            await _incomeRepository.RemoveIncomeAsync(income);
        }
    }
}
