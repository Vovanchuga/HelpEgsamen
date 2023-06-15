using Microsoft.Win32;
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
using System.Windows.Shapes;

namespace DemshkanPR1
{
    /// <summary>
    /// Логика взаимодействия для ProductAdd.xaml
    /// </summary>
    public partial class ProductAdd : Window
    {
        private Product _currentProduct = new Product();
        private string PhotoPath = "";

        public ProductAdd(Product selectedProduct)
        {
            InitializeComponent();

            if (selectedProduct != null)
            {
                _currentProduct = selectedProduct;
                TextBoxProdID.Visibility = Visibility.Visible;
                TextProdID.Visibility = Visibility.Visible;
            }
            
            PhotoPath = _currentProduct.ProductPhoto;

            DataContext = _currentProduct;
            
            ComboCategory.ItemsSource = TradeEntities1.GetContext().TypePorduct.ToList();
            ComboManuf.ItemsSource = TradeEntities1.GetContext().Manufacturer.ToList();
            ComboMeasur.ItemsSource = TradeEntities1.GetContext().UnitMeasurement.ToList();
            ComboTransfer.ItemsSource = TradeEntities1.GetContext().Transfer.ToList();
         
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentProduct.ProductName))
                errors.AppendLine("Укажите имя продукта");
            if (_currentProduct.TypePorduct == null)
                errors.AppendLine("Укажите тип продукта");
            if (_currentProduct.ProductQuantityInStock < 0)
                errors.AppendLine("Количество на складе не должно быть отрицательным");
            if (_currentProduct.UnitMeasurement == null)
                errors.AppendLine("Укажите единицу измерения");
            if (_currentProduct.ProductDiscountAmount < 0 || _currentProduct.ProductDiscountAmount > 100)
                errors.AppendLine("Действующая скидка не может может быть меньше 0 или больше 100");
            if (_currentProduct.PoductMaxDiscount < 0 || _currentProduct.PoductMaxDiscount > 100)
                errors.AppendLine("Максимальная скидка не может может быть меньше 0 или больше 100");
            if (_currentProduct.ProductDiscountAmount > _currentProduct.PoductMaxDiscount)
                errors.AppendLine("Действующая скидка не может может быть больше Максимальной скидки");
            if (_currentProduct.Transfer == null)
                errors.AppendLine("Укажите поставщика");
            if (_currentProduct.Manufacturer == null)
                errors.AppendLine("Укажите производителя");
            if (_currentProduct.ProductCost < 0)
                errors.AppendLine("Стоимость товара не может быть меньше 0");
            if (string.IsNullOrWhiteSpace(_currentProduct.ProductDescription))
                errors.AppendLine("Добавьте описание товара");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }         

            if (_currentProduct.ProductArticleNumber == null)
            {
                var Prod = TradeEntities1.GetContext().Product.ToList().LastOrDefault();
                var LastArticlSimbol = Prod.ProductArticleNumber.Length - 1;
                _currentProduct.ProductArticleNumber = Prod.ProductArticleNumber.Remove(LastArticlSimbol) + (int.Parse(Prod.ProductArticleNumber[Prod.ProductArticleNumber.Length - 1].ToString()) + 1).ToString();

                TradeEntities1.GetContext().Product.Add(_currentProduct);
            }

            try
            {
                TradeEntities1.GetContext().SaveChanges();
                MessageBox.Show("Данные сохранены!");
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }        
        }

        private void LoadingPhoto_Click(object sender, RoutedEventArgs e)
        {     
            OpenFileDialog openfiledialog = new OpenFileDialog();
            openfiledialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*";

            if (openfiledialog.ShowDialog() == true)
            {
                Uri fileUri = new Uri(openfiledialog.FileName);
                var PhotoPath = fileUri.ToString();
                var fileName = "";
                for (int i = 1; i < PhotoPath.Length; i++)
                {                                     
                    if (PhotoPath[PhotoPath.Length-i] == '/')
                    {
                        break;
                    }
                    fileName += PhotoPath[PhotoPath.Length - i];
                }
                var truFileName = "";
                for (int i = 1; i <= fileName.Length; i++)
                {
                    truFileName += fileName[fileName.Length - i];
                }
                
                File.Copy(fileUri.AbsolutePath, @"C:\Users\user.KITS\Desktop\Демешкан541РПМ\DemshkanPR1\DemshkanPR1\ProductPhoto\" + truFileName, true);
                _currentProduct.ProductPhoto = @"C:\Users\user.KITS\Desktop\Демешкан541РПМ\DemshkanPR1\DemshkanPR1\ProductPhoto\" + truFileName;

                if (_currentProduct.ProductPhoto != null)
                {
                    MemoryStream bytestream = new MemoryStream(File.ReadAllBytes(_currentProduct.ProductPhoto));
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.DecodePixelWidth = 100;
                    bi.StreamSource = bytestream;   
                    bi.EndInit();
                    PhotoProd.Source = bi;
                }

                //PhotoProd.Source = new BitmapImage(new Uri(PhotoPath)) ;
            }
        }

        private void DeletePhoto_Click(object sender, RoutedEventArgs e)
        {
            var delphoto = _currentProduct.ProductPhoto;
            PhotoProd.Source = null;
            _currentProduct.ProductPhoto = null;           
            File.Delete(delphoto);
            //Close();
        }
    }
}
