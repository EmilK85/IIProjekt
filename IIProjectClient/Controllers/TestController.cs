using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IIProjectClient.FordonsPassageServiceReference;
using System.Xml.Linq;
namespace IIProjectClient.Controllers
{
    public class TestController : Controller
    {
        int page;
        // GET: Test
        public ActionResult Index(int page = 1)
        {
            //antal passager per page
            int antal = 35;

            //kan aldrig vara på sida 0 eller mindre
            if (page < 1) page = 1;

            ViewBag.page = page;
            int senastTagna = (page - 1) * antal;

//            <förfrågan>
//    <datetimeFörfrågan></datetimeFörfrågan>
//    <anropsansvarig>
//        <namn>
//        <lösenord>
//</anropsansvarig>
//    <tidsintervall>
//        <start></start>
//        <slut></slut>
//    </tidsintervall>
//    <plats></plats>
//    <paginering>
//        <antal></antal>
//        <senastTagna></senastTagna>
//    </paginering>
//</förfrågan>
            return View(SkickaFörfrågan(senastTagna, antal));

        }



        public XElement SkickaFörfrågan(int senastTagna, int antal)
        {
            FordonsPassageServiceClient client = new FordonsPassageServiceClient();
            XElement förfrågan = new XElement("förfrågan",
                new XElement("datetimeFörfrågan", "2011-03-25"),
                new XElement("anropsansvarig", "Amanda"),
                new XElement("tidsintervall",
                    new XElement("start", DateTime.Parse("2011-03-25").ToString()),
                    new XElement("slut", DateTime.Parse("2011-10-10").ToString())),
                new XElement("plats", "urn:epc:id:sgln:735999271.000.9"),
                new XElement("paginering",
                    new XElement("antal", antal),
                    new XElement("senastTagna", senastTagna)));
            XElement svar = client.HämtaFordonsPassager(förfrågan);
            //XElement svar = client.TestFunktion();
            return svar;
            
        }
    }

//    Goteborg	2011-08-19	17:06	urn:epc:id:giai:735006027.2336849640691	336849640691
//Goteborg	2011-08-19	17:06	urn:epc:id:giai:735006027.1378049504749	378049504749
//Goteborg	2011-08-19	17:06	urn:epc:id:giai:735006027.2336849640659	336849640659
//Goteborg	2011-08-19	17:06	urn:epc:id:giai:735006027.2378049549165	378049549165
//Goteborg	2011-08-19	17:06	urn:epc:id:giai:735006027.2336849640667	336849640667
}