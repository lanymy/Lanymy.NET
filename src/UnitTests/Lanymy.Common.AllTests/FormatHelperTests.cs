using System.Xml;
using Lanymy.Common.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.Common.AllTests
{
    [TestClass]
    public class FormatHelperTests
    {
        [TestMethod]
        public void FormatXml_ShouldIndentXmlString()
        {
            var formattedXml = FormatHelper.FormatXml("<root><child>value</child></root>");

            StringAssert.Contains(formattedXml, "<root>");
            StringAssert.Contains(formattedXml, "<child>value</child>");
            StringAssert.Contains(formattedXml, "\r\n  <child>value</child>\r\n");
        }

        [TestMethod]
        public void FormatXml_ShouldReturnEmptyString_WhenInputIsNullOrEmpty()
        {
            XmlDocument xmlDocument = null;

            Assert.AreEqual(string.Empty, FormatHelper.FormatXml(string.Empty));
            Assert.AreEqual(string.Empty, FormatHelper.FormatXml(xmlDocument));
        }

        [TestMethod]
        public void Base64NameFormattingApis_ShouldRoundTripOriginalValue()
        {
            var rawBase64String = "folder/name+value==";

            var fileNameBase64String = FormatHelper.FormatBase64StringToFileNameBase64String(rawBase64String);
            var directoryNameBase64String = FormatHelper.FormatBase64StringToDirectoryNameBase64String(rawBase64String);

            Assert.AreEqual("folder@name+value==", fileNameBase64String);
            Assert.AreEqual(fileNameBase64String, directoryNameBase64String);
            Assert.AreEqual(rawBase64String, FormatHelper.FormatBase64StringFromFileNameBase64String(fileNameBase64String));
            Assert.AreEqual(rawBase64String, FormatHelper.FormatBase64StringFromDirectoryNameBase64String(directoryNameBase64String));
        }
    }
}
