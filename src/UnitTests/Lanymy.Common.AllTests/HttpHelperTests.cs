using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Lanymy.Common.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.Common.AllTests
{
    [TestClass]
    public class HttpHelperTests
    {
        private sealed class EchoRequest
        {
            public string Name { get; set; }
        }

        private sealed class EchoResponse
        {
            public string Value { get; set; }
        }

        [TestMethod]
        public async Task HttpHelper_HttpGetAsync_WhenParametersProvided_ShouldReturnResponseBody()
        {
            await WithHttpListenerAsync(
                async prefix =>
                {
                    var response = await HttpHelper.HttpGetAsync(prefix, new System.Collections.Generic.Dictionary<string, object>
                    {
                        ["name"] = "lanymy",
                        ["count"] = 2,
                    });

                    Assert.AreEqual("/?name=lanymy&count=2", response);
                },
                async context =>
                {
                    await WriteResponseAsync(context, context.Request.RawUrl);
                });
        }

        [TestMethod]
        public async Task HttpHelper_HttpPostAsync_GenericWrapper_ShouldDeserializeResponse()
        {
            await WithHttpListenerAsync(
                async prefix =>
                {
                    var response = await HttpHelper.HttpPostAsync<EchoResponse, EchoRequest>(prefix, new EchoRequest
                    {
                        Name = "lanymy",
                    });

                    Assert.IsNotNull(response);
                    Assert.AreEqual("lanymy", response.Value);
                },
                async context =>
                {
                    using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
                    var body = await reader.ReadToEndAsync();
                    var request = JsonSerializeHelper.DeserializeFromJson<EchoRequest>(body);

                    await WriteJsonResponseAsync(context, new EchoResponse
                    {
                        Value = request?.Name,
                    });
                });
        }

        private static async Task WithHttpListenerAsync(System.Func<string, Task> clientAction, System.Func<HttpListenerContext, Task> serverAction)
        {
            var port = GetFreePort();
            var prefix = $"http://127.0.0.1:{port}/";

            using var listener = new HttpListener();
            listener.Prefixes.Add(prefix);
            listener.Start();

            var serverTask = Task.Run(async () =>
            {
                var context = await listener.GetContextAsync();
                try
                {
                    await serverAction(context);
                }
                finally
                {
                    context.Response.OutputStream.Close();
                }
            });

            try
            {
                await clientAction(prefix);
                await serverTask;
            }
            finally
            {
                listener.Stop();
            }
        }

        private static async Task WriteResponseAsync(HttpListenerContext context, string body)
        {
            var bytes = Encoding.UTF8.GetBytes(body ?? string.Empty);
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.ContentType = "text/plain; charset=utf-8";
            context.Response.ContentLength64 = bytes.Length;
            await context.Response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
        }

        private static Task WriteJsonResponseAsync(HttpListenerContext context, object payload)
        {
            context.Response.ContentType = "application/json; charset=utf-8";
            return WriteResponseAsync(context, JsonSerializeHelper.SerializeToJson(payload));
        }

        private static int GetFreePort()
        {
            using var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            return ((IPEndPoint)listener.LocalEndpoint).Port;
        }
    }
}
