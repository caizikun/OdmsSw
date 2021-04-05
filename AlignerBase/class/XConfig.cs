using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Collections;

public class XConfig : IDisposable
{
    #region ==== XML IO ====

    static Dictionary<string, string> readXml(string strFilePath)
    {
        var dic = new Dictionary<string, string>();

        if (!File.Exists(strFilePath)) return dic;
        var configFile = Path.GetFileName(strFilePath);

        XmlReaderSettings settings = new XmlReaderSettings();
        settings.IgnoreWhitespace = false;

        using (var reader = XmlReader.Create(strFilePath, settings))
        {
            reader.MoveToContent();     //root node로 이동.
            while (true)
            {
                if (!reader.Read()) break;
                if (reader.NodeType == XmlNodeType.Element)
                {
                    //key
                    var strKey = reader.LocalName;
                    //value
                    if (!reader.Read()) throw new Exception($"설정파일 <{configFile}>에 키 <{strKey}>에 대한 값이 없습니다.");
                    var strValue = reader.Value;
                    dic.Add(strKey, strValue);
                }                
            }
            reader.Close();
        }
        return dic;
    }

    static void writeXml(Dictionary<string, string> dic, string strFilePath)
    {
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.Encoding = Encoding.UTF8;
        using (var writer = XmlWriter.Create(strFilePath, settings))
        {
            writer.WriteStartDocument();    //XML 선언 쓰기.
            writer.WriteStartElement("configs");    //<configs>

            foreach (var key in dic.Keys)
            {
                writer.WriteStartElement(key.ToString()); //element name
                writer.WriteString(dic[key].ToString());        //text
                writer.WriteEndElement();
            }
            writer.WriteEndElement();   //</configs>
            writer.WriteEndDocument();
            writer.Close();
        }
    }
    #endregion

    public static string mConfigFolder = "";

    private string mFilePath;
    private string mFileName;
    private Dictionary<string, string> mDic;
    public XConfig(string strFilePath)
    {
        var dir = Path.GetDirectoryName(strFilePath);
        if(!dir.Contains(mConfigFolder)) dir = Path.Combine(dir, mConfigFolder);
        mFileName = Path.GetFileName(strFilePath);
        mFilePath = Path.Combine(dir, mFileName);
        
        mDic = readXml(mFilePath);
        SaveOnDispose = true;
    }
    
    public string GetValue(string key)
    {
        return GetValue(key, "");
    }
    public string GetValue(string key, object initValue)
    {
        key = key.Replace("[", "");
        key = key.Replace("]", "");
        if (!mDic.ContainsKey(key))
        {
            mDic.Add(key, initValue.ToString());
            //throw new Exception($"설정파일 <{mFileName}>에 키 <{key}>가 없습니다.");
        }
        return mDic[key];
    }

    
    public void SetValue(string strKey, string strValue)
    {
        strKey = strKey.Replace("[", "").Replace("]", "");
        mDic[strKey] = strValue;
    }

    public void Load()
    {
        mDic = readXml(mFilePath);
    }
    public void Save()
    {
        writeXml(mDic, mFilePath);
    }


    public bool SaveOnDispose { get; set; }
    public void Dispose()
    {
        if(SaveOnDispose) writeXml(mDic, mFilePath);
    }
}
