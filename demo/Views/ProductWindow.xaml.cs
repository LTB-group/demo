using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using WpfApp4.Models;

namespace WpfApp4.Views
{
    public partial class ProductWindow : Window
    {
        private AppDbContext _db = new AppDbContext();
        private Product _currentProduct;

        public ProductWindow(Product product)
        {
            InitializeComponent();

            this.FontFamily = new FontFamily("Cooper Black");
            this.Background = Brushes.White;

            SaveBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00FA9A"));
            SaveBtn.BorderThickness = new Thickness(0);
            CancelBtn.Background = Brushes.LightGray;
            CancelBtn.BorderThickness = new Thickness(0);

            CbManufacturer.ItemsSource = _db.Manufacturers.ToList();
            CbCategory.ItemsSource = _db.Categories.ToList();
            CbLocation.ItemsSource = _db.Locations.ToList();

            _currentProduct = product;

            if (_currentProduct != null)
            {
                TbCode.Text = _currentProduct.ProductCode;
                TbCode.IsEnabled = false;
                TbName.Text = _currentProduct.Name;
                TbPrice.Text = _currentProduct.Price.ToString();
                TbDiscount.Text = _currentProduct.DiscountAmount.ToString();
                TbDescription.Text = _currentProduct.Description;

                CbManufacturer.SelectedItem = _db.Manufacturers.FirstOrDefault(m => m.ManufacturerId == _currentProduct.ManufacturerId);
                CbCategory.SelectedItem = _db.Categories.FirstOrDefault(c => c.CategoryId == _currentProduct.CategoryId);
                CbLocation.SelectedItem = _db.Locations.FirstOrDefault(l => l.LocationId == _currentProduct.LocationId);
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_currentProduct == null)
                {
                    _currentProduct = new Product
                    {
                        ProductCode = TbCode.Text
                    };
                    _db.Products.Add(_currentProduct);
                }

                _currentProduct.Name = TbName.Text;
                _currentProduct.Price = decimal.Parse(TbPrice.Text);
                _currentProduct.DiscountAmount = int.Parse(TbDiscount.Text);
                _currentProduct.Description = TbDescription.Text;

                _currentProduct.ManufacturerId = ((Manufacturer)CbManufacturer.SelectedItem).ManufacturerId;
                _currentProduct.CategoryId = ((Category)CbCategory.SelectedItem).CategoryId;
                _currentProduct.LocationId = ((Location)CbLocation.SelectedItem).LocationId;

                _db.SaveChanges();
                MessageBox.Show("Данные успешно сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Проверьте правильность введенных данных.\nВсе поля должны быть заполнены корректно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}