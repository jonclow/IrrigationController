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



        public int IrrigateDuration { get => _irrigateDuration; set => _irrigateDuration = value; }
        public TimeSpan IrrigationStartTime { get => irrigationStartTime; set => irrigationStartTime = value; }
        public double Frequency { get => frequency; set => frequency = value; }
        public string WhichSchedule { get => whichSchedule; set => whichSchedule = value; }

        public ScheduleControl()
        {
            this.InitializeComponent();
            appObj = App.Current as App;
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
                appObj.schedules[targetSchedule].targetAreas.Add("herbs");
            }

            if (Orchard.IsChecked == true)
            {
                appObj.schedules[targetSchedule].targetAreas.Add("orchard");
            }

            if (TunnelHouse.IsChecked == true)
            {
                appObj.schedules[targetSchedule].targetAreas.Add("tunnelhouse");
            }

            if (Garden.IsChecked == true)
            {
                appObj.schedules[targetSchedule].targetAreas.Add("garden");
            }
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
                    break;

                case "IrrigationSched2":
                    targetSchedule = 1;
                    break;

                case "IrrigationSched3":
                    targetSchedule = 2;
                    break;
            }
        }

        private void scheduleCancel(object sender, RoutedEventArgs e)
        {
            appObj.schedules[targetSchedule].IsSet = false;
        }
    }
}
