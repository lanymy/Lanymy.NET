
/********************************************************************

时间: 2015年01月19日, PM 04:47:19

作者: lanyanmiyu@qq.com

描述: 委托类

其它:     

********************************************************************/



using System;

namespace Lanymy.General.Extension.Models
{

    /// <summary>
    /// 委托类
    /// </summary>
    public class CommonEventHandler
    {


        ///// <summary>
        ///// 发送消息通知委托
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="token"></param>
        ///// <param name="args"></param>
        //public delegate void SendMessageNotificationEvent(object sender, object token, object args);



        /// <summary>
        /// 通用委托
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void CommonEvent<TArgs>(object sender, CommonEventArgs<TArgs> e);


        /// <summary>
        /// 通用委托参数类
        /// </summary>
        public class CommonEventArgs<TArgs> : EventArgs
        {

            /// <summary>
            /// 构造方法
            /// </summary>
            public CommonEventArgs()
            {
            }

            ///// <param name="sender">发送者</param>
            /// <summary>
            /// 构造方法
            /// </summary>
            /// <param name="args">参数数据</param>
            public CommonEventArgs(TArgs args)
            {
                Args = args;
            }

            ///// <summary>
            ///// 发送者
            ///// </summary>
            //public object Sender { get; set; }
            /// <summary>
            /// 参数数据
            /// </summary>
            public TArgs Args { get; set; }


        }



    }
}
