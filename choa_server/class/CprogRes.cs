using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class CprogRes
{

    private double itemProcTime;
    private List<double> itemProcTimeList;   //item 처리 하는데 걸리는 시간 list. [s]

    public bool compeleted;
    public DateTime startTime;               //시작 시간
    public DateTime endTime;                 //종료된 시간.
    public DateTime estiEndTime;             //예상 종료시간.
    public string msg;                       //Message  
    public int totalItemCnt;                 //전체 Item 갯수.
    public int compItemCnt;                  //완료된 Item 갯수.
    public string curItemNo;                 // 현재 진행중 Item No.
    




    #region constructor/desconstructor


    //생성자.
    public CprogRes()
    {
        compeleted = true;
        itemProcTimeList = new List<double>();
    }




    public CprogRes(double _procTime)
    {
        itemProcTime = _procTime;
        compeleted = true;
        itemProcTimeList = new List<double>();
    }


    #endregion





    #region private method



    /// <summary>
    /// 종료시간을 계산한다.
    /// </summary>
    /// <param name="procState">process state</param>
    /// <returns>예상 종료시간.</returns>
    public void CalcEstimateEndTime()
    {
        double nTimePerItem = (int)GetAvgProcTime();
        estiEndTime = startTime + (new TimeSpan(0, 0, (int)(nTimePerItem * totalItemCnt) ));
    }




    /// <summary>
    /// 초기 평균 아이템 처리 처리 시간을 설정.
    /// </summary>
    /// <param name="_time"></param>
    public void SetAvgProcTime(double _time)
    {
        itemProcTime = _time;
    }




    /// <summary>
    /// item을 처리하는데 걸린 시간을 설정한다.
    /// </summary>
    /// <param name="_time"></param>
    public void SetItemProcTime(double _time)
    {
        itemProcTimeList.Add(_time);
    }




    /// <summary>
    /// 평균 item 처리 시간을 얻는다.
    /// </summary>
    /// <returns> 평균 item 처리 시간[s] </returns>
    public double GetAvgProcTime()
    {
        double ret = 0.0;

        try
        {

            if (itemProcTimeList.Count() == 0)
                ret = itemProcTime;
            else
                ret = Jeffsoft.JeffMath.AverageExceptMinmax(itemProcTimeList.ToArray());

            ret = Math.Round(ret, 2);
        }
        catch
        {
            ret = 0.0;
        }

        return ret;
    }




    /// <summary>
    /// clear.
    /// </summary>
    public void Clear()
    {

        itemProcTime = GetAvgProcTime();

        compeleted = true;
        msg = "";                        //Message  
        totalItemCnt = 0;                //전체 Item 갯수.
        compItemCnt = 0;                 //완료된 Item 갯수.
        curItemNo = "";                  // 현재 진행중 Item No.
        itemProcTimeList.Clear();        //item 처리 하는데 걸리는 시간. [s]


    }



    #endregion


}
