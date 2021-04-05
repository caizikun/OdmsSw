using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neon.Aligner
{

    /// <summary>
    /// polarization controller에 대한 인터페이스.
    /// </summary>
    public interface IpolController
    {

        /// <summary>
        /// polarization filter의 위치를 설정한다.
        /// </summary>
        /// <param name="_pos">polarization filter의 위치[degree]</param>
        void SetPolFilterPos(double _pos);


        /// <summary>
        /// 현재 polarization filter의 위치를 얻는다.
        /// </summary>
        /// <returns>polarization filter의 위치[degree]</returns>
        double GetPolFilterPos();


        /// <summary>
        ///  λ/2 retarder의 위치를 설정한다.
        /// </summary>
        /// <param name="_pos">λ/2 retarder의 위치[degree]</param>
        void SetHalfRetarderPos(double _pos);


        /// <summary>
        /// 현재 λ/2 retarder의 위치를 얻는다.
        /// </summary>
        /// <returns>λ/2 retarder의 위치[degree]</returns>
        double GetHalfRetarderPos();

        /// <summary>
        ///  λ/4 retarder의 위치를 설정한다.
        /// </summary>
        /// <param name="_pos">λ/4 retarder의 위치[degree]</param>
        void SetQuarRetarderPos(double _pos);


        /// <summary>
        /// 현재 λ/4 retarder의 위치를 얻는다.
        /// </summary>
        /// <returns>λ/4 retarder의 위치[degree]</returns>
        double GetQuarRetarderPos();


        /// <summary>
        /// 편광을 LH(Linear Horizontal)로 설정한다.
        /// </summary>
        /// <param name="_polPos">polarization filter의 위치[degree]</param>
        void SetToLinearHorizontal(double _polPos);


        /// <summary>
        /// 편광을 LV(Linear Vertical)로 설정한다.
        /// </summary>
        /// <param name="_polPos">polarization filter의 위치[degree]</param>
        void SetToLinearVertical(double _polPos);


        /// <summary>
        /// 편광을 LD(Linear Diagonal)로 설정한다. (plus 45 degree)
        /// </summary>
        /// <param name="_polPos">polarization filter의 위치[degree]</param>
        void SetToLinearDiagonal(double _polPos);

        /// <summary>
        /// 편광을 LD(Linear Diagonal)로 설정한다. (minus 45 degree)
        /// </summary>
        /// <param name="_polPos">polarization filter의 위치[degree]</param>
        void SetToNegaLinearDiagonal(double _polPos);


        /// <summary>
        /// 편광을 RHC(right hand circular)로 설정한다.
        /// </summary>
        /// <param name="_polPos">polarization filter의 위치[degree]</param>
        void SetToRHcircular(double _polPos);


        /// <summary>
        /// 편광을 LHC(left hand circular)로 설정한다.
        /// </summary>
        /// <param name="_polPos"></param>
        void SetToLHcircular(double _polPos);


    }

}
