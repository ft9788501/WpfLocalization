using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Localization.I18N
{
    public enum I18NKeys
    {
        [Description("title")] Title,
        [Description("name")] Name,
        [Description("12")] Age,
        [Description("MessageBox")] MessageBox,
        [Description("Show New Window")] ShowNewWindow,
        [Description("newWindowTitle")] NewWindowTitle,
        [Description("Success")] Success,
        [Description("this is string start")] String1,
        [Description("this is string end")] String2,
        [Description("is Speaking")] SpeakingIndicatorSpeaking,
        [Description("is Reconnecting")] SpeakingIndicatorReconnecting,
    }
}
