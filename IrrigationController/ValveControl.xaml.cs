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
        private App appObj;


        private SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        private SolidColorBrush greenBrush = new SolidColorBrush(Windows.UI.Colors.Green);

        private int _irrigateDuration;
    
        public int IrrigateDuration { get => _irrigateDuration; set => _irrigateDuration = value; }  

        public ValveControl()
        {
            this.InitializeComponent();
            appObj = App.Current as App;

            if(!appObj.RelayBoard.Available)
            {
                appObj.RelayBoard.Begin();
            }

            Herbs.Background = SetButtonColour("Herbs");
            Orchard.Background = SetButtonColour("Orchard");
            TunnelHouse.Background = SetButtonColour("TunnelHouse");
            Garden.Background = SetButtonColour("Garden");

        }

        private void HerbsButton_Click(object sender, RoutedEventArgs e)
        {
            appObj.valveState["Herbs"] = appObj.RelayBoard.SwitchRelay(appObj.RelayBoard.HerbPin);
            Herbs.Background = SetButtonColour("Herbs");

            makeTheSwitches("Herbs", herbTimer, appObj.RelayBoard.HerbPin, Herbs);

        }

        private void OrchardButton_Click(object sender, RoutedEventArgs e)
        {
            appObj.valveState["Orchard"] = appObj.RelayBoard.SwitchRelay(appObj.RelayBoard.OrchardPin);
            Orchard.Background = SetButtonColour("Orchard");

            makeTheSwitches("Orchard", orchardTimer, appObj.RelayBoard.OrchardPin, Orchard);

        }

        private void TunnelHouseButton_Click(object sender, RoutedEventArgs e)
        {
            appObj.valveState["TunnelHouse"] = appObj.RelayBoard.SwitchRelay(appObj.RelayBoard.TunnelHousePin);
            TunnelHouse.Background = SetButtonColour("TunnelHouse");

            makeTheSwitches("TunnelHouse", tunnelHouseTimer, appObj.RelayBoard.TunnelHousePin, TunnelHouse);

        }

        private void GardenButton_Click(object sender, RoutedEventArgs e)
        {
            appObj.valveState["Garden"] = appObj.RelayBoard.SwitchRelay(appObj.RelayBoard.GardenPin);
            Garden.Background = SetButtonColour("Garden");

            makeTheSwitches("Garden", gardenTimer, appObj.RelayBoard.GardenPin, Garden);

        }

        private void makeTheSwitches(string whichArea, DispatcherTimer timer, GpioPin pin, Button button)
        {
            // Check if the valve is open - if so, start timer to then close it.
            if (appObj.valveState[whichArea])
            {
                timer = new DispatcherTimer();
                timer.Interval = new TimeSpan(0, IrrigateDuration, 0);

                timer.Start();

                timer.Tick += (s, ev) =>
                {
                    timer.Stop();
                    // If the valve is closed - dont re-open it.  The timer.Tick should only close a valve.
                    // The valve may have been manually closed after the timer was started
                    if (appObj.valveState[whichArea])
                    {
                        appObj.valveState[whichArea] = appObj.RelayBoard.SwitchRelay(pin);
                        button.Background = SetButtonColour(whichArea);
                    }
                    
                };
            }
            else
            {
                // We just closed the valve, but there may be a timer running if we closed it before the duration finished.
                if (timer != null && timer.IsEnabled)
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
            SolidColorBrush returnValue = appObj.valveState[button] ? greenBrush : redBrush;
            return returnValue;
        }

        private void homeNavigate_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void scheduleNavigate_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ScheduleControl));
        }
    }
}
