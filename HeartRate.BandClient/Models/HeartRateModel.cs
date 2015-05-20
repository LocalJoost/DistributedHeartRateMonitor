using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using HeartRate.Models;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;
using Microsoft.Band;
using Microsoft.Band.Sensors;

namespace HeartRate.BandClient.Models
{
  public class HeartRateModel
  {
    IBandClient bandClient;
    IHubProxy hubProxy;
    HubConnection hubConnection;
    string bandName;

    public async Task Init()
    {
      hubConnection = new HubConnection(Settings.HubUrl);
      hubProxy = hubConnection.CreateHubProxy(Settings.HubName);

      hubProxy.On<PulseData>(Settings.ClientMethod, pulse =>
      {
        if (PulseDataReceived != null)
        {
          PulseDataReceived(this, pulse);
        }
      });

      // LongPollingTransport: See http://www.4sln.com/Articles/creating-a-simple-application-using-universal-apps-with-signalr-and-mobile-servic
      await hubConnection.Start(new LongPollingTransport());
    }

    public async Task<bool> StartListening()
    {
      var pairedBands = await BandClientManager.Instance.GetBandsAsync();
      if (pairedBands.Any())
      {
        var band = pairedBands.FirstOrDefault();
        if (band != null)
        {
          bandName = band.Name;
          bandClient = await BandClientManager.Instance.ConnectAsync(band);
          var consent = await bandClient.SensorManager.HeartRate.RequestUserConsentAsync();
          if (consent)
          {
            var sensor = bandClient.SensorManager.HeartRate;
            sensor.ReadingChanged += SensorReadingChanged;
            await sensor.StartReadingsAsync();
          }
          return consent;
        }
      }
      return false;
    }

    public async Task StopListening()
    {
      if (bandClient != null)
      {
        var sensor = bandClient.SensorManager.HeartRate;
        sensor.ReadingChanged -= SensorReadingChanged;
        await sensor.StopReadingsAsync();
        bandClient.Dispose();
        bandClient = null;
      }
    }

    private async void SensorReadingChanged(object sender, 
      BandSensorReadingEventArgs<IBandHeartRateReading> e)
    {
      await SendPulse(
           new PulseData { HeartRate = e.SensorReading.HeartRate, Name = bandName});
      if (BandDataReceived != null)
      {
        BandDataReceived(this, e.SensorReading.HeartRate);
      }
    }

    public async Task SendPulse(PulseData p)
    {
      try
      {
        if (hubConnection.State == ConnectionState.Connected)
        {
          await hubProxy.Invoke(Settings.ServerMethod, p);
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex);
      }
    }

    public EventHandler<int> BandDataReceived;

    public EventHandler<PulseData> PulseDataReceived;
  }
}