using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Free302.MyLibrary.Utility;
using Neon.Aligner;


/// <summary>
/// raw data. from device.
/// </summary>
public class PortPowerRaw
{
    public int port;
    public List<double> power;
    public PortPowerRaw(int port, List<double> data)
    {
        this.port = port;
        this.power = data;
    }
}

