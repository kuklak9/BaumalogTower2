using System;
using System.Windows;

namespace ArticlesControl.Klasy
{
    internal class Type
    {
        #region Fields
        private readonly BTPDataBase.Add_DataTableAdapters.Kar_Gatunki_EditTableAdapter Add_type = new();
        public int? ID_Gatunku { get; set; }
        public string? Name { get; set; }
        public string? Opis { get; set; }
        public decimal? DomyslnaGestosc { get; set; }
        public string? AutoIndeks { get; set; }
        #endregion

        #region Constructors
        public Type()
        {

        }
        public Type(int? ID_Gatunku)
        {
            this.ID_Gatunku = ID_Gatunku;
        }
        public Type(string Name, string Opis, decimal DomyslnaGestosc, string AutoIndeks)
        {
            this.Name = Name;
            this.Opis = Opis;
            this.DomyslnaGestosc = DomyslnaGestosc;
            this.AutoIndeks = AutoIndeks;
        }
        public Type(int? ID_Gatunku, string Name, string Opis, decimal DomyslnaGestosc, string AutoIndeks)
        {
            this.ID_Gatunku = ID_Gatunku;
            this.Name = Name;
            this.Opis = Opis;
            this.DomyslnaGestosc = DomyslnaGestosc;
            this.AutoIndeks = AutoIndeks;
        }
        #endregion

        #region Functions
        public void Insert()
        {
            try
            {
                Add_type.Insert(Name, Opis, DomyslnaGestosc, AutoIndeks);
            }
            catch (Exception e)
            {
                if (HandyControl.Controls.MessageBox.Show(e.Message, Name, MessageBoxButton.OK, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {

                }
            }
        }
        public void Delete()
        {
            try
            {
                Add_type.Delete(ID_Gatunku);
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
                Add_type.Update(ID_Gatunku, Name, Opis, DomyslnaGestosc, AutoIndeks);
            }
            catch (Exception e)
            {
                if (HandyControl.Controls.MessageBox.Show(e.Message, ID_Gatunku.ToString(), MessageBoxButton.OK, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                }
            }
        }
        #endregion
    }
}
