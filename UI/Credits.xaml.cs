using System;
using System.Windows;
using System.Windows.Input;

namespace Darked
{
    public partial class Credits : Window
    {
        private static Credits _instance;

        public static Credits GetInstance()
        {
            if (_instance == null) _instance = new Credits();
            return _instance;
        }

        public Credits()
        {
            InitializeComponent();
            Credits.Instance = this;
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
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        public static Credits Instance;
    }
}