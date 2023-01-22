﻿using Microsoft.AspNetCore.Identity;
using Pustok_book_sales_app.Models;

namespace Pustok_book_sales_app.Areas.Manage.Services
{
    public class LayoutService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LayoutService(UserManager<AppUser> userManager,IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AppUser> GetUser()
        {
            string name = _httpContextAccessor.HttpContext.User.Identity.Name;

            AppUser appUser = await _userManager.FindByNameAsync(name);


            return appUser;
        }
    }
}
