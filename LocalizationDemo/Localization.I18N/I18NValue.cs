using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Localization.I18N
{
    abstract class I18NValue
    {
        class I18NValueString : I18NValue
        {
            private readonly string value;
            public I18NValueString(string value)
            {
                this.value = value;
            }
            public override string GetMultiConditionValue(out string[] convertedParams, params string[] @params)
            {
                convertedParams = @params;
                return value;
            }

            public override string GetAllValues()
            {
                return value;
            }
        }
        class I18NValuePlurals : I18NValue
        {
            enum PluralsQuantity
            {
                Unknow,
                Zero,
                One,
                Two,
                Few,
                Many,
                Other
            }
            class PluralsBlock
            {
                public PluralsQuantity Quantity { get; set; }
                public string Value { get; set; }
            }

            private readonly IEnumerable<PluralsBlock> pluralsBlock;

            public I18NValuePlurals(Dictionary<string, string>[] pluralsDictionary)
            {
                pluralsBlock = pluralsDictionary.Select(p => new PluralsBlock()
                {
                    Quantity = p["Quantity"] switch
                    {
                        nameof(PluralsQuantity.Zero) => PluralsQuantity.Zero,
                        nameof(PluralsQuantity.One) => PluralsQuantity.One,
                        nameof(PluralsQuantity.Two) => PluralsQuantity.Two,
                        nameof(PluralsQuantity.Few) => PluralsQuantity.Few,
                        nameof(PluralsQuantity.Many) => PluralsQuantity.Many,
                        _ => PluralsQuantity.Other
                    },
                    Value = p["Value"]
                });
            }

            public override string GetMultiConditionValue(out string[] convertedParams, params string[] @params)
            {
                convertedParams = @params;
                if (@params.Length >= 1 && int.TryParse(@params[0], out int number))
                {
                    switch (number)
                    {
                        case int x when x == 0:
                            return pluralsBlock.FirstOrDefault(t => t.Quantity == PluralsQuantity.Zero)?.Value;
                        case int x when x == 1:
                            return pluralsBlock.FirstOrDefault(t => t.Quantity == PluralsQuantity.One)?.Value;
                        case int x when x == 2:
                            return pluralsBlock.FirstOrDefault(t => t.Quantity == PluralsQuantity.Two)?.Value;
                        case int x when x > 1 && x < 10:
                            return pluralsBlock.FirstOrDefault(t => t.Quantity == PluralsQuantity.Few)?.Value;
                        case int x when x >= 10:
                            return pluralsBlock.FirstOrDefault(t => t.Quantity == PluralsQuantity.Many)?.Value;
                    }
                }
                return pluralsBlock.FirstOrDefault(t => t.Quantity == PluralsQuantity.Other)?.Value;
            }

            public override string GetAllValues()
            {
                return string.Join("\r\n", pluralsBlock.Select(t => t.Value));
            }
        }
        class I18NValueTime : I18NValue
        {
            enum TimeUnit
            {
                Sec,
                Min,
                Other
            }
            class TimeBlock
            {
                public TimeUnit Unit { get; set; }
                public string Value { get; set; }
            }

            private readonly IEnumerable<TimeBlock> timeBlocks;

            public I18NValueTime(Dictionary<string, string>[] timeDictionary)
            {
                timeBlocks = timeDictionary.Select(t => new TimeBlock()
                {
                    Unit = t["Unit"] switch
                    {
                        nameof(TimeUnit.Sec) => TimeUnit.Sec,
                        nameof(TimeUnit.Min) => TimeUnit.Min,
                        _ => TimeUnit.Other,
                    },
                    Value = t["Value"]
                });
            }
            public override string GetMultiConditionValue(out string[] convertedParams, params string[] @params)
            {
                convertedParams = @params;
                if (@params.Length >= 1 && int.TryParse(@params[0], out int seconds))
                {
                    switch (seconds)
                    {
                        case int x when x < 60:
                            return timeBlocks.FirstOrDefault(t => t.Unit == TimeUnit.Sec)?.Value;
                        case int x when x >= 60:
                            convertedParams = new string[] { ((int)((double)seconds / 60)).ToString() }.Concat(@params.Skip(1)).ToArray();
                            return timeBlocks.FirstOrDefault(t => t.Unit == TimeUnit.Min)?.Value;
                    }
                }
                return timeBlocks.FirstOrDefault(t => t.Unit == TimeUnit.Sec)?.Value; ;
            }

            public override string GetAllValues()
            {
                return string.Join("\r\n", timeBlocks.Select(t => t.Value));
            }
        }

        /// <summary>
        /// Some key mapping string not only one value. like 'Time' and 'Plurals'.
        /// </summary>
        /// <param name="convertedParams"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public abstract string GetMultiConditionValue(out string[] convertedParams, params string[] @params);
        public abstract string GetAllValues();

        public static I18NValue CreateI18NValue(Dictionary<string, object> dictionary)
        {
            if (dictionary.ContainsKey("Value"))
            {
                var value = dictionary["Value"].ToString();
                return new I18NValueString(value);
            }
            if (dictionary.ContainsKey("Plurals"))
            {
                var plurals = JsonSerializer.Deserialize<Dictionary<string, string>[]>(dictionary["Plurals"].ToString());
                return new I18NValuePlurals(plurals);
            }
            if (dictionary.ContainsKey("Time"))
            {
                var time = JsonSerializer.Deserialize<Dictionary<string, string>[]>(dictionary["Time"].ToString());
                return new I18NValueTime(time);
            }
            return null;
        }
    }
}
