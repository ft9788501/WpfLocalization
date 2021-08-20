using Localization.I18N;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace LocalizationDemo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //I18N.SaveAsJson(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @$"..\..\..\Properties\I18N\en-US.json"));
            I18N.SaveAsJson("x");
        }
    }
}
