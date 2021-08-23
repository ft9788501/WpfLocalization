using Localization.I18N;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
        public class ViewModel : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            private bool flag1 = false;
            private string speakingIndicatorSuffix = I18NKeys.SpeakingIndicatorSpeaking.GetLocalizationString();
            private bool enablePseudo = false;
            public string SpeakingIndicatorSuffix
            {
                get => speakingIndicatorSuffix;
                set
                {
                    speakingIndicatorSuffix = value;
                    OnPropertyChanged();
                }
            }
            public bool EnablePseudo
            {
                get => enablePseudo;
                set
                {
                    enablePseudo = value;
                    I18NManager.EnablePseudo = value;
                    OnPropertyChanged();
                }
            }

            public void SpeakingIndicatorChanged()
            {
                if (flag1)
                {
                    I18NKeys.SpeakingIndicatorSpeaking.BindingExpression(this, x => SpeakingIndicatorSuffix);
                }
                else
                {
                    I18NKeys.SpeakingIndicatorReconnecting.BindingExpression(this, x => SpeakingIndicatorSuffix);
                }
                flag1 = !flag1;
            }
        }
        private ViewModel viewModel = new ViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = viewModel;
            I18NKeys.Title.ToString();
            I18NKeys.Name.BindingExpression(textblock1, x => x.Text);
            I18NKeys.Age.BindingExpression(textblock2, x => x.Text);
            textblock3.Text = $"{I18NKeys.String1.GetLocalizationString()}+{I18NKeys.String2.GetLocalizationString()}";
            I18NManager.CultureChanged += (s, e) =>
            {
                textblock3.Text = $"{I18NKeys.String1.GetLocalizationString()}+{I18NKeys.String2.GetLocalizationString()}";
            };
            I18NKeys.FormatString.BindingExpression(textblock4, x => x.Text, "p1", "p2");
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var culture = ((ComboBoxItem)((ComboBox)sender).SelectedItem).Content.ToString();
            I18NManager.SetCulture(culture);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NewWindow window = new NewWindow()
            {
                Width = 550,
                Height = 100
            };
            window.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(I18NKeys.Success.GetLocalizationString());
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            viewModel.SpeakingIndicatorChanged();
            GC.Collect();
        }
    }
}
