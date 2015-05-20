using HeartRate.Models;
using Microsoft.AspNet.SignalR;

namespace Heartrate.SignalRHub
{
  public class BlinkHub : Hub<IBlinkHub>
  {
    public void PostNewRate(PulseData pulse)
    {
      Clients.All.NotifyNewRate(pulse);
    }
  }
}