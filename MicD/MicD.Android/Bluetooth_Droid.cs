using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using MicD.Droid;

using Android.Bluetooth;

[assembly: Dependency(typeof(Bluetooth_Droid))]
namespace MicD.Droid
{
    public class Bluetooth_Droid : IBluetooth
    {
        private BluetoothSocket socket;

        public async void Connect(string deviceName)
        {
            BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;

            BluetoothDevice device = (from bd in adapter.BondedDevices where bd.Name == deviceName select bd).FirstOrDefault();

            if (device == null)
            {
                System.Diagnostics.Debug.Print("Error device not found. Please make sure you are paired with the BOLUTEK device.");
                return;
            }

            socket = device.CreateRfcommSocketToServiceRecord(Java.Util.UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));

            if (!socket.IsConnected)
                await socket.ConnectAsync();
        }

        public void Disconnect()
        {
            socket.Close();
        }

        public async Task<uint> Send(string msg)
        {
            byte[] m = charsToBytes(msg.ToCharArray());

            await socket.OutputStream.WriteAsync(m, 0, m.Length);

            return ((uint)m.Length);
        }

        public string Recieve()
        {
            return "";
        }

        public static string bytesToString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public static byte[] charsToBytes(char[] chars)
        {
            int length = chars.Length;
            byte[] returnVal = new byte[length];
            for (int x = 0; x < length; x++)
                returnVal[x] = (byte)chars[x];
            return returnVal;
        }
    }
}