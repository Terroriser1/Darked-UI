using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.IO;
using System.IO.Pipes;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Darked
{
    internal class Pipes
    {
#pragma warning disable EF2705 // Invalid feature scope.

        [Obfuscation(Feature = "code control flow obfuscation", Exclude = false)]
#pragma warning restore EF2705 // Invalid feature scope.
        public static string DLL = "Darked.dll";

        public static string Output = ""; //snipped security reasons

        /*public static void Encryption() snipped private and public key
        {
            try
            {
                string textToDecrypt = Output;
                string ToReturn = "";
                string publickey = ""; ;//may need to change to utf8
                string privatekey = "";
                byte[] privatekeyByte = { };
                privatekeyByte = System.Text.Encoding.UTF32.GetBytes(privatekey);
                byte[] publickeybyte = { };
                publickeybyte = System.Text.Encoding.UTF32.GetBytes(publickey);
                MemoryStream ms = null;
                CryptoStream cs = null;
                byte[] inputbyteArray = new byte[textToDecrypt.Replace(" ", "+").Length];
                inputbyteArray = Convert.FromBase64String(textToDecrypt.Replace(" ", "+"));
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    ms = new MemoryStream();
                    cs = new CryptoStream(ms, des.CreateDecryptor(publickeybyte, privatekeyByte), CryptoStreamMode.Write);
                    cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                    cs.FlushFinalBlock();
                    Encoding encoding = Encoding.UTF32;
                    ToReturn = encoding.GetString(ms.ToArray());
                }
                Output = ToReturn;
            }
            catch (Exception ae)
            {
                throw new Exception(ae.Message, ae.InnerException);
            }
        }*/

        public static void OutputPipe(string command)
        {
            if (NamedPipeExist(Output))
            {
                new Thread(() =>
                {
                    using (NamedPipeClientStream namedPipeClientStream = new NamedPipeClientStream(".", Output, PipeDirection.Out))
                    {
                        namedPipeClientStream.Connect();
                        using (StreamWriter streamWriter = new StreamWriter(namedPipeClientStream))
                        {
                            streamWriter.Write(command);
                            streamWriter.Dispose();
                        }
                        namedPipeClientStream.Dispose();
                    }
                }).Start();
            }
        }

        public static bool NamedPipeExist(string pipeName)
        {
            bool result;
            try
            {
                int timeout = 0;
                if (!WaitNamedPipe(Path.GetFullPath(string.Format("\\\\\\\\.\\\\pipe\\\\{0}", pipeName)), timeout))
                {
                    int lastWin32Error = Marshal.GetLastWin32Error();
                    if (lastWin32Error == 0)
                    {
                        result = false;
                        return result;
                    }
                    if (lastWin32Error == 2)
                    {
                        result = false;
                        return result;
                    }
                }
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool WaitNamedPipe(string name, int timeout);
    }
}