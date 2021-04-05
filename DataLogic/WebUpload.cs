using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Globalization;
//using System.Windows.Forms;
using HtmlAgilityPack;

namespace Neon.Aligner
{
    public enum UploadCategory { CwdmMux, CwdmDemux, McMux, RichDemux }

    class WebUploader
    {
        #region ---- Param ----

        static HttpClient mClient = new HttpClient();

        const string mCsrfTokenName = "csrfmiddlewaretoken";
        const string mDataGetUrl_Mux = "http://192.168.0.6/uploadCWDMMUXmf/";
        const string mDataPostUrl_Mux = "http://192.168.0.6/postuploadCWDMMUXmf/";
        const string mDataGetUrl_Demux = "http://192.168.0.6/uploadCWDMDeMUXmf/";
        const string mDataPostUrl_Demux = "http://192.168.0.6/postuploadCWDMDeMUXmf/";
        const string mDataGetUrl_McMux = "http://192.168.0.6/upload_mcmux/";
        const string mDataPostUrl_McMux = "http://192.168.0.6/postupload_mcmux/";
        const string mDataGetUrl_RichDemux = "http://192.168.0.6/upload_richdemux/";
        const string mDataPostUrl_RichDemux = "http://192.168.0.6/postupload_richdemux/";

        static Dictionary<UploadCategory, string[]> mUrls = new Dictionary<UploadCategory, string[]>();

        static WebUploader()
        {
            mUrls.Add(UploadCategory.CwdmMux, new string[] { mDataGetUrl_Mux, mDataPostUrl_Mux });
            mUrls.Add(UploadCategory.CwdmDemux, new string[] { mDataGetUrl_Demux, mDataPostUrl_Demux });
            mUrls.Add(UploadCategory.McMux, new string[] { mDataGetUrl_McMux, mDataPostUrl_McMux });
            mUrls.Add(UploadCategory.RichDemux, new string[] { mDataGetUrl_RichDemux, mDataPostUrl_RichDemux });
        }

        #endregion



        // ex). return await Test("파일경로"+"파일명");
        public static async Task<int> Upload(string[] filePath, UploadCategory category, string worker, string comment)
        {

            MultipartFormDataContent multiData = new MultipartFormDataContent();

            var fileName0 = Path.GetFileName(filePath[0]);
            var sn0 = new SerialNumber(fileName0);
            string categoryName = "";
            if (category == UploadCategory.CwdmMux) categoryName = "CWDMMUX";
            else if (category == UploadCategory.CwdmDemux) categoryName = "CWDMDeMUX";
            else if (category == UploadCategory.McMux) categoryName = "mcmux";
            else categoryName = "richdemux";


            multiData.Add(new StringContent(categoryName), "category");

            multiData.Add(new StringContent(DateTime.Now.ToString("yyyy-MM-dd")), "indate");
            multiData.Add(new StringContent(worker), "operator");
            multiData.Add(new StringContent(""), "processcomment");
            var time = DateTime.Now.ToString("HH:mm:ss");
            multiData.Add(new StringContent($"{time} {comment}"), "comment");

            multiData.Add(new StringContent(""), "masknum");
            multiData.Add(new StringContent(sn0.Wafer), "wafername");


            for (int i = 0; i < filePath.Length; i++)
            {
                var fileName = Path.GetFileName(filePath[i]);
                var sn = new SerialNumber(fileName);
                multiData.Add(new StringContent(sn.ChipName), "chipname");// 여러개일경우 구분자는  Comma 임
                multiData.Add(new ByteArrayContent(File.ReadAllBytes(filePath[i])), "files", fileName);
            }

            var csrf = (mCsrf == null) ? await getCsrfToken(mUrls[category][0]) : mCsrf;
            multiData.Add(new StringContent(csrf), mCsrfTokenName);


            var responseMessage = await mClient.PostAsync(mUrls[category][1], multiData).ConfigureAwait(false);

            if (!responseMessage.IsSuccessStatusCode)
            {
                var errorFile = showUploadResponse(responseMessage);
                var index = Array.FindIndex(filePath, x => x.Contains(errorFile));

                return (errorFile == "") ? filePath.Length : index;
            }

            await Task.Delay(500);
            return 0;
        }



        static string showUploadResponse(HttpResponseMessage message)
        {
            string checkString = "oldfileprefix";
            string errorFile = "";
            var time = DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss");
            var fileName = $"./log/Response{time}.html";
            using (var sw = new StreamWriter(fileName))
            {
                var code = message.Content.ReadAsStringAsync().Result;
                sw.WriteLine(code.ToString());
                sw.Close();
                if (code.Contains(checkString))
                {
                    var temp = code.Substring(code.IndexOf(checkString), 102).Replace(checkString, "").Trim().TrimEnd('-');
                    errorFile = temp.Substring(temp.Length - 20).Trim('_');
                }
            }
            if(errorFile != "") File.Move(fileName, $"./log/Response_{errorFile}.html");
            return errorFile;
        }

  

        #region ---- Login ----

        const string mLoginGetUrl = @"http://192.168.0.6/login/";
        const string mLoginPostUrl = @"http://192.168.0.6/login/";

        static async Task<string> getCsrfToken(string url)
        {
            await Task.Delay(100);

            var resGet = await mClient.GetAsync(url);
            var resHtml = await resGet.Content.ReadAsStringAsync();

            //if (resHtml.Contains("로그인"))
            //{
            //    //await doLogin();
            //    resGet = await mClient.GetAsync(url);
            //    resHtml = await resGet.Content.ReadAsStringAsync();
            //}

            var web = new HtmlDocument();
            web.LoadHtml(resHtml);

            var inputs = web.DocumentNode.SelectSingleNode($"//*[@name='{mCsrfTokenName}']");
            var csrf = inputs.Attributes["value"].Value;

            if (csrf == null)
            {
                await Task.Delay(1000);
                csrf = await getCsrfToken(url);
                //if (csrf == null) Log.Write($"WebUploader.Upload(): csrf == null", true);
            }

            return csrf;
        }

        static string mCsrf = null;

        public static async Task doLogin()
        {
            var csrf = await getCsrfToken(mLoginGetUrl);
            mCsrf = csrf;

            var content = new MultipartFormDataContent();
            content.Add(new StringContent(csrf), mCsrfTokenName);
            content.Add(new StringContent("sybae", Encoding.ASCII), "username");
            content.Add(new StringContent("3569", Encoding.ASCII), "password");

            await mClient.PostAsync(mLoginPostUrl, content).Result.Content.ReadAsStringAsync();

        }
        #endregion


    }//class


}
