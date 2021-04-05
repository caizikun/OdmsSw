using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Jeffsoft;
using Free302.MyLibrary.Config;
using Neon.Aligner;

/// <summary>
/// 임시 라직
/// </summary>
public class MyLogic : IConfigUser
{


    #region ==== Class Framework ====

    //instance factory
    static MyLogic sMyLogic;//default unique instance
    public static MyLogic Instance { get { return sMyLogic; } }


    static MyLogic()
    {
        try
        {
            sMyLogic = new MyLogic();

            //config
            sConfig = new SelfConfig("MyLogic", sMyLogic);
            //sConfig.SaveConfig();
            sConfig.LoadConfig();
            sReference = new ReferenceData();

			initLog();

        }
        catch (Exception ex)
        {
            MessageBox.Show($"MyLogic.MyLogic():\n{ex.Message}");
        }
    }


    #endregion



    #region ==== Utility ====

    /// <summary>
    /// TForm 형식의 자식 창을 연다.
    /// </summary>
    /// <typeparam name="TForm"></typeparam>
    /// <param name="showIfExist">true : Form 생성 후 Show() 호출</param>
    /// <returns></returns>
    public static TForm CreateAndShow<TForm>(bool showIfExist = true, bool createNew = true) where TForm : Form, new()
    {
        return (TForm)Program.MainForm.Invoke((Func<TForm>)(() =>
        {
            var forms = Application.OpenForms.OfType<TForm>();

            TForm form;
            if (forms.Count() == 0)
            {
                if (createNew) form = new TForm();
                else return null;
            }
            else form = forms.First();

            form.MdiParent = Program.MainForm;
            if (showIfExist) form.Show();
            return form;
        }));

    }//ShowOrNew





    public static int[] parseIntArray(string strValue)
    {
        string[] strValues = strValue.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        var values = new int[strValues.Length];
        for (int i = 0; i < strValues.Length; i++) values[i] = int.Parse(strValues[i]);
        return values;
    }


    #region ---- log ----

    static StreamWriter sLogWriter;
    static int sLineCounter;
    static DateTime sLastTime;
    const int sMinLineCount = 3200;//min line count of log file
    const int sMinIntervalTime = 180;//min time between log writings, 180=spent time for 1 chip measuring

    static void initLog()
    {
        sLogWriter?.Close();

        //log
        var dir = Path.Combine(Application.StartupPath, "log");
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        var file = Path.Combine(dir, $"log_device_{DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss")}.txt");
        sLogWriter = new StreamWriter(file);
        sLineCounter = 0;
    }


    static bool checkIfNewLogFileIsNeeded()
    {
        if (sLineCounter < sMinLineCount) return false;
        TimeSpan time = DateTime.Now - sLastTime;
        if (time.Seconds < sMinIntervalTime) return false;

        return true;
    }

    /// <summary>
    /// write log message to file
    /// </summary>
    /// <param name="msg"></param>
    public static void log(string msg)
    {
        if (checkIfNewLogFileIsNeeded()) initLog();

        sLogWriter.WriteLine(msg);
        sLogWriter.Flush();
        sLineCounter++;
        sLastTime = DateTime.Now;
    }

    #endregion


    #endregion



    #region ==== IConfig ====

    static SelfConfig sConfig;

    #region ---- IConfigUser ----
    public void BuildDefaultConfig(ConfigBase config)
    {
        config.Add("LAST_REFERENCE_FILE", Path.Combine(Application.StartupPath, 
            $"{DEFAULT_REFERENCE_FILE_NAME}.{REFERENCE_EXT}"));


    }

    public ConfigBase BuildConfig()
    {
        //sConfig.Set("LAST_REFERENCE_FILE", lastReferenceFile);
        return sConfig;
    }

    public void ApplyConfig(ConfigBase config)
    {
        //lastReferenceFile = config["LAST_REFERENCE_FILE"];
    }

    public void SaveConfig()
    {
        sConfig.SaveConfig();
    }

    public void LoadConfig()
    {
        sConfig.LoadConfig();
    }
    #endregion

    #endregion



    #region ==== Reference Data ====

    const string DEFAULT_REFERENCE_FILE_NAME = "dwdm_reference";
    public const string REFERENCE_EXT = "ref";

    string lastReferenceFile
    {
        get { return sConfig["LAST_REFERENCE_FILE"]; }
        set { sConfig["LAST_REFERENCE_FILE"] = value; }
    }

    static ReferenceData sReference;
    public ReferenceData Reference { get { return sReference; } }


    /// <summary>
    /// 지난번 레퍼런스 파일을 읽어온다.
    /// 파일이 존재하지 않으면 파일선택창을 띄움.
    /// </summary>
    /// <param name="showDialog">참: 무조건 파일 선택창을 띄운다.</param>
    public void LoadReference(bool showDialog)
    {
        string fileName = lastReferenceFile;
        if (showDialog || fileName == null || !File.Exists(fileName))
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Title = "REFERENCE 파일??";
            fd.InitialDirectory = Application.StartupPath;
            fd.Filter =$"Reference File(*.{REFERENCE_EXT})|*.{REFERENCE_EXT}";
            if (fd.ShowDialog() != DialogResult.OK) return;
            fileName = fd.FileName;
        }

		sReference.LoadText(fileName);		

        lastReferenceFile = fileName;
    }


    public void ExportReference()
    {
        SaveFileDialog fd = new SaveFileDialog();
        fd.Title = "REFERENCE 파일??";
        fd.InitialDirectory = Path.GetDirectoryName(lastReferenceFile);
        fd.FileName = Path.GetFileName(lastReferenceFile);
        fd.Filter = $"Reference File(*.{REFERENCE_EXT})|*.{REFERENCE_EXT}";
        if (fd.ShowDialog() != DialogResult.OK) return;

		//save to file.
		sReference.SaveToTxt(fd.FileName);
		lastReferenceFile = fd.FileName;
	}


	#endregion



}
