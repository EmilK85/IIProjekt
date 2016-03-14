using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IIProjectClient.FordonsPassageServiceReference;
using System.Xml.Linq;
using IIProjectClient.Models;
namespace IIProjectClient.Controllers
{
    public class TestController : Controller
    {
        static Sokning senasteSökningen = new Sokning();
        public ActionResult Sokning(int page=-1)
        {
            if (page < 1) return View(new Sokning());

            return Sokning(null,page);
        }

        [HttpPost]
        public ActionResult Sokning(Sokning sökning, int page = 1)
        {
            //gammal sökning som vi forsätter på
            if (sökning == null) { sökning = senasteSökningen; }
            //ny sökning, vi vill börja om på sida 1
            else { page = 1; }

            int antal = sökning.antal;
            
            //kan aldrig vara på sida 0 eller mindre
            if (page < 1) page = 1;

            ViewBag.page = page;
            int senastTagna = (page - 1) * antal;

            sökning.resultat = SkickaFörfrågan(senastTagna, antal, sökning.plats, sökning.start, sökning.slut, sökning.anropsansvarig);

            senasteSökningen = sökning;

            return View(sökning);
        }



//        // GET: Test
//        public ActionResult Index(int page = 1)
//        {
//            //antal passager per page
//            int antal = 35;

//            //kan aldrig vara på sida 0 eller mindre
//            if (page < 1) page = 1;

//            ViewBag.page = page;
//            int senastTagna = (page - 1) * antal;

////            <förfrågan>
////    <datetimeFörfrågan></datetimeFörfrågan>
////    <anropsansvarig>
////        <namn>
////        <lösenord>
////</anropsansvarig>
////    <tidsintervall>
////        <start></start>
////        <slut></slut>
////    </tidsintervall>
////    <plats></plats>
////    <paginering>
////        <antal></antal>
////        <senastTagna></senastTagna>
////    </paginering>
////</förfrågan>
//            return View(SkickaFörfrågan(senastTagna, antal));

//        }



        public XElement SkickaFörfrågan(int senastTagna, int antal, string plats, DateTime start, DateTime slut, string anropsansvarig)
        {
            FordonsPassageServiceClient client = new FordonsPassageServiceClient();
            XElement förfrågan = new XElement("förfrågan",
                new XElement("datetimeFörfrågan", DateTime.Now),
                new XElement("anropsansvarig", anropsansvarig),
                new XElement("tidsintervall",
                    new XElement("start", start.Date.ToString()),
                    new XElement("slut", slut.Date.AddDays(1.0).ToString())),
                new XElement("plats", plats),
                new XElement("paginering",
                    new XElement("antal", antal),
                    new XElement("senastTagna", senastTagna)));
            XElement svar = client.HämtaFordonsPassager(förfrågan);

            return svar;
            
        }
    }

}