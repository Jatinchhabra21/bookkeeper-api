using BookkeeperAPI.Constants;

namespace BookkeeperAPI.Model
{
    public class IncomeView
    {
        public string Name { get; set; } = string.Empty;

        public string Type { get; set; }

        public double Amount { get; set; }
    }
}
