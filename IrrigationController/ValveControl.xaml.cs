using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Background;
using Windows.Devices.Gpio;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace IrrigationController
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ValveControl : Page
    {
        private DispatcherTimer irrigationTimer;
        private DispatcherTimer gardenTimer;
        private DispatcherTimer herbTimer;
        private DispatcherTimer tunnelHouseTimer;
        private DispatcherTimer orchardTimer;
        private TimeSpan duration;
        DateTimeOffset time;
        DateTimeOffset startTime;
        DateTimeOffset lastTime;


        private SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        private SolidColorBrush greenBrush = new SolidColorBrush(Windows.UI.Colors.Green);

        public Dictionary<string, bool> valveState = new Dictionary<string, bool>();

        private RelayBoard RelayBoard = new RelayBoard();

        private int _irrigateDuration;

        public int IrrigateDuration { get => _irrigateDuration; set => _irrigateDuration = value; }

        public ValveControl()
        {
            this.InitializeComponent();

            RelayBoard.Begin();
           
            valveState.Add("Herbs", false);
            valveState.Add("Orchard", false);
            valveState.Add("TunnelHouse", false);
            valveState.Add("Garden", false);
        }

        private void HerbsButton_Click(object sender, RoutedEventArgs e)
        {
            valveState["Herbs"] = RelayBoard.SwitchRelay(RelayBoard.HerbPin);
            Herbs.Background = SetButtonColour("Herbs");

            makeTheSwitches("Herbs", herbTimer, RelayBoard.HerbPin, Herbs);

            /*
            herbTimer = new DispatcherTimer();
            herbTimer.Interval = new TimeSpan(0, IrrigateDuration, 0);

            herbTimer.Start();

            herbTimer.Tick += (s, ev) =>
            {
                herbTimer.Stop();
                valveState["Herbs"] = RelayBoard.SwitchRelay(RelayBoard.HerbPin);
                Herbs.Background = SetButtonColour("Herbs");
            };
            */
        }

        private void OrchardButton_Click(object sender, RoutedEventArgs e)
        {
            valveState["Orchard"] = RelayBoard.SwitchRelay(RelayBoard.OrchardPin);
            Orchard.Background = SetButtonColour("Orchard");

            makeTheSwitches("Orchard", orchardTimer, RelayBoard.OrchardPin, Orchard);

            /*
            orchardTimer = new DispatcherTimer();
            orchardTimer.Interval = new TimeSpan(0, IrrigateDuration, 0);

            orchardTimer.Start();

            orchardTimer.Tick += (s, ev) =>
            {
                orchardTimer.Stop();
                valveState["Orchard"] = RelayBoard.SwitchRelay(RelayBoard.OrchardPin);
                Herbs.Background = SetButtonColour("Orchard");
            };
            */
        }

        private void TunnelHouseButton_Click(object sender, RoutedEventArgs e)
        {
            valveState["TunnelHouse"] = RelayBoard.SwitchRelay(RelayBoard.TunnelHousePin);
            TunnelHouse.Background = SetButtonColour("TunnelHouse");

            makeTheSwitches("TunnelHouse", tunnelHouseTimer, RelayBoard.TunnelHousePin, TunnelHouse);

            /*
            tunnelHouseTimer = new DispatcherTimer();
            tunnelHouseTimer.Interval = new TimeSpan(0, IrrigateDuration, 0);

            tunnelHouseTimer.Start();

            tunnelHouseTimer.Tick += (s, ev) =>
            {
                tunnelHouseTimer.Stop();
                valveState["TunnelHouse"] = RelayBoard.SwitchRelay(RelayBoard.TunnelHousePin);
                Herbs.Background = SetButtonColour("TunnelHouse");
            };
            */
        }

        private void GardenButton_Click(object sender, RoutedEventArgs e)
        {
            valveState["Garden"] = RelayBoard.SwitchRelay(RelayBoard.GardenPin);
            Garden.Background = SetButtonColour("Garden");

            makeTheSwitches("Garden", gardenTimer, RelayBoard.GardenPin, Garden);

            // Check if the valve is open - if so, start timer to then close it.
            /*
            if (valveState["Garden"])
            {
                gardenTimer = new DispatcherTimer();
                gardenTimer.Interval = new TimeSpan(0, IrrigateDuration, 0);

                gardenTimer.Start();

                gardenTimer.Tick += (s, ev) =>
                {
                    gardenTimer.Stop();
                    valveState["Garden"] = RelayBoard.SwitchRelay(RelayBoard.GardenPin);
                    Herbs.Background = SetButtonColour("Garden");
                };
            }
            else
            {
                // We just closed the valve, but there may be a timer running if we closed it before the duration finished.
                if (gardenTimer.IsEnabled)
                {
                    gardenTimer.Stop();
                }
            }
            */
            
        }

        private void makeTheSwitches(string whichArea, DispatcherTimer timer, GpioPin pin, Button button)
        {
            // Check if the valve is open - if so, start timer to then close it.
            if (valveState[whichArea])
            {
                timer = new DispatcherTimer();
                timer.Interval = new TimeSpan(0, IrrigateDuration, 0);

                timer.Start();

                timer.Tick += (s, ev) =>
                {
                    timer.Stop();
                    valveState[whichArea] = RelayBoard.SwitchRelay(pin);
                    button.Background = SetButtonColour(whichArea);
                };
            }
            else
            {
                // We just closed the valve, but there may be a timer running if we closed it before the duration finished.
                if (timer.IsEnabled)
                {
                    timer.Stop();
                }
            }
        }

        private void durationSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            //string msg = String.Format("Current value: {0}", e.NewValue);
            //durationValue.Text = msg;
            IrrigateDuration = (int) e.NewValue;
        }

        private SolidColorBrush SetButtonColour(string button)
        {
            if (valveState[button])
            {
                return greenBrush;
            }
            else
            {
                return redBrush;
            }
        }
    }
}
