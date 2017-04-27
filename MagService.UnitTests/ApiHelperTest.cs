#region

using MagService.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace MagService.UnitTests
{
    [TestClass]
    public class ApiHelperTest
    {
        [TestMethod]
        public void GetTest()
        {
            var entity = MagHelper.GetEntityById(2147152072);
        }
    }
}