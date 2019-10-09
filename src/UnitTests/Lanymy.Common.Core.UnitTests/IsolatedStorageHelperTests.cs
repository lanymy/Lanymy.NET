using System;
using Lanymy.Common.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.Common.Core.UnitTests
{

    [TestClass()]
    public class IsolatedStorageHelperTests
    {



        [TestMethod()]
        public void StringTest()
        {

            var token = nameof(AccountInfoModel) + "JsonToken";

            //var accountInfoModel = new AccountInfoModel
            //{

            //    UserName = "UserName-" + Guid.NewGuid().ToString("N").ToUpper(),
            //    Password = "Password-" + Guid.NewGuid().ToString("N").ToUpper(),

            //};

            //var json = SerializeHelper.SerializeToJson(accountInfoModel);

            var sourceJson = "{\"UserName\":\"UserName-48EA9672A2A14D849609B111A8DD7D70\",\"Password\":\"Password-704295189D69482EB2D447F1AD92D16A\"}";

            IsolatedStorageHelper.SaveString(sourceJson, token);

            var getJson = IsolatedStorageHelper.GetString(token);

            Assert.AreEqual(sourceJson, getJson);

        }

        [TestMethod()]
        public void ModelTest()
        {

            var json = "{\"UserName\":\"UserName-48EA9672A2A14D849609B111A8DD7D70\",\"Password\":\"Password-704295189D69482EB2D447F1AD92D16A\"}";

            var accountInfoModel = SerializeHelper.DeserializeFromJson<AccountInfoModel>(json);

            IsolatedStorageHelper.SaveModel(accountInfoModel);


            var getAccountInfoModel = IsolatedStorageHelper.GetModel<AccountInfoModel>();


            Assert.AreEqual(accountInfoModel.UserName, getAccountInfoModel.UserName);

        }



    }
}