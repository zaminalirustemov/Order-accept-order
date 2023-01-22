using Microsoft.AspNetCore.Identity;

namespace Pustok_book_sales_app.Models
{
    public class AppUser: IdentityUser
    {
        public string FullName { get; set; }
    }
}
