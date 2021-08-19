using LocalizationDemo.Properties;
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

namespace LocalizationDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            I18N.I18NKeys.Name.BindingLocalizationString(x => textblock1.Text = x);
            I18N.I18NKeys.Age.BindingLocalizationString(x => textblock2.Text = x);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var culture = ((ComboBoxItem)((ComboBox)sender).SelectedItem).Content;
            I18N.LoadFromJson(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @$"Properties\I18N\{culture}.json"));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Window window = new Window()
            {
                Width = 550,
                Height = 100,
                Title = I18N.I18NKeys.NewWindowTitle.GetLocalizationString()
            };
            window.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //var result = I18N.Instance.SaveAsJson(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @$"Properties\I18N\en-US.json"));
            //if (result)
            {
                MessageBox.Show(I18N.I18NKeys.Success.GetLocalizationString());
            }
        }
    }
}
