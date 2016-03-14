using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace IIProjectClient.Models
{
    public class Sokning
    {
        public static IEnumerable<string> platser = new List<string>(){
            "Hunstugan_USP", "Tväråbäck", "Pölsebo", "Lönsboda", "Sunderbyns_Sjukhus" };


        public DateTime start { get; set; }
        public DateTime slut { get; set; }
        public string plats{get;set;}
        public int antal { get; set; }
        public string anropsansvarig { get; set; }
        public XElement resultat { get; set; }

    }
}