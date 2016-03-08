using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IIProjectClient.FordonsPassageServiceReference;
namespace IIProjectClient.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult Index()
        {
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
            FordonsPassageServiceClient client = new FordonsPassageServiceClient();
            XElement förfrågan = new XElement("Förfrågan",
                new XElement("tidsintervall",
                    new XElement("start",  ),
                    new XElement("slut", )
                ),
                new XElement("plats", "urn:epc:id:sgln:735999271.000.13")
                
                )
            XElement svar = client.HämtaFordonsPassager(förfrågan);
            return View();
        }
    }
}