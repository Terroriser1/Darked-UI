using Darked.Classes;
using DiscordRPC;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;

namespace Darked
{
#pragma warning disable EF2705 // Invalid feature scope.

    [Obfuscation(Feature = "code control flow obfuscation", Exclude = false)]
#pragma warning restore EF2705 // Invalid feature scope.
    public partial class MainWindow : Window
    {
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        private static extern bool CheckRemoteDebuggerPresent(IntPtr hProcess, ref bool isDebuggerPresent);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsDebuggerPresent();

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        private static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        private string Version = "Darked Version: 3.1.8A";
        public DiscordRpcClient client;

        public MainWindow()
        {
            //Cleanup
            minimizeMemory();
            //Initialize
            InitializeComponent();
            //Settings
            MainWindow.Instance = this;
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
            //Version//
            LabelVersion.Content = Version;
            //Watcher
            try
            {
                FileSystemWatcher watcher = new FileSystemWatcher(AppDomain.CurrentDomain.BaseDirectory + "Scripts");

                watcher.EnableRaisingEvents = true;
                watcher.IncludeSubdirectories = true;

                watcher.Changed += Watcher_Changed;
                watcher.Created += Watcher_Created;
                watcher.Deleted += Watcher_Deleted;
                watcher.Renamed += Watcher_Renamed;
            }
            catch (Exception)
            {
                output.Document.Blocks.Add(new Paragraph(new Run("Watcher failed.")));
            }
            //Avalon Editor//
            textEditor.Options.EnableEmailHyperlinks = false;
            textEditor.Options.EnableHyperlinks = false;
            textEditor.Options.AllowScrollBelowDocument = true;
            textEditor.TextArea.TextView.ElementGenerators.Add(new Editor());
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                using (Stream s = assembly.GetManifestResourceStream("Darked.data.xshd"))
                {
                    using (XmlTextReader reader = new XmlTextReader(s))
                    {
                        textEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                    }
                }
            }
            catch (Exception)
            {
                output.Document.Blocks.Add(new Paragraph(new Run("Syntax data failed to load.")));
            }
            //Load text
            if ((bool)Properties.Settings.Default["Txt"] == true)
            {
                textEditor.Text = Properties.Settings.Default.Text;
            }
            else
            {
                textEditor.Text = "";
            }
            //Load scriptlist for launch
            Reload();
            //Auto inject
            int attachedPid = 0;
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(500.0);
            dispatcherTimer.Tick += delegate (object sender, EventArgs e)
            {
                if (!Settings.Instance.AutoAttach)
                {
                    return;
                }
                Process[] processesByName = Process.GetProcessesByName("RobloxPlayerBeta");
                if (processesByName.Length != 1 || processesByName[0].Id == attachedPid)
                {
                    return;
                }
                else
                {
                    output.Document.Blocks.Clear();
                    output.Document.Blocks.Add(new Paragraph(new Run("Disconnected from client.")));
                }
                if (processesByName[0].MainWindowHandle == IntPtr.Zero)
                {
                    return;
                }
                else
                {
                    output.Document.Blocks.Clear();
                    output.Document.Blocks.Add(new Paragraph(new Run("Disconnected from client.")));
                }
                attachedPid = processesByName[0].Id;
                //Injector//
                Injector();
            };
            dispatcherTimer.Start();
            //Clean up
            minimizeMemory();
        }

