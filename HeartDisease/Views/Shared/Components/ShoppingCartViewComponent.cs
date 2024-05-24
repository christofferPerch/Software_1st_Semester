using System.Security.Claims;
using HeartDisease.Services.Webshop;
using HeartDisease.ViewModels;
using Microsoft.AspNetCore.Mvc;

public class ShoppingCartViewComponent : ViewComponent {
    private readonly OrderService _orderService;
    private readonly OrderItemService _orderItemService;
    private readonly ProductService _productService;

    public ShoppingCartViewComponent(OrderService orderService, OrderItemService orderItemService, ProductService productService) {
        _orderService = orderService;
        _orderItemService = orderItemService;
        _productService = productService;
    }

    public async Task<IViewComponentResult> InvokeAsync() {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return View(new ShoppingCartViewModel());

        var cart = GetShoppingCart(userId);
        return View(cart);
    }

    private ShoppingCartViewModel GetShoppingCart(string userId) {
        var activeOrder = _orderService.GetActiveOrderForUser(userId).Result;
        if (activeOrder == null) return new ShoppingCartViewModel();

        var items = _orderItemService.GetOrderItemsByOrderId(activeOrder.OrderID).Result;
        return new ShoppingCartViewModel {
            Items = items.Select(i => new ShoppingCartItem {
                OrderItemID = i.OrderItemID,
                ProductID = i.ProductID,
                ProductName = _productService.GetProductByIdAsync(i.ProductID).Result.ProductName,
                Price = i.PriceAtTimeOfOrder,
                Quantity = i.Quantity,
                ImageURL = _productService.GetProductByIdAsync(i.ProductID).Result.ImageURL
            }).ToList(),
            Total = items.Sum(i => i.PriceAtTimeOfOrder * i.Quantity)
        };
    }
}
