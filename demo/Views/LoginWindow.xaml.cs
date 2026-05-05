using System.Linq;
using System.Windows;
using System.Windows.Media;
using WpfApp4.Models;

namespace WpfApp4.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            this.FontFamily = new FontFamily("Cooper Black");
            this.Background = Brushes.White;

            LoginBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00FA9A"));
            LoginBtn.BorderThickness = new Thickness(0);
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Login == LoginTb.Text && u.Password == PasswordTb.Password);

                if (user != null)
                {
                    var mainWindow = new MainWindow(user);
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}