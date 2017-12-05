﻿using System;
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
using Windows.Devices.Gpio;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace IrrigationController
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            var appObj = App.Current as App;

            if (!appObj.RelayBoard.Available)
            {
                appObj.RelayBoard.Begin();
            }
        }

        private void ValveControlButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ValveControl));
        }

        private void ScheduleControlButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ScheduleControl));
        }
    }
}
