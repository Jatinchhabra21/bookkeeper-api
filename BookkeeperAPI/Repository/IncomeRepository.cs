namespace BookkeeperAPI.Repository
{
    #region usings
    using BookkeeperAPI.Model;
    using BookkeeperAPI.Repository.Interface;
    using Microsoft.EntityFrameworkCore;
    using BookkeeperAPI.Data;
    using BookkeeperAPI.Entity;
    using BookkeeperAPI.Exceptions;
    #endregion

    public class IncomeRepository : IIncomeRepository
    {
        private readonly BookkeeperContext _context;
        public IncomeRepository(BookkeeperContext context) 
        { 
            _context = context;
        }
        public async Task<IEnumerable<IncomeView>> GetAllIncomesAsync(Guid userId)
        {
            return await _context.Income.Where(income => income.UserId == userId).Select(income => new IncomeView()
            {
                Amount = income.Amount,
                Name = income.Name,
                Type = income.Type.ToString(),
            }).ToListAsync();
        }

        public async Task<Income?> GetIncomeByNameAsync(Guid userId, string name)
        {
            return await _context.Income.Where(income => income.UserId == userId && income.Name == name).FirstOrDefaultAsync();
        }

        public async Task AddIncomeAsync(Income income)
        {
            _context.Income.Add(income);

            int result = await _context.SaveChangesAsync();

            if(result != 1)
            {
                throw new HttpOperationException("Something went wrong");
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task RemoveIncomeAsync(Income income)
        {
            _context.Income.Remove(income);

            await _context.SaveChangesAsync();
        }
    }
}
