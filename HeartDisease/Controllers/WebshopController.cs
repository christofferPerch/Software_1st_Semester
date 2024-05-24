﻿using HeartDisease.Services.Webshop;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Linq;


namespace HeartDisease.Controllers
{
    [Authorize]
    public class WebshopController : Controller
    {
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
            ProductService productService, ReviewService reviewService, SideEffectService sideEffectService, ManufacturerService manufacturerService)
        {
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
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 12, string searchTerm = "", string manufacturerIds = "", int minPrice = 0, int maxPrice = 500)
        {
            try
            {
                int[]? manufacturerIdArray = string.IsNullOrEmpty(manufacturerIds)
                                                ? null
                                                : manufacturerIds.Split(',').Select(int.Parse).ToArray();

                var totalProducts = await _productService.CountProductsAsync(searchTerm, manufacturerIdArray, minPrice, maxPrice);
                var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);
                ViewBag.TotalPages = totalPages;
                ViewBag.CurrentPage = pageNumber;
                ViewBag.PageSize = pageSize;

                var products = await _productService.GetProductsPagesAsync(pageNumber, pageSize, searchTerm, manufacturerIdArray, minPrice, maxPrice);
                ViewBag.SearchTerm = searchTerm;
                ViewBag.ManufacturerIds = manufacturerIds;
                ViewBag.MinPrice = minPrice;
                ViewBag.MaxPrice = maxPrice;

                return View("Webshop", products);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to load products: {Exception}", ex);
                return View("Error");
            }
        }



        public async Task<IActionResult> ProductDetails(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }
                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to load product details: {Exception}", ex);
                return View("Error");
            }
        }

        [HttpGet("api/Manufacturer/search")]
        public async Task<IActionResult> SearchManufacturers(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return BadRequest("Search term cannot be empty.");
            }

            var manufacturers = await _productService.GetManufacturersByName(term);
            return Ok(manufacturers);
        }

        [HttpGet("api/Manufacturer/{id}")]
        public async Task<IActionResult> GetManufacturer(int id)
        {
            var manufacturer = await _productService.GetManufacturerByIdAsync(id);
            if (manufacturer == null)
            {
                return NotFound();
            }
            return Ok(manufacturer);
        }

        [HttpGet("api/Product/search")]
        public async Task<IActionResult> SearchProducts(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return BadRequest("Search term cannot be empty.");
            }

            var products = await _productService.GetProductsByNameAsync(term);
            return Ok(products);
        }




        #endregion
    }
}
