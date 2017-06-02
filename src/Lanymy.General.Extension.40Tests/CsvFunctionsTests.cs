using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Lanymy.General.Extension.CustomAttributes;
using Lanymy.General.Extension.ExtensionFunctions;
using Lanymy.General.Extension.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.General.Extension._40Tests
{


    [TestClass()]
    public class CsvFunctionsTests : BaseTestModel
    {


        /// <summary>
        /// CSV测试数据实体类
        /// </summary>
        public class CsvTestModel
        {
            //[CsvDescription(0, "CSV数据索引")]
            [CsvDescription(0)]
            public int Index { get; set; }
            //[CsvDescription(1, "用户名")]
            [CsvDescription(1)]
            public string UserName { get; set; }
            //[CsvDescription(2, "创建时间")]
            [CsvDescription(2)]
            public DateTime CreateDateTime { get; set; }
            //[CsvDescription(3, "更新时间")]
            [CsvDescription(3)]
            public DateTime LastUpdateDateTime { get; set; }
        }

        /// <summary>
        /// CSV测试根目录名称
        /// </summary>
        private readonly string _CsvTestRootDirectoryName;

        /// <summary>
        /// CSV文件全名称
        /// </summary>
        private const string CSV_TEST_FILE_FULL_NAME = "CsvTestFile.csv";

        /// <summary>
        /// CSV文件全路径
        /// </summary>
        private readonly string _CsvFileFullPath;

        /// <summary>
        /// CSV文件根目录全路径
        /// </summary>
        private readonly string _CsvFileRootDirectoryFullPath;


        public CsvFunctionsTests() : this(CSV_TEST_FILE_FULL_NAME)
        {

        }

        public CsvFunctionsTests(string testFileFullName) : base(testFileFullName)
        {
            _CsvTestRootDirectoryName = _TestRootDirectoryName;
            _CsvFileFullPath = _TestFileFullPath;
            _CsvFileRootDirectoryFullPath = _TestFileRootDirectoryFullPath;
        }




        public List<CsvTestModel> GetCsvListSource()
        {
            var list = new List<CsvTestModel>();
            int listCount = 1000;
            string formatStr = "{0}[ {1:" + new string('0', listCount.ToString().Length) + "} ]";
            for (int i = 0; i < listCount; i++)
            {
                list.Add(new CsvTestModel
                {
                    Index = i,
                    UserName = string.Format(formatStr, ReflectionFunctions.GetPropertyName<CsvTestModel>(o => o.UserName), i),
                    LastUpdateDateTime = DateTime.Now,
                    CreateDateTime = DateTime.Now,
                });
            }
            return list;
        }



        [TestInitialize]
        public void Init()
        {
            CsvSerializeMappings.StartMap<CsvTestModel>()
                .MapCSVSerializeProperty(o => o.Index, o => (o.Index + 5).ToString(), o => o.ConvertToType<int>() + 10)
                .MapCSVSerializeProperty(o => o.CreateDateTime, o => o.CreateDateTime.ToString("yyyy-MM-dd"), o => o.ConvertToType<DateTime>())
                .MapCSVSerializeProperty(o => o.LastUpdateDateTime, o => o.CreateDateTime.ToString(GlobalSettings.DEFAULT_DATE_FORMAT_STRING), o => o.ConvertToDateTime(GlobalSettings.DEFAULT_DATE_FORMAT_STRING));
        }


        [TestMethod]
        public void SerializeToCSVTest()
        {
            var list = GetCsvListSource();

            //using (var writer = new FileReadWriteHelper(_CsvFileFullPath, true))
            //{
            //    writer.WriteLine(SerializeFunctions.GetCSVTitle<CsvTestModel>());
            //    foreach (var csvTestModel in list)
            //    {
            //        writer.WriteLine(SerializeFunctions.SerializeToCSV(csvTestModel));
            //    }
            //}

            SerializeFunctions.SerializeToCsvFile(_CsvFileFullPath, list, true);

        }


        [TestMethod]
        public void DeserializeFromCSVTest()
        {

            List<CsvTestModel> list = new List<CsvTestModel>();

            //foreach (var csvStr in CsvFunctions.ReadCsvFile(_CsvFileFullPath))
            //{
            //    list.Add(SerializeFunctions.DeserializeFromCSV<CsvTestModel>(csvStr));
            //}

            list = SerializeFunctions.DeserializeFromCsvFile<CsvTestModel>(_CsvFileFullPath);


            var sourceList = GetCsvListSource();

            CollectionAssert.AreEqual(list.Select(o => o.UserName).ToArray(), sourceList.Select(o => o.UserName).ToArray());

        }



    }


}
