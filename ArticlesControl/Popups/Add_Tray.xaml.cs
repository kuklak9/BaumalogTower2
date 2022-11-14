using ArticlesControl.Formatki;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace ArticlesControl
{
    /// <summary>
    /// Logika interakcji dla klasy Add_Tray.xaml
    /// </summary>
    public partial class Add_Tray : MainWindow
    {
        #region Fields
        private readonly BlurEffect blur = new();
        public ResourceDictionary dict = new();
        public Dictionary<string, string> dictionary;
        private readonly Add_DataTableAdapters.KategoriaTree_SELECTTableAdapter kategoriaTree_SELECT = new();
        private readonly Add_DataTableAdapters.Kar_Polki_EditTableAdapter polki_EditTableAdapter = new();
        public DataTable KategorieTree { get; set; } = new DataTable();
        private bool isEdit { get; set; }
        private readonly Dictionary<string, string> custom_columns;
        private readonly Dictionary<string, Tuple<string, Type>> cells = new();

        //private readonly DataTable Typ = Connection("Def_PolkaTyp");
        //private readonly DataTable Status = Connection("Def_PolkiStatus");
        //private readonly DataTable Kategorie = Connection("Kar_Kategorie");
        //private readonly DataTable Regal = Connection("Kar_Regaly");
        //private readonly DataTable Przeznaczenie = Connection("Def_PolkiPrzeznaczenie");

        DataRow DataToEdit;
        #endregion

        //Constructor for add new row
        public Add_Tray(ResourceDictionary resourceDictionary, Dictionary<string, string> lang)
        {
            //InitializeComponent();
            //dict = resourceDictionary;
            //dictionary = lang;
            //Load_Category_Tree(isEdit);
            //Load_Combos();
        }
        //Constructor for edit new row
        public Add_Tray(ResourceDictionary resourceDictionary, Dictionary<string, string> lang, Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns)
        {
            //InitializeComponent();
            //isEdit = true;
            //DataToEdit = (DataRow)(polki_EditTableAdapter.GetData().Select(String.Format("ID_Polki = " + cells["ID_Polki"].Item1.ToString())))[0];
            //this.cells = cells;
            //this.custom_columns = custom_columns;
            //dict = resourceDictionary;
            //dictionary = lang;
            //Load_Category_Tree(isEdit);
            //Load_Combos(isEdit);
            //TrayNumber.Text = DataToEdit["NrPolki"].ToString();
            //TrayNumber.IsEnabled = false;
            //Rack.IsEnabled = false;
        }

        #region Category_tree
        //private void BuildDataTable()
        //{
        //    DataColumn col1 = new("ID");
        //    DataColumn col2 = new("Poziomtree");
        //    KategorieTree.Columns.Add("ID");
        //    KategorieTree.Columns.Add("Poziomtree");
        //    DataRow row;
        //    List<TreeViewItem> list = FindTreeViewItems(Category_Tree);
        //    foreach (TreeViewItem item in list)
        //    {
        //        _ = int.TryParse(item.Uid, out int temp);
        //        row = KategorieTree.NewRow();
        //        row[0] = temp;
        //        row[1] = item.DisplayMemberPath + item.Tag;
        //        KategorieTree.Rows.Add(row);
        //    }
        //}
        //public List<TreeViewItem> FindTreeViewItems(Visual @this)
        //{
        //    if (@this == null)
        //        return null;

        //    var result = new List<TreeViewItem>();

        //    FrameworkElement? frameworkElement = @this as FrameworkElement;
        //    if (frameworkElement is not null)
        //    {
        //        frameworkElement.ApplyTemplate();
        //    }

        //    Visual? child = null;
        //    for (int i = 0, count = VisualTreeHelper.GetChildrenCount(@this); i < count; i++)
        //    {
        //        child = VisualTreeHelper.GetChild(@this, i) as Visual;

        //        var treeViewItem = child as TreeViewItem;
        //        if (treeViewItem is not null)
        //        {
        //            result.Add(treeViewItem);
        //            if (!treeViewItem.IsExpanded)
        //            {
        //                treeViewItem.IsExpanded = true;
        //                treeViewItem.UpdateLayout();
        //            }
        //        }
        //        foreach (var childTreeViewItem in FindTreeViewItems(@this: child ?? this))
        //        {
        //            result.Add(childTreeViewItem);
        //        }
        //    }
        //    return result;
        //}
        //private string BuildPoziomString()
        //{
        //    TreeViewItem item = (TreeViewItem)Category_Tree.SelectedItem;
        //    if (item == null)
        //    {
        //        ColorAnimation flash = new(Colors.White, Colors.Red, new Duration(TimeSpan.FromMilliseconds(75D))) { AutoReverse = true, RepeatBehavior = new RepeatBehavior(3D) };
        //        Category_Tree.Background = new SolidColorBrush(Colors.White); // no error???
        //        Category_Tree.BeginAnimation(SolidColorBrush.ColorProperty, flash);
        //    }
        //    else return item.DisplayMemberPath + item.Tag;
        //    return string.Empty;
        //}
        //private void Load_Category_Tree(bool isEdit)
        //{
        //    Category_Tree.Items.Clear();
        //    Add_Data DS = new();
        //    kategoriaTree_SELECT.Fill(DS.KategoriaTree_SELECT);

        //    List<Item> Items = new();
        //    foreach (Add_Data.KategoriaTree_SELECTRow poziomtree in DS.KategoriaTree_SELECT.Rows)
        //    {
        //        Items.Add(new Item(poziomtree.Nazwa, poziomtree.PoziomTree.ToString()[^3..], poziomtree.PoziomTree.ToString()[0..^3], poziomtree.ID_Kategorii.ToString()));
        //    }
        //    List<TreeViewItem> list = treeViewItems(Items);
        //    BuildCategoryTree(list, isEdit);
        //}
        //private void BuildCategoryTree(List<TreeViewItem> items, bool isEdit)
        //{
        //    TreeViewItem root = new();
        //    root.Header = dictionary["All"];
        //    root.Tag = "000";
        //    root.DisplayMemberPath = "";
        //    root.Background = (SolidColorBrush?)new BrushConverter().ConvertFrom(Application.Current.Resources["App_Background"].ToString() ?? string.Empty);
        //    root.IsExpanded = true;
        //    root.IsSelected = true;
        //    items.Add(root);
        //    foreach (TreeViewItem item in items)
        //    {
        //        for (int i = 0; i < items.Count; i++)
        //        {
        //            if (item.DisplayMemberPath == items[i].DisplayMemberPath + items[i].Tag)
        //            {
        //                items[i].Items.Add(item);
        //                if (isEdit)
        //                {
        //                    if (item.Uid.ToString() == DataToEdit["ID_Kategorii"].ToString())
        //                    {
        //                        item.IsSelected = true;
        //                    }
        //                }

        //            }
        //        }
        //    }
        //    Category_Tree.Items.Add(root);
        //}
        //private List<TreeViewItem> treeViewItems(List<Item> Children)
        //{
        //    List<TreeViewItem> treeViewItems = new();
        //    foreach (Item? item in Children)
        //    {
        //        TreeViewItem treeViewItem = new()
        //        {
        //            Header = item.Nazwa,
        //            Tag = item.Id,
        //            DisplayMemberPath = item.ParentId,
        //            Uid = item.ID_Kategorii
        //        };
        //        treeViewItems.Add(treeViewItem);
        //        treeViewItem.IsExpanded = true;
        //        treeViewItem.Background = (SolidColorBrush?)new BrushConverter().ConvertFrom(Application.Current.Resources["App_Background"].ToString() ?? string.Empty);
        //    }
        //    return treeViewItems;
        //}
        //private class Item
        //{
        //    public readonly string Nazwa;
        //    public readonly string Id;
        //    public readonly string? ParentId;
        //    public readonly string? ID_Kategorii;
        //    public Item(string Nazwa, string id, string? parent, string? ID_Kategorii)
        //    {
        //        this.Nazwa = Nazwa;
        //        Id = id;
        //        ParentId = parent;
        //        this.ID_Kategorii = ID_Kategorii;
        //    }
        //    public readonly List<Item> Children = new();

        //}
        //private void Category_Tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        //{
        //    TreeViewItem selected = (TreeViewItem)Category_Tree.SelectedItem;
        //    Category.Text = selected.Header.ToString();
        //}
        #endregion

        //     private void Load_Combos()
        //     {
        //         foreach (DataRow item in Regal.Rows)
        //         {
        //             Rack.Items.Add(item["Nazwa"].ToString());
        //         }
        //         foreach (DataRow item in Typ.Rows)
        //         {
        //             Type.Items.Add(item["Nazwa"].ToString());
        //         }
        //         foreach (DataRow item in Przeznaczenie.Rows)
        //         {
        //             Assignment.Items.Add(item["Nazwa"].ToString());
        //         }
        //         foreach (DataRow item in Status.Rows)
        //         {
        //             status.Items.Add(item["Nazwa"].ToString());
        //         }
        //     }
        //     private void Load_Combos(bool isEdit)
        //     {
        //         foreach (DataRow item in Regal.Rows)
        //         {
        //             Rack.Items.Add(item["Nazwa"].ToString());
        //             if (item["ID_Regalu"].ToString() == DataToEdit["ID_Regalu"].ToString())
        //             {
        //                 Rack.SelectedValue = item["Nazwa"].ToString();
        //             }
        //         }
        //         foreach (DataRow item in Typ.Rows)
        //         {
        //             Type.Items.Add(item["Nazwa"].ToString());
        //             if (item["Id_Typu"].ToString() == DataToEdit["ID_Typu"].ToString())
        //             {
        //                 Type.SelectedValue = item["Nazwa"].ToString();
        //             }
        //         }
        //         foreach (DataRow item in Przeznaczenie.Rows)
        //         {
        //             Assignment.Items.Add(item["Nazwa"].ToString());
        //             if (item["ID_Przeznaczenia"].ToString() == DataToEdit["ID_Przeznaczenia"].ToString())
        //             {
        //                 Assignment.SelectedValue = item["Nazwa"].ToString();
        //             }
        //         }
        //         foreach (DataRow item in Status.Rows)
        //         {
        //             status.Items.Add(item["Nazwa"].ToString());
        //             if (item["ID_Status"].ToString() == DataToEdit["Status"].ToString())
        //             {
        //                 status.SelectedValue = item["Nazwa"].ToString();
        //             }
        //         }
        //     }
        //     public static DataTable Connection(string db)
        //     {
        //         string connectionString = "Server=192.168.0.200\\testinstance;Database=SmartWMS;User ID=wms;Password=1";
        //         SqlConnection con = new(connectionString);
        //         //SQL command
        //         SqlCommand cmd = new(string.Format("select * from dbo.{0};", db), con);
        //         con.Open();
        //         SqlDataAdapter adapter = new(cmd);
        //         DataTable dt = new();
        //         adapter.Fill(dt);
        //         cmd.Dispose();
        //         con.Close();
        //         return dt;
        //     }
        private void TextBox_TextChanged(object sender, KeyEventArgs e)
        {
            HandyControl.Controls.TextBox? textBox = sender as HandyControl.Controls.TextBox ?? new();
            string name = textBox.Name;
            TextBox foundTextBox =
                FindChild<HandyControl.Controls.TextBox>(this, name);
            List<Key> numberlist = new()
                 {
                     Key.NumPad0,
                     Key.NumPad1,
                     Key.NumPad2,
                     Key.NumPad3,
                     Key.NumPad4,
                     Key.NumPad5,
                     Key.NumPad6,
                     Key.NumPad7,
                     Key.NumPad8,
                     Key.NumPad9,
                     Key.D0,
                     Key.D1,
                     Key.D2,
                     Key.D3,
                     Key.D4,
                     Key.D5,
                     Key.D6,
                     Key.D7,
                     Key.D8,
                     Key.D9,
                     Key.Back,
                     Key.Enter,
                     Key.Escape,
                     Key.Left,
                     Key.Right,
                     Key.Tab,
                     Key.OemComma
                 };
            if (textBox != null)
            {
                if (textBox.Tag.ToString() == "Only_Numbers")
                {
                    textBox.BorderThickness = new Thickness(0, 0, 0, 0);
                    if (!numberlist.Contains(e.Key) || (textBox.Text.Contains('.') && e.Key == Key.OemPeriod) || (e.Key == Key.OemPeriod && textBox.Text.Length == 0) || (textBox.Text.Contains(',') && e.Key == Key.OemComma) || (textBox.Text.Contains(',') && e.Key == Key.OemPeriod))
                    {
                        e.Handled = true;

                        ColorAnimation flash = new(Colors.White, Colors.Red, new Duration(TimeSpan.FromMilliseconds(75D))) { AutoReverse = true, RepeatBehavior = new RepeatBehavior(3D) };
                        textBox.Background = new SolidColorBrush(Colors.White); // no error???
                        textBox.Background.BeginAnimation(SolidColorBrush.ColorProperty, flash);
                        HandyControl.Controls.Growl.Warning(dictionary["Integer_validation"]);
                    }
                }
            }
        }
        public static T FindChild<T>(DependencyObject parent, string childName)
   where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null)
            {
                return null;
            }

            T? foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject? child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T? childType = child as T;
                if (childType is null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild is not null)
                    {
                        break;
                    }
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    FrameworkElement? frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement is not null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }
        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            //if (isEdit)
            //{
            //    Update_Tray();
            //    this.Close();
            //}
            //else
            //{
            //    Insert_Tray();
            //    this.Close();
            //}
            //void Insert_Tray()
            //{
            //    _ = int.TryParse(TrayNumber.Text, out int nrpolki);

            //    int id_regalu = 0;
            //    if (Rack.SelectedValue is not null)
            //    {
            //        for (int i = 0; i < Regal.Rows.Count; i++)
            //        {
            //            if (Rack.SelectedValue.ToString() == Regal.Rows[i]["Nazwa"].ToString())
            //            {
            //                id_regalu = Convert.ToInt32(Regal.Rows[i]["ID_Regalu"].ToString());
            //                break;
            //            }
            //        }
            //    }
            //    int id_typu = 0;
            //    if (Type.SelectedValue is not null)
            //    {
            //        for (int i = 0; i < Typ.Rows.Count; i++)
            //        {
            //            if (Type.SelectedValue.ToString() == Typ.Rows[i]["Nazwa"].ToString())
            //            {
            //                id_typu = Convert.ToInt32(Typ.Rows[i]["ID_Typu"].ToString());
            //                break;
            //            }
            //        }
            //    }
            //    int przeznaczenie = 0;
            //    if (Assignment.SelectedValue is not null)
            //    {
            //        for (int i = 0; i < Przeznaczenie.Rows.Count; i++)
            //        {
            //            if (Assignment.SelectedValue.ToString() == Przeznaczenie.Rows[i]["Nazwa"].ToString())
            //            {
            //                przeznaczenie = Convert.ToInt32(Przeznaczenie.Rows[i]["ID_Przeznaczenia"].ToString());
            //                break;
            //            }
            //        }
            //    }
            //    int id_status = 0;
            //    if (status.SelectedValue is not null)
            //    {
            //        for (int i = 0; i < Status.Rows.Count; i++)
            //        {
            //            if (status.SelectedValue.ToString() == Status.Rows[i]["Nazwa"].ToString())
            //            {
            //                id_status = Convert.ToInt32(Status.Rows[i]["ID_Status"].ToString());
            //                break;
            //            }
            //        }
            //    }
            //    int kategoria = 0;
            //    if (Category_Tree.SelectedValue is not null)
            //    {
            //        for (int i = 0; i < Kategorie.Rows.Count; i++)
            //        {
            //            if (Category.Text == Kategorie.Rows[i]["Nazwa"].ToString())
            //            {
            //                kategoria = Convert.ToInt32(Kategorie.Rows[i]["ID_Kategorii"].ToString());
            //            }
            //        }
            //    }

            //    BAUMALOG_APP.Klasy.Tray tray = new(id_regalu, nrpolki, id_typu, id_status, kategoria, przeznaczenie);
            //    tray.Insert();
            }

            //         void Update_Tray()
            //         {
            //             _ = int.TryParse(DataToEdit["ID_Polki"].ToString(), out int id_polki);
            //             int id_typu = 0;
            //             if (Type.SelectedValue is not null)
            //             {
            //                 for (int i = 0; i < Typ.Rows.Count; i++)
            //                 {
            //                     if (Type.SelectedValue.ToString() == Typ.Rows[i]["Nazwa"].ToString())
            //                     {
            //                         id_typu = Convert.ToInt32(Typ.Rows[i]["ID_Typu"].ToString());
            //                         break;
            //                     }
            //                 }
            //             }
            //             int przeznaczenie = 0;
            //             if (Assignment.SelectedValue is not null)
            //             {
            //                 for (int i = 0; i < Przeznaczenie.Rows.Count; i++)
            //                 {
            //                     if (Assignment.SelectedValue.ToString() == Przeznaczenie.Rows[i]["Nazwa"].ToString())
            //                     {
            //                         przeznaczenie = Convert.ToInt32(Przeznaczenie.Rows[i]["ID_Przeznaczenia"].ToString());
            //                         break;
            //                     }
            //                 }
            //             }
            //             int id_status = 0;
            //             if (status.SelectedValue is not null)
            //             {
            //                 for (int i = 0; i < Status.Rows.Count; i++)
            //                 {
            //                     if (status.SelectedValue.ToString() == Status.Rows[i]["Nazwa"].ToString())
            //                     {
            //                         id_status = Convert.ToInt32(Status.Rows[i]["ID_Status"].ToString());
            //                         break;
            //                     }
            //                 }
            //             }
            //             int kategoria = 0;
            //             if (Category_Tree.SelectedValue is not null)
            //             {
            //                 for (int i = 0; i < Kategorie.Rows.Count; i++)
            //                 {
            //                     if (Category.Text == Kategorie.Rows[i]["Nazwa"].ToString())
            //                     {
            //                         kategoria = Convert.ToInt32(Kategorie.Rows[i]["ID_Kategorii"].ToString());
            //                     }
            //                 }
            //             }
            //             BAUMALOG_APP.Klasy.Tray tray = new(id_polki, id_typu, id_status, kategoria, przeznaczenie);

            //             tray.Update();
            //         }
            //     }
        }
    }
