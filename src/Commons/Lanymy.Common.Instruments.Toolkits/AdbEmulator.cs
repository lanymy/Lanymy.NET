using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;

namespace Lanymy.Common.Instruments
{

    /// <summary>
    /// Adb 操作类
    /// </summary>
    public class AdbEmulator
    {


        //static readonly String ADBPATH = "D:/Microvirt/MEmu/"; //adb 所在目录
        //static readonly String ROOTPATH = AppDomain.CurrentDomain.BaseDirectory;
        //static readonly String SAVEPATH = AppDomain.CurrentDomain.BaseDirectory + "temp";
        //static readonly String SNAPEPATH = SAVEPATH + @"\gsnap.png";
        //static readonly String TEMPLATE = AppDomain.CurrentDomain.BaseDirectory + "template/";

        //internal String devices { get; set; }

        //internal AdbEmulator()
        //{
        //}

        //internal AdbEmulator(String devices)
        //{
        //    this.devices = devices;
        //}
        ////获取的所有能连接使用的设备
        //internal List<String> GetDevices()
        //{
        //    List<String> lst = new List<string>();
        //    String result = Common.Execute(ADBPATH + "adb devices");
        //    String[] lines = Regex.Split(result, "\r\n");// result.Split('\n');
        //    foreach (String line in lines)
        //    {
        //        if (line.Contains("\tdevice"))
        //        {
        //            lst.Add(line.Replace("\tdevice", ""));
        //        }
        //    }
        //    return lst;
        //}
        ////获取当前运行的APP 最顶层的Activity
        //internal string GetTopActivity()
        //{
        //    try
        //    {
        //        MatchCollection matchs;
        //        IEnumerator enumerator;
        //        String input = "";
        //        input = Common.Execute(ADBPATH + "adb -s " + devices + " shell dumpsys activity|findstr mFocusedActivity"); //获取最顶层的 activity
        //        if (input.IndexOf("HistoryRecord") > 0)
        //        {
        //            matchs = Regex.Matches(input, @"HistoryRecord.*?\}", RegexOptions.Singleline);
        //        }
        //        else
        //        {
        //            matchs = Regex.Matches(input, @"ActivityRecord.*?\}", RegexOptions.Singleline);
        //        }
        //        int num = 0;
        //        try
        //        {
        //            enumerator = matchs.GetEnumerator();
        //            while (enumerator.MoveNext())
        //            {
        //                Match current = (Match)enumerator.Current;
        //                num++;
        //                return current.Value;
        //            }
        //        }
        //        catch { }
        //        return "";
        //    }
        //    catch
        //    {
        //    }
        //    return "";
        //}

        ////截屏
        //internal void snape()
        //{
        //    Common.Execute(ADBPATH + "adb -s " + devices + " exec-out screencap -p > " + SNAPEPATH);
        //}
        ////载屏并保存为指定名称
        //internal void snape(String name)
        //{
        //    String path = SAVEPATH + "/" + name + ".bmp";
        //    Common.Execute(ADBPATH + "adb -s " + devices + " exec-out screencap -p > " + path);
        //}

        ////判断是否安装了某个包(APP)
        //internal Boolean CheckApp(String package)
        //{
        //    String Result = Common.Execute(ADBPATH + "adb -s " + devices + " shell pm list packages");
        //    return Result.IndexOf(package) > 0;
        //}

        ////安装APP
        //internal Boolean install(String package)
        //{
        //    String Result = Common.Execute(ADBPATH + "adb -s " + devices + " install " + package);
        //    return Result.IndexOf("Success", StringComparison.CurrentCultureIgnoreCase) > 0;
        //}

        ////卸载某个应用  //包名 可通过 adb shell pm list packages -s 来查看
        //internal void uninstall(String package)
        //{
        //    Common.Execute(ADBPATH + "adb -s " + devices + " uninstall " + package);
        //}

        ////启用APP 
        //internal void start(String package)
        //{
        //    Common.Execute(ADBPATH + "adb -s " + devices + " shell am start -n " + package);
        //}
        ////结束运行中的APP
        //internal void stop(String package)
        //{
        //    Common.Execute(ADBPATH + "adb -s " + devices + " shell am force-stop " + package);
        //}

        ////单击某区域
        //internal void click(Point point)
        //{
        //    String cmd = ADBPATH + "adb -s " + devices + " shell input tap " + (point.X) + " " + point.Y;
        //    Common.Execute(cmd);
        //}
        ////长按某个按
        //internal void LongPress(String Key)
        //{
        //    Common.Execute(ADBPATH + "adb - s " + devices + " input keyevent--longpress " + Key);
        //}
        ////按住某点滑动
        //internal void swipe(Point first, Point second, int time)
        //{
        //    Common.Execute(ADBPATH + "adb - s " + devices + " input swipe " + first.X + " " + first.Y + " " + second.X + " " + second.Y + " " + time);
        //}

        ////输入文本信息
        //internal void text(String text)
        //{
        //    Common.Execute(ADBPATH + "adb -s " + devices + " shell input text " + text);
        //}
        ////发送键值
        //internal void key(String key)
        //{
        //    Common.Execute(ADBPATH + "adb -s " + devices + " shell input keyevent " + key);
        //}

        ////检测某应用是否运行
        //internal bool isrun(String packname)
        //{
        //    String Result = Common.Execute(ADBPATH + "adb -s " + devices + " shell ps");
        //    return Result.IndexOf(packname, StringComparison.CurrentCultureIgnoreCase) > 0;
        //}
        ////滑动解锁
        //internal void unlock()
        //{
        //    StringBuilder cmd = new StringBuilder();
        //    cmd.AppendLine(ADBPATH + "adb -s " + devices + " shell sendevent /dev/input/event0 3 0 32");
        //    cmd.AppendLine(ADBPATH + "adb -s " + devices + " shell sendevent /dev/input/event0 3 1 353");
        //    cmd.AppendLine(ADBPATH + "adb -s " + devices + " shell sendevent /dev/input/event0 1 330 1");
        //    cmd.AppendLine(ADBPATH + "adb -s " + devices + " shell sendevent /dev/input/event0 0 0 0");
        //    cmd.AppendLine(ADBPATH + "adb -s " + devices + " shell sendevent /dev/input/event0 3 0 260");
        //    cmd.AppendLine(ADBPATH + "adb -s " + devices + " shell sendevent /dev/input/event0 0 0 0");
        //    cmd.AppendLine(ADBPATH + "adb -s " + devices + " shell sendevent /dev/input/event0 1 330 0");
        //    cmd.AppendLine(ADBPATH + "adb -s " + devices + " shell sendevent /dev/input/event0 0 0 0");
        //    Common.Execute(cmd.ToString());
        //}


    }

}