        //make this clean cpu and chache upcoming
        private static void minimizeMemory()
        {
            GC.Collect(GC.MaxGeneration);
            GC.WaitForPendingFinalizers();
            SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle,
                (UIntPtr)0xFFFFFFFF, (UIntPtr)0xFFFFFFFF);
        }

        public void Injector()
        {
            try
            {
                Process[] pname = Process.GetProcessesByName("RobloxPlayerBeta");
                if (pname.Length == 0)
                {
                    output.Document.Blocks.Clear();
                    output.Document.Blocks.Add(new Paragraph(new Run("Could not find client.")));
                }
                else
                {
                    Process process = Process.GetProcessesByName("RobloxPlayerBeta")[0];
                    if (process != null)
                    {
                        InjectAsync(AppDomain.CurrentDomain.BaseDirectory + "/Darked.dll");
                        output.Document.Blocks.Clear();
                        output.Document.Blocks.Add(new Paragraph(new Run("Connected with client.")));
                    }
                }
            }
            catch (Exception)
            {
                output.Document.Blocks.Clear();
                output.Document.Blocks.Add(new Paragraph(new Run("Could not connect with client.")));
            }
        }

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetProcessWorkingSetSize(IntPtr process,
       UIntPtr minimumWorkingSetSize, UIntPtr maximumWorkingSetSize);

        public void Reload()
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    Listbox1.Items.Clear();
                    PopulateListBox(Listbox1, (AppDomain.CurrentDomain.BaseDirectory + "Scripts"), "*.txt");
                    PopulateListBox(Listbox1, (AppDomain.CurrentDomain.BaseDirectory + "Scripts"), "*.lua");
                });
            }
            catch (Exception)
            {
                output.Document.Blocks.Add(new Paragraph(new Run("Scriptlist failed to load.")));
            }
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            Reload();
        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            Reload();
        }

        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            Reload();
        }

        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            Reload();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        public static void PopulateListBox(ListBox LuaScripts, string Folder, string FileType)
        {
            DirectoryInfo dinfo = new DirectoryInfo(Folder);
            FileInfo[] Files = dinfo.GetFiles(FileType);
            foreach (FileInfo file in Files)
            {
                LuaScripts.Items.Add(file.Name);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Text = textEditor.Text;
            Properties.Settings.Default.Save();
            Discord.Shutdown();
            Process.GetCurrentProcess().Kill();
            Environment.Exit(0);
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            bool isWindowOpen = false;

            foreach (Window w in Application.Current.Windows)
            {
                if (w is Credits)
                {
                    isWindowOpen = true;
                    w.Activate();
                }
            }

            if (!isWindowOpen)
            {
                Credits newwindow = new Credits();
                newwindow.Show();
            }
        }

        private void LoadItem_Click(object sender, RoutedEventArgs e)
        {
            if (Listbox1.SelectedIndex == -1) return;

            try
            {
                string str = (AppDomain.CurrentDomain.BaseDirectory + "Scripts/");
                object selectedItem = this.Listbox1.SelectedItem;
                textEditor.Text = File.ReadAllText(str + ((selectedItem != null) ? selectedItem.ToString() : null));
                this.Listbox1.UnselectAll();
            }
            catch (Exception)
            {
                output.Document.Blocks.Add(new Paragraph(new Run("Scriptlist failed to load.")));
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            textEditor.Text = "";
        }

        private void ReloadItem_Click_1(object sender, RoutedEventArgs e)
        {
            Listbox1.Items.Clear();
            PopulateListBox(Listbox1, (AppDomain.CurrentDomain.BaseDirectory + "Scripts"), "*.txt");
            PopulateListBox(Listbox1, (AppDomain.CurrentDomain.BaseDirectory + "Scripts"), "*.lua");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool isWindowOpen = false;

            foreach (Window w in Application.Current.Windows)
            {
                if (w is Script)
                {
                    isWindowOpen = true;
                    w.Activate();
                }
            }

            if (!isWindowOpen)
            {
                Script newwindow = new Script();
                newwindow.Show();
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            bool isWindowOpen = false;

            foreach (Window w in Application.Current.Windows)
            {
                if (w is SettingsWindow)
                {
                    isWindowOpen = true;
                    w.Activate();
                }
            }

            if (!isWindowOpen)
            {
                SettingsWindow newwindow = new SettingsWindow();
                newwindow.Show();
            }
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, EntryPoint = "GetPrivateProfileStringA", ExactSpelling = true, SetLastError = true)]
        private static extern int GetPrivateProfileString([MarshalAs(UnmanagedType.VBByRefStr)] ref string lpApplicationName, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpKeyName, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpDefault, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpReturnedString, int nSize, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool WaitNamedPipe(string name, int timeout);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, EntryPoint = "WritePrivateProfileStringA", ExactSpelling = true, SetLastError = true)]
        private static extern int WritePrivateProfileString([MarshalAs(UnmanagedType.VBByRefStr)] ref string lpApplicationName, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpKeyName, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpString, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpFileName);

        private async Task<int> InjectAsync(string dllName)
        {
            try

            {
                //prevent crash with wait
                await Task.Delay(5000);
                Process process = Process.GetProcessesByName("RobloxPlayerBeta")[0];
                if (process != null)
                {
                    if (process.MainWindowHandle.ToInt32() != 0)
                    {
                        IntPtr hProcess = MainWindow.OpenProcess(1082, false, process.Id);
                        IntPtr procAddress = MainWindow.GetProcAddress(MainWindow.GetModuleHandle("kernel32.dll"), "LoadLibraryA");
                        IntPtr intPtr = MainWindow.VirtualAllocEx(hProcess, IntPtr.Zero, (uint)((dllName.Length + 1) * Marshal.SizeOf(typeof(char))), 12288u, 4u);
                        MainWindow.WriteProcessMemory(hProcess, intPtr, Encoding.Default.GetBytes(dllName), (uint)((dllName.Length + 1) * Marshal.SizeOf(typeof(char))), out UIntPtr uintPtr);
                        MainWindow.CreateRemoteThread(hProcess, IntPtr.Zero, 0u, procAddress, intPtr, 0u, IntPtr.Zero);
                    }
                    //Pipes.Encryption();
                }
            }
            catch (Exception)
            {
                output.Document.Blocks.Add(new Paragraph(new Run("An error occurred on injection.")));
            }
            return 0;
        }

        private void Attach_Click(object sender, RoutedEventArgs e)
        {
            Injector();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            try
            {
                Pipes.OutputPipe(textEditor.Text);
            }
            catch (Exception)
            {
                output.Document.Blocks.Add(new Paragraph(new Run("An error occurred on executing.")));
            }
        }

        private void LabelVersion_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        public static MainWindow Instance;
    }
}