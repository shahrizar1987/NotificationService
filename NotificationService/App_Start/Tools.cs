using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NotificationService.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NotificationService.App_Start
{
    public class Tools
    {
        public async Task<(string Result, int StatusCode)> SendFCM(string LegacyServerKey, object Body)
        {
            string result = string.Empty;
            int statuscode = 0;

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://fcm.googleapis.com/fcm/send");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers.Add("Authorization", string.Format("key={0}", LegacyServerKey));

            try
            {
                using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(JsonConvert.SerializeObject(Body));
                }

                using (var response = await httpWebRequest.GetResponseAsync())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        result = await reader.ReadToEndAsync();
                        statuscode = 200;
                    }
                }
            }
            catch (WebException ex)
            {
                using (var response = ex.Response as HttpWebResponse)
                {
                    statuscode = (int)response.StatusCode;
                    result = await new StreamReader(ex.Response.GetResponseStream()).ReadToEndAsync();
                }
            }

            return (result, statuscode);
        }

        public async void Write(string logMessage, string logType)
        {
            string Path = @"D:\Logs\NotificationService\";
            string Filename = string.Format("{0}-{1}.txt", logType, DateTime.Now.ToString("dd-MM-yyyy"));
            string FullPath = Path + Filename;

            try
            {
                using (FileStream objFilestream = new FileStream(Path + Filename, FileMode.Append, FileAccess.Write))
                {
                    using (StreamWriter writer = new StreamWriter(objFilestream))
                    {
                        await writer.WriteLineAsync(string.Format("Start Log at DateTime: {0}{1}{2}End Log{3}", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"), Environment.NewLine, logMessage, Environment.NewLine));
                    }
                }
            }
            catch (Exception)
            {
                
            }
        }

        public async Task<string> SMS(string MID, string Body)
        {
            var OutletResult = await new Mcard().GetOutletDetail(MID);
            if (OutletResult != null)
            {
                if (!string.IsNullOrEmpty(OutletResult.MobileNo))
                {
                    var SMSStatus = await new Mcard().SMSSend(Guid.NewGuid().ToString("N"), OutletResult.MobileNo, Body);
                    StringBuilder log = new StringBuilder();
                    log.AppendLine("Mobile No : " + OutletResult.MobileNo);
                    log.AppendLine("MID       : " + MID);
                    log.AppendLine("SMS Status: " + Body);
                    new Tools().Write(log.ToString(), "Success");
                    return "Success";
                }
                else
                {
                    StringBuilder log = new StringBuilder();
                    log.AppendLine("Mobile No : Not Found");
                    log.AppendLine("MID       : " + MID);
                    log.AppendLine("SMS Status: N/A");
                    log.AppendLine("Error     : Mobile No For This MID Is Not Found Inside Table MMS.Outlet");
                    new Tools().Write(log.ToString(), "Fail");
                    return "Fail";
                }
            }
            else
            {
                StringBuilder log = new StringBuilder();
                log.AppendLine("Mobile No : Not Found");
                log.AppendLine("MID       : " + MID);
                log.AppendLine("SMS Status: N/A");
                log.AppendLine("Error     : MID Not Found Inside Table MMS.Outlet");
                new Tools().Write(log.ToString(), "Fail");
                return "Fail";
            }
        }

        public async Task<string> PushNotification(string TID, string MID, string Title, string Body)
        {
            var FCMResult = await new Mcard().GetTerminalDetail(TID, MID);
            if (FCMResult != null)
            {
                JObject obj = new JObject
                {
                    { "to", FCMResult.DeviceRegToken },
                    { "data",
                        new JObject
                        {
                            { "title", Title },
                            { "body", Body }
                        }
                    }
                };

                var (Result, StatusCode) = await new Tools().SendFCM(FCMResult.LegacyServerKey, obj);

                StringBuilder log = new StringBuilder();
                log.AppendLine("TID  : " + TID);
                log.AppendLine("MID  : " + MID);
                log.AppendLine("Title: " + Title);
                log.AppendLine("Body : " + Body);
                log.AppendLine("FCM Request : " + JsonConvert.SerializeObject(obj));
                log.AppendLine("FCM Response: " + Result);
                new Tools().Write(log.ToString(), "Success");
                return "Success";
            }
            else
            {
                StringBuilder log = new StringBuilder();
                log.AppendLine("TID  : " + TID);
                log.AppendLine("MID  : " + MID);
                log.AppendLine("Title: " + Title);
                log.AppendLine("Body : " + Body);
                log.AppendLine("Error: TID & MID Not Found Inside Table App.TermAppAcct");
                new Tools().Write(log.ToString(), "Fail");
                return "Fail";
            }
        }
    }
}