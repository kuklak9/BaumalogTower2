using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Media.Effects;
using BTPDataBase;

namespace ArticlesControl.Windows
{
    /// <summary>
    /// Logika interakcji dla klasy Select_an_article.xaml
    /// </summary>
    public partial class Select_an_article : BTPControlLibrary.MainWindow
    {
        private readonly BlurEffect blur = new();

        public ResourceDictionary dict = new();
        public Dictionary<string, string> dictionary;
        public string IndeksofArticle = string.Empty;
        public Select_an_article(ResourceDictionary resourceDictionary, Dictionary<string, string> lang)
        {
            InitializeComponent();
            Articles_Datagrid.Show_Menu();
            dictionary = lang;
            dict = resourceDictionary;
            Resources.MergedDictionaries.Add(dict);
            Articles_Datagrid.OpenAddWindowEvent += OpenAddWindow;
            Articles_Datagrid.OpenSuggestedAddWindowEvent += OpenSuggestedAddWindow;
            Articles_Datagrid.OpenEditWindowEvent += OpenEditWindow;
            Articles_Datagrid.OpenDeleteWindowEvent += OpenDeleteWindow;
            Articles_Datagrid.OnDefaulsData += DatagridInsertDataTableArticle;

            Add_Data.Kar_Artykuly_SELECTDataTable? Data = _dbWMS.Add_Kar_Artykul_ta.GetData();
            Articles_Datagrid.additionaldatatable = Data;
            DatagridInsertDataTableArticle();
        }

        #region Formats for Articles
        private void DatagridInsertDataTableArticle()
        {
            Add_Data.Kar_Artykuly_SELECTDataTable? Data = _dbWMS.Add_Kar_Artykul_ta.GetData();
            List<string> list = new();
            DataTable dt = Data;
            foreach (DataColumn item in dt.Columns)
            {
                list.Add(item.ColumnName.ToString());
            }
            Articles_Datagrid.columnnames = list;
            Articles_Datagrid.Maindatatable = Data;
            Articles_Datagrid.isArticle = true;
            Articles_Datagrid.isType = false;
            Articles_Datagrid.isStock = false;
            Articles_Datagrid.Load();
            Articles_Datagrid.CallHeaders();
            Articles_Datagrid.ReadSettings();
            Articles_Datagrid.SetButton();
            Articles_Datagrid.HideGetGiveButtonForSelect_an_article();
        }
        //Function opening window for article adding
        public void OpenAddWindow(int count)
        {
            Add_Article add_Article = new(dict, dictionary, count);
            add_Article.Owner = Window.GetWindow(this);
            Opacity = 0.5;
            Effect = blur;
            add_Article.ShowDialog();
            Opacity = 1;
            Effect = null;
            DatagridInsertDataTableArticle();
            Articles_Datagrid.Refreash();
        }
        private void OpenEditWindow(Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns)
        {
            Add_Article add_Article = new(dict, dictionary, cells, custom_columns);
            add_Article.Owner = Window.GetWindow(this);
            Opacity = 0.5;
            Effect = blur;
            add_Article.ShowDialog();
            Opacity = 1;
            Effect = null;
            DatagridInsertDataTableArticle();
            Articles_Datagrid.Refreash();
        }
        private void OpenDeleteWindow(string Indeks)
        {
            if (HandyControl.Controls.MessageBox.Show(dictionary["DeleteRowQuestion"], Indeks, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                Article article = new(_dbWMS, Indeks);
                article.Delete();
                DatagridInsertDataTableArticle();
            }
            else
            {
            }
        }
        public void OpenSuggestedAddWindow(int count, Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns)
        {
            Add_Article add_Article = new(dict, dictionary, cells, true, custom_columns);
            add_Article.Owner = Window.GetWindow(this);
            Opacity = 0.5;
            Effect = blur;
            add_Article.ShowDialog();
            Opacity = 1;
            Effect = null;
            DatagridInsertDataTableArticle();
            Articles_Datagrid.Refreash();
        }
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IndeksofArticle = Articles_Datagrid.Selectedrow[0].ToString() ?? "";
                Close();
            }
            catch { }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            IndeksofArticle = null;
            Close();
        }
    }
}
