using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Heartrate.SignalRHub.Startup))]
namespace Heartrate.SignalRHub
{
  public class Startup
  {
    public void Configuration(IAppBuilder app)
    {
      var hubConfiguration = new HubConfiguration
      {
        EnableDetailedErrors = true,
        EnableJavaScriptProxies = false
      };
      app.MapSignalR(hubConfiguration);
    }
  }
}
