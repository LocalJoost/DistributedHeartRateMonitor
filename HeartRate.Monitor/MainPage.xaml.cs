using System.Threading.Tasks;
using HeartRate.Models;
using HeartRate.Monitor.Models;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HeartRate.Monitor
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class MainPage : Page
  {
    public MainPage()
    {
      this.InitializeComponent();
    }

    HearthRateListener listener;
    LedSoundOperator lsOperator;

    protected async override void OnNavigatedTo(NavigationEventArgs e)
    {
      listener = new HearthRateListener();
      lsOperator = LedSoundOperator.GetBlinker();
      var blinker = Blinker.GetBlinker();
      listener.PulseDataReceived += (p, q) =>
      {
        blinker.HeartRate = q.HeartRate;
      };
      blinker.DoBlink += Blinker_DoBlink;
      blinker.Start();
      await listener.Init();
    }

    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
      lsOperator.Cleanup();
      Blinker.GetBlinker().Stop();
    }

    private void Blinker_DoBlink(object sender, BlinkRate e)
    {
      lsOperator.Blink(e);
      Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
      {
        Color c;
        switch (e)
        {
          case BlinkRate.High:
            c = Colors.Red;
            break;
          case BlinkRate.Medium:
            c = Colors.Green;
            break;
          default:
            c = Colors.Blue;
            break;
        }
        BlinkCircle.Fill = new SolidColorBrush(c);
        await Task.Delay(200);
        BlinkCircle.Fill = new SolidColorBrush(Colors.Gray);
      });
    }
  }
}
