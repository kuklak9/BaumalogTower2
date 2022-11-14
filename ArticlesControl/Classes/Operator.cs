using System;
using System.Windows;

namespace ArticlesControl.Klasy
{
    internal class Operator
    {
        #region Fields
        BTPDataBase.DB_WMS_3 DataBase;
        private string? Login { get; set; }
        private string? Haslo { get; set; }
        private string? Imie { get; set; }
        private string? Nazwisko { get; set; }
        private string? Cecha1 { get; set; }
        private int? ID_Grupy { get; set; }
        #endregion
        public Operator(BTPDataBase.DB_WMS_3 DataBase, string? Login, string? Haslo, string? Imie, string? Nazwisko, string? Cecha1, int? ID_Grupy)
        {
            this.DataBase = DataBase;
            this.Login = Login;
            this.Haslo = Haslo;
            this.Imie = Imie;
            this.Nazwisko = Nazwisko;
            this.Cecha1 = Cecha1;
            this.ID_Grupy = ID_Grupy;

        }

        public void Insert()
        {
            try
            {
                _ = DataBase.Add_Operator_Select.Insert(Login, Imie, Nazwisko, ID_Grupy, Cecha1, null, null, Haslo);
            }
            catch (Exception e)
            {
                if (HandyControl.Controls.MessageBox.Show(e.Message, e.Message, MessageBoxButton.OK, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {

                }
            }
        }
        public void Delete()
        {
            try
            {
                _ = DataBase.Add_Operator_Select.Delete(Login);
            }
            catch (Exception e)
            {
                if (HandyControl.Controls.MessageBox.Show(e.Message, e.Message, MessageBoxButton.OK, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {

                }
            }
        }
        public void Update()
        {
            try
            {
                _ = DataBase.Add_Operator_Select.Update(Login, Imie, Nazwisko, ID_Grupy, Cecha1, null, null, Haslo);
            }
            catch (Exception e)
            {
                if (HandyControl.Controls.MessageBox.Show(e.Message, e.Message, MessageBoxButton.OK, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {

                }
            }
        }

    }
}
