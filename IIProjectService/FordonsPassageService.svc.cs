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

            string toIncl = förfrågan.Element("tidsintervall").Element("sluttid").Value;
            DateTime to = DateTime.Parse(toIncl);

            string plats = förfrågan.Element("plats").Value;

            XElement events = GetEvents(from, to, plats);
           
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
                        new XElement("Applikationsnamn och version", "Grymma appen, Ver 1.0"),
                        new XElement("Tidpunkt för svaret", DateTime.Now)),
                    new XElement("Anropsinformation",
                        new XElement("Anropsansvarig", förfrågan.Element("anropsansvarig").Value),
                        new XElement("Argument som skickades med anropet", "insert argument från föfrågan"))),
                    from evnt in events.Elements("EventList")
                    select new XElement("FordonsPassager",
                        new XElement("Fordonets epc", evnt.Element("epc").Value),
                        new XElement("Platsens EPC", evnt.Element("id").Value),
                        new XElement("Tid", evnt.Element("eventTime").Value),
                        new XElement("Plats", förfrågan.Element("plats").Value),
                        new XElement("EVN", GetVehicle((string)evnt.Element("epc")).Element("Fordonsnummer")),
                        new XElement("Fordonsinnehavaren", GetVehicle((string)evnt.Element("epc")).Element("Fordonsinnehavare").Element("Foretag").Value),
                        new XElement("Underhållsansvarigt företag", GetVehicle((string)evnt.Element("epc")).Element("UnderhallsansvarigtForetag").Element("Foretag").Value),
                        new XElement("Fordonstyp", GetVehicle((string)evnt.Element("epc")).ElementsAfterSelf("FordonsTyp")),
                        new XElement("Giltigt godkännande", "query under Godkannande (finns massa tänkta attribut, antingen FordonsgodkannandeFullVardeSE eller intervall")));

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

    }
}