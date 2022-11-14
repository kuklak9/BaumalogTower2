using System;
using System.Windows;
using System.Data;
namespace ArticlesControl.Klasy
{
    internal class Category
    {
        #region Fields
        Add_DataTableAdapters.Kar_Kategorie_SELECTTableAdapter tableAdapter = new Add_DataTableAdapters.Kar_Kategorie_SELECTTableAdapter();
        public int? ID_Kategorii { get; set; }
        public string? Nazwa { get; set; }
        public string? Opis { get; set; }
        public string? DomyslnyPrzelicznikWagi { get; set; }
        public string? PoziomTree { get; set; }
        public string? MaskaIndeksu { get; set; }
        public DataTable? KategorieTree { get; set; }
        #endregion
        public Category(int? ID_Kategorii)
        {
            this.ID_Kategorii = ID_Kategorii;
        }
        public Category(string Nazwa, string Opis, string DomyslnyPrzelicznikWagi, string PoziomTree, string MaskaIndeksu)
        {
            this.Nazwa = Nazwa;
            this.Opis = Opis;
            this.DomyslnyPrzelicznikWagi = DomyslnyPrzelicznikWagi;
            this.PoziomTree = PoziomTree;
            this.MaskaIndeksu = MaskaIndeksu;
        }
        public Category(int? ID_Kategorii, string Nazwa, string Opis, string DomyslnyPrzelicznikWagi, string MaskaIndeksu, DataTable KategorieTree)
        {
            this.ID_Kategorii = ID_Kategorii;
            this.Nazwa = Nazwa;
            this.Opis = Opis;
            this.DomyslnyPrzelicznikWagi = DomyslnyPrzelicznikWagi;
            this.MaskaIndeksu = MaskaIndeksu;
            this.KategorieTree = KategorieTree;
        }
        public void Insert()
        {
            try
            {
                tableAdapter.Insert(Nazwa, Opis, MaskaIndeksu, DomyslnyPrzelicznikWagi, PoziomTree);
            }
            catch (System.Exception e)
            {
                if (HandyControl.Controls.MessageBox.Show(e.Message, Nazwa, MessageBoxButton.OK, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {

                }
            }
        }

        public void Delete()
        {
            try
            {
                tableAdapter.Delete(ID_Kategorii);
            }
            catch (System.Exception e)
            {
                if (HandyControl.Controls.MessageBox.Show(e.Message, Nazwa, MessageBoxButton.OK, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {

                }
            }
        }

        public void Update()
        {
            try
            {
                tableAdapter.Update(ID_Kategorii, Nazwa, Opis, MaskaIndeksu, DomyslnyPrzelicznikWagi, KategorieTree);
            }
            catch (System.Exception e)
            {
                if (HandyControl.Controls.MessageBox.Show(e.Message, Nazwa, MessageBoxButton.OK, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {

                }
            }
        }
    }
}
