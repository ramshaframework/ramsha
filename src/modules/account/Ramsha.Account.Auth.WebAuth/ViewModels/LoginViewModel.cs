using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;

namespace Ramsha.Account.Auth.WebAuth;

public class RamshaLoginViewModel
{
    [Required]
    public string UserName { get; set; } = default!;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = default!;

    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; }

    public IList<AuthenticationScheme>? ExternalLogins { get; set; }
    public string? ReturnUrl { get; set; }
    public string? ErrorMessage { get; set; }

}
