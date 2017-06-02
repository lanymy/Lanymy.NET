using System;
using System.IO;
using System.Linq;
using System.Xml.XPath;
using Lanymy.General.Extension.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.BouncyCastle.Asn1.Pkcs;

namespace Lanymy.General.Extension._40Tests
{

    [TestClass]
    public class ConfigFunctionsTests : BaseTestModel
    {

        private const string CONFIG_FILE_FULL_NAME = "ConfigFunctionsTestsFile.config";

        public ConfigFunctionsTests() : this(CONFIG_FILE_FULL_NAME)
        {

        }

        public ConfigFunctionsTests(string testFileFullName) : base(testFileFullName)
        {

        }

        private void CreateConfigFile()
        {
            if (!File.Exists(_TestFileFullPath))
            {
                ScheduleFileInfoModel scheduleFileInfoModel = new ScheduleFileInfoModel
                {
                    SourceFileFullPath = Path.Combine(PathFunctions.GetCallDomainPath(), _TestFileFullName),
                    TargetFileFullPath = _TestFileFullPath,
                };
                FileFunctions.CopyFile(scheduleFileInfoModel, false);
            }
        }


        [TestInitialize]
        public void Init()
        {
            CreateConfigFile();
        }

        [TestMethod]
        public void XmlConfigerTest()
        {
            var xmlConfiger = new XmlConfiger();
            var xmlDocument = xmlConfiger.GetXmlDocument(_TestFileFullPath);
            var xmlElements = xmlConfiger.GetXmlElements(xmlDocument, "configuration", "appSettings");
            var xmlElement = xmlConfiger.GetXmlElement(xmlElements, "add", "key", "Key02");
            var xmlAttributeValue = xmlConfiger.GetXmlAttributeValue(xmlElement, "value");
            var xmlElementValue = xmlConfiger.GetXmlElementValue(xmlConfiger.GetXmlElements(xmlDocument, "configuration", "TestXmlElementValue").FirstOrDefault());
            xmlElementValue = xmlConfiger.GetXmlElementValue(xmlElements.FirstOrDefault());

        }

        [TestMethod]
        public void ReadConfigAppSettingsTest()
        {
            var xmlConfiger = new XmlConfiger();
            var xmlDocument = xmlConfiger.GetXmlDocument(_TestFileFullPath);
            var appSettingsXmlElement = xmlConfiger.GetAppSettingsXmlElement(xmlDocument);
            var appSettingsValue = xmlConfiger.GetAppSettingsValueByKey(appSettingsXmlElement, "Key03");
            Assert.AreEqual(appSettingsValue, "Value03");
        }


    }


}
