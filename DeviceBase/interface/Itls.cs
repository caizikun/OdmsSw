using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neon.Aligner
{

    /// <summary>
    /// tunable laser source에 관한 interface.
    /// </summary>
    public interface Itls
    {

        /// <summary>
        /// LD를 켠다.
        /// </summary>
        void TlsOn();

        /// <summary>
        /// LD를 끈다.
        /// </summary>
        void TlsOff();


        /// <summary>
        /// 출력 광파워를 설정한다.
        /// </summary>
        /// <param name="_pwr">광파워[dBm]</param>
        void SetTlsOutPwr(double _pwr);


        /// <summary>
        /// 현재 출력 광파워를 얻는다.
        /// </summary>
        /// <returns>출력 광파워 [dBm]</returns>
        double GetTlsOutPwr();

        /// <summary>
        /// 파장을 설정
        /// </summary>
        /// <param name="_wl">파장[nm]</param>
        void SetTlsWavelen(double _wl);


        /// <summary>
        /// 현재 설정된 파장.
        /// </summary>
        /// <returns>파장 [nm]</returns>
        double GetTlsWavelen();


        /// <summary>
        /// Sweep range를 설정한다.
        /// </summary>
        /// <param name="_start">시작 파장 [nm]</param>
        /// <param name="_stop">끝 파장 [nm]</param>
        /// <param name="_step">파장 간격[nm]</param>
        void SetTlsSweepRange(int _start, int _stop, double _step);


        /// <summary>
        /// 현재 Sweep range 관련 값을 얻는다.
        /// </summary>
        /// <param name="_start">[out]시작 파장 [nm]</param>
        /// <param name="_stop">[out]끝 파장 [nm]</param>
        /// <param name="_step">[out]>파장 간격[nm]</param>
        void GetTlsSweepRange(ref int _start, ref int _stop, ref double _step);


        /// <summary>
        /// Sweep speed를 설정한다.
        /// </summary>
        /// <param name="_speed">sweep speed [nm/s]</param>
        void SetTlsSweepSpeed(double _speed);


        /// <summary>
        /// 현재 sweep speed를 얻는다.
        /// </summary>
        /// <returns></returns>
        double GetTlsSweepSpeed();


        /// <summary>
        /// 로깅된 파장데이터들을 획득한다.
        /// </summary>
        /// <returns>파장 데이트들</returns>
        List<double> GetTlsWavelenLog();


        /// <summary>
        /// Sweep을 실행한다.
        /// </summary>
        void ExecTlsSweepCont();


        /// <summary>
        /// 현재 sweep이 진행중인지 아닌지?
        /// </summary>
        /// <returns>true: 진행중 , false : 대기중</returns>
        bool IsTlsSweepOperating();


        /// <summary>
        /// TLS logging mode on.
        /// </summary>
        void TlsLogOn();


        /// <summary>
        /// TLS logging mode off.
        /// </summary>
        void TlsLogOff();

    }



}
