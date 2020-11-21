using Microsoft.Win32;
using System;
using System.Reflection;

namespace Darked
{
#pragma warning disable EF2705 // Invalid feature scope.

    [Obfuscation(Feature = "code control flow obfuscation", Exclude = false)]
#pragma warning restore EF2705 // Invalid feature scope.
    internal class Settings
    {
        private static Settings instance;

        public static Settings Instance
        {
            get
            {
                Settings result;
                if ((result = Settings.instance) == null)
                {
                    result = (Settings.instance = new Settings());
                }
                return result;
            }
        }

        public Settings()
        {
            this.registryKey = Registry.CurrentUser.OpenSubKey("Software\\Darked", true);
            if (this.registryKey == null)
            {
                this.registryKey = Registry.CurrentUser.CreateSubKey("Software\\Darked");
            }
        }

        public void Apply()
        {
            MainWindow.Instance.Topmost = this.TopMost;
            Credits.Instance.Topmost = this.TopMost;
            Script.Instance.Topmost = this.TopMost;
        }

        public bool AutoAttach
        {
            get
            {
                return Convert.ToBoolean(this.registryKey.GetValue("AutoAttach"));
            }
            set
            {
                this.registryKey.SetValue("AutoAttach", value);
            }
        }

        public bool TopMost
        {
            get
            {
                return Convert.ToBoolean(this.registryKey.GetValue("TopMost"));
            }
            set
            {
                this.registryKey.SetValue("TopMost", value);
            }
        }

        private RegistryKey registryKey;
    }
}