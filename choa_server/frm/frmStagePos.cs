using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Neon.Aligner;


public partial class frmStagePos : Form
{

    public frmStagePos()
    {
        InitializeComponent();
    }


    #region private variable.

    private Istage m_leftStage;
    private Istage m_rightStage;
    private IDispSensor m_distSens; //Distance sensor.

    private DataSet m_ds;

    #endregion




    #region private method


    /// <summary>
    /// clear information group.
    /// </summary>
    private void ClearInformation()
    {
        lbListNo.Text = "";

        txtLeftX.Text = "";
        txtLeftY.Text = "";
        txtLeftZ.Text = "";
        txtLeftTx.Text = "";
        txtLeftTy.Text = "";
        txtLeftTz.Text = "";
        chkLeftX.Checked = false;
        chkLeftY.Checked = false;
        chkLeftZ.Checked = false;
        chkLeftTx.Checked = false;
        chkLeftTy.Checked = false;
        chkLeftTz.Checked = false;

        txtRightX.Text = "";
        txtRightY.Text = "";
        txtRightZ.Text = "";
        txtRightTx.Text = "";
        txtRightTy.Text = "";
        txtRightTz.Text = "";
        chkRightX.Checked = false;
        chkRightY.Checked = false;
        chkRightZ.Checked = false;
        chkRightTx.Checked = false;
        chkRightTy.Checked = false;
        chkRightTz.Checked = false;

        txtInfoComments.Text = "";


    }





