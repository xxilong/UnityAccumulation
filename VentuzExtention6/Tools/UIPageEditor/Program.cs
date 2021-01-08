using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UIPageEditor
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if(args.Length < 1 && IsAdministrator())
            {
                SetFileOpen();
            }
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        static void SetFileOpen()
        {
            string keyName = "UIPageEditor";
            string keyValue = "UI Page Editor";

            RegistryKey key;
            key = Registry.ClassesRoot.CreateSubKey(keyName);
            key.SetValue("", keyValue);
            key = key.CreateSubKey("shell");
            key = key.CreateSubKey("open");
            key = key.CreateSubKey("command");
            key.SetValue("", System.Reflection.Assembly.GetEntryAssembly().Location + " %1");

            keyName = ".uipage"; //相应后缀
            keyValue = "UIPageEditor";
            key = Registry.ClassesRoot.CreateSubKey(keyName);
            key.SetValue("", keyValue);
        }

        static bool IsAdministrator()
        {
            bool result;
            try
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                result = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch
            {
                result = false;
            }
            return result;
        }
    }
}
