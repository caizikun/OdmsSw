namespace Free302.TnM.Device
{
    public interface IDevice<TDeviceId>
    {
        string ConfigName { get; }
        TDeviceId Id { get; set; }
        bool IsConfigSet { get; }
        bool IsIdSet { get; }
        bool IsOpen { get; }
        string Name { get; set; }
        DeviceSetup Setup { get; }

        void applyConfig(DeviceConfig newConfig);
        DeviceConfig buildConfig();
        void Close();
        void Open();
        string ToString();
    }
}