using Avalonia.Controls;
using Avalonia.Input;
using Localization.I18N;
using System.Globalization;

namespace AvaloniaApplication1.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            I18NManager.CurrentCulture= CultureInfo.GetCultureInfo("zh-CN");
        }
    }
}