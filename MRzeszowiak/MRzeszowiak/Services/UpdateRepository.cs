using Newtonsoft.Json;
using Prism.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;


namespace MRzeszowiak.Services
{
    class UpdateRepository
    {
        protected readonly ISetting _setting;

        public float VersionServer { get; private set; }
        public String VersionUpdateUrl { get; private set; } = String.Empty;
        public bool IsNewVersion { get => (VersionServer > _setting?.GetAppVersion) && (VersionUpdateUrl.Length > 0); }

        public UpdateRepository(ISetting setting)
        {
            _setting = setting ?? throw new NullReferenceException("ISetting setting == null !");
        }

        public async Task<bool> CheckUpdate()
        {
            #if DEBUG
            Debug.Write(System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "CheckUpdate(): " + _setting.UpdateServerUrl);
            #endif

            VersionServer = 0f;
            VersionUpdateUrl = String.Empty;

            if (!Uri.IsWellFormedUriString(_setting.UpdateServerUrl, UriKind.Absolute))
            {
                #if DEBUG
                Debug.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, "URI is incorrect");
                #endif
                return false;
            }

            WebRequest request = null;
            try
            {
                request = WebRequest.Create(_setting.UpdateServerUrl);
                request.Timeout = 10000;
            }
            catch (System.Exception e)
            {
                #if DEBUG
                Debug.Write(System.Reflection.MethodBase.GetCurrentMethod().Name,
                        "WebRequest Exception: " + e.Message.ToString());
                #endif
                return false;
            }

            WebResponse response = null;
            String responseString;
            try
            {
                Task<WebResponse> webResponseTask = request.GetResponseAsync();
                await Task.WhenAll(webResponseTask);

                if (webResponseTask.IsFaulted)
                {
                    #if DEBUG
                    Debug.Write(System.Reflection.MethodBase.GetCurrentMethod().Name,
                        "webResponseTask.IsFaulted");
                    #endif
                    return false;
                }

                response = webResponseTask.Result;
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    responseString = sr.ReadToEnd();
                }
            }
            catch (System.Exception e)
            {
                #if DEBUG
                Debug.Write(System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "WebResponse Exception: " + e.Message.ToString());
                #endif
                return false;
            }

            if (responseString.Length == 0)
            {
                #if DEBUG
                Debug.Write(System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "data.Length == 0");
                #endif
                return false;
            }

            ResponseUpdate responseModel;
            try
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                responseModel = JsonConvert.DeserializeObject<ResponseUpdate>(responseString, settings);
            }
            catch (System.Exception e)
            {
                #if DEBUG
                Debug.Write(System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "DeserializeObject<ResponseUpdate> problem: " + e.Message.ToString());
                #endif
                return false;
            }

            VersionServer = responseModel.AvailableVersion;
            VersionUpdateUrl = System.Net.WebUtility.UrlDecode(responseModel.UpdatePage);

            if(VersionUpdateUrl?.Length > 0)
                if (!Uri.IsWellFormedUriString(VersionUpdateUrl, UriKind.Absolute))
                {
                    #if DEBUG
                    Debug.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, "VersionUpdateUrl URI is incorrect");
                    #endif
                    VersionUpdateUrl = String.Empty;
                }
            return true;
        }

        class ResponseUpdate
        {
            public string UpdatePage;
            public float AvailableVersion;
        };
    }
}
