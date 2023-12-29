namespace BookkeeperAPI.Repository.Interface
{
    using BookkeeperAPI.Entity;
    #region usings
    using BookkeeperAPI.Model;
    #endregion

    public interface IIncomeRepository
    {
        public Task<IEnumerable<IncomeView>> GetAllIncomesAsync(Guid userId);

        public Task<Income?> GetIncomeByNameAsync(Guid userId, string name);

        public Task AddIncomeAsync(Income income);

        public Task SaveChangesAsync();

        public Task RemoveIncomeAsync(Income income);
    }
}
