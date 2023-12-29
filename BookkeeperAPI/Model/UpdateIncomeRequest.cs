using BookkeeperAPI.Constants;

namespace BookkeeperAPI.Model
{
    public class UpdateIncomeRequest
    {
        public string? Name { get; set; } = string.Empty;

        public IncomeType? Type { get; set; }

        public double? Amount { get; set; }
    }
}
