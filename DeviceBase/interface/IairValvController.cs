using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neon.Aligner
{

    /// <summary>
    /// air valve 상태!! [※Open : 레일고정]
    /// </summary>
    public enum AirValveState
    {
        close = 0,  
        open = 1
    }


	public enum AirValveAligner
	{
		Input = 1,
		Output = 2
	}


    /// <summary>
    /// inteface about air-valve controller
    /// </summary>
    public interface IairValvController
    {
        /// <summary>
        /// valve 갯수를 얻는다.
        /// </summary>
        int valveCnt { get; }

        /// <summary>
        /// 모든 밸브를 닫는다.
        /// </summary>
        void CloseValve();
        
        /// <summary>
        /// 특정 밸브를 닫는다.
        /// </summary>
        /// <param name="_valve">valve no.</param>
        void CloseValve(int _valve);

        /// <summary>
        /// 모든 밸브를 연다.
        /// </summary>
        void OpenValve();

        /// <summary>
        /// 특정 밸브를 연다.
        /// </summary>
        /// <param name="_valve">valve no.</param>
        void OpenValve(int _valve);


        /// <summary>
        /// 특정 밸브의 상태를 얻는다. (open / close)
        /// </summary>
        /// <param name="_valve"></param>
        /// <returns></returns>
        AirValveState ValveState(int _valve);

    }

}
