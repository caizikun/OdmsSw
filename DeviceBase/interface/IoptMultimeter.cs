using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neon.Aligner
{

    /// <summary>
    /// Multi channel(=port) optical powermeter에 관한 interface.
    /// </summary>
    public interface IoptMultimeter
    {

        /// <summary>
        /// 전체 포트 수를 얻는다.
        /// </summary>
        int NumPorts { get; }
        object[] ChList { get; }

        
        /// <summary>
        /// photo detector의 gain level를 manual로 설정한다.
        /// </summary>
        /// <param name="port">port no.</param>
        void SetGainManual(int port);

        /// <summary>
        /// 모든 photo detector들의 gain level를 manual로 설정한다.
        /// </summary>
        void SetGainManual();



        /// <summary>
        /// pd의 gain level를 설정한다.
        /// </summary>
        /// <param name="port">port no.</param>
        /// <param name="_level">gain level</param>
        void SetGainLevel(int port, int _level);

        /// <summary>
        /// pd들의 gain level를 설정한다.
        /// </summary>
        /// <param name="ports">port no. array</param>
        /// <param name="_level">gain level</param>
        void SetGainLevel(int[] ports, int _level);

        /// <summary>
        /// 모든 pd들의 gain level를 설정한다.
        /// </summary>
        /// <param name="_level">gain level</param>
        void SetGainLevel(int _level);



        /// <summary>
        /// pd의 현재 gain level를 설정한다.
        /// </summary>
        /// <param name="portNo"></param>
        /// <returns></returns>
        int GetGainLevel(int portNo);

        /// <summary>
        /// pd들의 현재 gain level를 설정한다.
        /// </summary>
        /// <param name="portNos">port no. array</param>
        /// <returns>pd들의 gain level list</returns>
        List<int> GetGainLevel(int[] portNos);

        /// <summary>
        /// pd의 파장을 설정한다.
        /// </summary>
        /// <param name="portNo">port no.</param>
        /// <param name="_wavelen">wavelength[nm]</param>
        void SetPdWavelen(int portNo, double _wavelen);
        
        /// <summary>
        /// 모든 pd들의 파장을 설정한다.
        /// </summary>
        /// <param name="_wavelen">wavelength array</param>
        void SetPdWavelen(double _wavelen);




        /// <summary>
        /// pd의 현재 설정된 파장을 얻는다.
        /// </summary>
        /// <param name="portNo">port no.</param>
        /// <returns>wavelength[nm]</returns>
        double GetPdWavelen(int portNo);
        
        /// <summary>
        /// pd를 logging mode로 설정한다.
        /// </summary>
        /// <param name="port">port no.</param>
        void SetPdLogMode(int port);

        /// <summary>
        /// pd들을 logging mode로 설정한다.
        /// </summary>
        /// <param name="ports">port no. array</param>
        void SetPdLogMode(int[] ports);
        
        /// <summary>
        /// pd의 logging mode를 해제한다.
        /// </summary>
        /// <param name="portNo"></param>
        void StopPdLogMode(int portNo);

        /// <summary>
        /// pd들의 logging mode를 해제한다.
        /// </summary>
        /// <param name="portNos">port no. array</param>
        void StopPdLogMode(int[] portNos);

        /// <summary>
        /// 모든 pd의 logging mode를 해제한다.
        /// </summary>
        void StopPdLogMode();



        /// <summary>
        /// pd를 sweep 모드로 설정한다.
        /// </summary>
        /// <param name="port">port no.</param>
        /// <param name="_startWave">시작 파장[nm]</param>
        /// <param name="_stopWave">끝 파장[nm]</param>
        /// <param name="_step">파장 간격[nm]</param>
        void SetPdSweepMode(int port, int _startWave, int _stopWave, double _step);

        /// <summary>
        /// pd들을 sweep 모드로 설정한다.
        /// </summary>
        /// <param name="portNos">port no. array</param>
        /// <param name="_startWave">시작 파장[nm]</param>
        /// <param name="_stopWave">끝 파장[nm]</param>
        /// <param name="_step">파장 간격[nm]</param>
        void SetPdSweepMode(int[] portNos, int _startWave, int _stopWave, double _step);
        
        /// <summary>
        /// pd들의 sweep 모드를 해제한다.
        /// </summary>
        /// <param name="portNos">port no. array</param>
        void StopPdSweepMode(int[] portNos);

        /// <summary>
        /// 모든 pd들의 sweep 모드를 해제한다.
        /// </summary>
        void StopPdSweepMode();



        /// <summary>
        /// pd의 logging data를 얻는다.
        /// </summary>
        /// <param name="port">port no.</param>
        /// <returns></returns>
        List<double> GetPwrLog(int port);




        /// <summary>
        /// pd의 광파워를 읽는다.
        /// </summary>
        /// <param name="port">port no.</param>
        /// <returns>광파워 [mW]</returns>
        double ReadPower(int port);  //like a 'petch'

        
    }

}
