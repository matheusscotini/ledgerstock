namespace LedgerStock.Domain.Constants;

public static class SystemRoles
{
    public const string Master = "Master";
    public const string Admin = "Admin";
    public const string Standard = "Standard";

    public static readonly string[] All = { Master, Admin, Standard };
}