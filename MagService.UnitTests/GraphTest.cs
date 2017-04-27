#region

using MagService.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace MagService.UnitTests
{
    [TestClass]
    public class GraphTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            for (var i = 0; i < 100; i++)
            {
                var id1 = 57898110;
                var id2 = 2014261844;
                var result = Graph.Start(id1, id2);
            }
        }
    }
}