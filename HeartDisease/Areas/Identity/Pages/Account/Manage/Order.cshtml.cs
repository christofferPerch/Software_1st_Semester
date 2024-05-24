using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HeartDisease.Models.Webshop;
using HeartDisease.Services.Webshop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeartDisease.Areas.Identity.Pages.Account.Manage {
    [Authorize]
    public class OrderModel : PageModel {
        private readonly OrderService _orderService;
        private readonly UserManager<IdentityUser> _userManager;

        public OrderModel(OrderService orderService, UserManager<IdentityUser> userManager) {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public List<Order> Orders { get; set; }

        public async Task OnGetAsync() {
            var user = await _userManager.GetUserAsync(User);
            Orders = await _orderService.GetUserOrdersAsync(user.Id);
        }
    }
}