namespace Lanymy.Common.Interfaces.ICompressers
{
    /// <summary>
    /// 压缩器接口
    /// </summary>
    public interface ICompresser : ICompresserBytesAndBytes, ICompresserBytesAndBase64String, ICompresserStringAndBytes, ICompresserStringAndBase64String, ICompresserFileToFile
    {




    }
}
