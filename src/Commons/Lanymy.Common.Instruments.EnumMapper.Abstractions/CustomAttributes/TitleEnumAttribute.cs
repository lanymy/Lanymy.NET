namespace Lanymy.Common.Instruments.CustomAttributes
{
    /// <summary>
    /// 带 标题 和 描述 属性 的 自定义属性特性,用于扩展枚举项的描述
    /// </summary>
    public class TitleEnumAttribute : BaseEnumAttribute
    {


        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }


        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }



        /// <summary>
        /// 枚举自定义特性构造方法
        /// </summary>
        public TitleEnumAttribute()
        {

        }

        /// <summary>
        /// 枚举自定义特性构造方法
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="description">描述</param>
        public TitleEnumAttribute(string title, string description = null)
        {
            Title = title;
            Description = description;
        }


    }
}
