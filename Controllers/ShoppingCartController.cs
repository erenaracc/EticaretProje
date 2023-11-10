using EticaretProje.Data;
using EticaretProje.Models;
using EticaretProje.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Text.Encodings.Web;

namespace EticaretProje.Controllers
{
    //sepete ekleme,silme işlemleri burada kullanılacak
    public class ShoppingCartController : Controller
    {

        //repository design pattern 
        private readonly EticaretDBContext _context;  //veri tabanına kayıt işlemi oldugu için ekledik.
        private ShoppingCart shoppingCart;

        //ctor : constructor: kurucu metot.
        public ShoppingCartController(EticaretDBContext context)
        {
            _context = context;
            shoppingCart = new ShoppingCart();
            shoppingCart._context = context;
        }


        //
        // GET: /ShoppingCart/
        //
        public ActionResult Index()
        {
            //shoppingCart._context = _context;
            var cart = shoppingCart.GetCart(this.HttpContext); //login olup olmadıgımızı getiren metot

            // Set up our ViewModel
            var viewModel = new ShoppingCartViewModel
            {
                CartItems = cart.GetCartItems(),
                CartTotal = cart.GetTotal()
            };
            // Return the view
           
            return View(viewModel);
        }


        //sepete album id bilgisine göre ürüm ekleyen metot, aynı zamanda sepet adet bilgisini de alır.
        //
        // GET: /Store/AddToCart/5
        public IActionResult AddToCart(int id)
        {
            // Retrieve the album from the database
            var addedAlbum = _context.Albums.Single(album => album.AlbumId == id);

            // Add it to the shopping cart
            var cart = shoppingCart.GetCart(this.HttpContext);

            cart.AddToCart(addedAlbum);
            HttpContext.Session.SetString("adet", cart.GetCount().ToString());
            ViewData["CartCount"] = HttpContext.Session.GetString("adet");
            // Go back to the main store page for more shopping
            HttpContext.Session.SetString("kategoriid", addedAlbum.GenreId.ToString());
            return RedirectToAction("Getir","Home", addedAlbum.GenreId.ToString());
        }

        //
        //sepetten ürün silmek içim aşağıdaki metot kullanılır.
        // AJAX: /ShoppingCart/RemoveFromCart/5
        [HttpPost]
        public IActionResult RemoveFromCart(int id)
        {
            // Remove the item from the cart
            var cart = shoppingCart.GetCart(this.HttpContext);

            // Get the name of the album to display confirmation
            string albumName = _context.Sepets.Include(p => p.Album).Single(item => item.RecordId == id).Album.Title;

            // Remove from cart
            int itemCount = cart.RemoveFromCart(id);
            HttpContext.Session.SetString("adet", cart.GetCount().ToString());

            // Display the confirmation message
            var results = new ShoppingCartRemoveViewModel
            {
                Message = HtmlEncoder.Default.Encode(albumName) + " has been removed from your shopping cart.",
                SepetTotal = cart.GetTotal(),
                SepetCount = cart.GetCount(),
                ItemCount = itemCount,
                DeleteId = id
            };
            return Json(results);
        }


        //Sepetteki ürünlerin özeti
        // GET: /ShoppingCart/CartSummary     
        //public IActionResult CartSummary()
        //{
        //    var cart = shoppingCart.GetCart(this.HttpContext);
        //    ViewData["CartCount"] = cart.GetCount();
        //    return PartialView("CartSummary");
        //}


        public IActionResult SepetiBosalt()
        {
            shoppingCart.EmptyCart();
            //HttpContext.Session.Clear(); //Session içerisindeki tüm değerleri temizler.
            HttpContext.Session.SetString("adet", "0");
            return RedirectToAction("Kategoriler", "Home");
        }
    }
}
