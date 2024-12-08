using Microsoft.AspNetCore.Mvc;
using OnlineSupermarket.Models;
using OnlineSupermarket.Services;

namespace OnlineSupermarket.Controllers
{
    public class CartController : Controller
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        public IActionResult Index()
        {
            var cartItems = _cartService.GetCartItems();
            return View(cartItems);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, string productName, string productImage, decimal price, int quantity)
        {
            var item = new CartItem
            {
                ProductId = productId,
                ProductName = productName,
                ProductImage = productImage,
                Price = price,
                Quantity = quantity
            };

            _cartService.AddToCart(item);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            _cartService.RemoveFromCart(productId);
            return RedirectToAction("Index");
        }
    }
}