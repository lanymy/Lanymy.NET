///********************************************************************

//时间: 2015年11月05日, AM 09:47:42

//作者: lanyanmiyu@qq.com

//描述: 消息通知实体类

//其它:     

//********************************************************************/

//namespace Lanymy.General.Extension.Models
//{
//    /// <summary>
//    /// 消息通知实体类
//    /// </summary>
//    public class MessageNotificationModel
//    {


//        /// <summary>
//        /// 消息通知委托方法
//        /// </summary>
//        public CommonEventHandler.SendMessageNotificationEvent SendMessageNotificationEvent { get; set; }


//        /// <summary>
//        /// 消息发送者
//        /// </summary>
//        public object Sender { get; set; }

//        /// <summary>
//        /// 消息类型标记
//        /// </summary>
//        public object MessageToken { get; set; }


//        /// <summary>
//        /// 消息传送的数据
//        /// </summary>
//        public object MessageData { get; set; }



//        /// <summary>
//        /// 发送消息
//        /// </summary>
//        public void SendMessage()
//        {
//            if (SendMessageNotificationEvent != null)
//            {
//                SendMessageNotificationEvent(Sender, MessageToken, MessageData);
//            }
//        }


//    }
//}
