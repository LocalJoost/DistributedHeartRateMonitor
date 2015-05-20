using System;
using System.Threading;
using System.Threading.Tasks;

namespace HeartRate.Models
{
  public class Blinker
  {
    private DateTime lastReceivedUpdate = DateTime.MinValue;
    private int heartRate;
    private Task task;

    private Blinker()
    {
    }

    public void Start()
    {
      if (task == null)
      {
        cancellationTokenSource = new CancellationTokenSource();
        task = new Task(() => ShowHeartRateBlinking(cancellationTokenSource.Token),
           cancellationTokenSource.Token);
        task.Start();
      }
    }

    public void Stop()
    {
      if (cancellationTokenSource != null)
      {
        cancellationTokenSource.Cancel();
        task = null;
      }
    }

    private CancellationTokenSource cancellationTokenSource;

    private async Task ShowHeartRateBlinking(CancellationToken cancellationToken)
    {
      while (!cancellationToken.IsCancellationRequested)
      {
        if (DateTime.Now - lastReceivedUpdate < TimeSpan.FromSeconds(5))
        {
          if(DoBlink!=null) DoBlink(this, GetRate());
          await Task.Delay(60000 / HeartRate, cancellationToken);
        }
        else
        {
          await Task.Delay(10000, cancellationToken);
        }
      }
    }

    public event EventHandler<BlinkRate> DoBlink;

    private BlinkRate GetRate()
    {
      if (HeartRate < 80) return BlinkRate.Low;
      return HeartRate < 130 ? BlinkRate.Medium : BlinkRate.High;
    }

    public int HeartRate
    {
      get { return heartRate; }
      set
      {
        if (value >= 0 && value <= 200)
        {
          lastReceivedUpdate = DateTime.Now;
          heartRate = value;
        }
      }
    }

    private static Blinker blinker;
    public static Blinker GetBlinker()
    {
      return blinker ?? (blinker = new Blinker());
    }
  }
}
