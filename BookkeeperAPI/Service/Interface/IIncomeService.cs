using BookkeeperAPI.Model;

namespace BookkeeperAPI.Service.Interface
{
    public interface IIncomeService
    {
        public Task<IEnumerable<IncomeView>> GetAllIncomesAsync(Guid userId);

        public Task<IncomeView> AddIncomeAsync(Guid userId, AddIncomeRequest request);

        public Task<IncomeView> UpdateIncomeAsync(Guid userId, string name, UpdateIncomeRequest request);

        public Task RemoveIncomeAsync(Guid userId, string name);
    }
}
