using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace Darked
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    ///
#pragma warning disable EF2705 // Invalid feature scope.

    [Obfuscation(Feature = "code control flow obfuscation", Exclude = false)]
#pragma warning restore EF2705 // Invalid feature scope.
    public partial class SettingsWindow : Window
    {
        private static SettingsWindow _instance;

        public static SettingsWindow GetInstance()
        {
            if (_instance == null) _instance = new SettingsWindow();
            return _instance;
        }

        public SettingsWindow()
        {
            InitializeComponent();
            //settings
            base.Topmost = Settings.Instance.TopMost;
            //Application name
            var chars = "`~{}[];:'|/<>.,+=-_*!@#$%^&*()ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[10];
            var random2 = new Random();
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random2.Next(chars.Length)];
            }
            var finalString = new String(stringChars);
            this.Title = (finalString);
            this.TopMostSetting.IsChecked = new bool?(Settings.Instance.TopMost);
            if ((bool)Properties.Settings.Default["Txt"] == true)
            {
                SaveText.IsChecked = true;
            }
            else
            {
                SaveText.IsChecked = false;
            }
            this.AutoAttachSetting.IsChecked = new bool?(Settings.Instance.AutoAttach);
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        public static SettingsWindow Instance;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Process[] processesByName = Process.GetProcessesByName("RobloxPlayerBeta");
            for (int i = 0; i < processesByName.Length; i++)
            {
                processesByName[i].Kill();
            }
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            if (SaveText.IsChecked == true)
            {
                Properties.Settings.Default["Txt"] = true;
                Properties.Settings.Default.Save();
            }
            else
            {
                Properties.Settings.Default["Txt"] = false;
                Properties.Settings.Default.Save();
            }
            //
            Settings.Instance.AutoAttach = this.AutoAttachSetting.IsChecked.GetValueOrDefault();
            Settings.Instance.TopMost = this.TopMostSetting.IsChecked.GetValueOrDefault();
            Settings.Instance.Apply();
            this.Close();
        }

        private void SaveText_Checked(object sender, RoutedEventArgs e)
        {
        }

        private void SaveText_Unchecked(object sender, RoutedEventArgs e)
        {
        }
    }
}