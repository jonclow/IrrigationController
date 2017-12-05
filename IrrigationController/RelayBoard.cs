using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace IrrigationController
{
    public sealed class RelayBoard
    {
        // Relay Control Pins
        private const int HERB_PIN = 5;
        private const int ORCHARD_PIN = 6;
        private const int TUNNEL_HOUSE_PIN = 13;
        private const int GARDEN_PIN = 19;

        // Control Flags
        private bool available;
        private bool enable;

        /// <summary>
        /// Relay switch control pins for the relevant areas solenoid valve
        /// </summary>
        /// <remarks>
        /// These object will be created in InitAsync(). The set method will
        /// be marked private, because the object itself will not change, only
        /// the value it drives to the pin.
        /// </remarks>
        public GpioPin HerbPin { get; private set; }
        public GpioPin OrchardPin { get; private set; }
        public GpioPin TunnelHousePin { get; private set; }
        public GpioPin GardenPin { get; private set; }
        public bool Available { get => available; set => available = value; }
        public bool Enable { get => enable; set => enable = value; }

        public void Begin()
        {
            // Default the control flags
            Available = false;
            Enable = false;
        /*
             * Acquire the GPIO controller
             * MSDN GPIO Reference: https://msdn.microsoft.com/en-us/library/windows/apps/windows.devices.gpio.aspx
             * 
             * Get the default GpioController
             */
        GpioController gpio = GpioController.GetDefault();

            /*
             * Test to see if the GPIO controller is available.
             *
             * If the GPIO controller is not available, this is
             * a good indicator the app has been deployed to a
             * computing environment that is not capable of
             * controlling the relay switch. Therefore we
             * will disable the functionality to
             * handle the failure case gracefully. This allows
             * the invoking application to remain deployable
             * across the Universal Windows Platform.
             */
            if (null == gpio)
            {
                Available = false;
                Enable = false;
                return;
            }

            /*
                 * Initialize the area solenoid drives
                 *
                 * Instantiate the relevant pin object
                 *
                 * Set the GPIO pin drive mode to output
                 */
            HerbPin = gpio.OpenPin(HERB_PIN, GpioSharingMode.Exclusive);
            //HerbPin.Write(GpioPinValue.Low);
            HerbPin.SetDriveMode(GpioPinDriveMode.Output);

            OrchardPin = gpio.OpenPin(ORCHARD_PIN, GpioSharingMode.Exclusive);
            //OrchardPin.Write(GpioPinValue.Low);
            OrchardPin.SetDriveMode(GpioPinDriveMode.Output);

            TunnelHousePin = gpio.OpenPin(TUNNEL_HOUSE_PIN, GpioSharingMode.Exclusive);
            //TunnelHousePin.Write(GpioPinValue.Low);
            TunnelHousePin.SetDriveMode(GpioPinDriveMode.Output);

            GardenPin = gpio.OpenPin(GARDEN_PIN, GpioSharingMode.Exclusive);
            //GardenPin.Write(GpioPinValue.Low);
            GardenPin.SetDriveMode(GpioPinDriveMode.Output);

            Available = true;
            Enable = true;

        }

        public bool SwitchRelay(GpioPin pinIdent)
        {
            if(Available)
            {
                try
                {
                    if(pinIdent.Read() == GpioPinValue.Low)
                    {
                        pinIdent.Write(GpioPinValue.High);
                        return false;
                        //return ("Relay is off!");
                    }
                    else
                    {
                        pinIdent.Write(GpioPinValue.Low);
                        return true;
                        //return ("Relay is on!");
                    }

                }
                catch (Exception ex)
                {
                    return false;
                    //return (ex.Message.ToString());
                }
            }

            return false;
            //return ("Relay NOT available - restart");
        }
    }
}
