#region

using System.IO;

#endregion

namespace MagService.Exceptions
{
    public static class Network
    {
        public static IOException LinkFailed = new IOException("网络连接已断开，请检查网络连接后重试当前操作。");
    }
}