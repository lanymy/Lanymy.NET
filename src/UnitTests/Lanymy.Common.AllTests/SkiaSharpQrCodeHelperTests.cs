﻿using System;
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
    public class SkiaSharpQrCodeHelperTests
    {





        [TestMethod()]
        public void SkiaSharpQrCodeHelperTest()
        {

            var domainFullPath = PathHelper.GetCallDomainPath();
            var fileName = string.Format("{0}.png", DateTime.Now.ToString(ConstKeys.DateTimeFormatKeys.DATE_TIME_FORMAT_2));
            var content = string.Format("[ {0} ] This is a sample message generated by SkiaSharp.QrCode.", DateTime.Now.ToString(ConstKeys.DateTimeFormatKeys.DATE_TIME_FORMAT_1));
            var qrCodeImageFileFullPath = Path.Combine(domainFullPath, fileName);

            SkiaSharpQrCodeHelper.CreateQrCode(qrCodeImageFileFullPath, content);

        }



    }



}