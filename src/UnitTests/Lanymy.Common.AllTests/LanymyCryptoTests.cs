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

            var sourceEncryptBase64StringDigestInfoModel = new EncryptBase64StringDigestInfoModel
            {

                SourceString = sourceString,
                SourceBytesHashCode = "19FCCFA2D5DBB32CFFA953BA9F2281EEEC5917CE",

                EncryptedBase64String = "AEFFRUM4Q0I4RjFCNkFCRjM5MENDMzE0N0MwQUREMTA1Q0UzMjkxN0HhAAAAH4sIAAAAAAAACm2QwYrCMBRF/yVrF9M4Ha27Jk1QQQTrDzzapxZqMrykCxX/3YDapuL2vMO5JDemTEWXf4+1AId/v6WnxhzZwnRtO2Gl7ajCb0xcPLo3GhoxXbmyqyp0ARygdRg8IkubQOCIg7UDU9vzq9G70UzZXIOe8H4ooj89lNZ4NB+3qLIEd5K2DpwlmZZS57xICyGmXGqdZ+lU5JnmfJ4opWSaJTOp2HhxKIyeHQ9/KgU+nSVCjbQyB7sJ13btrBn/6tafkArw8AaSEDwGgvvm/OrdH2gWOkivAQAAKwAAAAAAAAA0OEFCM0QyODcxMTU2ODMyOTg1MUU4MjMxMEJEREU2N0VGMjQ0RTU4H4sIAAAAAAAACurUrXJ60N3/fFXNN835YnUzAQAAAP//AwCMActzEAAAAA==",
                EncryptBytesHashCode = "5EC7FB1765738A23DC4B2BD1A3F8262078D30198",

            };


            var crypto = new LanymyCrypto();

            var encryptBase64StringDigestInfoModel = crypto.EncryptStringToBase64String(sourceString, securityKey, false);


            Assert.AreEqual(sourceEncryptBase64StringDigestInfoModel.SourceString, encryptBase64StringDigestInfoModel.SourceString);
            Assert.AreEqual(sourceEncryptBase64StringDigestInfoModel.SourceBytesHashCode, encryptBase64StringDigestInfoModel.SourceBytesHashCode);
            Assert.AreEqual(sourceEncryptBase64StringDigestInfoModel.EncryptedBase64String, encryptBase64StringDigestInfoModel.EncryptedBase64String);
            Assert.AreEqual(sourceEncryptBase64StringDigestInfoModel.EncryptBytesHashCode, encryptBase64StringDigestInfoModel.EncryptBytesHashCode);


            encryptBase64StringDigestInfoModel = crypto.DecryptStringFromBase64String(sourceEncryptBase64StringDigestInfoModel.EncryptedBase64String, securityKey);


            Assert.AreEqual(sourceEncryptBase64StringDigestInfoModel.SourceString, encryptBase64StringDigestInfoModel.SourceString);
            Assert.AreEqual(sourceEncryptBase64StringDigestInfoModel.SourceBytesHashCode, encryptBase64StringDigestInfoModel.SourceBytesHashCode);


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