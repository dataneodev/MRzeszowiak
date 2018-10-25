using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MRzeszowiak.Extends
{   public static class GetWeb
    {
        public static async Task<GetWebPageResponse> GetWebPage(string Url)
        {
            var webPageResponse = new GetWebPageResponse();
            bool urlCorrect = Uri.TryCreate(Url, UriKind.Absolute, out Uri uriResult)
                                        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            if (!urlCorrect)
            {
                Debug.Write("GetWeb." + System.Reflection.MethodBase.GetCurrentMethod().Name + " => URL not correct");
                return webPageResponse;
            }
            using (var handler = new HttpClientHandler())
            {
                var cookies = new CookieContainer();
                handler.CookieContainer = cookies;
                using (HttpClient client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(6) })
                {
                    try
                    {
                        using (HttpResponseMessage response = await client.GetAsync(Url).ConfigureAwait(false))
                        {
                            webPageResponse.ResponseCode = (short)response.StatusCode;
                            if (!response.IsSuccessStatusCode)
                            {
                                Debug.Write("GetAdvertListAsync(AdvertSearch searchParams) => !response.IsSuccessStatusCode");
                                webPageResponse.ErrorMessage = "Niepoprawna odpowiedź z serwera";
                                return webPageResponse;
                            }

                            Uri uri = new Uri(Url);
                            var responseCookies = cookies.GetCookies(uri);

                            foreach (Cookie cookie in responseCookies)
                                webPageResponse.CookieList.Add(cookie);

                            using (HttpContent content = response.Content)
                            {
                                var byteArray = await content.ReadAsByteArrayAsync();
                                Encoding iso = Encoding.GetEncoding("ISO-8859-2");
                                Encoding utf8 = Encoding.UTF8;
                                byte[] utf8Bytes = Encoding.Convert(iso, utf8, byteArray);
                                webPageResponse.BodyString.Append(System.Net.WebUtility.HtmlDecode(utf8.GetString(utf8Bytes)));
                            }
                        }
                    }
                    catch (System.Threading.Tasks.TaskCanceledException)
                    {
                        Debug.Write("GetAdvertListAsync(AdvertSearch searchParams) => System.Threading.Tasks.TaskCanceledException");
                        webPageResponse.ErrorMessage = "Błąd połączenia. Przekroczono limit połączenia";
                        return webPageResponse;
                    }
                    catch (Exception e)
                    {
                        Debug.Write("GetAdvertListAsync(AdvertSearch searchParams) => " + e.Message);
                        webPageResponse.ErrorMessage = "Błąd połączenia z serwerem.";
                        return webPageResponse;
                    }
                }
            }
            webPageResponse.Success = true;
            return webPageResponse;
        }

        public class GetWebPageResponse
        {
            public bool Success;
            public StringBuilder BodyString { get; private set; } = new StringBuilder();
            public List<Cookie> CookieList { get; private set; } = new List<Cookie>();
            public short ResponseCode { get; set; }
            public string ErrorMessage = String.Empty;
        }

        public static async Task<StringBuilder> PostWebPage(string Url, Dictionary<string, string> postData,
                            string RefererUrl, CookieCollection cookieCollection, Action<string> userNotify = null)
        {
            StringBuilder BodyString = new StringBuilder();
            bool urlCorrect = Uri.TryCreate(Url, UriKind.Absolute, out Uri URI)
                              && (URI.Scheme == Uri.UriSchemeHttp || URI.Scheme == Uri.UriSchemeHttps);
            if (!urlCorrect)
            {
                Debug.Write("GetWeb." + System.Reflection.MethodBase.GetCurrentMethod().Name + " => URL not correct");
                return BodyString;
            }

            var BaseURI = new Uri(URI.GetLeftPart(UriPartial.Authority));
            var PathURI = new Uri(URI.PathAndQuery);
            using (var handler = new HttpClientHandler())
            {
                if(cookieCollection != null)
                {
                    var cookies = new CookieContainer();
                    cookies.Add(cookieCollection);
                    handler.UseCookies = true;
                    handler.CookieContainer = cookies;
                }
                else
                {
                    handler.UseCookies = false;
                }
                
                using (HttpClient client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(6) })
                {
                    try
                    {
                        client.BaseAddress = BaseURI;
                        var keyValues = new List<KeyValuePair<string, string>>();
                        foreach (var item in postData)
                            keyValues.Add(new KeyValuePair<string, string>(item.Key, item.Value));

                        var inputContent = new FormUrlEncodedContent(postData);

                        using (HttpResponseMessage response = await client.PostAsync(PathURI, inputContent).ConfigureAwait(false))
                        {
                            if (!response.IsSuccessStatusCode)
                            {
                                Debug.Write("GetAdvertListAsync(AdvertSearch searchParams) => !response.IsSuccessStatusCode");
                                userNotify?.Invoke("Niepoprawna odpowiedź z serwera");
                                return BodyString;
                            }
                            using (HttpContent content = response.Content)
                            {
                                var byteArray = await content.ReadAsStringAsync();
                                BodyString.Append(byteArray);
                            }
                        }
                    }
                    catch (System.Threading.Tasks.TaskCanceledException)
                    {
                        Debug.Write("PostWebPage => System.Threading.Tasks.TaskCanceledException");
                        userNotify?.Invoke("Błąd połączenia. Przekroczono limit połączenia");
                        return BodyString;
                    }
                    catch (Exception e)
                    {
                        Debug.Write("PostWebPage => " + e.Message);
                        userNotify?.Invoke("Błąd połączenia z serwerem.");
                        return BodyString;
                    }
                }
                return BodyString;
            }
        }
    }
}
