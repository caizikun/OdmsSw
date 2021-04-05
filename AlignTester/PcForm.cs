using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Neon.Aligner;

namespace AlignTester
{
    public partial class PcForm : Form
    {
        public PcForm()
        {
            InitializeComponent();

            Load += form_Load;
        }

        private void form_Load(object sender, EventArgs e)
        {
            try
            {
                uiPol_Read.Enabled = false;
                mPc = new PcPsg100();

                mPol = new Dictionary<RadioButton, Action<double>>();
                mPol.Add(uiPol_H, mPc.SetToLinearHorizontal);
                mPol.Add(uiPol_V, mPc.SetToLinearVertical);
                mPol.Add(uiPol_p45, mPc.SetToLinearDiagonal);
                mPol.Add(uiPol_n45, mPc.SetToNegaLinearDiagonal);
                mPol.Add(uiPol_R, mPc.SetToRHcircular);
                mPol.Add(uiPol_L, mPc.SetToLHcircular);

                uiPol_H.CheckedChanged += pol_CheckedChanged;
                uiPol_V.CheckedChanged += pol_CheckedChanged;
                uiPol_p45.CheckedChanged += pol_CheckedChanged;
                uiPol_n45.CheckedChanged += pol_CheckedChanged;
                uiPol_R.CheckedChanged += pol_CheckedChanged;
                uiPol_L.CheckedChanged += pol_CheckedChanged;
            }
            catch (Exception ex)
            {
                showError(nameof(form_Load), ex);
            }
        }

        private void pol_CheckedChanged(object sender, EventArgs e)
        {
            mSelectedPol = sender as RadioButton;
        }

        IpolController mPc;
        Dictionary<RadioButton, Action<double>> mPol;
        RadioButton mSelectedPol;

        private void uiOpen_Click(object sender, EventArgs e)
        {
            try
            {
                PcPsg100 pc = mPc as PcPsg100;
                pc.Connect(int.Parse(uiGpib.Text));
            }
            catch (Exception ex)
            {
                showError(nameof(uiOpen_Click), ex);
            }
        }
        void showError(string header, Exception ex)
        {
            MessageBox.Show($"{header}\n{ex.Message}\n\n{ex.StackTrace}");
        }

        private void uiPol_Read_Click(object sender, EventArgs e)
        {
            
        }

        private void uiPol_Write_Click(object sender, EventArgs e)
        {
            try
            {
                mPol[mSelectedPol].Invoke(0);
            }
            catch (Exception ex)
            {
                showError(nameof(uiPol_Write_Click), ex);
            }
        }
    }//class
}
