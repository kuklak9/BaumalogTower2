using System;
using System.Windows;

namespace ArticlesControl
{
    public class Article
    {
        #region Fields
        private BTPDataBase.DB_WMS_3 DataBase;
        public delegate void IsOK();
        public event IsOK IsOkEvent;
        public bool IsOk = false;
        public string? Indeks { get; set; }
        public string? Nazwa { get; set; }
        public string? Opis { get; set; }
        public string? Atr1 { get; set; }
        public string? Atr2 { get; set; }
        public string? Atr3 { get; set; }
        public string? KlasaRotacji { get; set; }
        public int? ID_Jednostki { get; set; }
        public int? ID_Kategorii { get; set; }
        public int? ID_Gatunku { get; set; }
        public int? ID_KlasySkladowania { get; set; }
        public decimal? Wymiar1 { get; set; }
        public decimal? Wymiar2 { get; set; }
        public decimal? Wymiar3 { get; set; }
        public decimal? Wymiar4 { get; set; }
        public decimal? Wymiar5 { get; set; }
        public decimal? Wymiar6 { get; set; }
        public decimal? Wymiar7 { get; set; }
        public decimal? Wymiar8 { get; set; }
        public decimal? WagaJednostkowa { get; set; }
        public decimal? Stan { get; set; }
        public decimal? Rezerwacja { get; set; }
        public decimal? StanMinimalny { get; set; }
        #endregion

        #region Constructors
        public Article(BTPDataBase.DB_WMS_3 DataBase, string Indeks, string Nazwa, string Opis, string Atr1, string Atr2, string Atr3, string KlasaRotacji, int ID_Jednostki, int ID_Kategorii, int ID_Gatunku,
            int ID_KlasySkladowania, decimal Wymiar1, decimal Wymiar2, decimal Wymiar3, decimal Wymiar4, decimal Wymiar5, decimal Wymiar6, decimal Wymiar7, decimal Wymiar8,
            decimal WagaJednostkowa, decimal Stan, decimal Rezerwacja, decimal StanMinimalny)
        {
            IsOkEvent += Article_IsOkEvent;

            this.Indeks = Indeks;
            this.Nazwa = Nazwa;
            this.Opis = Opis;
            this.Atr1 = Atr1;
            this.Atr2 = Atr2;
            this.Atr3 = Atr3;
            this.KlasaRotacji = KlasaRotacji;
            this.ID_Jednostki = ID_Jednostki;
            this.ID_Kategorii = ID_Kategorii;
            this.ID_Gatunku = ID_Gatunku;
            this.ID_KlasySkladowania = ID_KlasySkladowania;
            this.Wymiar1 = Wymiar1;
            this.Wymiar2 = Wymiar2;
            this.Wymiar3 = Wymiar3;
            this.Wymiar4 = Wymiar4;
            this.Wymiar5 = Wymiar5;
            this.Wymiar6 = Wymiar6;
            this.Wymiar7 = Wymiar7;
            this.Wymiar8 = Wymiar8;
            this.WagaJednostkowa = WagaJednostkowa;
            this.Stan = Stan;
            this.Rezerwacja = Rezerwacja;
            this.StanMinimalny = StanMinimalny;
        }
        public Article(BTPDataBase.DB_WMS_3 DataBase)
        {
            IsOkEvent += Article_IsOkEvent;
            this.DataBase = DataBase;
        }
        public Article(BTPDataBase.DB_WMS_3 DataBase, string Indeks)
        {
            IsOkEvent += Article_IsOkEvent;
            this.DataBase = DataBase;
            this.Indeks = Indeks;
        }
        #endregion

        #region Functions
        public void Insert()
        {
            try
            {
                //Add_Kar_Artykul_ta.Update()
                //Add_Kar_Artykul_ta.Insert(Indeks, Nazwa, Opis, Atr1, Atr2, Atr3, KlasaRotacji,
                DataBase.Edit_Kar_Artykul_ta.Insert(Indeks, Nazwa, Opis, Atr1, Atr2, Atr3, KlasaRotacji,
                ID_Jednostki, ID_Kategorii, ID_Gatunku, ID_KlasySkladowania, Wymiar1, Wymiar2,
                Wymiar3, Wymiar4, Wymiar5, Wymiar6, Wymiar7, Wymiar8, WagaJednostkowa, Stan, Rezerwacja, StanMinimalny);
            }
            catch (Exception e)
            {
                if (HandyControl.Controls.MessageBox.Show(e.Message, Indeks, MessageBoxButton.OK, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    IsOkEvent();
                }
            }
        }

        public void Delete()
        {
            try
            {
                DataBase.Edit_Kar_Artykul_ta.Delete(Indeks);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void Update()
        {
            try
            {
                DataBase.Edit_Kar_Artykul_ta.Update(Indeks, Nazwa, Opis, Atr1, Atr2, Atr3, KlasaRotacji,
                ID_Jednostki, ID_Kategorii, ID_Gatunku, ID_KlasySkladowania, Wymiar1, Wymiar2,
                Wymiar3, Wymiar4, Wymiar5, Wymiar6, Wymiar7, Wymiar8, WagaJednostkowa, Stan, Rezerwacja, StanMinimalny);
            }
            catch (Exception e)
            {
                if (HandyControl.Controls.MessageBox.Show(e.Message, Indeks, MessageBoxButton.OK, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    IsOkEvent();
                }
            }
        }
        private void Article_IsOkEvent()
        {
            IsOk = true;
        }

        #endregion
    }
}
