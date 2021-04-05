using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Free302.MyLibrary.Utility;
using System.IO;

namespace Neon.Aligner.Test
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();

            mAligner = new SurugaAligner(1);
            mAligner.CoordUpdate += aligner_CoordUpdate;

            initParam();
        }

        private void aligner_ProgressChanged(string text)
        {
            uiTimer.Text = text;
        }

        private void aligner_CoordUpdate(double[] coord)
        {
            uiCoord.Value = (decimal)coord[0];
            uiCoord.Refresh();
        }

        private void initParam()
        {
            uiParamGrid.RowHeadersVisible = false;

            var col = uiParamGrid.Columns.Add("ParamName", "Name");
            uiParamGrid.Columns[col].Width = 150;
            col = uiParamGrid.Columns.Add("ParamValue", "Value");
            uiParamGrid.Columns[col].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            uiParamGrid.Columns[col].DefaultCellStyle.Font = new Font("Consolas", 11, FontStyle.Bold);

            uiParamGrid.Rows.Add("MC COM Port#", 10);//0
            uiParamGrid.Rows.Add("Unit", 1);//1
            uiParamGrid.Rows.Add("Stroke", 20000);//1
            uiParamGrid.Rows.Add("Origin Position", 5500);//2
            uiParamGrid.Rows.Add("Home Position", 10000);//2
            uiParamGrid.Rows.Add("Origin Type", 3);//3
            uiParamGrid.Rows.Add("Resolution", 2);//4
            uiParamGrid.Rows.Add("Division Data1", 0);//5
            uiParamGrid.Rows.Add("Division Data2", 7);//6
            uiParamGrid.Rows.Add("Init Data#", 1);//7
            uiParamGrid.Rows.Add("Low Speed", "100;1000;100");//8
            uiParamGrid.Rows.Add("High Speed", "2000;15000;400");//9

            uiAxis.SelectedIndex = 0;

        }
        private void uiReadConfigFile_Click(object sender, EventArgs e)
        {
            try
            {
                ((Control)sender).Enabled = false;

                var axisIndex = uiAxis.SelectedIndex;
                if (axisIndex < 0) return;

                var fd = new OpenFileDialog();
                fd.Filter = "Xml|*.xml";
                var res = fd.ShowDialog();
                //fd.InitialDirectory = Application.StartupPath;
                if (res != DialogResult.OK) return;

                mConfigFile = fd.FileName;
                uiConfigFilePath.Text = fd.FileName;
                readConfigFile(mConfigFile, axisIndex);
            }
            catch (Exception ex)
            {
                displayError("uiReadConfigFile_Click", ex);
            }
            finally
            {
                ((Control)sender).Enabled = true;
            }
        }

        string mConfigFile;
        void readConfigFile(string filePath, int axisIndex)
        {
            var config = new XConfig(filePath);
            var g = this.uiParamGrid;

            Action<int, string> set = (r, key) => g.Rows[r].Cells[1].Value = config.GetValue(key).Split(';')[axisIndex];
            var row = 1;
            set(row++, "Unit");
            set(row++, "Stroke");
            set(row++, "OriginPos");
            set(row++, "HomePos");
            set(row++, "OriginReturnType");
            set(row++, "Resolution");
            set(row++, "Division_Data1");
            set(row++, "Division_Data2");
            set(row++, "Division_DataNo");

            string[] axisName = { "X", "Y", "Z", "TX", "TY", "TZ" };
            var speed = config.GetValue($"SpeedTable_{axisName[axisIndex]}").Split('|');
            g.Rows[row++].Cells[1].Value = speed[0];
            g.Rows[row++].Cells[1].Value = speed[1];

        }
        private void uiAxis_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mConfigFile == null) return;
            readConfigFile(mConfigFile, uiAxis.SelectedIndex);
        }

        T paramValue<T>(int rowIndex)
        {
            return uiParamGrid.Rows[rowIndex].Cells[1].Value.ToString().To<T>();
        }

        //SurugaAlignerBase mAligner;
        SurugaAligner mAligner;
        int mCurrentAxisIndex = 0;

        private void uiInitMc_Click(object sender, EventArgs e)
        {
            try
            {
                uiInitMc.Enabled = false;

                var comPorts = paramValue<string>(0).Unpack<int>().ToArray();
                
                doConfigMc();

                //mAligner.InitMc(comPorts, mConfigFile);
                mAligner.InitMc(comPorts, null);
            }
            catch (Exception ex)
            {
                displayError("uiInitMc_Click", ex);
            }
            finally
            {
                ((Control)sender).Enabled = true;
            }
        }

        

        private void doConfigMc()
        {
            var rowIndex = 1;
            mAligner.mMcUnit = new int[] { paramValue<int>(rowIndex++) };
            mAligner.mStroke = new double[] { paramValue<double>(rowIndex++) };
            mAligner.mOriginCoord = new double[] { paramValue<double>(rowIndex++) };
            mAligner.mHomeCoord = new double[] { paramValue<double>(rowIndex++) };
            mAligner.mOriginReturnType = new int[] { paramValue<int>(rowIndex++) };
            mAligner.mResolution = new double[] { paramValue<double>(rowIndex++) };
            mAligner.mResolutionLast = new double[mAligner.mResolution.Length];
            mAligner.mResolution.CopyTo(mAligner.mResolutionLast, 0);
            mAligner.mDivisionCode = new int[][] { paramValue<string>(rowIndex++).Unpack<int>().ToArray(), paramValue<string>(rowIndex++).Unpack<int>().ToArray() };
            mAligner.mDataNo = new int[] { paramValue<int>(rowIndex++) };

            //Speed Table of Division Data1
            mAligner.mSpeedTable = new double[1][][][];//axis
            mAligner.mSpeedTable[0] = new double[2][][];//data
            mAligner.mSpeedTable[0][0] = new double[][] { paramValue<string>(rowIndex++).Unpack<double>().ToArray(), paramValue<string>(rowIndex++).Unpack<double>().ToArray() };

            //Speed Table of Division Data2
            var axis = 0;
            mAligner.mSpeedTable[axis][1] = new double[2][];
            for (int s = 0; s < 2; s++)
            {
                mAligner.mSpeedTable[axis][1][s] = new double[3];
                for (int v = 0; v < 2; v++)
                    mAligner.mSpeedTable[axis][1][s][v] = mAligner.mSpeedTable[axis][0][s][v] * CsurugaseikiMc.DivisionValue[mAligner.mDivisionCode[1][axis]];
                mAligner.mSpeedTable[axis][1][s][2] = mAligner.mSpeedTable[axis][0][s][2];
            }
        }

        

        void displayError(string title, Exception ex)
        {
            MessageBox.Show($"[{title}] {ex.Message}\n\n{ex.StackTrace}");
        }
        private void uiZeroing_Click(object sender, EventArgs e)
        {
            try
            {
                ((Control)sender).Enabled = false;
                //mAligner.Origin();
                mAligner.Zeroing(mAligner.AXIS_X);
            }
            catch (Exception ex)
            {
                displayError("uiZeroing_Click", ex);
            }
            finally
            {
                ((Control)sender).Enabled = true;
            }
        }

        private void uiMoveToHome_Click(object sender, EventArgs e)
        {
            try
            {
                ((Control)sender).Enabled = false;
                //mAligner.MoveToHome();
                mAligner.Homing(mAligner.AXIS_X);
            }
            catch (Exception ex)
            {
                displayError("uiMoveToHome_Click", ex);
            }
            finally
            {
                ((Control)sender).Enabled = true;
            }
        }
        private void uiMovePositive_Click(object sender, EventArgs e)
        {
            try
            {
                ((Control)sender).Enabled = false;
                var displacement = (double)Math.Abs(uiDistance.Value);
                //mAligner.MoveAs(0, displacement);

                var axis = SurugaAligner.mMcAxisList[mCurrentAxisIndex % 2];
                mAligner.RelMove(axis, displacement);
            }
            catch (Exception ex)
            {
                displayError("uiMovePositive_Click", ex);
            }
            finally
            {
                ((Control)sender).Enabled = true;
            }
        }
        private void uiMoveNegative_Click(object sender, EventArgs e)
        {
            try
            {
                ((Control)sender).Enabled = false;
                var displacement = -(double)Math.Abs(uiDistance.Value);
                //mAligner.MoveAs(0, displacement);

                var axis = SurugaAligner.mMcAxisList[mCurrentAxisIndex % 2];
                mAligner.RelMove(axis, displacement);
            }
            catch (Exception ex)
            {
                displayError("uiMoveNegative_Click", ex);
            }
            finally
            {
                ((Control)sender).Enabled = true;
            }
        }

        private void uiAbort_Click(object sender, EventArgs e)
        {
            try
            {
                ((Control)sender).Enabled = false;
                mStopRepeat = true;
                mAligner.Abort();
            }
            catch (Exception ex)
            {
                displayError("uiAbort_Click", ex);
            }
            finally
            {
                ((Control)sender).Enabled = true;
            }
        }

        private void uiReadCoord_Click(object sender, EventArgs e)
        {
            try
            {
                ((Control)sender).Enabled = false;
                mAligner.ReadCoord();
            }
            catch (Exception ex)
            {
                displayError("uiReadCoord_Click", ex);
            }
            finally
            {
                ((Control)sender).Enabled = true;
            }
        }

        private void uiWriteCoord_Click(object sender, EventArgs e)
        {
            try
            {
                ((Control)sender).Enabled = false;
                var coord = (double)uiCoord.Value;
                mAligner.WriteCoord(Enumerable.Repeat(coord, 6).ToArray());
            }
            catch (Exception ex)
            {
                displayError("uiWriteCoord_Click", ex);
            }
            finally
            {
                ((Control)sender).Enabled = true;
            }
        }

        bool mStopRepeat = false;
        private async void uiMoveRepeat_Click(object sender, EventArgs e)
        {
            var numLoop = (int)uiNumLoop.Value;
            try
            {
                ((Control)sender).Enabled = false;
                var displacement = (double)uiDistance.Value;

                for (int i = 0; i < numLoop; i++)
                {
                    if (mStopRepeat)
                    {
                        mStopRepeat = false;
                        break;
                    }

                    var axis = SurugaAligner.mMcAxisList[mCurrentAxisIndex % 2];
                    mAligner.RelMove(axis, displacement);

                    //uiNumLoop.Value = uiNumLoop.Value - 1;
                    uiNumLoop.Value--;
                    uiNumLoop.Refresh();
                    await Task.Delay(10);
                }
            }
            catch (Exception ex)
            {
                displayError("uiMoveRepeat_Click", ex);
            }
            finally
            {
                ((Control)sender).Enabled = true;
                uiNumLoop.Value = numLoop;
            }
        }

        bool mDoingAxisCheck = false;
        private void uiMcAxis_X_CheckedChanged(object sender, EventArgs e)
        {
            if (mDoingAxisCheck) return;
            try
            {
                mCurrentAxisIndex = 0;
                mDoingAxisCheck = true;
                uiMcAxis_Y.Checked = !uiMcAxis_X.Checked;
            }
            finally
            {
                mDoingAxisCheck = false;
            }
        }

        private void uiMcAxis_Y_CheckedChanged(object sender, EventArgs e)
        {
            if (mDoingAxisCheck) return;
            try
            {
                mCurrentAxisIndex = 1;
                mDoingAxisCheck = true;
                uiMcAxis_X.Checked = !uiMcAxis_Y.Checked;
            }
            finally
            {
                mDoingAxisCheck = false;
            }
        }
    }
}
