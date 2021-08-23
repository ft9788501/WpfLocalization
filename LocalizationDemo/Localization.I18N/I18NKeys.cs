using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Localization.I18N
{
    public enum I18NKeys
    {
        /// <summary>title</summary>
        [Description("title")] Title,
        /// <summary>name</summary>
        [Description("name")] Name,
        /// <summary>12</summary>
        [Description("12")] Age,
        /// <summary>MessageBox</summary>
        [Description("MessageBox")] MessageBox,
        /// <summary>Show New Window</summary>
        [Description("Show New Window")] ShowNewWindow,
        /// <summary>newWindowTitle</summary>
        [Description("newWindowTitle")] NewWindowTitle,
        /// <summary>Success</summary>
        [Description("Success")] Success,
        /// <summary>this is string start</summary>
        [Description("this is string start")] String1,
        /// <summary>this is string end</summary>
        [Description("this is string end")] String2,
        /// <summary>is Speaking</summary>
        [Description("is Speaking")] SpeakingIndicatorSpeaking,
        /// <summary>is Reconnecting</summary>
        [Description("is Reconnecting")] SpeakingIndicatorReconnecting,
        /// <summary>is Reconnecting</summary>
        [Description("This is a {old device} and will switch to {new device} .")] FormatString,
    }
}
