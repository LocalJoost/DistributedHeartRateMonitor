using HeartRate.Models;

namespace Heartrate.SignalRHub
{
  public interface IBlinkHub
  {
    void NotifyNewRate(PulseData pulse);
  }
}
