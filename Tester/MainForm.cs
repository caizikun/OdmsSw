using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tester
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            uiNewLine.SelectedIndex = 0;

            uiNewLine.Font = uiCom.Font;
            uiCmd.Font = uiCom.Font;

            uiCmd.KeyDown += UiCmd_KeyDown;
        }

        

        void log(object message)
        {
            uiLog.AppendText($"{message}\n");
        }

        private void uiNeon_CheckedChanged(object sender, EventArgs e)
        {
            uiBps.Value = uiNeon.Checked ? 115200 : 9600;
            uiNewLine.Text = uiNeon.Checked ? "\r" : "\n";
        }

        private void uiRun_Click(object sender, EventArgs e)
        {
            try
            {
                Program.InitOsw(uiCom.Value, uiAlign.Value, uiTls.Value, uiNeon.Checked);
            }
            catch(Exception ex)
            {
                log(ex);
            }
        }

        private void uiRunSerial_Click(object sender, EventArgs e)
        {
            try
            {
                Program.InitSerial(uiCom.Value, uiBps.Value, uiNewLine.Text);
            }
            catch (Exception ex)
            {
                log(ex);
            }
        }

        private void uiCmd_TextChanged(object sender, EventArgs e)
        {

        }

        private void UiCmd_KeyDown(object sender, KeyEventArgs e)
        {
            Program.query(uiCmd.Text, uiRemoveNL.Checked);
        }

    }//class
}
