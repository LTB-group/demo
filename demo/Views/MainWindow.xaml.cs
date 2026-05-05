using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.EntityFrameworkCore;
using WpfApp4.Models;

namespace WpfApp4.Views
{
    public partial class MainWindow : Window
    {
        private readonly User _user;

        public MainWindow(User user)
        {
            InitializeComponent();
            _user = user;

            this.FontFamily = new FontFamily("Cooper Black");
            this.Background = Brushes.White;

            HeaderPanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e4c418"));

            LogoutBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00FA9A"));
            AddBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00FA9A"));
            EditBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00FA9A"));
            DeleteBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00FA9A"));

            LogoutBtn.BorderThickness = new Thickness(0);
            AddBtn.BorderThickness = new Thickness(0);
            EditBtn.BorderThickness = new Thickness(0);
            DeleteBtn.BorderThickness = new Thickness(0);

            UserNameTb.Text = _user.FullName;

            if (_user.RoleId == 2) // Если Заказчик
            {
                SearchPanel.Visibility = Visibility.Collapsed;
                AdminPanel.Visibility = Visibility.Collapsed;
            }

            LoadData();
        }

        private void LoadData(string search = "")
        {
            using (var db = new AppDbContext())
            {
                var query = db.Products
                    .Include(p => p.Manufacturer)
                    .Include(p => p.Category)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(search) && _user.RoleId == 1)
                {
                    query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search));
                }

                ProductsList.ItemsSource = query.ToList();
            }
        }

        private void SearchTb_TextChanged(object sender, TextChangedEventArgs e) => LoadData(SearchTb.Text);

        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            this.Close();
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new ProductWindow(null);
            if (addWindow.ShowDialog() == true) LoadData(SearchTb.Text);
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = ProductsList.SelectedItem as Product;
            if (selectedProduct == null)
            {
                MessageBox.Show("Выберите товар для редактирования!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var editWindow = new ProductWindow(selectedProduct);
            if (editWindow.ShowDialog() == true) LoadData(SearchTb.Text);
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = ProductsList.SelectedItem as Product;
            if (selectedProduct == null) return;

            if (MessageBox.Show($"Вы точно хотите удалить товар '{selectedProduct.Name}'?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                using (var db = new AppDbContext())
                {
                    db.Products.Remove(selectedProduct);
                    db.SaveChanges();
                }
                LoadData(SearchTb.Text);
            }
        }
    }

    // СКИДКИ
    public class DiscountColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int discount && discount > 10)
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C8A2C8"));
            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class ImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string imageName = value as string;

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string imagesFolder = "";

            DirectoryInfo di = new DirectoryInfo(baseDir);
            while (di != null)
            {
                var testPath = Path.Combine(di.FullName, "Images");
                if (Directory.Exists(testPath))
                {
                    imagesFolder = testPath;
                    break;
                }
                di = di.Parent;
            }

            if (string.IsNullOrEmpty(imagesFolder)) return null;

            string finalPath = Path.Combine(imagesFolder, "picture.png");

            if (!string.IsNullOrWhiteSpace(imageName))
            {
                string targetPath = Path.Combine(imagesFolder, imageName.Trim());

                if (File.Exists(targetPath))
                {
                    finalPath = targetPath;
                }
                else
                {
                    string nameWithoutExt = Path.GetFileNameWithoutExtension(targetPath);
                    string[] altExtensions = { ".jpg", ".jpeg", ".png" };
                    foreach (var ext in altExtensions)
                    {
                        string altPath = Path.Combine(imagesFolder, nameWithoutExt + ext);
                        if (File.Exists(altPath))
                        {
                            finalPath = altPath;
                            break;
                        }
                    }
                }
            }

            try
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(finalPath, UriKind.Absolute);
                bitmap.EndInit();
                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}