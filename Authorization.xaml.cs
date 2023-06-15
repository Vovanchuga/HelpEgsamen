using System;
using System.Collections.Generic;
using System.Drawing;
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
using System.Windows.Threading;

namespace DemshkanPR1
{
    /// <summary>
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>
    public partial class Authorization : Page
    {
        TextBlock CText = new TextBlock();
        Line Lone = new Line();
        Line Ltwo = new Line();
        DispatcherTimer timer;
        TimeSpan time;
        public Authorization()
        {
            InitializeComponent();
        }

        private void BtnLogIn_Click(object sender, RoutedEventArgs e)
        {
            bool missUser = false;
            foreach (var user in TradeEntities1.GetContext().User.ToList())
            {
                if (UserLogin.Text == user.UserLogin && UserPassword.Text == user.UserPassword && TextCaptcha.Text == CText.Text)
                {
                    missUser = true;                   
                    Manager.MainFrame.Navigate(new Products(user));
                    (Application.Current.MainWindow as MainWindow).UserFIO.Text = user.UserFIO;
                    Captch.Visibility = Visibility.Hidden;
                    TextCaptcha.Visibility = Visibility.Hidden;
                    CText.Text = "";
                    TextCaptcha.Text = "";
                    break;
                }
                else
                {
                    missUser = false;
                } 
            }

            if (missUser == false)
            {
                Captch.Visibility = Visibility.Visible;
                TextCaptcha.Visibility = Visibility.Visible;
                if (TextCaptcha.Text != CText.Text)
                    MessageBox.Show("Капча не совпадает");
                else
                    MessageBox.Show("Логин и пароль не совпадают ");
                CaptchaDraw();
                BtnLogIn.IsEnabled = false;
                time = TimeSpan.FromSeconds(10);
                timer = new DispatcherTimer();
                timer.Tick += new EventHandler(timerTICK);
                timer.Interval = new TimeSpan(0, 0, 1);
                timer.Start();
            }
        }

        private void BtnLogInLikeStranger_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new Products(null));
        }

        private void CaptchaDraw()
        {        
            try
            {
                Random rnd = new Random();
                string Alphabit = "1234567890QWWERTYUIOASDFGHJKLZXCVBNM";
                int[] corn = new int[8] { 16, 6, 8, 7, 12, -12, -5, -9 };
                string text = "";

                for (int i = 0; i < 6; i++)
                {
                    text += Alphabit[rnd.Next(Alphabit.Length)];
                }

                CText.Text = text;
                CText.LayoutTransform = new RotateTransform(corn[rnd.Next(corn.Length)]);
                CText.VerticalAlignment = VerticalAlignment.Center;
                CText.HorizontalAlignment = HorizontalAlignment.Center;
                CText.Foreground = new SolidColorBrush(Colors.Green);
                CText.FontSize = 35;
                CText.Padding = new Thickness((int)Captch.Height - 80);
                Captch.Children.Add(CText);

                Lone.X1 = 3;
                Lone.Y1 = 19;
                Lone.X2 = Captch.Width - 4;
                Lone.Y2 = Captch.Height - 22;
                Lone.StrokeThickness = 4;
                Lone.Stroke = System.Windows.Media.Brushes.White;
                Captch.Children.Add(Lone);

                Ltwo.X1 = 3;
                Ltwo.Y1 = Captch.Height - 1;
                Ltwo.X2 = Captch.Width - 4;
                Ltwo.Y2 = 33;
                Ltwo.StrokeThickness = 4;
                Ltwo.Stroke = System.Windows.Media.Brushes.White;
                Captch.Children.Add(Ltwo);
            }
            catch { }
        }
        private void timerTICK(object sender, EventArgs e)
        {
            BtnLogIn.Content = time.ToString("c");
            if (time == TimeSpan.Zero)
            {
                timer.Stop();
                BtnLogIn.Content = "Войти";
                BtnLogIn.IsEnabled = true;          
            }
            time = time.Add(TimeSpan.FromSeconds(-1));
        }

        private void Grid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            
        }
    }
}
