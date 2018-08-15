using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Diagnostics;
using Plugin.BluetoothLE;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

using nexus.protocols.ble;
using nexus.protocols.ble.scan;
using nexus.protocols.ble.scan.advertisement;

namespace MicD
{
	public partial class MainPage : ContentPage
	{
        // Set speed delay for monitoring changes.
        SensorSpeed speed = SensorSpeed.UI;
        public ICommand Select { get; }

        public ObservableCollection<IAdapter> Adapters { get; } = new ObservableCollection<IAdapter>();
        public ICommand Scan { get; }

        IBluetoothLowEnergyAdapter ble;

        Guid guid =  new Guid("B959E7E5-B260-4E6C-8B6D-C0DBEB2D0AE5");

        public MainPage(IBluetoothLowEnergyAdapter ble)
		{
			InitializeComponent();
            // Register for reading changes, be sure to unsubscribe when finished
            OrientationSensor.ReadingChanged += OrientationSensor_ReadingChanged;

            ToggleOrientationSensor();

            this.ble = ble;

            this.Select = ReactiveCommand.CreateFromTask<IAdapter>(async adapter =>
            {
                CrossBleAdapter.Current = adapter;
                //await App.Current.MainPage.Navigation.PushAsync(new AdapterPage());
            });
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

        private async void ScanClicked(object sender, EventArgs e)
        {
            if (ble.AdapterCanBeEnabled && ble.CurrentState.IsDisabledOrDisabling())
            {
                await ble.EnableAdapter();
            }

            //ble.CurrentState.Value; // e.g.: EnabledDisabledState.Enabled
                                    // The adapter implements IObservable<EnabledDisabledState> so you can subscribe to its state
            ble.CurrentState.Subscribe(state => Debug.WriteLine("New State: {0}", state));

            await ble.ScanForBroadcasts(
               // Optional scan filter to ensure that the
               // observer will only receive peripherals
               // that pass the filter. If you want to scan
               // for everything around, omit this argument.
               //new ScanFilter()
               //   .SetAdvertisedDeviceName("foobar")
               //   .SetAdvertisedManufacturerCompanyId(76)
               //   // Discovered peripherals must advertise at-least-one
               //   // of any GUIDs added by AddAdvertisedService()
               //   .AddAdvertisedService(guid)
               //   .SetIgnoreRepeatBroadcasts(false),
               null,
               // IObserver<IBlePeripheral> or Action<IBlePeripheral>
               // will be triggered for each discovered peripheral
               // that passes the above can filter (if provided).
               (IBlePeripheral peripheral) =>
               {
                   // read the advertising data...
                   var adv = peripheral.Advertisement;
                   Debug.WriteLine("name: " + adv.DeviceName);
                  // Debug.WriteLine(adv.Services.Select(x => x.ToString()).Join(","));
                   Debug.WriteLine("manu: " + adv.ManufacturerSpecificData.FirstOrDefault().CompanyName());
                   Debug.WriteLine("sd: " + adv.ServiceData);

       // ...or connect to the device (see next example)...
   },
               // TimeSpan or CancellationToken to stop the scan
               TimeSpan.FromSeconds(30)
            // If you omit this argument, it will use
            // BluetoothLowEnergyUtils.DefaultScanTimeout
            );


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
    }
}
