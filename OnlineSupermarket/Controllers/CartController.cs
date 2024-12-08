using Microsoft.AspNetCore.Mvc;
using OnlineSupermarket.Models;
using OnlineSupermarket.Services;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace OnlineSupermarket.Controllers
{
    public class CartController : Controller
    {
        private readonly CartService _cartService;
        private readonly string _connectionString;

        public CartController(CartService cartService, IConfiguration configuration)
        {
            _cartService = cartService;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
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

        [HttpGet]
        public IActionResult Create(int productId, decimal celkovaCena)
        {
            var model = new Platba
            {
                ZboziIdZbozi = productId,
                CelkovaCena = celkovaCena
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(Platba model)
        {
            if (ModelState.IsValid)
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    using (var command = new OracleCommand("VLOZ_PLATBU", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new OracleParameter("p_celkovaCena", model.CelkovaCena));
                        command.Parameters.Add(new OracleParameter("p_typ", model.Typ));
                        command.Parameters.Add(new OracleParameter("p_zbozi_idzbozi", model.ZboziIdZbozi));
                        command.Parameters.Add(new OracleParameter("p_prodejna_idprodejny", model.ProdejnaIdProdejny));
                        var idTranzakceParam = new OracleParameter("p_idtranzakce", OracleDbType.Decimal)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(idTranzakceParam);

                        connection.Open();
                        try
                        {
                            command.ExecuteNonQuery();
                            var idTranzakce = ((OracleDecimal)idTranzakceParam.Value).ToInt32();
                            // Логика после успешного выполнения процедуры
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }

                // Удаление товара из корзины после успешного выполнения процедуры
                _cartService.RemoveFromCart(model.ZboziIdZbozi);

                // Установка сообщения в TempData
                TempData["SuccessMessage"] = "Nakup Uspesne Udelan";

                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}

