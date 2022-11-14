using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Windows.Input;
using System.Windows.Threading;
using HandyControl;




namespace BTPControlLibrary.Standard
{
    public partial class Tower : UserControl
    {
        public Tower()
        {
            InitializeComponent();
            ColumnsNumber = 2;
            TrayInColumnNumber = 15;
            RowPlaceForWindowOrStation = 3;
            CreateTower();
        }

        public int RowPlaceForWindowOrStation { get; set; }
        public int ColumnsNumber { get; set; }
        public int TrayInColumnNumber { get; set; }

        private void CreateTower()
        {
            for (int i = 0; i <= ColumnsNumber; i++)
            {
                ColumnDefinition cd = new ColumnDefinition();
                cd.Width = new GridLength(1, GridUnitType.Star);
                cd.MinWidth = 90;
                towerGrid.ColumnDefinitions.Add(cd);
            }

            for (int i = 0; i < TrayInColumnNumber + RowPlaceForWindowOrStation; i++) // dwie na dole wolne
            {
                RowDefinition rd = new RowDefinition();
                rd.Height = new GridLength(1, GridUnitType.Star);
                towerGrid.RowDefinitions.Add(rd);
            }
            CreateTray();
        }
        private void CreateTray()
        {
            int traypom = 1;
            bool elevatorcreated = false;
            for (int i = 0; i < TrayInColumnNumber; i++)
            {
                for (int j = 0; j <= ColumnsNumber; j++)
                {
                    if (j % 2 == 0 || j == 0)
                    {
                        //półka
                        //Grid grid = new Grid();
                        //grid.Name = "tray" + traypom.ToString();
                        //RegisterName(grid.Name, grid);
                        //grid.Background = Brushes.Orange;
                        //grid.Margin = new Thickness(3);
                        //grid.SetValue(Grid.RowProperty, TrayInColumnNumber - i - 1);
                        //grid.SetValue(Grid.ColumnProperty, j);

                        Border border1 = new Border();
                        border1.Name = "tray" + traypom.ToString();
                        RegisterName(border1.Name, border1);
                        border1.BorderThickness = new Thickness(1);
                        border1.CornerRadius = new CornerRadius(2);
                        border1.BorderBrush = Brushes.Black;
                        border1.Background = Brushes.Orange;
                        border1.Margin = new Thickness(3);
                        border1.SetValue(Grid.RowProperty, TrayInColumnNumber - i - 1);
                        border1.SetValue(Grid.ColumnProperty, j);

                        TextBlock tb = new TextBlock();
                        tb.Text = traypom.ToString();
                        traypom++;
                        tb.HorizontalAlignment = HorizontalAlignment.Center;
                        tb.VerticalAlignment = VerticalAlignment.Center;
                        tb.FontSize = 10;
                        tb.FontWeight = FontWeights.Bold;

                        Border border2 = new Border();
                        border2.BorderThickness = new Thickness(1);
                        border2.CornerRadius = new CornerRadius(2);
                        border2.BorderBrush = Brushes.Black;
                        border2.Background = Brushes.White;
                        border2.Height = 18;
                        border2.Width = 28;

                        border2.Child = tb;
                        border1.Child = border2;
                        
                        //grid.Children.Add(border1);
                        towerGrid.Children.Add(border1);

                        //prowadnica
                        Grid rtan = new Grid();
                        rtan.VerticalAlignment = VerticalAlignment.Bottom;
                        rtan.Height = 2;
                        rtan.Background = Brushes.Gray;
                        rtan.SetValue(Grid.RowProperty, TrayInColumnNumber - i - 1);
                        rtan.SetValue(Grid.ColumnProperty, j);

                        towerGrid.Children.Add(rtan);
                    }
                    else
                    {
                        if (!elevatorcreated)
                        {
                            //winda
                            Rectangle rtan = new Rectangle();
                            rtan.Name = "elevator";
                            RegisterName(rtan.Name, rtan);
                            rtan.VerticalAlignment = VerticalAlignment.Bottom;
                            rtan.Height = 15;
                            rtan.Margin = new Thickness(2, 0, 2, -13);
                            rtan.Fill = Brushes.DarkOrange;
                            rtan.SetValue(Grid.RowProperty, TrayInColumnNumber);
                            rtan.SetValue(Grid.ColumnProperty, j);

                            //półka
                            Grid grid = new Grid();
                            grid.Name = "ev_tray";
                            RegisterName(grid.Name, grid);
                            grid.Background = Brushes.Orange;
                            grid.Margin = new Thickness(3);
                            grid.SetValue(Grid.RowProperty, TrayInColumnNumber);
                            grid.SetValue(Grid.ColumnProperty, j);
                            grid.Visibility = Visibility.Hidden;

                            TextBlock tb = new TextBlock();
                            tb.Name = "ev_tray_text";
                            RegisterName(tb.Name, tb);
                            tb.TextAlignment = TextAlignment.Center;
                            tb.VerticalAlignment = VerticalAlignment.Center;
                            tb.FontSize = 12;
                            tb.Margin = new Thickness(50, 0, 50, 0);
                            tb.FontWeight = FontWeights.Bold;
                            tb.Background = Brushes.White;

                            grid.Children.Add(tb);
                            towerGrid.Children.Add(grid);
                            towerGrid.Children.Add(rtan);

                            elevatorcreated = true;
                        }

                    }
                }

            }
        }
    }
}
