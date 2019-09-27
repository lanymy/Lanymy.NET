using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Lanymy.Common.ConstKeys;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Models.AttachmentInfoModels;

namespace Lanymy.Common
{
    /// <summary>
    /// HTTP 请求通用辅助类
    /// </summary>
    public class HttpHelper
    {



        /// <summary>
        /// 从 HttpResponseMessage 中 解析出 对象
        /// </summary>
        /// <typeparam name="TReturnDataModel">返回对象的类型</typeparam>
        /// <param name="httpResponseMessage">当前 HttpResponseMessage 实例</param>
        /// <returns></returns>
        public static async Task<TReturnDataModel> GetResponseContentDataAsync<TReturnDataModel>(HttpResponseMessage httpResponseMessage)
        {

            return await httpResponseMessage.Content.ReadAsAsync<TReturnDataModel>();

        }

        /// <summary>
        /// 从 HttpResponseMessage 中 解析出 字符串
        /// </summary>
        /// <param name="httpResponseMessage">当前 HttpResponseMessage 实例</param>
        /// <returns></returns>
        public static async Task<string> GetResponseContentStringAsync(HttpResponseMessage httpResponseMessage)
        {

            return await httpResponseMessage.Content.ReadAsStringAsync();

        }

        /// <summary>
        /// GET 请求 地址
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="parameters">请求的参数 键值对 字典</param>
        public static async Task<string> HttpGetAsync(string url, IReadOnlyDictionary<string, object> parameters = null)
        {

            string html = string.Empty;

            using (var httpClient = HttpClientFactory.Create())
            {

                string strParameters = string.Empty;

                if (!parameters.IfIsNullOrEmpty())
                {

                    var buffer = new StringBuilder();

                    foreach (var parameter in parameters)
                    {
                        buffer.AppendFormat("&{0}={1}", parameter.Key, parameter.Value);
                    }

                    if (!url.Contains("?"))
                    {
                        buffer[0] = '?';
                    }

                    strParameters = buffer.ToString();
                    buffer.Clear();

                }

                var httpResponseMessage = await httpClient.GetAsync(url + strParameters);

                html = await GetResponseContentStringAsync(httpResponseMessage);

            }

            return html;

        }


        /// <summary>
        /// GET 请求 地址
        /// </summary>
        /// <typeparam name="TReturnDataModel">返回数据实体类型</typeparam>
        /// <param name="url">请求地址</param>
        /// <param name="parameters">请求的参数 键值对 字典</param>
        /// <returns></returns>
        public static async Task<TReturnDataModel> HttpGetAsync<TReturnDataModel>(string url, IReadOnlyDictionary<string, object> parameters = null) where TReturnDataModel : class
        {


            var html = await HttpPostAsync(url, parameters);
            return SerializeHelper.DeserializeFromJson<TReturnDataModel>(html);

        }

        /// <summary>
        /// POST 请求 地址
        /// </summary>
        /// <param name="url">请求 地址</param>
        /// <param name="postJson">post 的 json 字符串</param>
        /// <returns></returns>
        public static async Task<string> HttpPostAsync(string url, string postJson)
        {
            string html = string.Empty;

            using (var httpClient = HttpClientFactory.Create())
            {

                using (var httpContent = new StringContent(postJson))
                {

                    httpContent.Headers.ContentType = new MediaTypeHeaderValue(HttpContentTypeKeys.APPLICATION_JSON);

                    var httpResponseMessage = await httpClient.PostAsync(url, httpContent);

                    //data = await GetResponseContentDataAsync<TReturnDataModel>(httpResponseMessage);
                    html = await GetResponseContentStringAsync(httpResponseMessage);
                }

            }

            return html;

        }

        /// <summary>
        /// POST 请求 地址
        /// </summary>
        /// <typeparam name="TReturnDataModel">请求 返回的 实体类</typeparam>
        /// <param name="url">请求 地址</param>
        /// <param name="postJson">post 的 json 字符串</param>
        /// <returns></returns>
        public static async Task<TReturnDataModel> HttpPostAsync<TReturnDataModel>(string url, string postJson) where TReturnDataModel : class
        {

            var html = await HttpPostAsync(url, postJson);
            return SerializeHelper.DeserializeFromJson<TReturnDataModel>(html);

        }


