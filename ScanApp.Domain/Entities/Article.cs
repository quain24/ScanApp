using System;

namespace ScanApp.Domain.Entities
{
    public class Article
    {
        public string artNr { get; set; }
        public string supplierName { get; set; }
        public string FactoryNr { get; set; }
        public string ean { get; set; }
        public string artDesc1 { get; set; }
        public int amount { get; set; }
        public string lockreason { get; set; }
        public int lhmref { get; set; }
        public string charge { get; set; }
        public string zustand { get; set; }
        public DateTime einlagerung { get; set; }
        public string Lager { get; set; }
        public string Lagerort { get; set; }
        public string Lagerplatz { get; set; }
    }
}