    /// <summary>
    /// make new table.
    /// </summary>
    /// <returns></returns>
    private DataTable NewTable()
    {

        DataTable tbl = new DataTable();

        tbl.TableName = "DATA";

        //colums
        DataColumn col = new DataColumn();
        col.ColumnName = "no";
        col.DataType = System.Type.GetType("System.Int32");
        col.Unique = false;
        col.AutoIncrement = true;
        col.Caption = "number";
        col.ReadOnly = false;
        tbl.Columns.Add(col);

        col = null;
        col = new DataColumn();
        col.ColumnName = "leftX";
        col.DataType = System.Type.GetType("System.Double");
        col.Unique = false;
        col.AutoIncrement = false;
        col.Caption = "x-axis position of left-stage";
        col.ReadOnly = false;
        tbl.Columns.Add(col);

        col = new DataColumn();
        col.ColumnName = "leftY";
        col.DataType = System.Type.GetType("System.Double");
        col.Unique = false;
        col.AutoIncrement = false;
        col.Caption = "y-axis position of left-stage";
        col.ReadOnly = false;
        tbl.Columns.Add(col);

        col = new DataColumn();
        col.ColumnName = "leftZ";
        col.DataType = System.Type.GetType("System.Double");
        col.Unique = false;
        col.AutoIncrement = false;
        col.Caption = "z-axis position of left-stage";
        col.ReadOnly = false;
        tbl.Columns.Add(col);

        col = null;
        col = new DataColumn();
        col.ColumnName = "leftTx";
        col.DataType = System.Type.GetType("System.Double");
        col.Unique = false;
        col.AutoIncrement = false;
        col.Caption = "tx-axis position of left-stage";
        col.ReadOnly = false;
        tbl.Columns.Add(col);

        col = new DataColumn();
        col.ColumnName = "leftTy";
        col.DataType = System.Type.GetType("System.Double");
        col.Unique = false;
        col.AutoIncrement = false;
        col.Caption = "ty-axis position of left-stage";
        col.ReadOnly = false;
        tbl.Columns.Add(col);

        col = new DataColumn();
        col.ColumnName = "leftTz";
        col.DataType = System.Type.GetType("System.Double");
        col.Unique = false;
        col.AutoIncrement = false;
        col.Caption = "tz-axis position of left-stage";
        col.ReadOnly = false;
        tbl.Columns.Add(col);

        col = null;
        col = new DataColumn();
        col.ColumnName = "chkLeftX";
        col.DataType = System.Type.GetType("System.Boolean");
        col.Unique = false;
        col.AutoIncrement = false;
        col.Caption = "x-axis of left stage Enable or Disable";
        col.ReadOnly = false;
        tbl.Columns.Add(col);

        col = null;
        col = new DataColumn();
        col.ColumnName = "chkLeftY";
        col.DataType = System.Type.GetType("System.Boolean");
        col.Unique = false;
        col.AutoIncrement = false;
        col.Caption = "y-axis of left stage Enable or Disable";
        col.ReadOnly = false;
        tbl.Columns.Add(col);

        col = null;
        col = new DataColumn();
        col.ColumnName = "chkLeftZ";
        col.DataType = System.Type.GetType("System.Boolean");
        col.Unique = false;
        col.AutoIncrement = false;
        col.Caption = "z-axis of left stage Enable or Disable";
        col.ReadOnly = false;
        tbl.Columns.Add(col);

        col = null;
        col = new DataColumn();
        col.ColumnName = "chkLeftTx";
        col.DataType = System.Type.GetType("System.Boolean");
        col.Unique = false;
        col.AutoIncrement = false;
        col.Caption = "tx-axis of left stage Enable or Disable";
        col.ReadOnly = false;
        tbl.Columns.Add(col);

        col = null;
        col = new DataColumn();
        col.ColumnName = "chkLeftTy";
        col.DataType = System.Type.GetType("System.Boolean");
        col.Unique = false;
        col.AutoIncrement = false;
        col.Caption = "Ty-axis of left stage Enable or Disable";
        col.ReadOnly = false;
        tbl.Columns.Add(col);

        col = null;
        col = new DataColumn();
        col.ColumnName = "chkLeftTz";
        col.DataType = System.Type.GetType("System.Boolean");
        col.Unique = false;
        col.AutoIncrement = false;
        col.Caption = "tz-axis of left stage Enable or Disable";
        col.ReadOnly = false;
        tbl.Columns.Add(col);



        col = null;
        col = new DataColumn();
        col.ColumnName = "rightX";
        col.DataType = System.Type.GetType("System.Double");
        col.Unique = false;
        col.AutoIncrement = false;
        col.Caption = "x-axis position of right-stage";
        col.ReadOnly = false;
        tbl.Columns.Add(col);

        col = new DataColumn();
        col.ColumnName = "rightY";
        col.DataType = System.Type.GetType("System.Double");
        col.Unique = false;
        col.AutoIncrement = false;
        col.Caption = "y-axis position of right-stage";
        col.ReadOnly = false;
        tbl.Columns.Add(col);

        col = new DataColumn();
        col.ColumnName = "rightZ";
        col.DataType = System.Type.GetType("System.Double");
        col.Unique = false;
        col.AutoIncrement = false;
        col.Caption = "z-axis position of right-stage";
        col.ReadOnly = false;
        tbl.Columns.Add(col);

        col = null;
        col = new DataColumn();
        col.ColumnName = "rightTx";
        col.DataType = System.Type.GetType("System.Double");
        col.Unique = false;
        col.AutoIncrement = false;
        col.Caption = "tx-axis position of right-stage";
        col.ReadOnly = false;
        tbl.Columns.Add(col);

        col = new DataColumn();
        col.ColumnName = "rightTy";
        col.DataType = System.Type.GetType("System.Double");
        col.Unique = false;
        col.AutoIncrement = false;
        col.Caption = "ty-axis position of right-stage";
        col.ReadOnly = false;
        tbl.Columns.Add(col);

        col = new DataColumn();
        col.ColumnName = "rightTz";
        col.DataType = System.Type.GetType("System.Double");
        col.Unique = false;
        col.AutoIncrement = false;
        col.Caption = "tz-axis position of right-stage";
        col.ReadOnly = false;
        tbl.Columns.Add(col);

        col = null;
        col = new DataColumn();
        col.ColumnName = "chkRightX";
        col.DataType = System.Type.GetType("System.Boolean");
        col.Unique = false;
        col.AutoIncrement = false;
        col.Caption = "x-axis of right stage Enable or Disable";
        col.ReadOnly = false;
        tbl.Columns.Add(col);

        col = null;
        col = new DataColumn();
        col.ColumnName = "chkRightY";
        col.DataType = System.Type.GetType("System.Boolean");
        col.Unique = false;
        col.AutoIncrement = false;
        col.Caption = "y-axis of right stage Enable or Disable";
        col.ReadOnly = false;
        tbl.Columns.Add(col);

        col = null;
        col = new DataColumn();
        col.ColumnName = "chkRightZ";
        col.DataType = System.Type.GetType("System.Boolean");
        col.Unique = false;
        col.AutoIncrement = false;
        col.Caption = "z-axis of right stage Enable or Disable";
        col.ReadOnly = false;
        tbl.Columns.Add(col);

        col = null;
        col = new DataColumn();
        col.ColumnName = "chkRightTx";
        col.DataType = System.Type.GetType("System.Boolean");
        col.Unique = false;
        col.AutoIncrement = false;
        col.Caption = "tx-axis of right stage Enable or Disable";
        col.ReadOnly = false;
        tbl.Columns.Add(col);

        col = null;
        col = new DataColumn();
        col.ColumnName = "chkRightTy";
        col.DataType = System.Type.GetType("System.Boolean");
        col.Unique = false;
        col.AutoIncrement = false;
        col.Caption = "Ty-axis of right stage Enable or Disable";
        col.ReadOnly = false;
        tbl.Columns.Add(col);

        col = null;
        col = new DataColumn();
        col.ColumnName = "chkRightTz";
        col.DataType = System.Type.GetType("System.Boolean");
        col.Unique = false;
        col.AutoIncrement = false;
        col.Caption = "tz-axis of right stage Enable or Disable";
        col.ReadOnly = false;
        tbl.Columns.Add(col);


        col = null;
        col = new DataColumn();
        col.ColumnName = "comments";
        col.DataType = System.Type.GetType("System.String");
        col.Unique = false;
        col.AutoIncrement = false;
        col.Caption = "comments";
        col.ReadOnly = false;
        tbl.Columns.Add(col);


        return tbl;

    }


