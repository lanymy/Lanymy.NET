using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Channels;
using System.Threading.Tasks;
using Lanymy.Common.Abstractions.Models;
using Lanymy.Common.Helpers;
using Lanymy.Common.Instruments;
using Lanymy.Common.Instruments.CryptoModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.Common.AllTests
{



    [TestClass()]
    public class WorkTaskTests
    {



        //[TestMethod()]
        //public async Task WorkTaskTest()
        //{

        //    var channel = Channel.CreateUnbounded<int>();

        //    for (var i = 0; i < 10; i++)
        //    {
        //        await channel.Writer.WriteAsync(i);
        //    }


        //    var reader = channel.Reader;

        //    if (reader.TryRead(out var number))
        //    {
        //        Debug.WriteLine(number);
        //    }

        //    var canCount = reader.CanCount;
        //    var count = reader.Count;

        //    var list = new List<int>();

        //    await foreach (var item in reader.ReadAllAsync())
        //    {
        //        list.Add(item);
        //        if (item == 8)
        //        {
        //            channel.Writer.Complete();
        //        }
        //    }

        //    canCount = reader.CanCount;
        //    count = reader.Count;

        //    if (reader.TryRead(out number))
        //    {
        //        Debug.WriteLine(number);
        //    }


        //    //while (await reader.WaitToReadAsync())
        //    //{
        //    //    if (reader.TryRead(out var number))
        //    //    {
        //    //        Debug.WriteLine(number);
        //    //    }
        //    //}

        //    var strEnd = string.Empty;

        //}



    }



}