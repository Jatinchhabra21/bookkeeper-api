namespace BookkeeperAPI.Entity
{
    using Microsoft.EntityFrameworkCore;
    #region usings
    using System.ComponentModel;
    using System.Text.Json.Serialization;
    #endregion

    [Keyless]
    public class UserPreference
    {
        public Currency DefaultCurrency { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Theme DefaultTheme { get; set; }

        public bool DailyReminder { get; set; }
    }
}
