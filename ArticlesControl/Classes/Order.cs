using System;
using System.Windows;

namespace ArticlesControl
{
    public class Order
    {
        #region Fields

        public string? NrZlecenia { get; set; }

        public DateTime? DataZlecenia { get; set; }

        public string? NrZewnetrzny { get; set; }

        public string? Klient { get; set; }

        public string? Opis { get; set; }

        public int? Status { get; set; }

        public int? Priorytet { get; set; }

        public DateTime? DataImportu { get; set; }

        public int? ZrodloZlecenia { get; set; }

        public string? Typ { get; set; }

        public Order(string? nrZlecenia, DateTime? dataZlecenia, string? nrZewnetrzny, string? klient, string? opis, int? status, int? priorytet, DateTime? dataImportu, int? zrodloZlecenia, string? typ)
        {
            NrZlecenia = nrZlecenia;
            DataZlecenia = dataZlecenia;
            NrZewnetrzny = nrZewnetrzny;
            Klient = klient;
            Opis = opis;
            Status = status;
            Priorytet = priorytet;
            DataImportu = dataImportu;
            ZrodloZlecenia = zrodloZlecenia;
            Typ = typ;
        }

        #endregion
        public void Insert()
        {

        }

        public void Delete()
        {

        }

        public void Update()
        {

        }

    }
}
