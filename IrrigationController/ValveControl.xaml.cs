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
    public sealed partial class ValveControl : Page
    {
        private int _irrigateDuration;

        public int IrrigateDuration { get => _irrigateDuration; set => _irrigateDuration = value; }

        public ValveControl()
        {
            this.InitializeComponent();
        }

        private void HerbsButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void OrchardButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TunnelHouseButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GardenButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void durationSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            string msg = String.Format("Current value: {0}", e.NewValue);
            this.durationValue.Text = msg;
            IrrigateDuration = (int) e.NewValue;
        }

        private void IrrigationStartButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void IrrigationStopButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
