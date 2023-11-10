using EticaretProje.Models;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace EticaretProje.Controllers
{
    public class APITestController : Controller
    {
        public IActionResult HavaDurumu()
        {
            var qq=HavaDurumuBilgileriniCek("sivas");
            ViewData["Sıcaklık"] = qq.OrtalamaSicaklik;
            return PartialView("HavaDurumu");
        }


        private Hava HavaDurumuBilgileriniCek(string sehir)
        {
            string connection = "https://api.openweathermap.org/data/2.5/weather?q=" + sehir + "&mode=xml&appid=7a92ce2a13a8e0b383dc773223ce2f86";

            //MessageBox.Show(connection);

            XDocument veri = XDocument.Load(connection);
            Hava hava = new Hava();
            hava.OrtalamaSicaklik = veri.Descendants("temperature").ElementAt(0).Attribute("value").Value;
            hava.MinSicaklik = veri.Descendants("temperature").ElementAt(0).Attribute("min").Value;
            hava.MaxSicaklik = veri.Descendants("temperature").ElementAt(0).Attribute("max").Value;
            hava.HavaDurumu = veri.Descendants("weather").ElementAt(0).Attribute("value").Value;
            return hava;
        }

    }
}
