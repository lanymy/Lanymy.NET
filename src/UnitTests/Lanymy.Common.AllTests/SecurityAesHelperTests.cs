using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Channels;
using System.Threading.Tasks;
using Lanymy.Common.Abstractions.Models;
using Lanymy.Common.Helpers;
using Lanymy.Common.Instruments;
using Lanymy.Common.Instruments.CryptoModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.Common.AllTests
{



    [TestClass()]
    public class SecurityAesHelperTests
    {



        [TestMethod()]
        public async Task SecurityAesHelperTest()
        {

            var securityKey = "securityKey Hello World";
            var iv = "0123456789";
            var sourceString = "good-morning";


            var encryptString = SecurityAesHelper.EncryptStringToString(sourceString, securityKey);
            var decryptString = SecurityAesHelper.DecryptStringFromString(encryptString, securityKey);


            encryptString = SecurityAesHelper.EncryptStringToString(sourceString, securityKey, iv);
            decryptString = SecurityAesHelper.DecryptStringFromString(encryptString, securityKey, iv);



            var scheduleFileInfoModel = new ScheduleFileInfoModel
            {
                SourceFileFullPath = DateTime.Now.ToString(),
                TargetFileFullPath = DateTime.Now.ToString(),
            };

            var json = JsonSerializeHelper.SerializeToJson(scheduleFileInfoModel);

            encryptString = SecurityAesHelper.EncryptStringToString(json, securityKey, iv);
            decryptString = SecurityAesHelper.DecryptStringFromString(encryptString, securityKey, iv);


            await Task.CompletedTask;

        }



    }



}