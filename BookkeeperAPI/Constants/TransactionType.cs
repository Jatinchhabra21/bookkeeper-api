namespace BookkeeperAPI.Constants
{
    #region usings
    using System.Text.Json.Serialization;
    #endregion

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TransactionType
    {
        debit,
        credit
    }
}
