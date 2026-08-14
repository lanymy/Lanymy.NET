using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Lanymy.Common.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.Common.AllTests
{
    [TestClass]
    public class VersionHelperTests
    {
        [TestMethod]
        public void GetAssemblyVersion_ShouldReturnAssemblyNameVersion()
        {
            var assembly = typeof(VersionHelperTests).Assembly;
            var version = VersionHelper.GetAssemblyVersion(assembly);

            Assert.AreEqual(assembly.GetName().Version, version);
        }

        [TestMethod]
        public void GetCallingAssemblyVersion_ShouldReturnTestAssemblyVersion()
        {
            var version = GetCallingAssemblyVersionFromTestAssembly();

            Assert.AreEqual(typeof(VersionHelperTests).Assembly.GetName().Version, version);
        }

        [TestMethod]
        public void GetFileVersion_ShouldReturnVersionInfo_WhenFileExists()
        {
            var assemblyFileFullPath = typeof(VersionHelperTests).Assembly.Location;
            var expectedVersionString = FileVersionInfo.GetVersionInfo(assemblyFileFullPath).FileVersion;

            Assert.AreEqual(expectedVersionString, VersionHelper.GetFileVersionString(assemblyFileFullPath));
            Assert.AreEqual(new Version(expectedVersionString), VersionHelper.GetFileVersion(assemblyFileFullPath));
        }

        [TestMethod]
        public void GetFileVersion_ShouldReturnNullAndEmpty_WhenFileDoesNotExist()
        {
            var missingFileFullPath = @"Z:\Lanymy\Missing\VersionHelperTests.dll";

            Assert.AreEqual(string.Empty, VersionHelper.GetFileVersionString(missingFileFullPath));
            Assert.IsNull(VersionHelper.GetFileVersion(missingFileFullPath));
        }

        [TestMethod]
        public void GetCallDomainAssemblyApis_ShouldResolveEntryAssemblyOrHelperAssembly()
        {
            var expectedAssembly = Assembly.GetEntryAssembly() ?? typeof(VersionHelper).Assembly;
            var expectedAssemblyVersion = expectedAssembly.GetName().Version;
            var expectedFileVersion = VersionHelper.GetFileVersion(expectedAssembly.Location);

            Assert.AreEqual(expectedAssemblyVersion, VersionHelper.GetCallDomainAssemblyVersion());
            Assert.AreEqual(expectedFileVersion, VersionHelper.GetCallDomainAssemblyFileVersion());
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Version GetCallingAssemblyVersionFromTestAssembly()
        {
            return VersionHelper.GetCallingAssemblyVersion();
        }
    }
}
