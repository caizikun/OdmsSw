using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using DrBAE.TnM.TcpLib;
using Universe.TcpServer;

namespace Neon.Aligner
{
    public partial class SweepLogic
    {

        #region ==== Class Freamwork ====

        private Itls mTls;
        private IoptMultimeter mPm;
        private IpolController mPc;

        private List<double> mWaves;                    //calibrated(equal interval)
        private List<PortPowers> mPortPowerList;        //calibrated(equal interval , stiching)

        public event Action<string> mReporter;

        public SweepLogic(Itls _tls, IoptMultimeter _mpm)
        {
            mTls = _tls;
            mPm = _mpm;
        }

        public SweepLogic(Itls _tls, IoptMultimeter _mpm, IpolController _pc) : this(_tls, _mpm)
        {
            mPc = _pc;
        }

        #endregion



        #region ==== TCP Server ====

        ITcpAgentClient mTcp;
        bool mUsingLocalTls = false;

        public bool TcpServer_Init(bool usingTcpServer)//out TcpAgentClient tcp)
        {
            try
            {
                mUsingLocalTls = !usingTcpServer;

                if (usingTcpServer)
                {
					mTcp = new Client((x) => { }, (x) => { });
					mTcp.Connect();
                }
                //tcp = mTcp;
                return mPm != null && (usingTcpServer || mTls != null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"SweepLogic.Init(): \n{ex.Message}\n\n설정파일 존재여부나 TLS서버 네트웍크 설정을 확인하세요");
                //tcp = null;
                return false;
            }
        }

        public void TcpServer_Register(bool register)
        {
            var msg = register ? "Registering" : "UnRegistering";
            mReporter?.Invoke($"SweepLogic.Register(): {msg}");
            if (register) mTcp.Register();
            else mTcp.UnRegister();
        }

        public void TcpServer_BeginAlign()
        {
            mTcp.BeginAlign();
        }
        public void TcpServer_EndAlign()
        {
            mTcp.EndAlign();
        }
        public void TcpServer_Align(int wave_nm)
        {
            mTcp.Align(wave_nm);
        }


        #endregion



        #region ==== public method ====


        /// <summary>
        /// after sweep(Non polarization), get the power datas of port.
        /// </summary>
        /// <param name="port">port no.</param>
        /// <returns>instance of CswpPortPwrNonpol</returns>
        public PortPowers GetPortPower(int port)
        {
            PortPowers ret = null;
            ret = mPortPowerList.Find(p => p.Port == port);
            return ret;
        }

		#endregion


	} 
}

