using Microsoft.AspNetCore.Identity;

namespace LedgerStock.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}