#region

using System.Web.Http;

#endregion

namespace MagService
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务

            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute("MagApi", "mag/{controller}/{id}", new {id = RouteParameter.Optional}
                );
        }
    }
}