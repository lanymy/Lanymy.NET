using System;
using System.Windows.Input;
using Lanymy.Common.Abstractions.Models;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Helpers;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Lanymy.Common.AppTests.ViewModels
{

    public class AboutViewModel : BaseViewModel
    {

        public ICommand OpenWebCommand { get; }


        public AboutViewModel()
        {

            Title = "About";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://aka.ms/xamarin-quickstart"));

            IsolatedStorageTest();

        }

        private void IsolatedStorageTest()
        {

            var model = new ScheduleFileInfoModel
            {
                SourceFileFullPath = "a",
                TargetFileFullPath = "b",
            };



            //fix
            //var sourceEncryptStr = "AEIyM0VEMkY2QUM2NjgzRjVFNUNGRjc2Njg5ODM1NzkxMjExRDY2NUX2AAAAH4sIAAAAAAAAA22RQW6DMBBF7+J1F0BwGrKLcVBSKa1UcoER/CRIxI5ss6BV714rLWDSLv3m6X/N+JMddI322N/wSlewtera9mmChX+Gg1J3psJ9PKCtqkx/c6gFWSzT0plGnef+f0z0DvZvRkj3tuyqCtaDE7UW3jNGm4MndMZkvZOq9fU3Y3SDmrL58DqPx6KARiPMtXJQD7MgZUf2kvvN2ZoVMl4snxOeJXEsN3yTp6uUi0wu0m1WRCueJCLygmDzxilhtnZY/KhI/Dg7UA2zVyd9P/6L1Wp+1Td3gZHkaAC5ATl4gmMzfODXN2oxHazwAQAATwAAAAAAAAA4NjkyMkRENjcwMjY5ODNFNjg0Rjc1QkJGQUQ0OTRENkE5QzU3MkFEH4sIAAAAAAAAAwE4AMf/W2PA68hItcwZTjdvQxBsNgKT6m23BVoU7HdQu2HaPRAXuykhpX8CmsqisTmPatPLx2RlGGKey46/80bUOAAAAA==";
            //net5
            var sourceEncryptStr = "ADE4MEU2RUY4MkYxQjQ0NTEzRkRFRDhGQTU3NzNEODhCMDFGMEE3N0P2AAAAH4sIAAAAAAAACm2RwY6CMBCG36VnD4DUFW+WSmQT3WTxBSYwKgm2ZloOaHx3G3fBoh77zZf/77RXttEVNrvujFs4IVuotmkmT5i5oz8odEslPsY9WqmSurPFSoDBWVxYqtVh7H9iorNo3jN8mpuiLUs0DuyhMeg8Ik0bR+AwXCk3v6AqffrPGFyvpqgvTufhUOTRYICpVhbVy8xLWYM5pm5ztmCZDKezr4gnURjKJV+m8TzmIpHTeJVkwZxHkQicINi48ZkwWtsvflUk/jlrhAopV3v9ePxvo9X4VX/sEUmChR6khGDREdzV/Qfe7moxHazwAQAAVgAAAAAAAABFRTdGQ0I2NDRFRTU0MThGRERDOTFCMjZCQjMxOUQzNDdBNEE0NDBDH4sIAAAAAAAACgA4AMf/W2PA68hItcwZTjdvQxBsNgKT6m23BVoU7HdQu2HaPRAXuykhpX8CmsqisTmPatPLx2RlGGKey44AAAD//wMAv/NG1DgAAAA=";

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
            sourceEncryptStr = "ATE4NDQxOTU1MTkxNzAyMjAyODJGODVFMDE1QkUxMkQzNzZCMEE3RDc1QTA3NDNFQTRBQTI0Rjk1OAwBAAAfiwgAAAAAAAADbZHRbsIgFIZfZeF6GqCgljvb2ugSt2T1BUh71CYVDNCLuuzdx9y04HbJd778f87hA211A91uOMOrPAESqu+65xGW/hkOKt2bGq7jG1qp2gxnB00mLcxY5UyrDrH/H8sGB/ZvRkg3turrGqwHe9lZ8J4x2mw9kQcYrXepGn36zUDCmR6ilqq9eJuTe09A8R3mWjlQD7MgZS3tMfeLI4HKgiSzOeUpJaRY8mXOFoxnaZGwVVriBac0w17IUNw4JkRbh8WPSgE/zhpkA2aj9vp6+xerVXzUN3cEU0gnbyA3IB14Arv2+/8QxZRO8HxC0ifCBU8FYdOEYfT5BajIN3MEAgAATwAAAAAAAAA4NjkyMkRENjcwMjY5ODNFNjg0Rjc1QkJGQUQ0OTRENkE5QzU3MkFEH4sIAAAAAAAAAwE4AMf/W2PA68hItcwZTjdvQxBsNgKT6m23BVoU7HdQu2HaPRAXuykhpX8CmsqisTmPatPLx2RlGGKey46/80bUOAAAAA==";

            encryptModel = SecurityHelper.EncryptModelToBase64String(model);
            if (!encryptModel.IfIsNull())
            {
                var encryptStr = encryptModel.EncryptedBase64String;
                if (sourceEncryptStr != encryptStr)
                {
                    var dencryptModel = SecurityHelper.DecryptModelFromBase64String<ScheduleFileInfoModel>(sourceEncryptStr).SourceModel;
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