    /// <summary>
    /// show information 
    /// </summary>
    /// <param name="_listNo"></param>
    private void ShowInformation(int _listNo)
    {

        try
        {


            //get datarow.
            IEnumerable<DataRow> query = from data in m_ds.Tables["DATA"].AsEnumerable()
                                         where data.Field<int>("no") == _listNo
                                         select data;

            IEnumerator<DataRow> enu = query.GetEnumerator();
            enu.MoveNext();


            //display.
            double dbVal = 0;

            dbVal = enu.Current.Field<double>("leftX");
            dbVal = Math.Round(dbVal, 1);
            txtLeftX.Text = dbVal.ToString();

            dbVal = enu.Current.Field<double>("leftY");
            dbVal = Math.Round(dbVal, 1);
            txtLeftY.Text = dbVal.ToString();

            dbVal = enu.Current.Field<double>("leftZ");
            dbVal = Math.Round(dbVal, 1);
            txtLeftZ.Text = dbVal.ToString();

            dbVal = enu.Current.Field<double>("leftTx");
            dbVal = Math.Round(dbVal, 3);
            txtLeftTx.Text = dbVal.ToString();

            dbVal = enu.Current.Field<double>("leftTy");
            dbVal = Math.Round(dbVal, 4);
            txtLeftTy.Text = dbVal.ToString();

            dbVal = enu.Current.Field<double>("leftTz");
            dbVal = Math.Round(dbVal, 4);
            txtLeftTz.Text = dbVal.ToString();

            chkLeftX.Checked = enu.Current.Field<bool>("chkLeftX");
            chkLeftY.Checked = enu.Current.Field<bool>("chkLeftY");
            chkLeftZ.Checked = enu.Current.Field<bool>("chkLeftZ");
            chkLeftTx.Checked = enu.Current.Field<bool>("chkLeftTx");
            chkLeftTy.Checked = enu.Current.Field<bool>("chkLeftTy");
            chkLeftTz.Checked = enu.Current.Field<bool>("chkLeftTz");



            dbVal = enu.Current.Field<double>("rightX");
            dbVal = Math.Round(dbVal, 1);
            txtRightX.Text = dbVal.ToString();

            dbVal = enu.Current.Field<double>("rightY");
            dbVal = Math.Round(dbVal, 1);
            txtRightY.Text = dbVal.ToString();

            dbVal = enu.Current.Field<double>("rightZ");
            dbVal = Math.Round(dbVal, 1);
            txtRightZ.Text = dbVal.ToString();

            dbVal = enu.Current.Field<double>("rightTx");
            dbVal = Math.Round(dbVal, 3);
            txtRightTx.Text = dbVal.ToString();

            dbVal = enu.Current.Field<double>("rightTy");
            dbVal = Math.Round(dbVal, 4);
            txtRightTy.Text = dbVal.ToString();

            dbVal = enu.Current.Field<double>("rightTz");
            dbVal = Math.Round(dbVal, 4);
            txtRightTz.Text = dbVal.ToString();

            chkRightX.Checked = enu.Current.Field<bool>("chkRightX");
            chkRightY.Checked = enu.Current.Field<bool>("chkRightY");
            chkRightZ.Checked = enu.Current.Field<bool>("chkRightZ");
            chkRightTx.Checked = enu.Current.Field<bool>("chkRightTx");
            chkRightTy.Checked = enu.Current.Field<bool>("chkRightTy");
            chkRightTz.Checked = enu.Current.Field<bool>("chkRightTz");

            txtInfoComments.Text = enu.Current.Field<string>("comments");


        }
        catch
        {
            //do nothing.
        }


    }





