using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using Free302.MyLibrary.Config;
using Free302.MyLibrary.Utility;

namespace Free302.TnM.Device
{
	/// <summary>
	/// DeviceConfig를 이용하는 클래스가 구현해야 할 인터페이스
	/// </summary>
	public interface IDeviceConfig
	{
		void applyConfig(DeviceConfig newConfig);//apply new configuration

		DeviceConfig buildConfig();//build current configuration

		string ConfigName { get; }

		//string DynamicDataFile { get; set; }
	}


	public interface IDeviceDynamicConfig
	{
		void loadDynamicData();
		void saveDynamicData();
    }


	public class DeviceConfig : ConfigBase
	{
		
		#region === Class Framework ===

		private const string DefaultConfigName = "DeviceConfig";

		public DeviceConfig() : base(DefaultConfigName)
		{}
		public DeviceConfig(string configName) : base(configName)
		{}

		#endregion



		#region ==== Dynamic Data ====


		//XmlConfig mDynamicConfig;

		//public void loadDynamicData()
		//{
		//	mDynamicConfig = new XmlConfig(this["DynamciDataFile"]);
		//	this.addConfig(mDynamicConfig);
		//}



		#endregion



	}
}
