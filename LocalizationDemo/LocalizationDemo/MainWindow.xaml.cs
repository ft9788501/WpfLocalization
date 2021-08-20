using LocalizationDemo.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            I18N.I18NKeys.Name.BindingLocalizationString(this, x => textblock1.Text = x);
            I18N.I18NKeys.Age.BindingLocalizationString(this, x => textblock2.Text = x);
            I18N.BindingLocalizationString(this, () => textblock3.Text = $"{I18N.I18NKeys.String1.GetLocalizationString()}+{I18N.I18NKeys.String2.GetLocalizationString()}");
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var culture = ((ComboBoxItem)((ComboBox)sender).SelectedItem).Content;
            I18N.LoadFromJson(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @$"Properties\I18N\{culture}.json"));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NewWindow window = new NewWindow()
            {
                Width = 550,
                Height = 100,
                Title = I18N.I18NKeys.NewWindowTitle.GetLocalizationString()
            };
            window.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(I18N.I18NKeys.Success.GetLocalizationString());
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            I18N.RemoveBinding(this);
        }
    }
}
