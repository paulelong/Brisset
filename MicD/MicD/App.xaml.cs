using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using nexus.protocols.ble;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace MicD
{
	public partial class App : Application
	{
		public App (IBluetoothLowEnergyAdapter ble)
		{
			InitializeComponent();

			MainPage = new MainPage(ble);
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
