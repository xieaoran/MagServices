#region

using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using MagService.Base;

#endregion

namespace MagService.Controllers
{
    public class QueryController : ApiController
    {
        public IEnumerable<IEnumerable<long>> Get(long id1, long id2)
        {
            return Graph.Start(id1, id2).ToArray();
        }
    }
}