using Localization.I18N;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            I18NManager.SaveAsJson("x");
            //bool f = true;
            //Task.Run(async ()=> 
            //{
            //    while (true)
            //    {
            //        await Task.Delay(3000);
            //        if (f)
            //        {
            //            I18N.SetCulture("en-US");
            //        }
            //        else
            //        {
            //            I18N.SetCulture("zh-CN");
            //        }
            //        f = !f;
            //    }
            //});
        }
    }
}
