using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Darked
{
#pragma warning disable EF2705 // Invalid feature scope.

    [Obfuscation(Feature = "code control flow obfuscation", Exclude = false)]
#pragma warning restore EF2705 // Invalid feature scope.
    public partial class Script : Window
    {
        private static Script _instance;

        public static Script GetInstance()
        {
            if (_instance == null) _instance = new Script();
            return _instance;
        }

        private void SelectScript(ValueTuple<string, string, string, string> script)
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(script.Item2);
            bitmapImage.EndInit();
            this.ScriptImage.Source = bitmapImage;
            this.Output.Text = script.Item3;
            this.SelectedScript = script.Item4;
        }
//snip security feature

        public Script()
        {
            InitializeComponent();
            Script.Instance = this;
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
            //Settings
            base.Topmost = Settings.Instance.TopMost;
            //Scripts
            this.SelectedScript = "";
            //Security
            AntiHTTP();
            List<ValueTuple<string, string, string, string>> list = new List<ValueTuple<string, string, string, string>>
            {
                //snip scripts 
            };
            StackPanel stackPanel = new StackPanel();
            using (List<ValueTuple<string, string, string, string>>.Enumerator enumerator = list.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    ValueTuple<string, string, string, string> script = enumerator.Current;
                    System.Windows.Controls.Button button = new System.Windows.Controls.Button
                    {
                        Content = script.Item1
                    };
                    button.Click += delegate (object sender, RoutedEventArgs e)
                    {
                        this.SelectScript(script);
                    };
                    stackPanel.Children.Add(button);
                }
            }
            this.SHContainer.Child = stackPanel;
            this.SelectScript(list[0]);
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

        public static Script Instance;

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void ListBoxItem_Selected(object sender, RoutedEventArgs e)
        {
        }

        private string SelectedScript;

        private void Execute_Click(object sender, RoutedEventArgs e)
        {
            Pipes.OutputPipe(this.SelectedScript);
        }

        // internal Border SHContainer;
    }
}