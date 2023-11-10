using EticaretProje.Data;
using EticaretProje.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;

namespace EticaretProje.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly EticaretDBContext _context;
        private ShoppingCart shoppingCart;

        public HomeController(ILogger<HomeController> logger, EticaretDBContext context)
        {
            _logger = logger;
            _context = context;
            shoppingCart = new ShoppingCart();
            shoppingCart._context = context;
        }

        public IActionResult Index()
        {
            SessionDegerAtama();
            SessionaAdBilgisiAtama();
            UrunleriSessionaAta();


            ViewData["ad"] = HttpContext.Session.GetString("adbilgisi");
           

            var aaa = KategorileriGetir();

            int? fiyatbilgisi = HttpContext.Session.GetInt32("fiyat");
            if (fiyatbilgisi == null)
            {
                //sepet alanında gösterilecek olan html içerisine 0 basmak 
            }
            else
            {
                //sepet alanında gösterilecek olan html içerisine fiyatbilgisindeki değişkenin değerini atamak.
            }

            OrnekVeri veri = new OrnekVeri(_context);
            veri.AddGenres(); //veri tabanındaki genre tablosuna kayıt ekler.
            veri.AddArtist(); //veri tabanındaki artist tablosuna kayıt ekler.
            veri.AddAlbums();

            //veritabanındaki Tur tablosundan türleri getirip liste olarak ekrana ekleyiniz. 5 dk.
            //<ol>
            //<li> </li> 
            //<li> </li> 
            //<li> </li> 
            //<li> </li> 
            //<li> </li> 
            //</ol>




            List<Genre> turler = _context.Genres.ToList();
            var cart = shoppingCart.GetCart(this.HttpContext);
            
            HttpContext.Session.SetString("adet", cart.GetCount().ToString());
            ViewData["CartCount"] = HttpContext.Session.GetString("adet");
            return View(turler);
        }


        public IActionResult Getir(int id)
        {
            if (HttpContext.Session.GetString("kategoriid") != null)
            {
                id = Convert.ToInt32(HttpContext.Session.GetString("kategoriid"));
            }

            ViewData["CartCount"] = HttpContext.Session.GetString("adet");
            List <Album> albumler = _context.Albums.Where(satir => satir.GenreId == id).ToList();
            return View(albumler);

        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Liste()
        {
            return View();
        }


        //Home/KategorileriGetir
        //kategorileri listeleyip getirecek.
        //json datası haline getirilecek
        //div içerisinde gösterilecek.
        public JsonResult KategorileriGetir()
        {
            //1.yol
            //Genre genre = new Genre();
            //genre.Path = "foto.png";
            //genre.Name = "kategori1";
            //genre.GenreId = 1;
            //genre.Description = "yeni kategori";

            //2.yol
            //Genre genre1 = new Genre() { Name = "yeni kategori2", Path = "foto1.png", Description = "açıklama", GenreId = 3 };

            var liste = _context.Genres.ToList();
            string jsonDeger = "";
            foreach (var item in liste)
            {
                jsonDeger += JsonConvert.SerializeObject(item);
            }
            return Json(jsonDeger);
        }


        //1.Layout içerisine Kategoriler adında yeni bir menü ekleyelim.  ++
        //2.o menüyü homecontroller içerisindeki metoda baglayalım  +++
        //3.yeni bir view ekleyelim 
        //4.sayfada bir textbox bir button olsun,
        //5.buttona tıklanınca o idli bilgileri getirip ekrana göstersin.


        public IActionResult Kategoriler()
        {
            HttpContext.Session.Clear();
            return View();
        }


        public IActionResult KategoriIdyeGoreGetir(int id)
        {
            var kayit = _context.Genres.Where(satir => satir.GenreId == id).FirstOrDefault();
            var secilenKategori = JsonConvert.SerializeObject(kayit);
            return Json(secilenKategori);
        }

        public IActionResult KategoriKaydet(string kategoriAd, string path)
        {
            Genre genre = new Genre();
            genre.Name = kategoriAd;
            genre.Path = "yok.png";
            genre.Description = "deneme";
            _context.Genres.Add(genre);
            _context.SaveChanges();
            return Json("");
        }


        public IActionResult KategoriGetirIdyeGore(int id)
        {
            var kayit = _context.Genres.Where(satir => satir.GenreId == id).FirstOrDefault();

            var secilenKategori = JsonConvert.SerializeObject(kayit);
            return Json(secilenKategori);
        }

        // url: "/Home/KategoriGuncelle/?kategoriId=" + kategoriId + "&desc=" + kategoriDesc + "&kategoriAd="kategoriAd+ "&kategoriFotoPath="+kategoriFotoPath,

        //Home: controller Adı
        //KategoriGuncelle : metot Adı
        //metot parametreleri : ?kategoriId=" + kategoriId + "&desc=" + kategoriDesc + "&kategoriAd="kategoriAd+ "&kategoriFotoPath="+kategoriFotoPath


        //url: "/Home/KategoriGuncelle/?id=" + id + "&name=" + name + "&path=" + path + "&description=" + description,
        public IActionResult KategoriGuncelle(int id, string description, string name, string path)
        {
            //Genre tur = new Genre();
            Genre genre = _context.Genres.Where(satir => satir.GenreId == id).FirstOrDefault();
            genre.Path = path;
            genre.Description = description;
            genre.Name = name;
            _context.Update(genre);
            _context.SaveChanges();
            return Json("guncelleme başarılı");
        }


        //silme işleminde sadece id gönderilecek parametre olarak
        //silerken de aşağıdaki gibi olmalı
        //Genre genre = _context.Genres.Where(satir => satir.GenreId == id).FirstOrDefault();       
        //_context.Remove(genre);
        //_context.SaveChanges();


        //AldumDetails adında yeni bir metot

        public IActionResult AlbumDetails(int id)
        {
            //Album Detaylarında aydegeri value'su ile atanmış olan değeri sessiondan okuyalım.
            //int - int? 
            //int  : default 0 değerini alır.
            //int? : null - deger


            int? result = HttpContext.Session.GetInt32("aydegeri");
            string? isim = HttpContext.Session.GetString("adbilgisi");

            //var urun = ByteArrayToObject(HttpContext.Session.Get("urunler"));
            return View(_context.Albums.Where(satir => satir.AlbumId == id).FirstOrDefault());
        }

        //metot yazalım.
        //metot Session içerisine şuanki bulundugumuz zamanın ay sayısal bilgisini alıp session içerisine "aydegeri" adında ekleyelim.
        public void SessionDegerAtama()
        {
            int ay = DateTime.Now.Month;
            HttpContext.Session.SetInt32("aydegeri", ay);  //session içerisine aydegeri value'su ile ay değişkeni içerisindeki bilgiyi atadım.
        }

        //kendi adımızı sessiona adbilgisi key değeri ile ekleyelim, sonra bu değeri albumdetails içerisinde okuyalım.

        public void SessionaAdBilgisiAtama()
        {
            HttpContext.Session.SetString("adbilgisi", "nagihan");
        }

        //session içerisine ürün atayalım.
        //ürün diye bir tane class yazalım. ++
        //classın id,ad,fiyat bilgileri olsun. ++
        //3 tane ürün nesnesi örnekleyelim. 
        //bu nesneleri session içerisine urun1,urun2,urun3 olacak sekilde dolduralım.
        public void UrunleriSessionaAta()
        {
            Urun u1 = new Urun() { Id = 100, Ad = "Telefon", Fiyat = 50000 };
            Urun u2 = new Urun() { Id = 200, Ad = "Kulaklık", Fiyat = 5000 };
            Urun u3 = new Urun() { Id = 300, Ad = "Akıllı Saat", Fiyat = 25000 };

            // List<Urun> urunler = new List<Urun>();
            // urunler.Add(u1);
            // urunler.Add(u2);
            // urunler.Add(u3);

            // var qq=HomeController.ObjectToByteArray(u1);
            // HttpContext.Session.Set("urunler", qq);

        }

        //public static byte[] ObjectToByteArray(Object obj)
        //{
        //    BinaryFormatter bf = new BinaryFormatter();
        //    using (var ms = new MemoryStream())
        //    {
        //        bf.Serialize(ms, obj);
        //        return ms.ToArray();
        //    }
        //}

        //public static Object ByteArrayToObject(byte[] arrBytes)
        //{
        //    using (var memStream = new MemoryStream())
        //    {
        //        var binForm = new BinaryFormatter();
        //        memStream.Write(arrBytes, 0, arrBytes.Length);
        //        memStream.Seek(0, SeekOrigin.Begin);
        //        var obj = binForm.Deserialize(memStream);
        //        return obj;
        //    }
        //}
        
        //public IActionResult SepeteEkle(int id)
        //{
        //    //id bilgisini SessionaEkleme        
        //    HttpContext.Session.SetInt32("adet",  );

        //    //ViewData["adet"]=HttpContext.Session.GetString("urunid"); 
        //    ViewData["adet"]=HttpContext.Session.GetString("adet"); //adet olarak değiştirelim.

        //    int kategoriid=_context.Albums.Where(satir => satir.AlbumId == id).FirstOrDefault().GenreId;

        //    List<Album> albumler = _context.Albums.Where(satir => satir.GenreId == kategoriid).ToList();
        //    return RedirectToAction("Getir", albumler);
        //}


    }
}