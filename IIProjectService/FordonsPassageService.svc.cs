using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Xml.Linq;

namespace IIProjectService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class FordonsPassageService : IFordonsPassageService
    {
        //<förfrågan>
        //    <datetimeFörfrågan></datetimeFörfrågan>
        //    <anropsansvarig></anropsansvarig>
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

        public XElement HämtaFordonsPassager(XElement förfrågan)
        {
            string fromIncl = förfrågan.Element("tidsintervall").Element("start").Value;
            DateTime from = DateTime.Parse(fromIncl);
            string toIncl = förfrågan.Element("tidsintervall").Element("slut").Value;
            DateTime to = DateTime.Parse(toIncl);

            string platsEPC = förfrågan.Element("plats").Value;
            string plats = GetLocation((string)förfrågan.Element("plats")).Element("Location").Element("Name").Value;

            XElement events = GetEvents(from, to, platsEPC);
           
            /* SVARET
             * 
             * <Svar>
             *     <Tjänstemeddelande>
             *         <Svarsinformation>
             *             <Svarskod>
             *             <Meddelande>
             *             <Tjänsteansvarig>
             *             <Applikationsnamn och version>
             *             <Tidpunkt för svaret>
             *         </Svarsinformation>
             *         <Anropsinformation>
             *             <Anropsansvarig>
             *             <Argument som skickades med anropet>
             *         </Anropsinformation>
             *     </Tjänstemeddelande>
             *     <FordonsPassager>
             *         <Fordonets epc>
             *         <Platsens EPC>
             *         <Tid> (för eventet)
             *         <Plats>
             *         <EVN> (European Vehicle Number)
             *         <Fordonsinnehavaren>
             *         <Underhållsansvarigt företag>
             *         <Fordonstyp> (samt underkategori)
             *         <Giltigt godkännande>
             *     </FordonsPassager>
             * </Svar>
             */

            XElement svar = new XElement("Svar",
                new XElement("Tjänstemeddelande",
                    new XElement("Svarsinformation",
                        new XElement("Svarskod", "tillfällig kod"),
                        new XElement("Meddelande", "tillfälligt meddelande"),
                        new XElement("Tjänsteansvarig", "Grymma gruppen AB"),
                        new XElement("Applikationsnamn", "Grymma appen, Ver 1.0"),
                        new XElement("Tidpunkt", DateTime.Now)),
                    new XElement("Anropsinformation",
                        new XElement("Anropsansvarig", förfrågan.Element("anropsansvarig").Value),
                        new XElement("Argument", "insert argument från föfrågan"))),
                    from evnt in events.Descendants("ObjectEvent")
                    select new XElement("FordonsPassager",
                        new XElement("FordonEpc", (string)evnt.Element("epcList").Element("epc")),
                        new XElement("PlatsEPC", (string)evnt.Element("readPoint").Element("id")),
                        new XElement("Tid", (string)evnt.Element("eventTime")),
                        new XElement("Plats", plats),
                        from vehicle in GetVehicle((string)evnt.Element("epcList").Element("epc")).Descendants("Fordonsindivider")
                        select  new XElement("FordonsData",
                            new XElement("EVN", (string)vehicle.Element("FordonsIndivid").Element("Fordonsnummer")),
                            new XElement("Fordonsinnehavaren", (string)vehicle.Element("FordonsIndivid").Element("Fordonsinnehavare").Element("Foretag")),
                            new XElement("UnderhållsansvarigtFöretag", (string)vehicle.Element("FordonsIndivid").Element("UnderhallsansvarigtForetag").Element("Foretag")),
                            new XElement("Fordonstyp", 
                                from vehic in GetVehicle((string)evnt.Element("epcList").Element("epc")).Descendants("FordonsTyp")
                                select (string)vehic.Element("FordonskategoriKodFullVardeSE")),
                            new XElement("GiltigtGodkännande", "query under Godkannande (finns massa tänkta attribut, antingen FordonsgodkannandeFullVardeSE eller intervall"))));

            return svar;
        }

        private XElement GetEvents(DateTime fromIncl, DateTime toIncl, string platsEPC)
        {
            IIServiceReference.EpcisEventServiceClient epcis = new IIServiceReference.EpcisEventServiceClient();
            XElement events = new XElement("Events", epcis.GetEvents(fromIncl, toIncl, platsEPC));
            
            epcis.Close();
            return events;

        }

        private XElement GetVehicle(string vehicleEPC)
        {
            IIServiceReference.NamingServiceClient master = new IIServiceReference.NamingServiceClient();
            XElement vehicle = master.GetVehicle(vehicleEPC);
            master.Close();
            return vehicle;

        }

        private XElement GetLocation(string vehicleEPC)
        {
            IIServiceReference.NamingServiceClient master = new IIServiceReference.NamingServiceClient();
            XElement location = master.GetLocation(vehicleEPC);
            master.Close();
            return location;
        }
    }
}