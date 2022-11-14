using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Effects; 

namespace ArticlesControl.Formatki
{
    /// <summary>
    /// Interaction logic for Add_OrderLine.xaml
    /// </summary>
    public partial class Add_OrderLine : Window
    {
        private readonly BlurEffect blur = new();
        public ResourceDictionary dict = new();
        public Dictionary<string, string> dictionary;
        private readonly Add_DataTableAdapters.Kar_Artykuly_SELECTTableAdapter Add_Kar_Artykul_ta = new();
        private readonly List<string?> list = new();
        private readonly Dictionary<string, Tuple<string, Type>> cells = new();
        //Add Constructor
        public Add_OrderLine(ResourceDictionary dict, Dictionary<string, string> dictionary)
        {
            InitializeComponent();
            this.dict = dict;
            this.dictionary = dictionary;
            DataTable Data = Add_Kar_Artykul_ta.GetData();
            list = Data.Rows.OfType<DataRow>()
    .Select(dr => dr.Field<string>("Indeks")).ToList();
            Indeks.ItemsSource = list;
        }
        //Add Suggested Constructor
        public Add_OrderLine(ResourceDictionary dict, Dictionary<string, string> dictionary, Dictionary<string, Tuple<string, Type>> cells)
        {
            InitializeComponent();
            this.dict = dict;
            this.dictionary = dictionary;
            this.cells = cells;
            DataTable Data = Add_Kar_Artykul_ta.GetData();
            list = Data.Rows.OfType<DataRow>()
    .Select(dr => dr.Field<string>("Indeks")).ToList();
            Indeks.ItemsSource = list;
            TextBox_Fill();
        }
        //Edit Constructor
        public Add_OrderLine(ResourceDictionary dict, Dictionary<string, string> dictionary, Dictionary<string, Tuple<string, Type>> cells, bool isEdit)
        {
            InitializeComponent();
            this.dict = dict;
            this.dictionary = dictionary;
            this.cells = cells;
            DataTable Data = Add_Kar_Artykul_ta.GetData();
            list = Data.Rows.OfType<DataRow>()
    .Select(dr => dr.Field<string>("Indeks")).ToList();
            Indeks.ItemsSource = list;
            TextBox_Fill();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Windows.Select_an_article _Article = new(dict, dictionary);
            //_Article.Owner = Window.GetWindow(this);
            //Opacity = 0.5;
            //Effect = blur;
            //_Article.ShowDialog();
            //Opacity = 1;
            //Effect = null;
            //Indeks.Text = _Article.IndeksofArticle;
        }

        private void TextBox_Fill()
        {
            Indeks.Text = cells["Indeks"].Item1.ToString();
            Quantity.Text = cells["Ilosc"].Item1.ToString();
            Quantity_Approved.Text = cells["IloscZatwierdzona"].Item1.ToString();
            Status.Text = cells["Status"].Item1.ToString();
            DateTime oldtime = DateTime.Now;
            if (DateTime.TryParse(cells["DataOperacji"].Item1, out oldtime)) { Operation_Date.SelectedDate = oldtime; }
            Description.Text = cells["Opis"].Item1.ToString();
            Operator.Text = cells["Operator"].Item1.ToString();
            Target_window.Text = cells["OknoDocelowe"].Item1.ToString();
            Target_operator.Text = cells["Operatordocelowy"].Item1.ToString();
            Order_Number.Text = cells["NrZlecenia"].Item1.ToString();
            Position_Number.Text = cells["NrPozycji"].Item1.ToString();
        }
    }
}
