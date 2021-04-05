using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neon.Aligner
{

    /// <summary>
    /// electric sourcemeter inteface.
    /// </summary>
    public interface Isrcmeter
    {


        #region property

        /// <summary>
        /// measurment speed : fast
        /// </summary>
        int SPEED_FAST { get; }


        /// <summary>
        /// measurement speed : medium
        /// </summary>
        int SPEED_MED { get; }


        /// <summary>
        /// measurement speed : normal
        /// </summary>
        int SPEED_NORMAL { get; }





        /// <summary>
        /// voltage measurement mode
        /// </summary>
        int MEASMODE_VOLT { get; }


        /// <summary>
        /// current measurement mode
        /// </summary>
        int MEASMODE_CURR { get; }


        /// <summary>
        /// resistance measurement mode
        /// </summary>
        int MEASMODE_RES { get; }




        #endregion






        #region method


        /// <summary>
        /// turn on source output.
        /// </summary>
        void SourceOutOn();

        /// <summary>
        /// turn off source output.
        /// </summary>
        void SourceOutOff();

        /// <summary>
        /// read voltage
        /// </summary>
        /// <returns>voltage [V]</returns>
        double ReadVolt();

        /// <summary>
        /// read current
        /// </summary>
        /// <returns>Current [A]</returns>
        double ReadCurrent();


        /// <summary>
        /// read Resistance
        /// </summary>
        /// <returns>resistance [ohm]</returns>
        double ReadResistance();


        /// <summary>
        /// voltage와 current를 읽는다.
        /// </summary>
        /// <param name="_volt">[out]voltage [V]</param>
        /// <param name="_current">[out]current [A]</param>
        /// <returns></returns>
        bool ReadVoltCurrent(ref double _volt, ref double _current);


        /// <summary>
        /// set voltage compliance.
        /// </summary>
        /// <param name="_limit"> voltage limit [V] </param>
        void SetVoltCompliance(double _limit);


        /// <summary>
        /// get voltage compliance.
        /// </summary>
        /// <returns> current compliance [A] </returns>
        double GetVoltCompliance();


        /// <summary>
        /// set current compliance.
        /// </summary>
        /// <param name="_limit"> current limit [A] </param>
        void SetCurrentCompliance(double _limit);



        /// <summary>
        /// get current compliance.
        /// </summary>
        /// <returns> current compliance [A] </returns>
        double GetCurrentCompliance();



        /// <summary>
        /// SourceOut voltage를 설정한다.
        /// </summary>
        /// <param name="_volt">voltage [v]</param>
        void SetSourceVolt(double _volt);


        /// <summary>
        /// SourceOut voltage 값을 얻는다.
        /// </summary>
        /// <returns>voltage [v]</returns>
        double GetSourceVolt();



        /// <summary>
        /// SourceOut current를 설정한다.
        /// </summary>
        /// <param name="_curr">current [A]</param>
        void SetSourceCurrent(double _curr);





        /// <summary>
        /// SourceOut Current 값을 얻는다.
        /// </summary>
        /// <returns>current [A]</returns>
        double GetSourceCurrent();


        /// <summary>
        /// 측정 스피드를 설정한다.
        /// voltage, current, resistance.
        /// </summary>
        /// <param name="_spd"></param>
        void SetMeasSpeed(double _spd);


        /// <summary>
        /// 측정 스피드를 얻는다.
        /// </summary>
        /// <returns> fast,med,normal   </returns>
        int GetMeasSpeed();


        /// <summary>
        /// source output power state를 얻는다.
        /// on / off
        /// </summary>
        /// <returns> true: on , false: off </returns>
        bool OutputState();




        /// <summary>
        /// measurement mode 설정...
        /// </summary>
        /// <param name="_mode">measurement mode</param>
        void SetMeasMode(int _mode);


        /// <summary>
        /// Get Measurement mode.
        /// </summary>
        /// <returns> measurement mode </returns>
        int GetMeasMode();


        #endregion


    }

}
