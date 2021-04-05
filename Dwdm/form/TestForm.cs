using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Neon.Aligner;
using NationalInstruments.DAQmx;

namespace Neon.Dwdm
{
	public partial class TestForm : Form
	{
		public TestForm()
		{
			InitializeComponent();
		}

		private void TestForm_Load(object sender, EventArgs e)
		{
			//initUi();
			mAir = CGlobal.AirValve;
		}

		IairValvController mAir;

		Daq mDaq;
		int line;

		private void initUi()
		{
			cboDaqPrimary.DataSource = DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.DOLine, PhysicalChannelAccess.External);
		}

		private void btnDaqInit_Click(object sender, EventArgs e)
		{
			var address = cboDaqPrimary.SelectedItem.ToString().Split('/');
			int dev = int.Parse(Regex.Replace(address[0], @"\D", ""));
			int port = int.Parse(Regex.Replace(address[1], @"\D", ""));
			line = int.Parse(Regex.Replace(address[2], @"\D", ""));

			mDaq = new Daq();
			mDaq.CreateDoCh(dev, port, line);

		}

		private void btnHigh_Click(object sender, EventArgs e)
		{
			if (mDaq == null) return;
			mDaq.WriteDo(line, true);

		}

		private void btnLow_Click(object sender, EventArgs e)
		{
			if (mDaq == null) return;
			mDaq.WriteDo(line, false);
		}





		private void button1_Click(object sender, EventArgs e)
		{
			try
			{
				mAir.OpenValve((int)AirValveAligner.Input);
			}
			catch (Exception)
			{

				throw;
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			try
			{
				mAir.CloseValve((int)AirValveAligner.Input);
			}
			catch (Exception)
			{

				throw;
			}
		}

		private void button4_Click(object sender, EventArgs e)
		{
			try
			{
				mAir.OpenValve((int)AirValveAligner.Output);
			}
			catch (Exception)
			{

				throw;
			}
		}

		private void button3_Click(object sender, EventArgs e)
		{
			try
			{
				mAir.CloseValve((int)AirValveAligner.Output);
			}
			catch (Exception)
			{

				throw;
			}
		}
	}
}
