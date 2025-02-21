using System.ComponentModel.DataAnnotations;

namespace Payne.ViewModels.Account;

public class ForgetPasswordVm
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}