using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neon.Aligner
{

    /// <summary>
    /// pdlmeter에 관한 inteface.
    /// </summary>
    public interface Ipdlmeter
    {

        /// <summary>
        /// 한 포트의 optical power를 읽는다.
        /// </summary>
        /// <param name="_port">port no.</param>
        /// <returns></returns>
        double ReadModePow(int _port);

        /// <summary>
        /// 포트들의 optical power를 읽는다.
        /// </summary>
        /// <param name="_ports">port no. list</param>
        /// <returns>pdl list.</returns>
        List<double> ReadModePow(int[] _ports);



        /// <summary>
        /// 한 포트에서 pdl을 읽는다.
        /// </summary>
        /// <param name="_port">port no.</param>
        /// <returns></returns>
        double ReadModePdl(int _port);

        /// <summary>
        /// 여러개의 포트에서 PLD을 읽는다.
        /// </summary>
        /// <param name="_ports">port no. array.</param>
        /// <returns>pdl list.</returns>
        List<double> ReadModePdl(int[] _ports);


    }

}
