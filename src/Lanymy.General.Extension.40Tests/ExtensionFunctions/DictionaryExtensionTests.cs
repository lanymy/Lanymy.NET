using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lanymy.General.Extension.ExtensionFunctions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.General.Extension._40Tests.ExtensionFunctions
{



    [TestClass()]
    public class DictionaryExtensionTests
    {


        private class DicComparer : IComparer
        {

            public int Compare(object x, object y)
            {

                if (x == null || y == null || !(x is KeyValuePair<int, string>) || !(y is KeyValuePair<int, string>))
                    return -1;

                var xItem = (KeyValuePair<int, string>)x;
                var yItem = (KeyValuePair<int, string>)y;

                return xItem.Key == yItem.Key && xItem.Value == yItem.Value ? 0 : -1;
            }
        }


        private Dictionary<int, string> _DicSource = GetSourceDic();


        private static Dictionary<int, string> GetSourceDic()
        {
            return new Dictionary<int, string>
            {
                {0,"a"},
                {1,"b"},
                {2,"c"},
                {3,"d"},
                {4,"e"},
            };
        }



        [TestMethod()]
        public void AddTestMethod()
        {

            KeyValuePair<int, string> newItem = new KeyValuePair<int, string>(5, "f");
            int sourceDicCount = _DicSource.Count;

            _DicSource.AddOrReplace(newItem.Key, newItem.Value);

            var lastItem = _DicSource.LastOrDefault();

            Assert.AreEqual(_DicSource.Count, sourceDicCount + 1);
            Assert.AreEqual(lastItem.Key, newItem.Key);
            Assert.AreEqual(lastItem.Value, newItem.Value);

        }

        [TestMethod()]
        public void ReplaceTestMethod()
        {

            KeyValuePair<int, string> newItem = new KeyValuePair<int, string>(1, "f");
            int sourceDicCount = _DicSource.Count;
            var sourceValue = _DicSource[newItem.Key];
            var sourceItem = _DicSource.Where(o => o.Key == newItem.Key).FirstOrDefault();

            Assert.AreEqual(sourceItem.Key, newItem.Key);
            Assert.AreEqual(sourceItem.Value, sourceValue);

            _DicSource.AddOrReplace(newItem.Key, newItem.Value);

            Assert.AreEqual(_DicSource.Count, sourceDicCount);

            var replaceItem = _DicSource.Where(o => o.Key == newItem.Key).FirstOrDefault();

            Assert.AreEqual(sourceItem.Key, replaceItem.Key);
            Assert.AreEqual(newItem.Key, replaceItem.Key);
            Assert.AreEqual(replaceItem.Value, newItem.Value);

        }

        [TestMethod()]
        public void GetValueTestMethod()
        {

            int itemKey = 1;
            var sourceVaule = _DicSource[itemKey];
            var haveValue = _DicSource.GetValue(itemKey);

            Assert.AreEqual(sourceVaule, haveValue);

            itemKey = 6;

            try
            {
                sourceVaule = _DicSource[itemKey];
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(KeyNotFoundException));
            }

            var noHaveValue = _DicSource.GetValue(itemKey);
            Assert.AreEqual(noHaveValue, null);

            string defaultValue = "NoValue";
            noHaveValue = _DicSource.GetValue(itemKey, defaultValue);
            Assert.AreEqual(noHaveValue, defaultValue);

        }


        [TestMethod()]
        public void AddRangeTestMethod()
        {

            Dictionary<int, string> dic1 = GetSourceDic();

            Dictionary<int, string> dic2 = new Dictionary<int, string>
            {
                {5,"f"},
                {6,"g"},
                {7,"h"},
            };

            Dictionary<int, string> dicTarget = new Dictionary<int, string>
            {
                {0,"a"},
                {1,"b"},
                {2,"c"},
                {3,"d"},
                {4,"e"},
                {5,"f"},
                {6,"g"},
                {7,"h"},
            };


            dic1.AddRange(dic2);
            CollectionAssert.AreEqual(dic1, dicTarget, new DicComparer());




            dic1 = GetSourceDic();

            dic2 = new Dictionary<int, string>
            {
                {1,"f"},
                {6,"g"},
                {7,"h"},
            };

            dicTarget = new Dictionary<int, string>
            {
                {0,"a"},
                {1,"f"},
                {2,"c"},
                {3,"d"},
                {4,"e"},
                {6,"g"},
                {7,"h"},
            };

            dic1.AddRange(dic2, true);
            CollectionAssert.AreEqual(dic1, dicTarget, new DicComparer());



            dic1 = GetSourceDic();

            dic2 = new Dictionary<int, string>
            {
                {1,"f"},
                {6,"g"},
                {7,"h"},
            };

            dicTarget = new Dictionary<int, string>
            {
                {0,"a"},
                {1,"b"},
                {2,"c"},
                {3,"d"},
                {4,"e"},
                {6,"g"},
                {7,"h"},
            };

            dic1.AddRange(dic2, false);
            CollectionAssert.AreEqual(dic1, dicTarget, new DicComparer());


        }

    }



}
