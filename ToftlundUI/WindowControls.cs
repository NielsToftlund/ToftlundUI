﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using Windows.Services.Store;

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


        Path? RestoreImage, MaximiseImage, PinOff, PinOn;
        Button? CloseWindowButton, RestoreButton, MinimizeButton, PinWindow;
        
        public override void OnApplyTemplate()
        {
            CloseWindowButton = GetTemplateChild("CloseWindowButton") as Button;
            CloseWindowButton!.Click += CloseWindow_Click;

            RestoreImage = GetTemplateChild("RestoreImage") as Path;
            MaximiseImage = GetTemplateChild("MaximiseImage") as Path;
            RestoreButton = GetTemplateChild("restoreButton") as Button;
            RestoreButton!.Loaded += RestoreButton_Loaded;
            RestoreButton!.Click += RestoreButton_Click;

            MinimizeButton = GetTemplateChild("MinimizeButton") as Button;
            MinimizeButton!.Click += MinimizeButton_Click;

            PinWindow = GetTemplateChild("PinWindow") as Button;
            PinOff = GetTemplateChild("PinOff") as Path;
            PinOn = GetTemplateChild("PinOn") as Path;
            PinWindow!.Loaded += PinWindow_Loaded;
            PinWindow!.Click += PinWindow_Click;
            
            
            

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
                this.MaximiseImage!.Visibility = Visibility.Collapsed;
                this.RestoreImage!.Visibility = Visibility.Visible;
            }
            else
            {
                this.MaximiseImage!.Visibility = Visibility.Visible;
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
