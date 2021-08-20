using Localization.I18N;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public NewWindow()
        {
            InitializeComponent();
            I18NKeys.NewWindowTitle.BindingExpression(this, x => x.Title);
            I18NKeys.Name.BindingExpression(name, x => x.Text);
        }

        ~NewWindow()
        {
        }
    }
}
