using HeartDisease.Services.Webshop;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeartDisease.Controllers {
    [Authorize]
    public class WebshopController : Controller {
        private readonly ILogger<WebshopController> _logger;
        private readonly HttpClient _httpClient;
        private readonly OrderService _orderService;
        private readonly OrderItemService _orderItemService;
        private readonly OrderHistoryService _orderHistoryService;
        private readonly ProductService _productService;
        private readonly ReviewService _reviewService;
        private readonly SideEffectService _sideEffectService;
        private readonly ManufacturerService _manufacturerService;


        public WebshopController(
            ILogger<WebshopController> logger, OrderService orderService, OrderItemService orderItemService, OrderHistoryService orderHistoryService,
            ProductService productService, ReviewService reviewService, SideEffectService sideEffectService, ManufacturerService manufacturerService) {
            _logger = logger;
            _httpClient = new HttpClient();
            _orderService = orderService;
            _orderItemService = orderItemService;
            _orderHistoryService = orderHistoryService;
            _productService = productService;
            _reviewService = reviewService;
            _sideEffectService = sideEffectService;
            _manufacturerService = manufacturerService;
        }


        #region Products
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 12) {
            try {
                var totalProducts = await _productService.CountProductsAsync();
                var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);
                ViewBag.TotalPages = totalPages;
                ViewBag.CurrentPage = pageNumber;
                ViewBag.PageSize = pageSize;

                var products = await _productService.GetProductsPagesAsync(pageNumber, pageSize);
                return View("Webshop", products);  // Assuming Webshop.cshtml is set up for pagination
            } catch (Exception ex) {
                _logger.LogError("Failed to load products: {Exception}", ex);
                return View("Error");
            }
        }


        public async Task<IActionResult> ProductDetails(int id) {
            try {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null) {
                    return NotFound();
                }
                return View(product);
            } catch (Exception ex) {
                _logger.LogError("Failed to load product details: {Exception}", ex);
                return View("Error");
            }
        }
        #endregion

        #region Shopping Cart
        //[HttpPost]
        //public async Task<IActionResult> AddToCart(int productId, int quantity) {
        //    try {
        //        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get user ID from the authentication claims
        //        var activeOrder = await _orderService.GetActiveOrderByUserId(userId);

        //        if (activeOrder == null) {
        //            activeOrder = await _orderService.CreateOrder(userId);
        //            await _orderHistoryService.CreateOrderHistory(activeOrder.OrderID, "Pending");
        //        }

        //        var product = await _productService.GetProductByIdAsync(productId);
        //        if (product == null) {
        //            return NotFound("Product not found");
        //        }

        //        var orderItem = new OrderItem {
        //            OrderID = activeOrder.OrderID,
        //            ProductID = productId,
        //            Quantity = quantity,
        //            PriceAtTimeOrder = product.Price
        //        };

        //        await _orderItemService.InsertOrderItem(orderItem);
        //        await _orderService.UpdateOrderTotal(activeOrder.OrderID);

        //        return RedirectToAction("Cart"); // Redirect to cart view
        //    } catch (Exception ex) {
        //        _logger.LogError("Error adding to cart: {Message}", ex.Message);
        //        return View("Error");
        //    }
        //}


        #endregion

    }
}
