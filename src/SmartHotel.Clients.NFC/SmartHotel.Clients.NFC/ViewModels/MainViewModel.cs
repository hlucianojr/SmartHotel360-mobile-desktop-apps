using Newtonsoft.Json;
using Plugin.Vibrate;
using SmartHotel.Clients.NFC.Models;
using SmartHotel.Clients.NFC.ViewModels.Base;
using System;
using System.Diagnostics;
using System.Windows.Input;
using Xamarin.Forms;
using Microsoft.Azure.Devices;
using System.Threading.Tasks;
using System.Text;

namespace SmartHotel.Clients.NFC.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private int _percentage;
        private string _title;
        private string _subTitle;
        private string _avatar;
        private static ServiceClient serviceClient;

        public MainViewModel()
        {
            Title = Resources.NfcMayus;
            SubTitle = Resources.GetPhone;
            Percentage = 0;

            MessagingCenter.Subscribe<string>(this, MessengerKeys.SendNFCToken, StartNFCService);
        }

        public int Percentage
        {
            get { return _percentage; }
            set
            {
                _percentage = value;
                OnPropertyChanged();
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        public string SubTitle
        {
            get { return _subTitle; }
            set
            {
                _subTitle = value;
                OnPropertyChanged();
            }
        }

        public string Avatar
        {
            get { return _avatar; }
            set
            {
                _avatar = value;
                OnPropertyChanged();
            }
        }

        public ICommand ResetCommand => new Command(Reset);

        private void StartNFCService(string message)
        {
            try
            {
                CrossVibrate.Current.Vibration();

                var nfcParameter = JsonConvert.DeserializeObject<NfcParameter>(message);
             
                if (nfcParameter != null)
                {
                    Title = Resources.HelloMayus;
                    SubTitle = nfcParameter.Username;
                    Avatar = nfcParameter.Avatar;
                    Percentage = 100;
                }

                serviceClient = ServiceClient.CreateFromConnectionString(MessengerKeys.iothubKey);
                SendCloudToDeviceMessageAsync().Wait();

            }
            catch (Exception ex)
            {
                Error();

                Debug.WriteLine($"[NFC] Error: {ex}");
            }
        }

        private async static Task SendCloudToDeviceMessageAsync()
        {
            string mockedJsonData =
                "{ \"Locked\":true}";
            var commandMessage = new Message(Encoding.ASCII.GetBytes(mockedJsonData));
            await serviceClient.SendAsync("AZ3166", commandMessage);
        }

        private void Reset()
        {
            Title = Resources.NfcMayus;
            SubTitle = Resources.GetPhone;
            Avatar = string.Empty;
            Percentage = 0;
        }

        private void Error()
        {
            Title = Resources.HelloMayus;
            SubTitle = "James Montemagno";
            Avatar = "james";
            Percentage = 100;
        }
    }
}