    #endregion




    /// <summary>
    /// 현재 위치를 저장한다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAdd_Click(object sender, EventArgs e)
    {

        string msg = "현재 위치를 저장하시겠습니까?";
        DialogResult dialRes = MessageBox.Show(msg, 
                                                "확인",
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Question);
        if (dialRes == DialogResult.No)
            return;



        try
        {

            //load position from stage controller.
            CStageAbsPos  leftPos = m_leftStage.GetAbsPositions();
            CStageAbsPos  rightPos = m_rightStage.GetAbsPositions();


            //add row.
            DataTable tbl = m_ds.Tables["DATA"];
            DataRow dr = tbl.NewRow();

            dr["leftX"] = leftPos.x;
            dr["leftY"] = leftPos.y;
            dr["leftZ"] = leftPos.z;
            dr["leftTx"] = leftPos.tx;
            dr["leftTy"] = leftPos.ty;
            dr["leftTz"] = leftPos.tz;
            dr["chkLeftX"] = true;
            dr["chkLeftY"] = true;
            dr["chkLeftZ"] = true;
            dr["chkLeftTx"] = true;
            dr["chkLeftTy"] = true;
            dr["chkLeftTz"] = true;

            dr["rightX"] = rightPos.x;
            dr["rightY"] = rightPos.y;
            dr["rightZ"] = rightPos.z;
            dr["rightTx"] = rightPos.tx;
            dr["rightTy"] = rightPos.ty;
            dr["rightTz"] = rightPos.tz;
            dr["chkRightX"] = true;
            dr["chkRightY"] = true;
            dr["chkRightZ"] = true;
            dr["chkRightTx"] = true;
            dr["chkRightTy"] = true;
            dr["chkRightTz"] = true;

            dr["comments"] = textAddComments.Text;

            m_ds.Tables["DATA"].Rows.Add(dr);


            //display.
            int listNo = (System.Int32)(tbl.Rows[tbl.Rows.Count - 1]["no"]);
            ShowInformation(listNo);

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
    }


    /// <summary>
    /// init form.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmStagePos_Load(object sender, EventArgs e)
    {
        try
        {


            m_leftStage = (Istage)(CGlobal.LeftAligner);
            m_rightStage = (Istage)(CGlobal.RightAligner);
            m_distSens = CGlobal.Ds2000;


            //load xml file.
            m_ds = new DataSet();
            try
            {
                m_ds.ReadXml(Application.StartupPath + @"\config\stagePos.xml", XmlReadMode.ReadSchema);
            }
            catch
            {
                //xml 파일 없음...
                DataTable tbl = NewTable();
                m_ds.Tables.Add(tbl);
            }




            //data binding.
            hgdvList.HanDefaultSetting();
            hgdvList.Font = new System.Drawing.Font("Source Code Pro", 7, FontStyle.Regular);
            hgdvList.DeleteAllRows();
            hgdvList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            hgdvList.ReadOnly = true;
            hgdvList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            /*
            DataView dv = m_ds.Tables["DATA"].DefaultView;
            string[] cols = { "No", "Comments" };
            hgdvList.DataSource = dv.ToTable(true, cols);
            */
            hgdvList.DataSource = m_ds.Tables["DATA"];

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }


    }


    /// <summary>
    /// move.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnGo_Click(object sender, EventArgs e)
    {

        string msg = "이동 하시겠습니까?";
        DialogResult dialRes = MessageBox.Show(msg,
                                                "확인",
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Question);
        if (dialRes == DialogResult.No)
            return;



        try
        {



            //postions.
            CStageAbsPos  leftPos = new CStageAbsPos ();
            CStageAbsPos  rightPos = new CStageAbsPos ();
            leftPos.x = Math.Round(Convert.ToDouble(txtLeftX.Text), 1);
            leftPos.y = Math.Round(Convert.ToDouble(txtLeftY.Text), 1);
            leftPos.z = Math.Round(Convert.ToDouble(txtLeftZ.Text), 1);
            leftPos.tx = Math.Round(Convert.ToDouble(txtLeftTx.Text), 1);
            leftPos.ty = Math.Round(Convert.ToDouble(txtLeftTy.Text), 1);
            leftPos.tz = Math.Round(Convert.ToDouble(txtLeftTz.Text), 1);

            rightPos.x = Math.Round(Convert.ToDouble(txtRightX.Text), 1);
            rightPos.y = Math.Round(Convert.ToDouble(txtRightY.Text), 1);
            rightPos.z = Math.Round(Convert.ToDouble(txtRightZ.Text), 1);
            rightPos.tx = Math.Round(Convert.ToDouble(txtRightTx.Text), 1);
            rightPos.ty = Math.Round(Convert.ToDouble(txtRightTy.Text), 1);
            rightPos.tz = Math.Round(Convert.ToDouble(txtRightTz.Text), 1);


            //------------- move -------------
            Istage stage = null;
            System.Windows.Forms.CheckBox chk = null;
            double pos = 0;
            int axis = 0;

            //------------left except z-axis ------
            stage = m_leftStage;

            chk = chkLeftX;
            axis = stage.AXIS_X;
            pos = leftPos.x;
            if (chk.Checked == true)
                stage.AbsMove(axis, pos);

            chk = chkLeftY;
            axis = stage.AXIS_Y;
            pos = leftPos.y;
            if (chk.Checked == true)
                stage.AbsMove(axis, pos);


            chk = chkLeftTx;
            axis = stage.AXIS_TX;
            pos = leftPos.tx;
            if (chk.Checked == true)
                stage.AbsMove(axis, pos);

            chk = chkLeftTy;
            axis = stage.AXIS_TY;
            pos = leftPos.ty;
            if (chk.Checked == true)
                stage.AbsMove(axis, pos);

            chk = chkLeftTz;
            axis = stage.AXIS_TZ;
            pos = leftPos.tz;
            if (chk.Checked == true)
                stage.AbsMove(axis, pos);



            //------------right except z-axis ------
            stage = m_rightStage;

            chk = chkRightX;
            axis = stage.AXIS_X;
            pos = rightPos.x;
            if (chk.Checked == true)
                stage.AbsMove(axis, pos);

            chk = chkRightY;
            axis = stage.AXIS_Y;
            pos = rightPos.y;
            if (chk.Checked == true)
                stage.AbsMove(axis, pos);

            chk = chkRightTx;
            axis = stage.AXIS_TX;
            pos = rightPos.tx;
            if (chk.Checked == true)
                stage.AbsMove(axis, pos);

            chk = chkRightTy;
            axis = stage.AXIS_TY;
            pos = rightPos.ty;
            if (chk.Checked == true)
                stage.AbsMove(axis, pos);

            chk = chkRightTz;
            axis = stage.AXIS_TZ;
            pos = rightPos.tz;
            if (chk.Checked == true)
                stage.AbsMove(axis, pos);


            //------------left, right z-axis ------

            //left
            stage = m_leftStage;
            chk = chkLeftZ;
            axis = stage.AXIS_Z;
            pos = leftPos.z;
            if (chk.Checked == true)
                stage.AbsMove(axis, pos);


            //right.
            stage = m_rightStage;
            chk = chkRightZ;
            axis = stage.AXIS_Z;
            pos = rightPos.z;
            if (chk.Checked == true)
                stage.AbsMove(axis, pos);



            //detect the collision.
            bool bLeftComp = false;
            bool bRightComp = false;

            if (chkLeftZ.Checked == false)
                bLeftComp = true;

            if (chkRightZ.Checked == false)
                bRightComp = true;

            const double CONTACT_THRES = 4.0;
            double distSensLeft = 0.0; //left distance sensor value.
            double distSensRight = 0.0; //right distance sensor value.
            while (true)
            {


                //check collision.
                //Left,right 둘중 하나만 충돌해도 멈춘다.
                distSensLeft = m_distSens.ReadDist(SensorID.Left);
                distSensRight = m_distSens.ReadDist(SensorID.Right);
                if ((distSensLeft <= CONTACT_THRES) || (distSensRight <= CONTACT_THRES))
                {
                    m_leftStage.StopMove(m_leftStage.AXIS_Z);
                    m_rightStage.StopMove(m_rightStage.AXIS_Z);
                    break;
                }


                //check the moving-complete?
                if ((chkLeftZ.Checked == true) && (bLeftComp == false))
                {
                    if (m_leftStage.IsMovingOK())
                        bLeftComp = false;
                    else
                        bLeftComp = true;
                }


                if ((chkRightZ.Checked == true) && (bRightComp == false))
                {
                    if (m_rightStage.IsMovingOK())
                        bRightComp = false;
                    else
                        bRightComp = true;
                }

                if ((bLeftComp == true) && (bRightComp == true))
                    break;

            }//while


            

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
        finally
        {
            //update stage position 
            if (Application.OpenForms.OfType<uiStageControl>().Count() > 0)
            {
				uiStageControl frm = Application.OpenForms.OfType<uiStageControl>().FirstOrDefault();
                frm.UpdateAxisPos();
            }
        }


    }


    /// <summary>
    /// 선택한 것을 지운다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnDelete_Click(object sender, EventArgs e)
    {

        //list number.
        int listNo = 0;
        try
        {
            listNo = Convert.ToInt32(lbListNo.Text);
        }
        catch
        {
            MessageBox.Show("wrong List number ");
            return;
        }



        string strMsg = "정말 리스트" + listNo.ToString() + "를 삭제하시겠습니까?";
        if (DialogResult.No == MessageBox.Show(strMsg, "확인", MessageBoxButtons.YesNo))
            return;



        //find row.
        IEnumerable<DataRow> query = from data in m_ds.Tables["DATA"].AsEnumerable()
                                     where data.Field<int>("no") == listNo
                                     select data;

        IEnumerator<DataRow> enu = query.GetEnumerator();
        enu.MoveNext();


        //delete row.
        DataRow dr = enu.Current;
        dr.Delete();



        ClearInformation();

    }


    /// <summary>
    /// terminate form.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmStagePos_FormClosing(object sender, FormClosingEventArgs e)
    {
        //save dataset to xmlfile.
        if (m_ds != null)
            m_ds.WriteXml(Application.StartupPath + @"\config\stagePos.xml", XmlWriteMode.WriteSchema);

    }

    private void btnCancel_Click(object sender, EventArgs e)
    {

    }

    private void btnApply_Click(object sender, EventArgs e)
    {

    }
}


