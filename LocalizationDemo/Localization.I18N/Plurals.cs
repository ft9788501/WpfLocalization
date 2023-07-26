using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localization.I18N
{
    internal class Plurals
    {
        public string? Zero { get; set; }
        public string? One { get; set; }
        public string? Two { get; set; }
        public string? Few { get; set; }
        public string? Many { get; set; }
        public string? Other { get; set; }

        public static Plurals? LoadFromJson(string? jsonContent)
        {
            if (jsonContent == null)
            {
                return null;
            }
            try
            {
                var plurals = JsonConvert.DeserializeObject<Plurals>(jsonContent);
                return plurals;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private Plurals() { }
    }
}
