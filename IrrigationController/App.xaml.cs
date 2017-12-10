using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace IrrigationController
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public Dictionary<string, bool> valveState = new Dictionary<string, bool>();
        public RelayBoard RelayBoard = new RelayBoard();
        public Schedule[] schedules = new Schedule[3];

        private DispatcherTimer timer;
        private TimeSpan schedCheck = new TimeSpan(0, 1, 0);        

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;

            for (int sched = 0; sched < 3; sched++)
            {
                schedules[sched] = new Schedule(false, sched);
            }

            timer = new DispatcherTimer();
            timer.Interval = schedCheck;
            timer.Start();
            timer.Tick += Timer_Tick;

            if (!RelayBoard.Available)
            {
                RelayBoard.Begin();
            }

            valveState.Add("Herbs", false);
            valveState.Add("Orchard", false);
            valveState.Add("TunnelHouse", false);
            valveState.Add("Garden", false);

            // TODO: build out service to run in background task to check Azure IoT Hub messaging
            var builder = new BackgroundTaskBuilder
            {
                Name = "Hub Messaging"
            };
            builder.SetTrigger(new TimeTrigger(15, false));
            // Do not set builder.TaskEntryPoint for in-process background tasks
            // Here we register the task and work will start based on the time trigger.
            BackgroundTaskRegistration task = builder.Register();

        }

        private void Timer_Tick(object sender, object e)
        {
            DateTime present = DateTime.Now;
            foreach(Schedule sched in schedules)
            {
                if (sched.IsSet)
                {
                    int timeResult = DateTime.Compare(present, sched.SchedStart);
                    if (timeResult >= 0)
                    {
                        List<string> tempTargets = new List<string>();
                        sched.SchedStart = sched.SchedStart.AddDays(sched.SchedInterval);
                        foreach (string target in sched.TargetAreas)
                        {
                            string where = target;
                            switch(where)
                            {
                                case "herbs":
                                    valveState["Herbs"] = RelayBoard.SwitchRelay(RelayBoard.HerbPin);
                                    tempTargets.Add(where);
                                    break;

                                case "orchard":
                                    valveState["Orchard"] = RelayBoard.SwitchRelay(RelayBoard.OrchardPin);
                                    tempTargets.Add(where);
                                    break;

                                case "tunnelhouse":
                                    valveState["TunnelHouse"] = RelayBoard.SwitchRelay(RelayBoard.TunnelHousePin);
                                    tempTargets.Add(where);
                                    break;

                                case "garden":
                                    valveState["Garden"] = RelayBoard.SwitchRelay(RelayBoard.GardenPin);
                                    tempTargets.Add(where);
                                    break;

                                default:
                                    break;
                            }

                        }

                        DispatcherTimer watering = new DispatcherTimer();
                        watering.Interval = new TimeSpan(0, sched.WaterDuration, 0);

                        watering.Start();

                        watering.Tick += (s, ev) =>
                        {
                            watering.Stop();
                            // If the valve is closed - dont re-open it.  The timer.Tick should only close a valve.
                            // The valve may have been manually closed after the timer was started
                            foreach (string target in tempTargets)
                            {
                                string where = target;
                                switch (where)
                                {
                                    case "herbs":
                                        if (valveState["Herbs"])
                                        {
                                            valveState["Herbs"] = RelayBoard.SwitchRelay(RelayBoard.HerbPin);
                                        }
                                        break;

                                    case "orchard":
                                        if (valveState["Orchard"])
                                        {
                                            valveState["Orchard"] = RelayBoard.SwitchRelay(RelayBoard.OrchardPin);
                                        }
                                        break;

                                    case "tunnelhouse":
                                        if (valveState["TunnelHouse"])
                                        {
                                            valveState["TunnelHouse"] = RelayBoard.SwitchRelay(RelayBoard.TunnelHousePin);
                                        }
                                        break;

                                    case "garden":
                                        if (valveState["Garden"])
                                        {
                                            valveState["Garden"] = RelayBoard.SwitchRelay(RelayBoard.GardenPin);
                                        }
                                        break;

                                    default:
                                        break;

                                }
                            }
                        };
                    }
                }
            }
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            base.OnBackgroundActivated(args);
            IBackgroundTaskInstance taskInstance = args.TaskInstance;

            if(taskInstance.Task.Name == "Hub Messaging")
            {
                
            }
            //DoYourBackgroundWork(taskInstance);
        }

    }
}
