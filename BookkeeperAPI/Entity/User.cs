using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookkeeperAPI.Entity
{
    public class User
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column(TypeName = "jsonb")]
        public UserPreference? Preference { get; set; }


        public IEnumerable<Expense>? Expenses { get; set; }
    }
}
