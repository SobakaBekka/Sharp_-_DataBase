using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OnlineSupermarket.Models;

namespace OnlineSupermarket.Services
{
    public class CartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string CartSessionKey = "CartItems";

        public CartService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private List<CartItem> GetCartItemsFromSession()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var cartItemsJson = session.GetString(CartSessionKey);
            return cartItemsJson != null ? JsonConvert.DeserializeObject<List<CartItem>>(cartItemsJson) : new List<CartItem>();
        }

        private void SaveCartItemsToSession(List<CartItem> cartItems)
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var cartItemsJson = JsonConvert.SerializeObject(cartItems);
            session.SetString(CartSessionKey, cartItemsJson);
        }

        public IEnumerable<CartItem> GetCartItems() => GetCartItemsFromSession();

        public void AddToCart(CartItem item)
        {
            var cartItems = GetCartItemsFromSession();
            var existingItem = cartItems.FirstOrDefault(i => i.ProductId == item.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                cartItems.Add(item);
            }
            SaveCartItemsToSession(cartItems);
        }

        public void RemoveFromCart(int productId)
        {
            var cartItems = GetCartItemsFromSession();
            var item = cartItems.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                cartItems.Remove(item);
            }
            SaveCartItemsToSession(cartItems);
        }

        public decimal GetTotalPrice() => GetCartItemsFromSession().Sum(i => i.TotalPrice);
    }
}