        /// <summary>
        /// POST 请求 地址
        /// </summary>
        /// <typeparam name="TReturnDataModel">请求 返回的 实体类</typeparam>
        /// <typeparam name="TPostDataModel">POST请求数据实体类类型</typeparam>
        /// <param name="url">请求 地址</param>
        /// <param name="postDataModel">POST请求数据实体类实例</param>
        /// <returns></returns>
        public static async Task<TReturnDataModel> HttpPostAsync<TReturnDataModel, TPostDataModel>(string url, TPostDataModel postDataModel)
            where TReturnDataModel : class
            where TPostDataModel : class
        {
            return await HttpPostAsync<TReturnDataModel>(url, SerializeHelper.SerializeToJson(postDataModel));
        }

        /// <summary>
        /// POST 请求 地址
        /// </summary>
        /// <typeparam name="TPostDataModel">POST请求数据实体类类型</typeparam>
        /// <param name="url">请求 地址</param>
        /// <param name="postDataModel">POST请求数据实体类实例</param>
        /// <returns></returns>
        public static async Task<string> HttpPostAsync<TPostDataModel>(string url, TPostDataModel postDataModel) where TPostDataModel : class
        {

            return await HttpPostAsync(url, SerializeHelper.SerializeToJson(postDataModel));

        }


        /// <summary>
        /// POST附件
        /// </summary>
        /// <param name="url">POST地址</param>
        /// <param name="attachmentList">附件元数据列表</param>
        /// <returns></returns>
        public static async Task<string> HttpPostMultipartFormDataAsync(string url, List<BaseAttachmentInfoModel> attachmentList)
        {

            string html = string.Empty;

            using (var httpClient = HttpClientFactory.Create())
            {

                using (var httpContent = new MultipartFormDataContent())
                {

                    foreach (var attachmentInfoModel in attachmentList)
                    {

                        if (attachmentInfoModel is StringAttachmentInfoModel stringAttachmentInfoModel)
                        {
                            httpContent.Add(new StringContent(stringAttachmentInfoModel.AttachmentData), stringAttachmentInfoModel.KeyName);

                        }
                        else if (attachmentInfoModel is FileAttachmentInfoModel fileAttachmentInfoModel)
                        {
                            var fileFullPath = fileAttachmentInfoModel.AttachmentData;

                            var pushFileStreamModel = new PushFileStreamModel(fileFullPath);

                            var pushStreamContent = new PushStreamContent(pushFileStreamModel.WriteFileToPushStream);

                            httpContent.Add(pushStreamContent, fileAttachmentInfoModel.KeyName, Path.GetFileName(fileFullPath));

                        }

                    }


                    var httpResponseMessage = await httpClient.PostAsync(url, httpContent);

                    html = await GetResponseContentStringAsync(httpResponseMessage);


                    //var content = new MultipartFormDataContent();
                    ////添加字符串参数，参数名为qq

                    //string path = Path.Combine(System.Environment.CurrentDirectory, "1.png");
                    ////添加文件参数，参数名为files，文件名为123.png
                    //content.Add(new ByteArrayContent(System.IO.File.ReadAllBytes(path)), "file", "123.png");

                    ////using (var httpContent = new StringContent(postJson))
                    ////{

                    ////    httpContent.Headers.ContentType = new MediaTypeHeaderValue(HttpContentTypeKeys.APPLICATION_JSON);

                    ////    var httpResponseMessage = await httpClient.PostAsync(url, httpContent);

                    ////    //data = await GetResponseContentDataAsync<TReturnDataModel>(httpResponseMessage);
                    ////    html = await GetResponseContentStringAsync(httpResponseMessage);
                    ////}


                }

            }

            return html;

        }


        /// <summary>
        /// POST附件
        /// </summary>
        /// <typeparam name="TReturnDataModel">请求返回的数据实体类</typeparam>
        /// <param name="url">POST地址</param>
        /// <param name="attachmentList">附件元数据列表</param>
        /// <returns></returns>
        public static async Task<TReturnDataModel> HttpPostMultipartFormDataAsync<TReturnDataModel>(string url, List<BaseAttachmentInfoModel> attachmentList) where TReturnDataModel : class
        {

            var html = await HttpPostAsync(url, attachmentList);
            return SerializeHelper.DeserializeFromJson<TReturnDataModel>(html);

        }


    }
}
