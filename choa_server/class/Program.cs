using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Threading;

static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        try
        {
            Application.ApplicationExit += (s, e) => final();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Thread.CurrentThread.Name = "Main";
            //ThreadPool.SetMinThreads(20, 20);
            Application.Run(new frmMain());
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Main():\n{ex.Message}\n\n{ex.StackTrace}");

            ExitApp = true;
            Application.Exit();
            final();
        }
        finally
        {
            final();
        }
    }

    static void final() { try { CGlobal.DaqBase?.Dispose(); } catch { }; }
        
    public static bool ExitApp = false;

}

