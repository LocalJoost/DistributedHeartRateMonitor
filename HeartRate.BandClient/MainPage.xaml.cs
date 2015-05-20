using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using HeartRate.BandClient.Models;
using HeartRate.Models;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace HeartRate.BandClient
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class MainPage : Page
  {
    public MainPage()
    {
      this.InitializeComponent();

      this.NavigationCacheMode = NavigationCacheMode.Required;
    }
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      Init();
    }

    HeartRateModel model;

    private async Task Init()
    {
      if (model == null)
      {
        model = new HeartRateModel();
        await model.Init();
        model.BandDataReceived += (p, q) => FlashCircle(BandDataCircle, Colors.Gold, 100);
        model.PulseDataReceived += (p, q) => FlashCircle(AzureDataCircle, Colors.Gold, 100);

        var blinker = Blinker.GetBlinker();
        model.PulseDataReceived += (p, q) =>
                                   {
                                     blinker.HeartRate = q.HeartRate;
                                   };
        blinker.DoBlink += BlinkerDoBlink;
        blinker.Start();
      }
    }

    private void BlinkerDoBlink(object sender, BlinkRate e)
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
        FlashCircle(HeartRateCirlce, c, 200);
    }

    private void FlashCircle(Ellipse ellipse, Color color, int delay)
    {
      Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
      {
        ellipse.Fill = new SolidColorBrush(color);
        await Task.Delay(delay);
        ellipse.Fill = new SolidColorBrush(Colors.Gray);
      });
    }

    private async void SendTestPulse(object sender, RoutedEventArgs e)
    {
      int rate;
      if (int.TryParse(TxtTestRate.Text, out rate))
      {
        var p = new PulseData { HeartRate = rate, Name = Settings.TestBandName };
        await model.SendPulse(p);
      }
    }

    private async void BandToggleToggled(object sender, RoutedEventArgs e)
    {
      BtnSendTestRate.IsEnabled = !BandToggle.IsOn;
      TxtTestRate.IsReadOnly = BandToggle.IsOn;
      if (BandToggle.IsOn)
      {
        var consent = await model.StartListening();
        if (!consent)
        {
          BandToggle.IsOn = false;
        }
      }
      else
      {
        await model.StopListening();
      }
    }
  }
}
