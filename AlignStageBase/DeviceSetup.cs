using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using Free302.MyLibrary.Config;
using Free302.MyLibrary.Utility;

namespace Free302.TnM.Device
{
	/// <summary>
	/// DeviceSetup를 이용하는 클래스가 구현해야 할 인터페이스
	/// </summary>
	public interface IDeviceSetup
	{
		void applySetup(ConfigBase newSetup);
        ConfigBase buildSetup();
		string SetupName { get; }
	}


	public class DeviceSetup : ConfigBase
    {

		#region === Class Framework ===

		private const string DefaultSetupName = "DeviceSetup";

		public DeviceSetup() : base(DefaultSetupName)
		{}
		public DeviceSetup(string configName) : base(configName)
		{}

		#endregion


	}
}
