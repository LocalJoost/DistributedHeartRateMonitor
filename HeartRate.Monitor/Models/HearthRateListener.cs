using System;
using System.Threading.Tasks;
using HeartRate.Models;
using Microsoft.AspNet.SignalR.Client;

namespace HeartRate.Monitor.Models
{
  public class HearthRateListener
  {
    public async Task Init()
    {
      var hubConnection = new HubConnection(Settings.HubUrl);
      var hubProxy = hubConnection.CreateHubProxy(Settings.HubName);
      hubProxy.On<PulseData>(Settings.ClientMethod, pulse =>
        {
          PulseDataReceived?.Invoke(this, pulse);
        }
      );
      await hubConnection.Start();
    }

    public event EventHandler<PulseData> PulseDataReceived;
  }
}
