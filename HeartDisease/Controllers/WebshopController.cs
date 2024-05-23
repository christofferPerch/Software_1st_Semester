using System.Security.Claims;
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
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity) {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) return Unauthorized();

            var order = await _orderService.EnsureActiveOrder(userId);
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null) return NotFound("Product not found");

            await _orderItemService.AddProductToOrder(order.OrderID, productId, quantity, product.Price);

            // Update order's total amount
            order.TotalAmount += product.Price * quantity;
            await _orderService.UpdateOrder(order);

            return RedirectToAction("Index");
        }
        #endregion

        [HttpDelete]
        public async Task<IActionResult> DeleteOrderItem(int orderItemId) {
            _logger.LogInformation("Attempting to delete order item with ID: {Id}", orderItemId);
            try {
                await _orderItemService.DeleteOrderItemAsync(orderItemId);
                return Json(new { success = true });
            } catch (Exception ex) {
                _logger.LogError(ex, "Error deleting item.");
                return Json(new { success = false, message = ex.Message });
            }
        }



        [HttpPost]
        public async Task<IActionResult> EmptyCart(int orderId) {
            await _orderService.DeleteOrder(orderId);
            return Ok();
        }
    }
}
