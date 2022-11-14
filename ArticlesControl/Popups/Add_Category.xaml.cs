using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ArticlesControl.Klasy;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace ArticlesControl.Formatki
{
    /// <summary>
    /// Logika interakcji dla klasy Add_Category.xaml
    /// </summary>
    public partial class Add_Category : Window
    {
        public ResourceDictionary dict = new();
        public Dictionary<string, string> dictionary;
        public string PoziomTree = string.Empty;
        private readonly Add_DataTableAdapters.KategoriaTree_SELECTTableAdapter kategoriaTree_SELECT = new();
        private readonly Add_DataTableAdapters.Kar_Kategorie_SELECTTableAdapter tableAdapter = new Add_DataTableAdapters.Kar_Kategorie_SELECTTableAdapter();
        private readonly Dictionary<string, string>? custom_columns;
        private readonly Dictionary<string, Tuple<string, System.Type>> cells = new();
        public DataTable KategorieTree { get; set; } = new DataTable();
        private bool isEdit { get; set; }
        DataRow DataToEdit;
        //Constructor for adding category
        public Add_Category(ResourceDictionary resourceDictionary, Dictionary<string, string> lang)
        {
            
            dictionary = lang;
            dict = resourceDictionary;
            Resources.MergedDictionaries.Add(dict);
            InitializeComponent();
            Load_Category_Tree();
            Up.Visibility = Visibility.Collapsed;
            Down.Visibility = Visibility.Collapsed;
        }
        //Constructor for editing category
        public Add_Category(ResourceDictionary resourceDictionary, Dictionary<string, string> lang, Dictionary<string, Tuple<string, System.Type>> cells, Dictionary<string, string> custom_columns)
        {
            isEdit = true;
            this.cells = cells;
            dictionary = lang;
            dict = resourceDictionary;
            Resources.MergedDictionaries.Add(dict);
            DataToEdit = (DataRow)(tableAdapter.GetData().Select(String.Format("[ID_Kategorii] = '" + cells["ID_Kategorii"].Item1 + "'")))[0];
            InitializeComponent();
            Load_Category_Tree();
            
            this.custom_columns = custom_columns;
            FillTextBox();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (isEdit)
            {
                BuildDataTable();
                _ = int.TryParse(DataToEdit["ID_Kategorii"].ToString(), out int temp1);
                Category category = new(temp1, Name.Text, Description.Text, Weight_Converter.Text, Index_Mask.Text, KategorieTree);
                category.Update();
                Close();
            }
            else
            {
                string poziomtree = BuildPoziomString();
                if (String.IsNullOrEmpty(poziomtree))
                {

                    e.Handled = true;
                }
                Category category = new(Name.Text, Description.Text, Weight_Converter.Text, poziomtree, Index_Mask.Text);
                category.Insert();
                Close();
            }

        }
        private void FillTextBox()
        {
            Name.Text = DataToEdit["Nazwa"].ToString();
            Description.Text = DataToEdit["Opis"].ToString();
            Weight_Converter.Text = DataToEdit["DomyslnyPrzelicznikWagi"].ToString();
            Index_Mask.Text = DataToEdit["Maska_Indeksu"].ToString();
        }
        private void BuildDataTable()
        {
            DataColumn col1 = new("ID");
            DataColumn col2 = new("Poziomtree");
            KategorieTree.Columns.Add("ID");
            KategorieTree.Columns.Add("Poziomtree");
            DataRow row;
            List<TreeViewItem> list = FindTreeViewItems(Category_Tree);
            foreach (TreeViewItem item in list)
            {
                _ = int.TryParse(item.Uid, out int temp);
                row = KategorieTree.NewRow();
                row[0] = temp;
                row[1] = item.DisplayMemberPath + item.Tag;
                KategorieTree.Rows.Add(row);
            }
        }
        public List<TreeViewItem> FindTreeViewItems(Visual @this)
        {
            if (@this == null)
                return null;

            var result = new List<TreeViewItem>();

            FrameworkElement? frameworkElement = @this as FrameworkElement;
            if (frameworkElement is not null)
            {
                frameworkElement.ApplyTemplate();
            }

            Visual? child = null;
            for (int i = 0, count = VisualTreeHelper.GetChildrenCount(@this); i < count; i++)
            {
                child = VisualTreeHelper.GetChild(@this, i) as Visual;

                var treeViewItem = child as TreeViewItem;
                if (treeViewItem is not null)
                {
                    result.Add(treeViewItem);
                    if (!treeViewItem.IsExpanded)
                    {
                        treeViewItem.IsExpanded = true;
                        treeViewItem.UpdateLayout();
                    }
                }
                foreach (var childTreeViewItem in FindTreeViewItems(@this: child?? this))
                {
                    result.Add(childTreeViewItem);
                }
            }
            return result;
        }
        private string BuildPoziomString()
        {
            TreeViewItem item = (TreeViewItem)Category_Tree.SelectedItem;
            if (item == null)
            {
                ColorAnimation flash = new(Colors.White, Colors.Red, new Duration(TimeSpan.FromMilliseconds(75D))) { AutoReverse = true, RepeatBehavior = new RepeatBehavior(3D) };
                Category_Tree.Background = new SolidColorBrush(Colors.White); // no error???
                Category_Tree.BeginAnimation(SolidColorBrush.ColorProperty, flash);
            }
            else return item.DisplayMemberPath + item.Tag;
            return string.Empty;
        }
        #region TreeView
        private void Load_Category_Tree()
        {
            Category_Tree.Items.Clear();
            Add_Data DS = new();
            kategoriaTree_SELECT.Fill(DS.KategoriaTree_SELECT);

            List<Item> Items = new();
            foreach (Add_Data.KategoriaTree_SELECTRow poziomtree in DS.KategoriaTree_SELECT.Rows)
            {
                Items.Add(new Item(poziomtree.Nazwa, poziomtree.PoziomTree.ToString()[^3..], poziomtree.PoziomTree.ToString()[0..^3], poziomtree.ID_Kategorii.ToString()));
            }
            List<TreeViewItem> list = treeViewItems(Items);
            BuildCategoryTree(list);
        }
        private void BuildCategoryTree(List<TreeViewItem> items)
        {
            TreeViewItem root = new();
            root.Header = dictionary["All"];
            root.Tag = "000";
            root.DisplayMemberPath = "";
            root.Background = (SolidColorBrush?)new BrushConverter().ConvertFrom(Application.Current.Resources["App_Background"].ToString() ?? string.Empty);
            root.IsExpanded = true;
            items.Add(root);
            foreach (TreeViewItem item in items)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (item.DisplayMemberPath == items[i].DisplayMemberPath + items[i].Tag)
                    {
                        items[i].Items.Add(item);
                    }
                }
            }
            Category_Tree.Items.Add(root);
        }
        private List<TreeViewItem> treeViewItems(List<Item> Children)
        {
            List<TreeViewItem> treeViewItems = new();
            foreach (Item? item in Children)
            {
                TreeViewItem treeViewItem = new()
                {
                    Header = item.Nazwa,
                    Tag = item.Id,
                    DisplayMemberPath = item.ParentId,
                    Uid = item.ID_Kategorii
                };
                treeViewItems.Add(treeViewItem);
                treeViewItem.IsExpanded = true;
                treeViewItem.Background = (SolidColorBrush?)new BrushConverter().ConvertFrom(Application.Current.Resources["App_Background"].ToString() ?? string.Empty);
                if (isEdit && treeViewItem.Uid == DataToEdit["ID_Kategorii"].ToString())
                {
                    treeViewItem.IsSelected = true;
                    treeViewItem.Focus();
                }
            }
            return treeViewItems;
        }
        private class Item
        {
            public readonly string Nazwa;
            public readonly string Id;
            public readonly string? ParentId;
            public readonly string? ID_Kategorii;
            public Item(string Nazwa, string id, string? parent, string? ID_Kategorii)
            {
                this.Nazwa = Nazwa;
                Id = id;
                ParentId = parent;
                this.ID_Kategorii = ID_Kategorii;
            }
            public readonly List<Item> Children = new();

        }
        private void Category_Tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem selected = (TreeViewItem)Category_Tree.SelectedItem;
            PoziomTree = selected.DisplayMemberPath + selected.Tag;
        }
        private void Up_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem selected = (TreeViewItem)Category_Tree.SelectedItem;
            TreeViewItem parent = FindParentOfType<TreeViewItem>(selected);
            if (parent is not null)
            {
                for (int i = 0; i < parent.Items.Count; i++)
                {
                    if (parent.Items[i] == selected)
                    {
                        try
                        {
                            TreeViewItem temp = (TreeViewItem)parent.Items[i - 1];
                            parent.Items.RemoveAt(i);
                            parent.Items.RemoveAt(i - 1);
                            (selected.Tag, temp.Tag) = (temp.Tag, selected.Tag);
                            parent.Items.Insert(i - 1, selected);
                            parent.Items.Insert(i, temp);
                        }
                        catch (Exception)
                        {
                            HandyControl.Controls.MessageBox.Show(dictionary["Cannot_leave_node"], parent.Header.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            var x = RenameTreeItems((TreeViewItem)Category_Tree.Items[0]);
            Category_Tree.Items.Refresh();
            selected.IsSelected = true;
        }
        private void Down_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem selected = (TreeViewItem)Category_Tree.SelectedItem;
            TreeViewItem parent = FindParentOfType<TreeViewItem>(selected);
            if (parent is not null)
            {
                for (int i = 0; i < parent.Items.Count; i++)
                {
                    if (parent.Items[i] == selected)
                    {
                        try
                        {
                            TreeViewItem temp = (TreeViewItem)parent.Items[i + 1];
                            parent.Items.RemoveAt(i + 1);
                            parent.Items.RemoveAt(i);
                            (selected.Tag, temp.Tag) = (temp.Tag, selected.Tag);
                            parent.Items.Insert(i, temp);
                            parent.Items.Insert(i + 1, selected);
                            break;
                        }
                        catch (Exception)
                        {
                            HandyControl.Controls.MessageBox.Show(dictionary["Cannot_leave_node"], parent.Header.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            var x = RenameTreeItems((TreeViewItem)Category_Tree.Items[0]);
            Category_Tree.Items.Refresh();
            selected.IsSelected = true;
        }
        public static T FindParentOfType<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentDepObj = child;
            do
            {
                parentDepObj = VisualTreeHelper.GetParent(parentDepObj);
                T? parent = parentDepObj as T;
                if (parent is not null)
                {
                    return parent;
                }
            }
            while (parentDepObj != null);
            return null;
        }
        private TreeViewItem RenameTreeItems(TreeViewItem parent)
        {
            int tag = 1;
            foreach (TreeViewItem treeViewItem in parent.Items)
            {
                treeViewItem.DisplayMemberPath = parent.DisplayMemberPath + parent.Tag;
                treeViewItem.Tag = tag.ToString("D3");
                tag++;
                if (treeViewItem.Items.Count is not 0)
                {
                    RenameTreeItems(treeViewItem);
                }
            }
            return parent;
        }
        #endregion
    }
}
