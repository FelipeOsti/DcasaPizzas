using AppRomagnole.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppRomagnole.PIN
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PinView : INotifyPropertyChanged
	{
        private ImageSource _emptyCircle;
        private ImageSource _filledCircle;
        private Image imgEmpty;
        private Image imgFilled;
        private string _circleEmpty;
        private string _circleFilled;

        public string Title
        {
            get { return titleLabel.Text; }
            set
            {
                titleLabel.Text = value;
            }
        }

        private bool _bboCarregando;
        public bool bboCarregando
        {
            get { return _bboCarregando; }
            set
            {
                _bboCarregando = value;
                OnPropertyChanged("bboCarregando");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed(this, new PropertyChangedEventArgs(name));
        }


        PinViewModel viewModel;

        public PinView ()
		{
			InitializeComponent();

            BindingContext = this;

            //_emptyCircle = MainPage.urlIMG + "img_circle.png";
            //_filledCircle = MainPage.urlIMG + "img_circle_filled.png";

            _circleEmpty = MainPage.urlIMG + "img_circle.png";
            _circleFilled = MainPage.urlIMG + "img_circle_filled.png";

            viewModel = new PinViewModel()
            {
                TargetPinLength = 4
            };

            imgEmpty = new Image
            {
                Source = new UriImageSource { Uri = new Uri(_circleEmpty), CachingEnabled = true, CacheValidity = new TimeSpan(2, 0, 0, 0) },
                HeightRequest = 28,
                WidthRequest = 28,
                MinimumWidthRequest = 28,
                MinimumHeightRequest = 28
            };
            imgFilled = new Image
            {
                Source = new UriImageSource { Uri = new Uri(_circleFilled), CachingEnabled = true, CacheValidity = new TimeSpan(2, 0, 0, 0) },
                HeightRequest = 28,
                WidthRequest = 28,
                MinimumWidthRequest = 28,
                MinimumHeightRequest = 28,                
            };

            OnUpdateDisplayedText(viewModel, EventArgs.Empty);
        }

        private async void ClickButton(object sender, EventArgs e)
        {
            try
            {
                Button bt = sender as Button;
                char numero = bt.Text[0];

                viewModel.EnteredPin.Add(numero);

                if (viewModel.EnteredPin.Count == 4)
                {
                    try
                    {
                        OnUpdateDisplayedText(viewModel, e);

                        bboCarregando = true;
                        IList<char> pin = new List<char>();
                        pin = viewModel.EnteredPin;

                        if (!await LoginSegundoNivel.ValidaPIN(pin))
                        {
                            Handle_OnError(this, EventArgs.Empty);
                            viewModel.EnteredPin.Clear();
                            OnUpdateDisplayedText(viewModel, e);
                        }
                        else
                        {
                            viewModel.EnteredPin.Clear();
                            OnUpdateDisplayedText(viewModel, e);
                            App.Current.MainPage = new Menu.Menu();
                        }
                    }
                    finally
                    {
                        bboCarregando = false;
                    }
                }
                else
                {
                    OnUpdateDisplayedText(viewModel, e);
                }
            }
            catch { throw; }
            
        }

        private void OnUpdateDisplayedText(object sender, EventArgs e)
        {
            var vm = sender as PinViewModel;
            if (vm.EnteredPin != null && vm.TargetPinLength > 0)
            {
                if (circlesStackLayout.Children.Count == 0)
                {
                    for (int i = 0; i < vm.TargetPinLength; ++i)
                    {
                        imgEmpty = new Image
                        {
                            Source = new UriImageSource { Uri = new Uri(_circleEmpty), CachingEnabled = true, CacheValidity = new TimeSpan(2, 0, 0, 0) },
                            HeightRequest = 28,
                            WidthRequest = 28,
                            MinimumWidthRequest = 28,
                            MinimumHeightRequest = 28
                        };
                        circlesStackLayout.Children.Add(imgEmpty);
                    }
                }
                else
                {
                    for (int i = 0; i < vm.EnteredPin.Count; ++i)
                    {
                        imgFilled = new Image
                        {
                            Source = new UriImageSource { Uri = new Uri(_circleFilled), CachingEnabled = true, CacheValidity = new TimeSpan(2, 0, 0, 0) },
                            HeightRequest = 28,
                            WidthRequest = 28,
                            MinimumWidthRequest = 28,
                            MinimumHeightRequest = 28
                        };
                        circlesStackLayout.Children[i] = imgFilled;
                    }
                    for (int i = vm.EnteredPin.Count; i < vm.TargetPinLength; ++i)
                    {
                        imgEmpty = new Image
                        {
                            Source = new UriImageSource { Uri = new Uri(_circleEmpty), CachingEnabled = true, CacheValidity = new TimeSpan(2, 0, 0, 0) },
                            HeightRequest = 28,
                            WidthRequest = 28,
                            MinimumWidthRequest = 28,
                            MinimumHeightRequest = 28
                        };
                        circlesStackLayout.Children[i] = imgEmpty;
                    }
                }
            }
        }

        private void Handle_OnError(object sender, EventArgs e)
        {
            Content.AbortAnimation("shake");
            Content.Animate("shake",
                            (arg) =>
                                {
                                    var shift = Math.Sin(2 * 2 * Math.PI * arg);
                                    Content.TranslationX = 6 * shift;
                                },
                            16 * 4,
                            250,
                            Easing.Linear,
                                (arg1, arg2) =>
                                {
                                    Content.TranslationX = 0;
                                });
        }
    }

    class PinViewModel : INotifyPropertyChanged
    {
        public event EventHandler Success;
        public event EventHandler Error;
        public event EventHandler DisplayedTextUpdated;

        public event PropertyChangedEventHandler PropertyChanged;
        private string _passwordDisplayedText = string.Empty;
        public string PasswordDisplayedText
        {
            get { return _passwordDisplayedText; }
            private set
            {
                _passwordDisplayedText = value;
                RaisePropertyChanged(nameof(PasswordDisplayedText));
            }
        }

        private int _targetPinLength;
        public int TargetPinLength
        {
            get { return _targetPinLength; }
            set
            {
                _targetPinLength = value;
                RaisePropertyChanged(nameof(TargetPinLength));
                DisplayedTextUpdated?.Invoke(this, EventArgs.Empty);
            }
        }

        private Func<IList<char>, bool> _validatorFunc;
        public Func<IList<char>, bool> ValidatorFunc
        {
            get { return _validatorFunc; }
            set
            {
                _validatorFunc = value;
                RaisePropertyChanged(nameof(ValidatorFunc));
            }
        }

        private IList<char> _enteredPin = new List<char>();
        public IList<char> EnteredPin
        {
            get { return _enteredPin; }
            set
            {
                _enteredPin = value;
                RaisePropertyChanged(nameof(EnteredPin));
            }
        }

        public Command<string> KeyPressCommand { get; private set; }

        public PinViewModel()
        {
            KeyPressCommand = new Command<string>(arg =>
            {
                if (arg == "Backspace")
                {
                    if (EnteredPin.Count > 0)
                    {
                        EnteredPin.RemoveAt(EnteredPin.Count - 1);
                        DisplayedTextUpdated?.Invoke(this, EventArgs.Empty);
                    }
                }
                else if (EnteredPin.Count < TargetPinLength)
                {
                    EnteredPin.Add(arg[0]);
                    if (EnteredPin.Count == TargetPinLength)
                    {
                        if (ValidatorFunc.Invoke(EnteredPin))
                        {
                            EnteredPin.Clear();
                            Success?.Invoke(this, EventArgs.Empty);
                            DisplayedTextUpdated?.Invoke(this, EventArgs.Empty);
                        }
                        else
                        {
                            EnteredPin.Clear();
                            Error?.Invoke(this, EventArgs.Empty);
                            DisplayedTextUpdated?.Invoke(this, EventArgs.Empty);
                        }
                    }
                    else
                    {
                        DisplayedTextUpdated?.Invoke(this, EventArgs.Empty);
                    }
                }
            });
        }

        protected void RaisePropertyChanged(params string[] propertyNames)
        {
            if (PropertyChanged != null && propertyNames != null)
            {
                foreach (string propertyName in propertyNames)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }
    }

}