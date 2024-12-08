using System.Collections.Generic;
using System.Linq;
using OnlineSupermarket.Models;

namespace OnlineSupermarket.Services
{
    public class CartService
    {
        private readonly List<CartItem> _cartItems = new List<CartItem>();

        public IEnumerable<CartItem> GetCartItems() => _cartItems;

        public void AddToCart(CartItem item)
        {
            var existingItem = _cartItems.FirstOrDefault(i => i.ProductId == item.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                _cartItems.Add(item);
            }
        }

        public void RemoveFromCart(int productId)
        {
            var item = _cartItems.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                _cartItems.Remove(item);
            }
        }

        public decimal GetTotalPrice() => _cartItems.Sum(i => i.TotalPrice);
    }
}