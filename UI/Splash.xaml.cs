using Darked.Classes;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Darked
{
    /// <summary>
    /// Interaction logic for Splash.xaml
    /// </summary>
    ///

#pragma warning disable EF2705 // Invalid feature scope.

    [Obfuscation(Feature = "code control flow obfuscation", Exclude = false)]
#pragma warning restore EF2705 // Invalid feature scope.
    public partial class Splash : Window
    {
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        private static extern bool CheckRemoteDebuggerPresent(IntPtr hProcess, ref bool isDebuggerPresent);

//snip security

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        public Splash()
        {
            //Anti debugger security snip
            //
            InitializeComponent();
            designMethods = new InterfaceDesign();
            rand = new Random();
        }

        private readonly InterfaceDesign designMethods;
        private readonly Random rand;
        private bool loading = true;
        private Discord.EventHandlers handlers;
        private Discord.RichPresence presence;
        private string Version = "3.1.8A";

        private async void SplashScreen_OnLoaded(object sender, RoutedEventArgs e)
        {
            //UI
            MainWindow exploit = new MainWindow();
            exploit.Hide();
            await Task.Delay(100);
            //DiscordRpc
            presence.details = "Darked (Beta)";
            presence.state = "Version: " + Version;
            presence.largeImageKey = "1";
            presence.largeImageText = "Created by Void, Trollicus and Versage.";
            presence.smallImageKey = "2";
            presence.smallImageText = "";
            string clientId = "734377438439800833";
            bool isNumeric = ulong.TryParse(clientId, out ulong n);
            if (!isNumeric)
            {
                return;
            }

            this.Initialize(clientId);
            //potential fix but needs improvement (makes settings crash)
            Credits newwindow = new Credits();
            newwindow.Close();
            //potential fix but needs improvement (makes settings crash)
            Script newwindow2 = new Script();
            newwindow2.Close();
            //Discord joiner
            try
            {
                if ((bool)Properties.Settings.Default["FirstStart"] == true)
                {
                    Process.Start("discord:///DcpTBB5");
                    Properties.Settings.Default["FirstStart"] = false;
                    Properties.Settings.Default.Save();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Darked ran into a error inviting you to our discord server.");
            }
            // do all loading here //
            new Thread(async () =>
            {
                Thread.CurrentThread.IsBackground = true;
                string hexFrom = "#FFFFFF";
                string hexTo = $"#{rand.Next(0x1000000):X6}";
                while (loading)
                {
                    var @from = hexFrom;
                    var to = hexTo;
                    hexFrom = hexTo;
                    hexTo = $"#{rand.Next(0x1000000):X6}";
                    await Task.Delay(1000);
                }
            }).Start();
            foreach (FrameworkElement element in logos.Children)
            {
                designMethods.FadeIn(element);
            }
            //Clean up
            minimizeMemory();
            designMethods.Shift(loadLogo, loadLogo.Margin, new Thickness(262, 62, 262, 98));
            designMethods.Shift(loadText, loadText.Margin, new Thickness(270, 248, 255, 60));
            designMethods.Shift(statusText, statusText.Margin, new Thickness(0, 255, 0, 20));
            await Task.Delay(1000);
            designMethods.Resize(loadEllipse, 300, 300);
            //Clean up
            minimizeMemory();
            Discord.UpdatePresence(ref presence);
            handlers = new Discord.EventHandlers();
            Discord.Initialize(clientId, ref handlers, true, null);
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

            statusText.Content = "Version: " + Version;

            await Task.Delay(1300);
            designMethods.Shift(loadLogo, loadLogo.Margin, new Thickness(262, 42, 262, 118));
            designMethods.Shift(loadText, loadText.Margin, new Thickness(270, 228, 255, 80));
            designMethods.Shift(statusText, statusText.Margin, new Thickness(0, 235, 0, 40));
            foreach (FrameworkElement element in logos.Children)
            {
                designMethods.FadeOut(element);
            }
            try
            {
                designMethods.Resize(loadEllipse, 1000, 1000);
            }
            catch (Exception)
            {
            }
            minimizeMemory();
            loadLogo.Opacity = 0;
            await Task.Delay(1200);
            designMethods.Resize(loadEllipse, 0, 0);
            loading = false;
            await Task.Delay(1);
            exploit.Show();
            this.Focus();
            await Task.Delay(1200);
            minimizeMemory();
            Close();
        }

       //snip security feature

        private static void minimizeMemory()
        {
            GC.Collect(GC.MaxGeneration);
            GC.WaitForPendingFinalizers();
            SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle,
                (UIntPtr)0xFFFFFFFF, (UIntPtr)0xFFFFFFFF);
        }

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetProcessWorkingSetSize(IntPtr process,
       UIntPtr minimumWorkingSetSize, UIntPtr maximumWorkingSetSize);

        private void Initialize(string clientId)
        {
            handlers = new Discord.EventHandlers();
            Discord.Initialize(clientId, ref handlers, true, null);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (FrameworkElement element in logos.Children)
            {
                designMethods.FadeOut(element);
            }
        }
    }
}