using EticaretProje.Data;
using EticaretProje.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.Xml;

namespace EticaretProje.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        //CTRL+R+R : tüm kullanılan yerleri değiştirir.
        private readonly ILogger<AdminController> _logger;
        private readonly EticaretDBContext _context;

        public AdminController(ILogger<AdminController> logger, EticaretDBContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Turler()
        {
            var turler = _context.Genres.ToList();
            return View(turler);
        }

        //Edit tür sayfasını açan metot
        public IActionResult EditTur(int id)
        {
            var editTur = _context.Genres.Where(satir => satir.GenreId == id).FirstOrDefault();
            return View(editTur);
        }

        //Edit Tür fileupload örneği
        public IActionResult EditTurfileUpload(int id)
        {
            var editTur = _context.Genres.Where(satir => satir.GenreId == id).FirstOrDefault();
            return View(editTur);
        }

        //aşağıdaki buttona tıklanınca çalışan metot
        [HttpPost]
        public IActionResult EditTurfileUpload(IFormFile formFile, Genre genre)
        {
            if (formFile != null)
            {
                var extent = Path.GetExtension(formFile.FileName);
                var randomName = ($"{Guid.NewGuid()}{extent}");
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img", randomName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    formFile.CopyToAsync(stream);
                }
                genre.Path = "img/"+ randomName;
                _context.Update(genre);
                _context.SaveChanges();

            }

           



           
            return View();
            //return RedirectToAction("MetotAdi", "ControllerAdi");

        }

        public IActionResult Details(int id)
        {
            var editTur = _context.Albums.Include(satir=>satir.Artist).Include(satir=>satir.Genre).Where(satir => satir.AlbumId == id).FirstOrDefault();

            return View(editTur);
        }

        public IActionResult Delete(int id)
        {
            var silinecekTur = _context.Genres.Where(satir => satir.GenreId == id).FirstOrDefault();
            return View(silinecekTur);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int GenreId)
        {
            var silinecekTur = _context.Genres.Where(satir => satir.GenreId == GenreId).FirstOrDefault();
            _context.Remove(silinecekTur);
            _context.SaveChanges(); ;
            return View("Turler");
        }

        public IActionResult Albumler()
        {
            var liste = _context.Albums.Include(satir=>satir.Genre).Include(satir=>satir.Artist).ToList();
            return View(liste);
        }
        public IActionResult Sanatcilar()
        {

            var liste = _context.Artists.ToList();
            return View(liste);
        }

        public IActionResult SanatciDetails(int id)
        {
            Artist artist = _context.Artists.Where(satir => satir.ArtistId == id).FirstOrDefault();
            return View(artist);
        }

        //SanatcininAlbumleriniGetir
        public IActionResult SanatcininAlbumleriniGetir(int id)
        {
            List<Album> albumler=_context.Albums.Where(satir => satir.ArtistId == id).ToList();
            return View(albumler);
        }


        //Yeni bir album oluşturmak istediğmizde aşağıdaki kod
        public IActionResult Create()
        {
            //Genre tablosuna gidelim,
            //genre tablosundaki tüm kayıtların sadece adlarını getirelim.

            //ViewBag.GenreId = new SelectList(veriler, "tıklanıncaSeçilecekDeger", "Önyüzde gözükecek bilgi");
            ViewBag.GenreId = new SelectList(TurleriGetir(), "GenreId", "Name");
            ViewBag.ArtistId = new SelectList(sanatcilariGetir(), "ArtistId", "Name");
            return View();
        }

        [HttpPost]
        public IActionResult Create(IFormFile formFile, Album album)
        {
            HttpContext.Session.SetInt32("fiyat", 35);   //fiyat adında bir 

            HttpContext.Session.GetInt32("fiyat"); 
            if (formFile != null)
            {
                var extent = Path.GetExtension(formFile.FileName); //dosya uzantısını alır

                if(formFile.Length> 15000000)
                {
                    return View("Error");
                    //return Json("Hata");
                }
                else
                {
                    if (extent != ".exe")
                    {
                        var randomName = ($"{Guid.NewGuid()}{extent}"); //yeni bir dosya adı üretir
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img", randomName);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            formFile.CopyToAsync(stream);
                        }
                        album.AlbumArtUrl = "img/" + randomName;
                        _context.Albums.Add(album);
                        _context.SaveChanges();
                        return View("Albumler",_context.Albums.ToList());  //album ekleme işlemi sonrasında otomatik olarak Albumler sayfasını acacak kod
                    }
                    else
                    {
                        return View("Error");
                    }
                }                

            }
            else
            {
                _context.Albums.Add(album);
                _context.SaveChanges();
                return View("Albumler");  //album ekleme işlemi sonrasında otomatik olarak Albumler sayfasını acacak kod
            }
            
        }

        //Genre bilgilerini getirecek ve list yapısına dolduracak metot.
        public List<Genre> TurleriGetir()
        {
            return _context.Genres.ToList();
        }


        //Sanatçı bilgilerini getirecek ve list yapısına dolduracak metot.
        public List<Artist> sanatcilariGetir()
        {
            return _context.Artists.ToList();
        }



    }
}
