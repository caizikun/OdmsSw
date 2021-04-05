using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.CompilerServices;
using System.Reflection;
using Free302.MyLibrary.SwInfo;

[assembly: InternalsVisibleTo("Tester")]


static class Program
{
    [STAThread]
    static void Main()
    {
        try
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //pcTestForm = new PcTest();
            MainForm = new MainForm();
            FSwInfoGrid swForm = new FSwInfoGrid();
            //MainForm.Text = $"{swForm.buildProductString()} - {swForm.buildVersionString()}";
            swForm.Dispose();
            Application.Run(MainForm);
            //Application.Run(pcTestForm);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Program.Main():\n{ex.Message}\n\n{ex.StackTrace}");
        }
    }


    #region ---- Form Closing Event ----

    //public static PcTest pcTestForm;
    public static MainForm MainForm;
    public static bool AppicationBeingQuit = false;

    public static bool CanIBeClosed(FormClosingEventArgs e)
    {
        e.Cancel = e.CloseReason == CloseReason.MdiFormClosing; ;
        return !e.Cancel;
    }

    #endregion


    public static void SetValue(this Neon.Aligner.CStageAbsPos  position, double[] values)
    {
        if (values.Length == 6)
        {
            position.x = values[0];
            position.y = values[1];
            position.z = values[2];
            position.tx = values[3];
            position.ty = values[4];
            position.tz = values[5];
        }

        else if (values.Length == 1) position.x = values[0];

        else throw new IndexOutOfRangeException($"Program.SetValue():\nLenght of values={values.Length} != 6|1");

    }

}//class