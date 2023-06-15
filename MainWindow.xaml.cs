using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new Authorization());
            Manager.MainFrame = MainFrame;
            //ImportProduct();
            //ImportOrder();
        }

        private void Frame_ContentRendered(object sender, EventArgs e)
        {
            if (MainFrame.CanGoBack)
            {
                BtnBack.Visibility = Visibility.Visible;              
            }
            else
            {
                BtnBack.Visibility = Visibility.Hidden;
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.GoBack();
            UserFIO.Text = "";
        }

        private void ImportProduct()
        {
            var datafile = File.ReadAllLines(@"C:\Users\vovad\Desktop\Демешкан541РПМ\вариант1\Общие ресурсы\1123Товар_import_Товарыдляживотных.txt");
            foreach(var line in datafile)
            {
                var data = line.Split('\t');

                var TempProduct = new Product
                {
                    ProductArticleNumber = data[0],
                    ProductName = data[1].Replace("\"", ""),
                    ProductDescription = data[2].Replace("\"", ""),
                    ProductCategory = int.Parse(data[3]),
                    ProductPhoto = data[4],
                    ProductManufacturer = int.Parse(data[5]),
                    ProductCost = decimal.Parse(data[6]),
                    ProductDiscountAmount = byte.Parse(data[7]),
                    ProductQuantityInStock = int.Parse(data[8]),
                    ProductTransfer = int.Parse(data[9]),
                    PoductMaxDiscount = byte.Parse(data[10]),
                    PoductUnitMeasurement = int.Parse(data[11])

                };
                TradeEntities1.GetContext().Product.Add(TempProduct);
                TradeEntities1.GetContext().SaveChanges();
            }
        }

        private void ImportOrder()
        {
            var datafile = File.ReadAllLines(@"C:\Users\vovad\Desktop\Демешкан541РПМ\UnicodeЗаказ_import.txt");
            foreach (var line in datafile)
            {
                var data = line.Split('\t');

                var TempOrder = new Order
                {
                    OrderStatus = data[0],
                    OrderDate = DateTime.Parse(data[1] + " 0:00:00"),
                    OrderDeliveryDate = DateTime.Parse(data[2] + " 0:00:00"),
                    OrderPickupPoint = int.Parse(data[3]),
                    OrederKlient = data[4],
                    OrderPickupCode = int.Parse(data[5])
                };
                TradeEntities1.GetContext().Order.Add(TempOrder);
                TradeEntities1.GetContext().SaveChanges();
            }
        }
    }
}
