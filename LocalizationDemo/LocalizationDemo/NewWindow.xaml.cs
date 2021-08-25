using Localization.I18N;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LocalizationDemo
{
    /// <summary>
    /// Interaction logic for NewWindow.xaml
    /// </summary>
    public partial class NewWindow : Window
    {
        public class ViewModel : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            private string versionInfo = "1.0.0";
            public string VersionInfo
            {
                get => versionInfo;
                set
                {
                    versionInfo = value;
                    OnPropertyChanged();
                }
            }
        }
        private ViewModel viewModel = new ViewModel();

        public NewWindow()
        {
            InitializeComponent();
            DataContext = viewModel;
            I18NKeys.Name.BindingExpression(name, x => x.Text);
        }

        ~NewWindow()
        {
        }
    }
}
