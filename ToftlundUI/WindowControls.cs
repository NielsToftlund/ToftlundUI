using System;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Path = System.Windows.Shapes.Path;

namespace ToftlundUI
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ToftlundUI"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ToftlundUI;assembly=ToftlundUI"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:WindowControls/>
    ///
    /// </summary>
    public class WindowControls : Control
    {
        static WindowControls()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowControls), new FrameworkPropertyMetadata(typeof(WindowControls)));
        }
        public Brush AlertForeground
        {
            get { return (Brush)GetValue(AlertForegroundProperty); }
            set { SetValue(AlertForegroundProperty, value); }
        }
        public static readonly DependencyProperty AlertForegroundProperty
            = DependencyProperty.Register(
                "AlertForeground",
                typeof(Brush),
                typeof(WindowControls),
                new PropertyMetadata(new SolidColorBrush(Colors.Red)));

        public Brush OnMouseOverForeground
        {
            get { return (Brush)GetValue(OnMouseOverForegroundProperty); }
            set { SetValue(OnMouseOverForegroundProperty, value); }
        }
        public static readonly DependencyProperty OnMouseOverForegroundProperty
            = DependencyProperty.Register(
                "OnMouseOverForeground",
                typeof(Brush),
                typeof(WindowControls),
                new PropertyMetadata(new SolidColorBrush(Colors.White)));

        public Brush OnMouseOverBackground
        {
            get { return (Brush)GetValue(OnMouseOverBackgroundProperty); }
            set { SetValue(OnMouseOverBackgroundProperty, value); }
        }
        public static readonly DependencyProperty OnMouseOverBackgroundProperty
            = DependencyProperty.Register(
                "OnMouseOverBackground",
                typeof(Brush),
                typeof(WindowControls),
                new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        public Brush OnMouseOverCloseBackground
        {
            get { return (Brush)GetValue(OnMouseOverCloseProperty); }
            set { SetValue(OnMouseOverCloseProperty, value); }
        }
        public static readonly DependencyProperty OnMouseOverCloseProperty
            = DependencyProperty.Register(
                "OnMouseOverClose",
                typeof(Brush),
                typeof(WindowControls),
                new PropertyMetadata(new SolidColorBrush(Colors.Red)));

        public bool ToggleWindowOnTop
        {
            get { return (bool)GetValue(ToggleWindowOnTopProperty); }
            set { SetValue(ToggleWindowOnTopProperty, value); }
        }
        public static readonly DependencyProperty ToggleWindowOnTopProperty
            = DependencyProperty.Register(
                "ToggleWindowOnTop",
                typeof(bool),
                typeof(WindowControls),
                new PropertyMetadata(false));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        public static readonly DependencyProperty TitleProperty
            = DependencyProperty.Register(
                "Title",
                typeof(string),
                typeof(WindowControls),
                new PropertyMetadata("Title on Window"));

        public string VPNaddress
        {
            get
            {
                return Application.Current.Dispatcher.Invoke(() => (string)GetValue(VPNaddressProperty));
            }
            set { SetValue(VPNaddressProperty, value); }
        }
        public static readonly DependencyProperty VPNaddressProperty
            = DependencyProperty.Register(
                "VPNaddress",
                typeof(string),
                typeof(WindowControls),
                new PropertyMetadata("no-vpn"));

        public bool RemoveMaximizeRestore
        {
            get { return (bool)GetValue(RemoveMaximizeRestoreProperty); }
            set { SetValue(RemoveMaximizeRestoreProperty, value); }
        }
        public static readonly DependencyProperty RemoveMaximizeRestoreProperty
            = DependencyProperty.Register(
                "RemoveMaximizeRestore",
                typeof(bool),
                typeof(WindowControls),
                new PropertyMetadata(false));

        public string Icon
        {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        public static readonly DependencyProperty IconProperty
            = DependencyProperty.Register(
                "Icon",
                typeof(string),
                typeof(WindowControls),
                new PropertyMetadata(string.Empty));

        public bool IconOnTaskbar
        {
            get { return (bool)GetValue(IconOnTaskbarProperty); }
            set { SetValue(IconOnTaskbarProperty, value); }
        }
        public static readonly DependencyProperty IconOnTaskbarProperty
            = DependencyProperty.Register(
                "IconOnTaskbar",
                typeof(bool),
                typeof(WindowControls),
                new PropertyMetadata(true));

        private Path? _restoreImage, _maximizeImage, _pinOff, _pinOn, _cloudOn, _cloudOff, _noNet;
        private Button? _closeWindowButton, _restoreButton, _minimizeButton, _pinWindow;
        private ContentControl? _connectionIcon;
        private Label? _titleBar, _emptySpace;
        private Image? _titleImage;

        private string _connectionStatusValue = "NoConnection";
        public string ConnectionStatus
        {
            get
            {
                TestIP(VPNaddress);
                return _connectionStatusValue;
            }
        }

        public override void OnApplyTemplate()
        {
            _titleImage = GetTemplateChild("TitleImage") as Image;
            if (IconOnTaskbar == false)
            {
                _titleImage!.Visibility = Visibility.Collapsed;
            }
            _closeWindowButton = GetTemplateChild("CloseWindowButton") as Button;
            _closeWindowButton!.Click += CloseWindow_Click;

            _restoreButton = GetTemplateChild("restoreButton") as Button;
            _minimizeButton = GetTemplateChild("MinimizeButton") as Button;
            if (RemoveMaximizeRestore == false)
            {
                _restoreImage = GetTemplateChild("RestoreImage") as Path;
                _maximizeImage = GetTemplateChild("MaximizeImage") as Path;
                _restoreButton!.Loaded += RestoreButton_Loaded;
                _restoreButton!.Click += RestoreButton_Click;
                _minimizeButton!.Click += MinimizeButton_Click;
            }
            else
            {
                _restoreButton!.Visibility = Visibility.Collapsed;
                _minimizeButton!.Visibility = Visibility.Collapsed;
            }

            if (ToggleWindowOnTop == true)
            {
                _pinWindow = GetTemplateChild("PinWindow") as Button;
                _pinOff = GetTemplateChild("PinOff") as Path;
                _pinOn = GetTemplateChild("PinOn") as Path;
                _pinWindow!.Loaded += PinWindow_Loaded;
                _pinWindow!.Click += PinWindow_Click;
            }
            else
            {
                _pinWindow = GetTemplateChild("PinWindow") as Button;
                _pinWindow!.Visibility = Visibility.Collapsed;
            }

            _connectionIcon = GetTemplateChild("ConnectionIcon") as ContentControl;
            if (VPNaddress != "no-vpn")
            {
                _cloudOn = GetTemplateChild("CloudOn") as Path;
                _cloudOff = GetTemplateChild("CloudOff") as Path;
                _noNet = GetTemplateChild("NoNet") as Path;
                _connectionIcon!.Loaded += ConnectionIcon_Loaded;
                TestIP(VPNaddress);
            }
            else
            {
                _connectionIcon!.Visibility = Visibility.Collapsed;
            }

            _titleBar = GetTemplateChild("Title") as Label;
            _titleBar!.MouseDown += TitleBar_MouseDown;
            _titleBar!.MouseDoubleClick += TitleBar_MouseDoubleClick;

            _titleImage = GetTemplateChild("TitleImage") as Image;
            _titleImage!.Loaded += TitleImage_Loaded;


            _emptySpace = GetTemplateChild("EmptySpace") as Label;
            _emptySpace!.MouseDown += TitleBar_MouseDown;
            _emptySpace!.MouseDoubleClick += TitleBar_MouseDoubleClick;

        }

        private void TitleImage_Loaded(object sender, RoutedEventArgs e)
        {

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (Icon != string.Empty)
                {
                    if (File.Exists(Icon))
                    {
                        _titleImage!.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath(Icon)));
                        Window window = Window.GetWindow(this);
                        if (IconOnTaskbar)
                        {
                            window.Icon = _titleImage!.Source;
                        }
                    }
                    else
                    {
                        Icon = string.Empty;
                        this._titleImage!.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    this._titleImage!.Visibility = Visibility.Collapsed;
                }
            }

        }

        private void TitleBar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            RestoreWindow();
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Input.MouseEventArgs me = e;
            if (me.LeftButton == MouseButtonState.Pressed)
            {
                Window window = Window.GetWindow(this);
                window.DragMove();
            }
        }

        private void ConnectionIcon_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                BackgroundWorker worker = new()
                {
                    WorkerReportsProgress = true
                };
                worker.DoWork += ErUdvendigIpOk_DoWork!;
                worker.RunWorkerAsync();

                RefreshMaximizeRestoreButton();
                // start VPN overvågning hvert x minut (afhængig af om VPN er nødvendig eller ej)
                System.Timers.Timer timer = new(TimeSpan.FromMinutes(1).TotalMilliseconds)
                {
                    AutoReset = true
                };
                timer.Elapsed += new ElapsedEventHandler(ErUdvendigIpOkTimer!);
                timer.Start();
            }
        }

        private void ErUdvendigIpOkTimer(object sender, ElapsedEventArgs e)
        {
            TestIP(VPNaddress);
        }

        void ErUdvendigIpOk_DoWork(object sender, DoWorkEventArgs e)
        {
            TestIP(VPNaddress);
        }

        private void TestIP(string VPNadresse)
        {
            string UdvendigIP = HentPublicIP().Result.Trim().ToString();
            if (VPNaddress == "no-vpn")
            {
                if (UdvendigIP == "no-net")
                {
                    _connectionStatusValue = "NoConnection";
                }
                else
                {
                    _connectionStatusValue = "Connected";
                }
                _noNet!.Visibility = Visibility.Collapsed;
                _cloudOn!.Visibility = Visibility.Collapsed;
                _cloudOff!.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (UdvendigIP == "no-net")
                {
                    SetConnectionIcon("NoNet");
                    _connectionStatusValue = "NoConnection";
                }
                else
                {
                    if (VPNadresse == UdvendigIP)
                    {
                        SetConnectionIcon("VPNon");
                        _connectionStatusValue = "VpnConnection";
                    }
                    else
                    {
                        SetConnectionIcon("VPNoff");
                        _connectionStatusValue = "LostVpnConnection";
                    }
                }
            }
        }

        private void SetConnectionIcon(string IconName)
        {
            switch (IconName)
            {
                case "VPNon":
                    if (Application.Current.Dispatcher.CheckAccess())
                    {
                        _noNet!.Visibility = Visibility.Collapsed;
                        _cloudOn!.Visibility = Visibility.Visible;
                        _cloudOff!.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            _noNet!.Visibility = Visibility.Collapsed;
                            _cloudOn!.Visibility = Visibility.Visible;
                            _cloudOff!.Visibility = Visibility.Collapsed;
                        });
                    }
                    break;

                case "VPNoff":
                    if (Application.Current.Dispatcher.CheckAccess())
                    {
                        _noNet!.Visibility = Visibility.Collapsed;
                        _cloudOn!.Visibility = Visibility.Collapsed;
                        _cloudOff!.Visibility = Visibility.Visible;
                        if (VPNaddress != "no-vpn")
                        {
                            _cloudOff!.Fill = AlertForeground; // new SolidColorBrush(System.Windows.Media.Colors.Red);
                        }
                        else
                        {
                            _cloudOff!.Fill = Foreground; // new SolidColorBrush(System.Windows.Media.Colors.Black);
                        }
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            _noNet!.Visibility = Visibility.Collapsed;
                            _cloudOn!.Visibility = Visibility.Collapsed;
                            _cloudOff!.Visibility = Visibility.Visible;
                            if (VPNaddress != "no-vpn")
                            {
                                _cloudOff!.Fill = AlertForeground; // new SolidColorBrush(System.Windows.Media.Colors.Red);
                            }
                            else
                            {
                                _cloudOff!.Fill = Foreground; // new SolidColorBrush(System.Windows.Media.Colors.Black);
                            }
                        });
                    }
                    break;

                default: // No connection
                    if (Application.Current.Dispatcher.CheckAccess())
                    {
                        _noNet!.Visibility = Visibility.Visible;
                        _cloudOn!.Visibility = Visibility.Collapsed;
                        _cloudOff!.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            _noNet!.Visibility = Visibility.Visible;
                            _cloudOn!.Visibility = Visibility.Collapsed;
                            _cloudOff!.Visibility = Visibility.Collapsed;
                        });
                    }
                    break;
            }
        }

        private static Task<string> HentPublicIP()
        {
            string result = "no-net";
            try
            {
                using HttpClient client = new();
                using HttpResponseMessage response = client.GetAsync("https://ipv4.icanhazip.com/").Result;
                using HttpContent content = response.Content;
                result = content.ReadAsStringAsync().Result;
            }
            catch
            {
                return Task.FromResult("no-net");
            }
            return Task.FromResult(result);
        }

        private void PinWindow_Loaded(object sender, RoutedEventArgs e)
        {
            PinWindowRefresh();
        }

        private void PinWindow_Click(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);
            if (window.Topmost == true)
            {
                Window.GetWindow(this).Topmost = false;
            }
            else
            {
                Window.GetWindow(this).Topmost = true;
            }
            PinWindowRefresh();
        }

        private void PinWindowRefresh()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Window window = Window.GetWindow(this);
                if (window.Topmost == true)
                {
                    _pinOn!.Visibility = Visibility.Visible;
                    _pinOff!.Visibility = Visibility.Collapsed;
                }
                else
                {
                    _pinOn!.Visibility = Visibility.Collapsed;
                    _pinOff!.Visibility = Visibility.Visible;
                }
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);
            window.WindowState = WindowState.Minimized;
        }

        private void RestoreButton_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                RefreshMaximizeRestoreButton();
            }
        }

        private void RestoreButton_Click(object sender, RoutedEventArgs e)
        {
            RestoreWindow();
        }

        private void RestoreWindow()
        {
            Window window = Window.GetWindow(this);
            if (window.WindowState == WindowState.Maximized)
            {
                window.WindowState = WindowState.Normal;
                RefreshMaximizeRestoreButton();
            }
            else
            {
                window.WindowState = WindowState.Maximized;
                RefreshMaximizeRestoreButton();
            }
        }

        private void RefreshMaximizeRestoreButton()
        {
            if (RemoveMaximizeRestore == false)
            {
                Window window = Window.GetWindow(this);
                if (window.WindowState == WindowState.Maximized)
                {
                    this._maximizeImage!.Visibility = Visibility.Collapsed;
                    this._restoreImage!.Visibility = Visibility.Visible;
                }
                else
                {
                    this._maximizeImage!.Visibility = Visibility.Visible;
                    this._restoreImage!.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);
            window?.Close();
        }
    }
}
