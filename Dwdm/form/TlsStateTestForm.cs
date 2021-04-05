using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Neon.Aligner;
using Free302.MyLibrary.Config;
using Free302.TnMLibrary.DataAnalysis;
using Free302.TnM.DataAnalysis;
using Free302.MyLibrary.Utility;

public partial class TlsStateTestForm : Form
{


	public TlsStateTestForm()
	{
		InitializeComponent();

	}

	C8164 mTls;
	int mDelay_sec = 1;

	private void TlsStateTestForm_Load(object sender, EventArgs e)
	{
		mTls = CGlobal.Tls8164;
	}

	private void btnRun_Click(object sender, EventArgs e)
	{
		if (mTaskRun) return;
		txtLog.Clear();
		txtLog.Focus();

		mDelay_sec = txtDelay.Text.To<int>();
		Task.Run(RunTask);
	}

	private async Task RunTask()
	{
		mStop = false;
		mTaskRun = true;
		var state = -1;
		float[] temp = new float[2];
		writeLog("Test Start");	

		try
		{
			while (true)
			{
				try
				{
					state = mTls.tlsState(true);
					temp = mTls.TlsTemp();
					writeLog($"[{DateTime.Now.ToString("MMdd_HH:mm:ss")}]\tTLS State : {state}\t\tTempLast : {temp[0]:F02}\tTempCurrent : {temp[1]:F02}");
					//mTls.readError();

					if (state != 1)
					{
						mTls.TryLdOn();
					}

					await Task.Delay(mDelay_sec * 1000);
					if (mStop) break;
				}
				catch (NationalInstruments.NI4882.GpibException ex)
				{
					writeLog($"GPIB exception error : {ex.Message}");
					if (ex.InnerException != null) writeLog($"{ex.InnerException.Message}");
					writeLog($"......Waiting 10 minute");
					await Task.Delay(600000);
					writeLog("Restart");
				}
				catch(Exception ex)
				{
					writeLog($"{ex.Message}\r\n{ex.StackTrace}");
					throw ex;
				}
			}
		}
		catch( Exception ex)
		{
			writeLog($"{ex.Message}\r\n{ex.StackTrace}");
		}
		finally
		{
			mTaskRun = false;
			writeLog("Test Stop");
		}
					
	}

	bool mTaskRun = false;
	bool mStop = false;

	private void btnStop_Click(object sender, EventArgs e)
	{
		mStop = true;
	}

	internal void writeLog(string msg, [CallerMemberName] string callingMethod = "")
	{
		var now = DateTime.Now;
		var message = $"[{callingMethod}()]\t{msg}\n";
		if (this.InvokeRequired) this.Invoke((Action)(() => txtLog.AppendText(message)));
		else txtLog.AppendText(message);
	}

}

