using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Xml.Linq;
using System.Web.Hosting;

namespace IIProjectService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class FordonsPassageService : IFordonsPassageService
    {
        string appDataFolder = HostingEnvironment.MapPath("/App_Data/");

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

        private bool OkAnvändarnamn(string namn)
        {
           // string XMLFile = HostingEnvironment.MapPath(appDataFolder + "Anvandare.xml");
          //  XElement anvandarlista = XElement.Load(XMLFile);
            XElement anvandarlista = XElement.Load(appDataFolder + "Anvandare.xml");
            IEnumerable<string> matchande = from k in anvandarlista.Descendants("namn")
                                            where k.Value == namn
                                            select k.Value;

            if (matchande.Count() > 0)
            {
                //det fanns någon användare som mactahde
                return true;
            }
            else
            {
                return false;
            }
        }

        public XElement TestFunktion()
        {
            XElement testXml = new XElement("Test", "Test");
            return testXml;
        }

        public XElement HämtaFordonsPassager(XElement förfrågan)
        {
           XElement svar= new XElement("Svar",
                new XElement("Tjänstemeddelande",
                    new XElement("Svarsinformation",
                        new XElement("Svarskod", "1"),
                        new XElement("Meddelande", "Allt gick bra"),
                        new XElement("Tjänsteansvarig", "Grymma gruppen AB"),
                        new XElement("Applikationsnamn", "Grymma appen, Ver 1.0"),
                        new XElement("Tidpunkt", DateTime.Now)),
                    new XElement("Anropsinformation",
                        new XElement("Anropsansvarig", förfrågan.Element("anropsansvarig").Value),
                        new XElement("Argument", förfrågan))));
 
            //kolla om inlogg är ok med metoden OkAnvändarnamn
            if(!OkAnvändarnamn(förfrågan.Element("anropsansvarig").Value)){
                XElement svarTwo = new XElement("Svar",
                    new XElement("Tjänstemeddelande",
                        new XElement("Svarsinformation",
                            new XElement("Svarskod", "2"),
                            new XElement("Meddelande", "Ogiltig användare"),
                            new XElement("Tjänsteansvarig", "Grymma gruppen AB"),
                            new XElement("Applikationsnamn", "Grymma appen, Ver 1.0"),
                            new XElement("Tidpunkt", DateTime.Now)),
                            new XElement("Anropsinformation",
                                new XElement("Anropsansvarig", förfrågan.Element("anropsansvarig").Value),
                                new XElement("Argument", förfrågan))));
                    return svarTwo;
            }

            try
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
                     *      <Fordonspassage>
                     *         <Fordonets epc>
                     *         <Platsens EPC>
                     *         <Tid> (för eventet)
                     *         <Plats>
                     *         <EVN> (European Vehicle Number)
                     *         <Fordonsinnehavaren>
                     *         <Underhållsansvarigt företag>
                     *         <Fordonstyp> (samt underkategori)
                     *         <Giltigt godkännande>
                 *         <Fordonspassage>
                 *         <Fordonspassage>
                     *         <Fordonets epc>
                     *         <Platsens EPC>
                     *         <Tid> (för eventet)
                     *         <Plats>
                     *         <EVN> (European Vehicle Number)
                     *         <Fordonsinnehavaren>
                     *         <Underhållsansvarigt företag>
                     *         <Fordonstyp> (samt underkategori)
                     *         <Giltigt godkännande>
                 *         <Fordonspassage>
                 *        
                 *     </FordonsPassager>
                 * </Svar>
                 */
               // IEnumerable<string> godkännanden;
                int antal = (int)förfrågan.Descendants("antal").FirstOrDefault();
                int senastTagna = (int)förfrågan.Descendants("senastTagna").FirstOrDefault();
                svar.Add(from evnt in events.Descendants("ObjectEvent").OrderBy(E=>(DateTime)E.Element("eventTime")).Skip(senastTagna).Take(antal)
                         //orderby (DateTime)evnt.Element("eventTime")
                         select new XElement("FordonsPassager",
                             new XElement("FordonEpc", (string)evnt.Element("epcList").Element("epc")),
                             new XElement("PlatsEPC", (string)evnt.Element("readPoint").Element("id")),
                             new XElement("Datum", GetDate((string)evnt.Element("eventTime"))),
                             new XElement("Tid", GetTime((string)evnt.Element("eventTime"))),
                             new XElement("Plats", plats),
                             from vehicle in GetVehicle((string)evnt.Element("epcList").Element("epc")).Descendants("Fordonsindivider")
                             select new XElement("FordonsData",
                                 new XElement("EVN", (string)vehicle.Element("FordonsIndivid").Element("Fordonsnummer")),
                                 new XElement("Fordonsinnehavaren", (string)vehicle.Descendants("Fordonsinnehavare").FirstOrDefault().Element("Foretag")),
                                 new XElement("UnderhållsansvarigtFöretag", (string)vehicle.Descendants("UnderhallsansvarigtForetag").FirstOrDefault().Element("Foretag")),
                                 new XElement("Fordonstyp",(string) vehicle.Descendants("FordonskategoriKodFullVardeSE").FirstOrDefault()),
                                 new XElement("Godkännande",
                                     new XElement("Godkänd", (string)vehicle.Descendants("FordonsgodkannandeFullVardeSE").FirstOrDefault(),
                                     new XElement("GodkändFrån", (string)vehicle.Descendants("GiltigtFrom").FirstOrDefault()),
                                     new XElement("GodkändTill", (string)vehicle.Descendants("GiltigtTom").FirstOrDefault()))))));

                //godkännanden = from godkännande  in svar.Descendants("Godkänd")
                //               select (string)godkännande;

                //godkännanden = godkännanden.Distinct();

                return svar;
            }
            catch
            {
                XElement svarThree = new XElement("Svar",
                    new XElement("Tjänstemeddelande",
                        new XElement("Svarsinformation",
                            new XElement("Svarskod", "3"),
                            new XElement("Meddelande", "Internt fel"),
                            new XElement("Tjänsteansvarig", "Grymma gruppen AB"),
                            new XElement("Applikationsnamn", "Grymma appen, Ver 1.0"),
                            new XElement("Tidpunkt", DateTime.Now)),
                            new XElement("Anropsinformation",
                                new XElement("Anropsansvarig", förfrågan.Element("anropsansvarig").Value),
                                new XElement("Argument", förfrågan))));
                return svarThree;
            }
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
            if (!vehicle.Descendants("ResponseMessage").Any()) 
            {
                return vehicle;
            }
            else
            {
                XElement tempElement = XElement.Load(appDataFolder + "FelFordonData.xml");
                return tempElement;
            }

        }

        private XElement GetLocation(string placeEPC)
        {
            IIServiceReference.NamingServiceClient master = new IIServiceReference.NamingServiceClient();
            XElement location = master.GetLocation(placeEPC);
            master.Close();
            return location;
        }

        private string GetDate(string dateTime)
        {
            return dateTime.Substring(0, 10);
        }

        private string GetTime(string dateTime)
        {
            return dateTime.Substring(11, 5);
        }
    }
}
