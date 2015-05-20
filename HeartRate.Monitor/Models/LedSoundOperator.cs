using System.Threading.Tasks;
using HeartRate.Models;
using Windows.Devices.Gpio;

namespace HeartRate.Monitor.Models
{
  public class LedSoundOperator
  {
    GpioController gpioCtrl;

    private LedSoundOperator()
    {
      InitGPIO();
    }

    private static LedSoundOperator lsOperator;
    public static LedSoundOperator GetBlinker()
    {
      return lsOperator ?? (lsOperator = new LedSoundOperator());
    }

    private void InitGPIO()
    {
      if (Windows.Foundation.Metadata.ApiInformation‏.IsTypePresent("Windows.Devices.Gpio.GpioController"))
      {
        gpioCtrl = GpioController.GetDefault();
        if (gpioCtrl != null)
        {
          RedPin = InitPin(RedPinId);
          GreenPin = InitPin(GreenPidId);
          BluePin = InitPin(BluePinId);
          SoundPin = InitPin(SoundPinId);
        }
      }
    }

    public async Task Blink(BlinkRate rate)
    {
      if (gpioCtrl != null)
      {
        var pin = GetPin(rate);
        pin.Write(GpioPinValue.High);
        SoundPin.Write(GpioPinValue.High);
        await Task.Delay(200);
        pin.Write(GpioPinValue.Low);
        SoundPin.Write(GpioPinValue.Low);
      }
    }

    public void Cleanup()
    {
      RedPin?.Dispose();
      GreenPin?.Dispose();
      BluePin?.Dispose();
      SoundPin?.Dispose();
    }

    private GpioPin InitPin(int pinId)
    {
      var pin = gpioCtrl.OpenPin(pinId);
      pin.SetDriveMode(GpioPinDriveMode.Output);
      pin.Write(GpioPinValue.Low);
      return pin;
    }

    private GpioPin GetPin(BlinkRate rate)
    {
      switch (rate)
      {
        case BlinkRate.High:
          return RedPin;
        case BlinkRate.Medium:
          return GreenPin;
        default:
          return BluePin;
      }
    }

    private GpioPin RedPin;
    private GpioPin GreenPin;
    private GpioPin BluePin;
    private GpioPin SoundPin;

    private const int RedPinId = 5;
    private const int GreenPidId = 6;
    private const int BluePinId = 13;
    private const int SoundPinId = 16;

  }
}
