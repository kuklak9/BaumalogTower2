using System;
using System.Windows;

namespace ArticlesControl
{
    public class OrderLine
    {
        #region Fields

        public string? NrZlecenia { get; set; }
        public int? NrPozycji { get; set; }
        public string? Indeks { get; set; }
        public float? Ilosc { get; set; }
        public float? IloscZatwierdzona { get; set;}
        public DateTime? DataOperacji { get; set; }
        public string? Operator { get; set; }
        public string? Operatordocelowy { get; set; }
        public int? OknoDocelowe { get; set; }
        public int? Status { get; set; }
        public string? Opis { get; set; }
        public int? PR { get; set; }
        #endregion

        public OrderLine(string? nrZlecenia, int? nrPozycji, string? indeks, float? ilosc, float? iloscZatwierdzona, DateTime? dataOperacji, string? @operator, string? operatordocelowy, int? oknoDocelowe, int? status, string? opis, int? pR)
        {
            NrZlecenia = nrZlecenia;
            NrPozycji = nrPozycji;
            Indeks = indeks;
            Ilosc = ilosc;
            IloscZatwierdzona = iloscZatwierdzona;
            DataOperacji = dataOperacji;
            Operator = @operator;
            Operatordocelowy = operatordocelowy;
            OknoDocelowe = oknoDocelowe;
            Status = status;
            Opis = opis;
            PR = pR;
        }

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
