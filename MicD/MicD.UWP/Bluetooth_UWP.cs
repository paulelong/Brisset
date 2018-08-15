using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Xamarin.Forms;
using MicD.UWP;

[assembly: Dependency(typeof(Bluetooth_UWP))]
namespace MicD.UWP
{
    public class Bluetooth_UWP : IBluetooth
    {
        private StreamSocket _socket;

        private RfcommDeviceService _service;

        public async void Connect(string deviceName)
        {
            try
            {
                var devices =
                await DeviceInformation.FindAllAsync(
                  RfcommDeviceService.GetDeviceSelector(
                    RfcommServiceId.SerialPort));

                foreach(var dev in devices)
                {
                    Debug.Print("{0} {1}\n", dev.Name, dev.Kind.ToString());
                }

                var device = devices.Single(x => x.Name == deviceName);

                _service = await RfcommDeviceService.FromIdAsync(
                                                        device.Id);

                _socket = new StreamSocket();

                await _socket.ConnectAsync(
                      _service.ConnectionHostName,
                      _service.ConnectionServiceName,
                      SocketProtectionLevel.
                      BluetoothEncryptionAllowNullAuthentication);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public async void Disconnect()
        {
            try
            {
                await _socket.CancelIOAsync();
                _socket.Dispose();
                _socket = null;
                _service.Dispose();
                _service = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public bool Recieve(string data)
        {
            throw new NotImplementedException();
        }

        public async Task<uint> Send(string msg)
        {
            try
            {
                var writer = new DataWriter(_socket.OutputStream);

                writer.WriteString(msg);

                // Launch an async task to 
                //complete the write operation
                var store = writer.StoreAsync().AsTask();

                return await store;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                return 0;
            }
        }
    }
}
