using System.IO;
using Lanymy.Common.Abstractions.Models;
using Lanymy.Common.Helpers;
using Lanymy.Common.Instruments;
using Lanymy.Common.Instruments.CryptoModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.Common.AllTests
{



    [TestClass()]
    public class LanymyCryptoTests
    {



        [TestMethod()]
        public void LanymyCryptoTest()
        {

            var securityKey = "Hello World";
            var sourceString = "good-morning";


            var crypto = new LanymyCrypto();

            var encryptBase64StringDigestInfoModel = crypto.EncryptStringToBase64String(sourceString, securityKey, false);
            var encryptBase64StringDigestInfoModel2 = crypto.EncryptStringToBase64String(sourceString, securityKey, false);


            Assert.AreEqual(sourceString, encryptBase64StringDigestInfoModel.SourceString);
            Assert.AreEqual(encryptBase64StringDigestInfoModel.SourceString, encryptBase64StringDigestInfoModel2.SourceString);
            Assert.AreEqual(encryptBase64StringDigestInfoModel.SourceBytesHashCode, encryptBase64StringDigestInfoModel2.SourceBytesHashCode);
            Assert.AreEqual(encryptBase64StringDigestInfoModel.EncryptedBase64String, encryptBase64StringDigestInfoModel2.EncryptedBase64String);
            Assert.AreEqual(encryptBase64StringDigestInfoModel.EncryptBytesHashCode, encryptBase64StringDigestInfoModel2.EncryptBytesHashCode);


            encryptBase64StringDigestInfoModel = crypto.DecryptStringFromBase64String(encryptBase64StringDigestInfoModel.EncryptedBase64String, securityKey);


            Assert.AreEqual(sourceString, encryptBase64StringDigestInfoModel.SourceString);
            Assert.AreEqual(encryptBase64StringDigestInfoModel2.SourceBytesHashCode, encryptBase64StringDigestInfoModel.SourceBytesHashCode);


            var encryptBase64StringDigestInfoModelRandom1 = crypto.EncryptStringToBase64String(sourceString, securityKey, true);
            var encryptBase64StringDigestInfoModelRandom2 = crypto.EncryptStringToBase64String(sourceString, securityKey, true);


            Assert.AreEqual(encryptBase64StringDigestInfoModelRandom1.SourceString, encryptBase64StringDigestInfoModelRandom2.SourceString);
            Assert.AreEqual(encryptBase64StringDigestInfoModelRandom1.SourceBytesHashCode, encryptBase64StringDigestInfoModelRandom2.SourceBytesHashCode);


            Assert.AreNotEqual(encryptBase64StringDigestInfoModelRandom1.EncryptedBase64String, encryptBase64StringDigestInfoModelRandom2.EncryptedBase64String);
            Assert.AreNotEqual(encryptBase64StringDigestInfoModelRandom1.EncryptBytesHashCode, encryptBase64StringDigestInfoModelRandom2.EncryptBytesHashCode);


            var imageFileFullPath = Path.Combine(PathHelper.GetCallDomainPath(), "1.jpg");


            ////var encryptStringImageFileDigestInfoModel = crypto.EncryptStringToImageFile(sourceEncryptBase64StringDigestInfoModel.SourceString, imageFileFullPath, securityKey);
            //var encryptStringImageFileDigestInfoModel = crypto.DecryptStringFromImageFile(imageFileFullPath, securityKey);

            //Assert.AreEqual(sourceEncryptBase64StringDigestInfoModel.SourceString, encryptStringImageFileDigestInfoModel.SourceString);
            //Assert.AreEqual(sourceEncryptBase64StringDigestInfoModel.SourceBytesHashCode, encryptStringImageFileDigestInfoModel.SourceBytesHashCode);
            //Assert.AreNotEqual(sourceEncryptBase64StringDigestInfoModel.EncryptBytesHashCode, encryptStringImageFileDigestInfoModel.EncryptBytesHashCode);


            var sourceModel = new ScheduleFileInfoModel
            {
                SourceFileFullPath = "1",
                TargetFileFullPath = "2",
            };


            ////var encryptModelImageFileDigestInfoModel = crypto.EncryptModelToImageFile(sourceModel, imageFileFullPath, securityKey);
            //var encryptModelImageFileDigestInfoModel = crypto.DecryptModelFromImageFile<ScheduleFileInfoModel>(imageFileFullPath, securityKey);

            //Assert.AreEqual(sourceModel.SourceFileFullPath, encryptModelImageFileDigestInfoModel.SourceModel.SourceFileFullPath);
            //Assert.AreEqual(sourceModel.TargetFileFullPath, encryptModelImageFileDigestInfoModel.SourceModel.TargetFileFullPath);




        }



    }



}
