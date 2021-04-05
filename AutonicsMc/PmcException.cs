using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Free302.TnM.Device
{
    public enum PmcExceptionId
    {
        Open, Info, Init, Close, Read, setMode, setNORG, setORG, Reset, NotConnectedAxis,  Move, SetPosition
    }

    public class PmcException : Exception
    {
        PmcExceptionId pexId;

        public PmcException(PmcExceptionId exId)
            : base()
        {
            this.pexId = exId;
            Data["Exception ID"] = exId;
        }

		public override string Message
		{
			get
			{
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("< PMC4B Exception >");

                if (Data != null)
                {
                    foreach (DictionaryEntry de in Data)
                        sb.AppendLine(string.Format("{0} = {1}", de.Key, de.Value));
                }
                //sb.AppendLine(this.StackTrace);

                return sb.ToString();
            }
		}

    }


}
