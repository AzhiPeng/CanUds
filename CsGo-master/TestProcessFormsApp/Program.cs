using JLRScan.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestProcessFormsApp.Log;

namespace TestProcessFormsApp
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //处理UI线程异常  
            Application.ThreadException += ExceptionEventHandler;
            //处理非UI线程异常   
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new WinForm());
        }
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var error = e.ExceptionObject as Exception;
            var str = error != null ? string.Format("Application UnhandledException:{0};\n\r堆栈信息:{1}", error.Message, error.StackTrace) : string.Format("Application UnhandledError:{0}", e);
            LogHelper.Warn(str);
            MessageBox.Show("发生错误，请查看程序日志！", "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public static void ExceptionEventHandler(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
        //    ErrHdl(e.Exception.Message);
            LogHelperError.Error(string.Format("{1}\r\n{2}", e.GetType().ToString(), e.Exception.Message, e.Exception.StackTrace));
        }
    }
}
