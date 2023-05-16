using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using Windows.Services.Store;
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
            get { return (string)GetValue(VPNaddressProperty); }
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


        Path? RestoreImage, MaximizeImage, PinOff, PinOn, CloudOn, CloudOff, NoNet;
        Button? CloseWindowButton, RestoreButton, MinimizeButton, PinWindow;
        ContentControl? ConnectionIcon;
        Label? TitleBar, EmptySpace;

        public override void OnApplyTemplate()
        {
            CloseWindowButton = GetTemplateChild("CloseWindowButton") as Button;
            CloseWindowButton!.Click += CloseWindow_Click;

            RestoreButton = GetTemplateChild("restoreButton") as Button;
            MinimizeButton = GetTemplateChild("MinimizeButton") as Button;
            if (RemoveMaximizeRestore == false)
            {
                RestoreImage = GetTemplateChild("RestoreImage") as Path;
                MaximizeImage = GetTemplateChild("MaximizeImage") as Path;
                RestoreButton!.Loaded += RestoreButton_Loaded;
                RestoreButton!.Click += RestoreButton_Click;
                MinimizeButton!.Click += MinimizeButton_Click;
            }
            else
            {
                RestoreButton!.Visibility = Visibility.Collapsed;
                MinimizeButton!.Visibility = Visibility.Collapsed;
            }

            if(ToggleWindowOnTop == true)
            {
                PinWindow = GetTemplateChild("PinWindow") as Button;
                PinOff = GetTemplateChild("PinOff") as Path;
                PinOn = GetTemplateChild("PinOn") as Path;
                PinWindow!.Loaded += PinWindow_Loaded;
                PinWindow!.Click += PinWindow_Click;
            }
            else
            {
                PinWindow = GetTemplateChild("PinWindow") as Button;
                PinWindow!.Visibility = Visibility.Collapsed;
            }

            ConnectionIcon = GetTemplateChild("ConnectionIcon") as ContentControl;
            if (VPNaddress != "no-vpn")
            {
                CloudOn = GetTemplateChild("CloudOn") as Path;
                CloudOff = GetTemplateChild("CloudOff") as Path;
                NoNet = GetTemplateChild("NoNet") as Path;
                ConnectionIcon!.Loaded += ConnectionIcon_Loaded;
                TestIP(VPNaddress);
            }
            else
            {
                ConnectionIcon!.Visibility = Visibility.Collapsed;
            }

            TitleBar = GetTemplateChild("Title") as Label;
            TitleBar!.MouseDown += TitleBar_MouseDown;
            TitleBar!.MouseDoubleClick += TitleBar_MouseDoubleClick;

            EmptySpace = GetTemplateChild("EmptySpace") as Label;
            EmptySpace!.MouseDown += TitleBar_MouseDown;
            EmptySpace!.MouseDoubleClick += TitleBar_MouseDoubleClick;
            
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
                System.Timers.Timer timer = new(TimeSpan.FromMinutes(5).TotalMilliseconds)
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

        private Task TestIP(string VPNadresse)
        {
            if(VPNaddress == "no-vpn")
            {
                NoNet!.Visibility = Visibility.Collapsed;
                CloudOn!.Visibility = Visibility.Collapsed;
                CloudOff!.Visibility = Visibility.Collapsed;
            }
            else
            {
                string UdvendigIP = HentPublicIP().Result.Trim().ToString();
                Debug.WriteLine(UdvendigIP);
                if (UdvendigIP == "no-net")
                {
                    NoNet!.Visibility = Visibility.Visible;
                    CloudOn!.Visibility = Visibility.Collapsed;
                    CloudOff!.Visibility = Visibility.Collapsed;
                }
                else
                {
                    // int idx = VPNadresse.IndexOf(UdvendigIP);
                    if (VPNadresse == UdvendigIP)
                    {
                        NoNet!.Visibility = Visibility.Collapsed;
                        CloudOn!.Visibility = Visibility.Visible;
                        CloudOff!.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        NoNet!.Visibility = Visibility.Collapsed;
                        CloudOn!.Visibility = Visibility.Collapsed;
                        CloudOff!.Visibility = Visibility.Visible;
                    }
                }
            }
            return Task.CompletedTask;
        }

        private static async Task<string> HentPublicIP()
        {
            //using HttpClient client = new();
            //using HttpResponseMessage response = client.GetAsync("https://ipv4.icanhazip.com/").Result;
            //using HttpContent content = response.Content;
            //string result = content.ReadAsStringAsync().Result;
            string result = "no-net";

            //HttpClient client = new();
            //var responseBody = string.Empty;
            //using var client = new HttpClient();

            try
            {
                using HttpClient client = new();
                using HttpResponseMessage response = client.GetAsync("https://ipv4.icanhazip.com/").Result;
                using HttpContent content = response.Content;
                result = content.ReadAsStringAsync().Result;
            }
            catch
            {
                return "no-net";
            }

            return result;
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
                Debug.WriteLine(window.Topmost);
                if (window.Topmost == true)
                {
                    PinOn!.Visibility = Visibility.Visible;
                    PinOff!.Visibility = Visibility.Collapsed;
                }
                else
                {
                    PinOn!.Visibility = Visibility.Collapsed;
                    PinOff!.Visibility = Visibility.Visible;
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
            Window window = Window.GetWindow(this);
            if (window.WindowState == WindowState.Maximized)
            {
                this.MaximizeImage!.Visibility = Visibility.Collapsed;
                this.RestoreImage!.Visibility = Visibility.Visible;
            }
            else
            {
                this.MaximizeImage!.Visibility = Visibility.Visible;
                this.RestoreImage!.Visibility = Visibility.Collapsed;
            }
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);
            window?.Close();
        }
    }
}
