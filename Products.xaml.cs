using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DemshkanPR1
{
    /// <summary>
    /// Логика взаимодействия для Products.xaml
    /// </summary>
    public partial class Products : Page
    {
        private User _currentUser = null;
        public Products(User user)
        {
            InitializeComponent();
            var allManuf = TradeEntities1.GetContext().Manufacturer.ToList();
            allManuf.Insert(0, new Manufacturer
            {
                ManufacturerName = "Все производители"
            });
            if (user != null)
            {
                _currentUser = user;

                if (_currentUser.Role.RoleID == 1)
                {
                    BtnDeleteProduct.Visibility = Visibility.Visible;
                    BtnAddProduct.Visibility = Visibility.Visible;
                }
                else
                {
                    BtnDeleteProduct.Visibility = Visibility.Hidden;
                    BtnAddProduct.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                BtnDeleteProduct.Visibility = Visibility.Hidden;
                BtnAddProduct.Visibility = Visibility.Hidden;
            }
            ComboManuf.ItemsSource = allManuf;
            ComboManuf.SelectedIndex = 0;

            UpdateProducts();
        }

        public void UpdateProducts()
        {

            var currentProducts = TradeEntities1.GetContext().Product.ToList();

            if (ComboManuf.SelectedIndex > 0)
                currentProducts = currentProducts.Where(p => p.Manufacturer == (ComboManuf.SelectedItem as Manufacturer)).ToList();

            currentProducts = currentProducts.Where(p => p.ProductName.ToLower().Contains(TBoxSearching.Text.ToLower())).ToList();

            if (ComboSort.SelectedIndex == 1)
            {
                currentProducts = currentProducts.OrderBy(p => p.ProductCost).ToList();
            }
            else if (ComboSort.SelectedIndex == 2)
            {
                currentProducts = currentProducts.OrderByDescending(p => p.ProductCost).ToList();
            }

            LViewProducts.ItemsSource = currentProducts;

            CountPoducts.Text = currentProducts.Count + " из " + TradeEntities1.GetContext().Product.Count();
        }

        private void TBoxSearching_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateProducts();
        }

        private void ComboManuf_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProducts();
        }

        private void ComboSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProducts();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           new ProductAdd(null).Show();
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                TradeEntities1.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                UpdateProducts();
            }
        }

        ProductAdd ProdAddCheck;

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_currentUser != null)
                if (_currentUser.Role.RoleID == 1)
                {
                    if (ProdAddCheck == null)
                    {
                        ProdAddCheck = new ProductAdd((sender as ListViewItem).DataContext as Product);
                        ProdAddCheck.Show();
                    }
                    else if (!ProdAddCheck.IsVisible)
                    {
                        ProdAddCheck = null;
                        ProdAddCheck = new ProductAdd((sender as ListViewItem).DataContext as Product);
                        ProdAddCheck.Show();
                    }
                }
        }

        private void BtnDeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            var ProductDel = LViewProducts.SelectedItems.Cast<Product>().ToList();
            bool RemOrder = false;
            if (ProductDel.Count != 0)
            {
                if (MessageBox.Show($"Вы точно хотите удалить этот товар {ProductDel[0].ProductName} ?", "Внимание!",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    foreach (var ProdOrd in TradeEntities1.GetContext().OrderProduct.ToList())
                    {
                        if (ProductDel[0].ProductArticleNumber == ProdOrd.ProductArticleNumber)
                        {
                            RemOrder = true;
                            MessageBox.Show("Этот продукт есть в заказах");
                            break;
                        }
                    }
                    if (RemOrder == false)
                    {
                        try
                        {
                            TradeEntities1.GetContext().Product.RemoveRange(ProductDel);
                            TradeEntities1.GetContext().SaveChanges();
                            MessageBox.Show("Данные удалены!");

                            UpdateProducts();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message.ToString());
                        }
                    }
                }
            }
        }

        private void Page_MouseEnter(object sender, MouseEventArgs e)
        {
            UpdateProducts();
        }
    }
}
