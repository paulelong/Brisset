﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Diagnostics;

namespace MicD
{
	public partial class MainPage : ContentPage
	{
        // Set speed delay for monitoring changes.
        SensorSpeed speed = SensorSpeed.UI;

        public MainPage()
		{
			InitializeComponent();
            // Register for reading changes, be sure to unsubscribe when finished
            OrientationSensor.ReadingChanged += OrientationSensor_ReadingChanged;

            ToggleOrientationSensor();
        }

        void OrientationSensor_ReadingChanged(object sender, OrientationSensorChangedEventArgs e)
        {
            var data = e.Reading;
            //Debug.WriteLine($"Reading: X: {data.Orientation.X}, Y: {data.Orientation.Y}, Z: {data.Orientation.Z}, W: {data.Orientation.W}");
            // Process Orientation quaternion (X, Y, Z, and W)
        }

        public void ToggleOrientationSensor()
        {
            try
            {
                if (OrientationSensor.IsMonitoring)
                    OrientationSensor.Stop();
                else
                    OrientationSensor.Start(speed);
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature not supported on device
            }
            catch (Exception ex)
            {
                // Other error has occurred.
            }
        }

        private void ScanClicked(object sender, EventArgs e)
        {
        }

        private void ConnectClicked(object sender, EventArgs e)
        {
            DependencyService.Get<IBluetooth>().Connect("Dev B");
        }

        private void MotorAOn(object sender, EventArgs e)
        {
            DependencyService.Get<IBluetooth>().Send("1A");
        }

        private void MotorAOff(object sender, EventArgs e)
        {
            DependencyService.Get<IBluetooth>().Send("1z");
        }

        private void AllMotorOff(object sender, EventArgs e)
        {
            DependencyService.Get<IBluetooth>().Send("o");
        }

        private void DisconnectClicked(object sender, EventArgs e)
        {
            DependencyService.Get<IBluetooth>().Disconnect();
        }

        private void SliderValChanged(object sender, ValueChangedEventArgs e)
        {
            double val = e.NewValue * 0x4000;
            int p = (int)val;
            int p2 = p >> 7;
            p &= 0x7F;

            var c = Encoding.Default.GetString(new byte[] { (byte)p });
            var c2 = Encoding.Default.GetString(new byte[] { (byte)p2 });
            Debug.Print("slider val is {0}", val);
            string command = string.Format("2{0}{1}", c, c2);
            dbg.Text = val.ToString() + " c=" + command;
            DependencyService.Get<IBluetooth>().Send(command);
        }
    }
}
