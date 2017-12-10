using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace IrrigationController
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ScheduleControl : Page
    {
        private App appObj;
        private int _irrigateDuration = 15;
        private TimeSpan irrigationStartTime;
        private double frequency = 1;
        private int targetSchedule = 0;
        private string whichSchedule;

        private SolidColorBrush blueBrush = new SolidColorBrush(Windows.UI.Colors.Blue);
        private SolidColorBrush grayBrush = new SolidColorBrush(Windows.UI.Colors.Gray);
        private SolidColorBrush yellowBrush = new SolidColorBrush(Windows.UI.Colors.Yellow);

        public int IrrigateDuration { get => _irrigateDuration; set => _irrigateDuration = value; }
        public TimeSpan IrrigationStartTime { get => irrigationStartTime; set => irrigationStartTime = value; }
        public double Frequency { get => frequency; set => frequency = value; }
        public string WhichSchedule { get => whichSchedule; set => whichSchedule = value; }

        public ScheduleControl()
        {
            this.InitializeComponent();
            appObj = App.Current as App;

            SetButtonColour();
            
        }

        private void SetButtonColour()
        {
            foreach (Schedule schedule in appObj.schedules)
            {
                int id = schedule.Id;
                bool isSet = schedule.IsSet;
                switch (id)
                {
                    case 0:
                        IrrigationSched1.Background = isSet ? blueBrush : grayBrush;
                        break;

                    case 1:
                        IrrigationSched2.Background = isSet ? blueBrush : grayBrush;
                        break;

                    case 2:
                        IrrigationSched3.Background = isSet ? blueBrush : grayBrush;
                        break;

                    default:
                        break;
                }
            }
            
        }

        private void loadCurrentSchedData()
        {
            if (appObj.schedules[targetSchedule].IsSet)
            {
                foreach (string target in appObj.schedules[targetSchedule].TargetAreas)
                {
                    switch (target)
                    {
                        case "herbs":
                            Herbs.IsChecked = true;
                            break;

                        case "orchard":
                            Orchard.IsChecked = true;
                            break;

                        case "tunnelhouse":
                            TunnelHouse.IsChecked = true;
                            break;

                        case "garden":
                            Garden.IsChecked = true;
                            break;

                        default:
                            break;
                    }
                }

                TimeSpan userStart = appObj.schedules[targetSchedule].SchedStart.TimeOfDay;
                timeStart.Time = userStart;

                durationSlider.Value = appObj.schedules[targetSchedule].WaterDuration;

                frequencySlider.Value = appObj.schedules[targetSchedule].SchedInterval;
            }
        }

        private void durationSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            string msg = String.Format("Current value: {0}", e.NewValue);
            //durationValue.Text = msg;
            _irrigateDuration = (int)e.NewValue;
        }

        private void scheduleSet(object sender, RoutedEventArgs e)
        {
            appObj.schedules[targetSchedule].IsSet = true;
            appObj.schedules[targetSchedule].WaterDuration = IrrigateDuration;
            appObj.schedules[targetSchedule].SchedInterval = Frequency;
            appObj.schedules[targetSchedule].setSchedStart(irrigationStartTime);

            if(Herbs.IsChecked == true)
            {
                appObj.schedules[targetSchedule].TargetAreas.Add("herbs");
            }

            if (Orchard.IsChecked == true)
            {
                appObj.schedules[targetSchedule].TargetAreas.Add("orchard");
            }

            if (TunnelHouse.IsChecked == true)
            {
                appObj.schedules[targetSchedule].TargetAreas.Add("tunnelhouse");
            }

            if (Garden.IsChecked == true)
            {
                appObj.schedules[targetSchedule].TargetAreas.Add("garden");
            }

            SetButtonColour();

        }

        private void setTime(object sender, TimePickerValueChangedEventArgs e)
        {
            irrigationStartTime = e.NewTime;
        }

        private void frequencySlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            frequency = e.NewValue;
        }

        private void scheduleSelect(object sender, RoutedEventArgs e)
        {
            whichSchedule = ((Button)sender).Name;
            switch (whichSchedule)
            {
                case "IrrigationSched1":
                    targetSchedule = 0;
                    IrrigationSched1.Background = yellowBrush;
                    loadCurrentSchedData();
                    break;

                case "IrrigationSched2":
                    targetSchedule = 1;
                    IrrigationSched2.Background = yellowBrush;
                    break;

                case "IrrigationSched3":
                    targetSchedule = 2;
                    IrrigationSched3.Background = yellowBrush;
                    break;
            }
        }

        private void scheduleCancel(object sender, RoutedEventArgs e)
        {
            appObj.schedules[targetSchedule].IsSet = false;
            SetButtonColour();
        }

        private void valveControlNavigate_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ValveControl));
        }

        private void homeNavigate_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }
    }
}
