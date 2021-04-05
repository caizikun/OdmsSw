using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neon.Aligner
{


    /// <summary>
    /// optical switch에 대한 인터페이스.
    /// </summary>
    public interface IoptSwitch
    {
        
        /// <summary>
        /// get current closed input port
        /// </summary>
        /// <returns>port no.</returns>
        int GetInClosedPort();


        /// <summary>
        /// close input port.
        /// </summary>
        /// <param name="_portNo"></param>
        //void CloseOutPort(int _portNo);


        /// <summary>
        /// get current closed output port
        /// </summary>
        /// <returns>port no.</returns>
        int GetOutClosedPort();

        bool Connect(int _comport);

        void SetToTls();
        void SetToAlign();

        event Action<int> PortChanged;

    }

}
