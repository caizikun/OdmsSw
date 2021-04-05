using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

namespace Neon.Aligner
{
    class DataFileProcessor
    {
        DataTable mDt;
        public DataTable dt { get; private set; }
        public int dtCount { get { return mDt.Rows.Count; }  }
        public bool[] dtCheckList { get { return mDt.AsEnumerable().Select(r => r.Field<bool>("Check")).ToArray(); } }

        public readonly string[] mColNames = { "Name", "Path", "Ok", "Check" };



        #region Contructor


        public DataFileProcessor()
        {
            InitDt();
            dt = mDt;
        }



        public void InitDt()
        {
            mDt = new DataTable("RawFile");
            mDt.BeginInit();

            mDt.Rows.Clear();
            mDt.Columns.Clear();

            mDt.Columns.Add(mColNames[0], typeof(string));
            mDt.Columns.Add(mColNames[1], typeof(string));
            mDt.Columns.Add(mColNames[2], typeof(int));
            var col = mDt.Columns.Add(mColNames[3], typeof(bool));

            mDt.RowDeleting += MDt_RowDeleting;

            mDt.EndInit();
        }


        public bool EraseFile { get; set; } = true;

        private void MDt_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            var filePath = e.Row[1].ToString();
            if (EraseFile)
            {
                DataBackup.BackupFile(filePath, "errorFile");
                File.Delete(filePath); 
            }
        }


        #endregion




        public void AddFiles(string[] filePath)
        {
            foreach (var item in filePath)
            {
                mDt.Rows.Add(Path.GetFileName(item), item, 1, false);
            }

        }



        public void ClearFiles()
        {
            mDt.Rows.Clear();
        }



        public void EraseUploadData()
        {
            //Upload 완료된 항목 제거
            EraseFile = false;
            for (int i = (mDt.Rows.Count - 1); i >= 0; i--)
            {
                if ((int)mDt.Rows[i]["Ok"] == 0) mDt.Rows.Remove(mDt.Rows[i]);
            }
            EraseFile = true;
        }



        public int EraseError()
        {
            int count = 0;
            for (int i = (mDt.Rows.Count - 1); i >= 0; i--)
            {
                if ((bool)mDt.Rows[i]["Check"])
                {
                    mDt.Rows.Remove(mDt.Rows[i]);
                    count += 1;
                }
            }
            return count;
        }



        public void uploadCheck(int rowIndex, int value = 0)
        {
            mDt.Rows[rowIndex]["Ok"] = value; 
        }



        public bool[] ErrorCheck(double errorIL)
        {
            //에러 체크
            mDt.Columns["Check"].DefaultValue = false;
            for (int i = 0; i < mDt.Rows.Count ; i++)
            {
                if (Path.GetExtension(mDt.Rows[i]["Path"].ToString()).Contains("txt"))
                {
                    var dut = DutData.LoadFileNp(mDt.Rows[i]["Path"].ToString());
                    try
                    {
                        var peak = WdmAnalyzer.AnalyzeNp(dut);
                        for (int j = 0; j < peak[0].Length; j++)
                        {
                            if (peak[0][j] < errorIL || peak[0][j] > 10) mDt.Rows[i]["Check"] = true;
                        }
                    }
                    catch (Exception)
                    {
                        mDt.Rows[i]["Check"] = true;
                    }

                }

            }

            return mDt.AsEnumerable().Select(c => c.Field<bool>("Check")).ToArray();

        }



        public void ErrorCheckChip(int index, bool check)
        {
            mDt.Rows[index]["Check"] = check;
        }



        public bool[] OverlabCheck()
        {
            //중복 데이터 체크
            mDt.Columns["Check"].DefaultValue = false;

            var ChipNameList = (from n in mDt.AsEnumerable().Select(r => r.Field<string>("Name"))
                                select string.Join("", n.Take(20))).Distinct().ToArray();

            for (int i = 0; i < ChipNameList.Length; i++)
            {
                var list = (from n in mDt.AsEnumerable()
                            where n.Field<string>("Name").Contains(ChipNameList[i])
                            select n.Field<string>("Path")).ToArray();
                if (list.Length > 1)
                {
                    double[] il = new double[list.Length];

                    //IL값 가져오기
                    for (int j = 0; j < list.Length; j++)
                    {
                        var dut = DutData.LoadFileNp(list[j]);
                        try
                        {
                            il[j] = WdmAnalyzer.AnalyzeNp(dut)[0][5];
                            if (il[j] > 10) il[j] = -100;
                        }
                        catch (Exception)
                        {
                            il[j] = -100;
                        }
                    }

                    Array.Sort(il, list);
                    Array.Reverse(list);
                    Array.Sort(il);
                    Array.Reverse(il);
                    if (il[0] == il[1]) list.OrderByDescending(c => c).ToArray();

                    for (int k = 1; k < list.Length; k++)
                    {
                        var row = mDt.AsEnumerable().Select(r => r.Field<string>("Path")).ToList().FindIndex(x => x == list[k]);
                        mDt.Rows[row]["Check"] = true;
                    } 
                }

            }
                        
            return mDt.AsEnumerable().Select(c => c.Field<bool>("Check")).ToArray();

        }
               


    }
}
