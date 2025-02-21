using Microsoft.AspNetCore.Identity;

namespace Payne.Models;

public class AppUser : IdentityUser
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public ICollection<Review> Reviews { get; set; }
}