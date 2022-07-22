using Lanymy.Common.Abstractions.Models;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.Common.AllTests
{
    [TestClass()]
    public class LanymyIsolatedStorageTests
    {
        [TestMethod()]
        public void SaveStringTest()
        {


            var model = new ScheduleFileInfoModel
            {
                SourceFileFullPath = "a",
                TargetFileFullPath = "b",
            };



            //fix
            var sourceEncryptStr = "AEIyM0VEMkY2QUM2NjgzRjVFNUNGRjc2Njg5ODM1NzkxMjExRDY2NUX2AAAAH4sIAAAAAAAAA22RQW6DMBBF7+J1F0BwGrKLcVBSKa1UcoER/CRIxI5ss6BV714rLWDSLv3m6X/N+JMddI322N/wSlewtera9mmChX+Gg1J3psJ9PKCtqkx/c6gFWSzT0plGnef+f0z0DvZvRkj3tuyqCtaDE7UW3jNGm4MndMZkvZOq9fU3Y3SDmrL58DqPx6KARiPMtXJQD7MgZUf2kvvN2ZoVMl4snxOeJXEsN3yTp6uUi0wu0m1WRCueJCLygmDzxilhtnZY/KhI/Dg7UA2zVyd9P/6L1Wp+1Td3gZHkaAC5ATl4gmMzfODXN2oxHazwAQAATwAAAAAAAAA4NjkyMkRENjcwMjY5ODNFNjg0Rjc1QkJGQUQ0OTRENkE5QzU3MkFEH4sIAAAAAAAAAwE4AMf/W2PA68hItcwZTjdvQxBsNgKT6m23BVoU7HdQu2HaPRAXuykhpX8CmsqisTmPatPLx2RlGGKey46/80bUOAAAAA==";
            //var sourceEncryptStr = "AEIyM0VEMkY2QUM2NjgzRjVFNUNGRjc2Njg5ODM1NzkxMjExRDY2NUX2AAAAH4sIAAAAAAAAA22RQW6DMBBF7+J1F0BwGrKLcVBSKa1UcoER/CRIxI5ss6BV714rLWDSLv3m6X/N+JMddI322N/wSlewtera9mmChX+Gg1J3psJ9PKCtqkx/c6gFWSzT0plGnef+f0z0DvZvRkj3tuyqCtaDE7UW3jNGm4MndMZkvZOq9fU3Y3SDmrL58DqPx6KARiPMtXJQD7MgZUf2kvvN2ZoVMl4snxOeJXEsN3yTp6uUi0wu0m1WRCueJCLygmDzxilhtnZY/KhI/Dg7UA2zVyd9P/6L1Wp+1Td3gZHkaAC5ATl4gmMzfODXN2oxHazwAQAATwAAAAAAAABENENGODE2MDAzNkQ1OEVDOTI0RTJDRTkxQjdEQTIxNkRCQ0VGRDg3H4sIAAAAAAAAAwE4AMf/W2PA68hItcwZTjdvQxBsNgKT6m23BVoU7HdQu2HaPRAXuykhpX8CmsqisTmPatPLjBk3LILmtGdgbAXFOAAAAA==";
            var encryptModel = SecurityHelper.EncryptModelToBase64String(model, ifRandom: false);
            if (!encryptModel.IfIsNull())
            {
                var encryptStr = encryptModel.EncryptedBase64String;
                var dencryptModel = SecurityHelper.DecryptModelFromBase64String<ScheduleFileInfoModel>(sourceEncryptStr).SourceModel;
                if (model.SourceFileFullPath == dencryptModel.SourceFileFullPath &&
                    model.TargetFileFullPath == dencryptModel.TargetFileFullPath)
                {
                    //success
                    var str = string.Empty;
                }
                if (sourceEncryptStr == encryptStr)
                {
                    dencryptModel = SecurityHelper.DecryptModelFromBase64String<ScheduleFileInfoModel>(sourceEncryptStr).SourceModel;
                    if (model.SourceFileFullPath == dencryptModel.SourceFileFullPath &&
                        model.TargetFileFullPath == dencryptModel.TargetFileFullPath)
                    {
                        //success
                        var str = string.Empty;
                    }

                }
            }



            //random
            //sourceEncryptStr = "ATE4NDQxOTU1MTkxNzAyMjAyODJGODVFMDE1QkUxMkQzNzZCMEE3RDc1QTA3NDNFQTRBQTI0Rjk1OAwBAAAfiwgAAAAAAAADbZHRbsIgFIZfZeF6GqCgljvb2ugSt2T1BUh71CYVDNCLuuzdx9y04HbJd778f87hA211A91uOMOrPAESqu+65xGW/hkOKt2bGq7jG1qp2gxnB00mLcxY5UyrDrH/H8sGB/ZvRkg3turrGqwHe9lZ8J4x2mw9kQcYrXepGn36zUDCmR6ilqq9eJuTe09A8R3mWjlQD7MgZS3tMfeLI4HKgiSzOeUpJaRY8mXOFoxnaZGwVVriBac0w17IUNw4JkRbh8WPSgE/zhpkA2aj9vp6+xerVXzUN3cEU0gnbyA3IB14Arv2+/8QxZRO8HxC0ifCBU8FYdOEYfT5BajIN3MEAgAATwAAAAAAAAA4NjkyMkRENjcwMjY5ODNFNjg0Rjc1QkJGQUQ0OTRENkE5QzU3MkFEH4sIAAAAAAAAAwE4AMf/W2PA68hItcwZTjdvQxBsNgKT6m23BVoU7HdQu2HaPRAXuykhpX8CmsqisTmPatPLx2RlGGKey46/80bUOAAAAA==";
            //netstandard2.1
            sourceEncryptStr = "ATM4NzU0ODM2MTkxNzAyMjAyRUIzOThGNEUzQjlGNzVBOUQ2QzdCNThCNkJENkEwNjY5QTI2QjQwQQ0BAAAfiwgAAAAAAAADbZHLbsIwEEV/pfK6VLYT57UjCRFUopUafmCUDBAp2Mh2FmnVf69LC4lplz5zdK9m/EG2qsV+N57xBU5IMjn0/eMEK/ecD2o16AYv4ytayUaPZ4ttDgajsLa6kwff/4/lo0XzN2NON6YemgaNA3voDTpPa6W3jsABJ+sNZKtOvxkks3pAr6Xu3p0t2K1nRukNFkpalHezWcoazLFwi5OMVCULopiLlDNWLsWyCJNQ5GkZhKu0oongPKdOyInfOCV4W8+L75USf5w1Qot6I/fqcvtno6R/1Fd7RF2ChSsoNIJFR3DXff8f4ZTzBY0XLH1gURYkWSieRBKTzy9WXMTnBAIAAE8AAAAAAAAAODY5MjJERDY3MDI2OTgzRTY4NEY3NUJCRkFENDk0RDZBOUM1NzJBRB+LCAAAAAAAAAMBOADH/1tjwOvISLXMGU43b0MQbDYCk+pttwVaFOx3ULth2j0QF7spIaV/AprKorE5j2rTy8dkZRhinsuOv/NG1DgAAAA=";

            encryptModel = SecurityHelper.EncryptModelToBase64String(model);
            if (!encryptModel.IfIsNull())
            {

                var encryptStr = encryptModel.EncryptedBase64String;

                var dencryptModel = SecurityHelper.DecryptModelFromBase64String<ScheduleFileInfoModel>(sourceEncryptStr).SourceModel;
                if (model.SourceFileFullPath == dencryptModel.SourceFileFullPath &&
                    model.TargetFileFullPath == dencryptModel.TargetFileFullPath)
                {
                    //success
                    var str = string.Empty;
                }

                if (sourceEncryptStr != encryptStr)
                {
                    dencryptModel = SecurityHelper.DecryptModelFromBase64String<ScheduleFileInfoModel>(sourceEncryptStr).SourceModel;
                    if (model.SourceFileFullPath == dencryptModel.SourceFileFullPath &&
                        model.TargetFileFullPath == dencryptModel.TargetFileFullPath)
                    {
                        //success
                        var str = string.Empty;
                    }
                }
            }



            //IsolatedStorageHelper.SaveModel(model);

            var isolatedStorageModel = IsolatedStorageHelper.GetModel<ScheduleFileInfoModel>();


        }
    }
}