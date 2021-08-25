//TT generate can not run at build time, this is a know issue:https://github.com/mono/t4/issues/47
//Pre-build event:"$(DevEnvDir)TextTransform.exe" "$(ProjectDir)TextTemplate1.tt" 
// This is an auto-generated file.
using System.ComponentModel;

namespace Localization.I18N
{
    // This is an auto-generated file. Do not modify this file manually, if you need to modify the contents, Please motify the I18NKeys.tt file.
    public enum I18NKeys
    {
        /// <summary>title</summary>
        Title,
        /// <summary>name</summary>
        Name,
        /// <summary>12</summary>
        Age,
        /// <summary>MessageBox</summary>
        MessageBox,
        /// <summary>Show New Window</summary>
        ShowNewWindow,
        /// <summary>newWindowTitle</summary>
        NewWindowTitle,
        /// <summary>Success</summary>
        Success,
        /// <summary>this is string start</summary>
        String1,
        /// <summary>this is string end</summary>
        String2,
        /// <summary>is Speaking</summary>
        SpeakingIndicatorSpeaking,
        /// <summary>is Reconnecting</summary>
        SpeakingIndicatorReconnecting,
        /// <summary>This is a {old device} and will switch to {new device} .</summary>
        FormatString,
        /// <summary>Version ({version number}).</summary>
        Version,
    }
}