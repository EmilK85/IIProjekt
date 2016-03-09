﻿using System;
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
            XElement förfrågan = new XElement("förfrågan",
                new XElement("tidsintervall",
                    new XElement("start", DateTime.Parse("2011-03-25")),
                    new XElement("slut", DateTime.Parse("2011-04-02"))),
                new XElement("plats", "urn:epc:id:sgln:735999271.000.13"),
                new XElement("paginering",
                    new XElement("antal", "randomNummer"),
                    new XElement("senastTagna", "randomNummer")));
            XElement svar = client.HämtaFordonsPassager(förfrågan);
            return View(svar);
        }
    }
}