using System;
using System.IO;
using System.Net;
using System.Net.Http;
using Lanymy.Common.ConstKeys;

namespace Lanymy.Common.Abstractions.Models.AttachmentInfoModels
{
    public class PushFileStreamModel
    {

        public string PushFileFullPath { get; }

        public PushFileStreamModel(string pushFileFullPath)
        {

            PushFileFullPath = pushFileFullPath;

        }



        public async void WriteFileToPushStream(Stream outputStream, HttpContent content, TransportContext context)
        {

            try
            {

                var buffer = new byte[BufferSizeKeys.BUFFER_SIZE_80K];

                using (var fileStream = new FileStream(PushFileFullPath, FileMode.Open, FileAccess.Read, FileShare.Read, BufferSizeKeys.BUFFER_SIZE_80K))
                {

                    await fileStream.CopyToAsync(outputStream, BufferSizeKeys.BUFFER_SIZE_80K);

                }

                //using (var video = File.Open(PushFileFullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                //{
                //    var length = (int)video.Length;
                //    var bytesRead = 1;

                //    while (length > 0 && bytesRead > 0)
                //    {
                //        bytesRead = video.Read(buffer, 0, Math.Min(length, buffer.Length));
                //        await outputStream.WriteAsync(buffer, 0, bytesRead);
                //        length -= bytesRead;
                //    }
                //}

            }
            //catch (HttpException ex)
            //{
            //    return;
            //}
            catch (Exception ex)
            {
                return;
            }
            finally
            {
                outputStream.Close();
            }

        }


    }
}
