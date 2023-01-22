using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace Pustok_book_sales_app.Areas.Manage.ViewModels
{
    public class AdminLoginViewModel
    {
        [Required]
        [StringLength(maximumLength:30)]
        public string UserName { get; set; }

        [Required]
        [StringLength(maximumLength: 30,MinimumLength =8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
