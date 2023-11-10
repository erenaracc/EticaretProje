using EticaretProje.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EticaretProje.Models
{
    public partial class ShoppingCart
    {
        public EticaretDBContext _context { get; set; }
        string ShoppingCartId { get; set; }

        public const string CartSessionKey = "CartId"; //const: sabit

        //aşağıdaki metot veritabanına kaydedilecek olan datanın loginid ve alışveriş sepeti bilgisini getiren metot.
        public ShoppingCart GetCart(HttpContext context)
        {
            var cart = new ShoppingCart();
            cart._context = _context;
            cart.ShoppingCartId = cart.GetCartId(context);
            return cart;
        }


        // Helper method to simplify shopping cart calls
        public ShoppingCart GetCart(Controller controller)
        {
            return GetCart(controller.HttpContext);
        }


        //sepete album eklemek istediğimizde bu metot kullanılacak, sql tarafına kayıt eklenecek
        public void AddToCart(Album album)
        {
            // Get the matching cart and album instances
            var cartItem = _context.Sepets.SingleOrDefault(satir => satir.CartId == ShoppingCartId
                && satir.AlbumId == album.AlbumId);

            if (cartItem == null)
            {
                // Create a new cart item if no cart item exists
                cartItem = new Sepet
                {
                    AlbumId = album.AlbumId,
                    CartId = ShoppingCartId,
                    Count = 1,
                    DateCreated = DateTime.Now
                };
                _context.Sepets.Add(cartItem);
            }
            else
            {
                // If the item does exist in the cart, 
                // then add one to the quantity
                cartItem.Count++;
            }
            // Save changes
            _context.SaveChanges();
        }

        //sepetten o ürünü silecek ve kaç tane oldugunun bilgisi bize döndürülecek.
        public int RemoveFromCart(int id)
        {
            // Get the cart
            var cartItem = _context.Sepets.Single(cart => cart.CartId == ShoppingCartId
                && cart.RecordId == id);

            int itemCount = 0;

            if (cartItem != null)
            {
                if (cartItem.Count > 1)
                {
                    cartItem.Count--;
                    itemCount = cartItem.Count;
                }
                else
                {
                    _context.Sepets.Remove(cartItem);
                }
                // Save changes
                _context.SaveChanges();
            }
            return itemCount;
        }

        //sepeti boşalt , komple herşeyi sil
        public void EmptyCart()
        {
            //jelibon bakılacak
            //var cartItems = _context.Sepets.Where(cart => cart.CartId == cart.ShoppingCartId);

            //foreach (var cartItem in cartItems)
            //{
            //    _context.Sepets.Remove(cartItem);
            //}

            // Save changes
            _context.SaveChanges();
        }

        //alışveriş sepetindeki ürünleri getirecek
        public List<Sepet> GetCartItems()
        {
            return _context.Sepets.Include(p => p.Album).Where(cart => cart.CartId == ShoppingCartId).ToList();
            // _context.Carts.Where(cart => cart.CartId == ShoppingCartId).ToList();
        }

        public int GetCount()
        {
            // Get the count of each item in the cart and sum them up
            int? count = (from cartItems in _context.Sepets
                          where cartItems.CartId == ShoppingCartId
                          select (int?)cartItems.Count).Sum();
            // Return 0 if all entries are null
            return count ?? 0; //eğer count bilgisi null ise bu durumda 0, diğer durumlarda kendi değerini dönecektir.
        }
        public decimal GetTotal()
        {
            // Multiply album price by count of that album to get 
            // the current price for each of those albums in the cart
            // sum all album price totals to get the cart total
            decimal? total = (from cartItems in _context.Sepets
                              where cartItems.CartId == ShoppingCartId
                              select (int?)cartItems.Count *
                              cartItems.Album.Price).Sum();  //sepettteki ürünlern hepsinin toplam tutarı, ödeme tutarı.

            return total ?? decimal.Zero;
        }
        public int CreateOrder(Order order)
        {
            decimal orderTotal = 0;

            var cartItems = GetCartItems();
            // Iterate over the items in the cart, 
            // adding the order details for each
            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    AlbumId = item.AlbumId,
                    OrderId = order.OrderId,
                    UnitPrice = item.Album.Price,
                    Quantity = item.Count
                };
                // Set the order total of the shopping cart
                orderTotal += (item.Count * item.Album.Price);

                _context.OrderDetails.Add(orderDetail);

            }
            // Set the order's total to the orderTotal count
            order.Total = orderTotal;

            // Save the order
            _context.SaveChanges();
            // Empty the shopping cart
            EmptyCart();

            // Return the OrderId as the confirmation number
            return order.OrderId;
        }



        //o an alışveriş yapan kişi sisteme giriş yapmış ise giriş yaptıgı maili, eğer login durumda değilse o an üretilen Guid  değerini geriye döner
        public string GetCartId(HttpContext context)
        {
            if (context.Session.GetString("CartSessionKey") == null)
            {
                if (!string.IsNullOrWhiteSpace(context.User.Identity.Name))
                {
                    context.Session.SetString("CartSessionKey", context.User.Identity.Name);
                }
                else
                {
                    // Generate a new random GUID using System.Guid class
                    Guid tempCartId = Guid.NewGuid();
                    // Send tempCartId back to client as a cookie
                    context.Session.SetString("CartSessionKey", tempCartId.ToString());
                }
            }
            return context.Session.GetString("CartSessionKey");
        }


        //login değilken sepete eklenen ürünleri, login olunca ilgili kişiye taşımak için aşağıdaki metot kullanılır
        public void LoginseSepettekileriSQLeTasi(string userName)
        {
            var shoppingCart = _context.Sepets.Where(
                c => c.CartId == ShoppingCartId);

            foreach (Sepet item in shoppingCart)
            {
                item.CartId = userName;
            }
            _context.SaveChanges();
        }
    }
}
