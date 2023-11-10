using Microsoft.AspNetCore.Mvc;

namespace EticaretProje.Controllers
{
    
    public class FileUploadController : Controller
    {
        [HttpGet]
        public IActionResult ImageUpload()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ImageUpload(IFormFile formFile)
        {
            return View();
        }




    }
}
