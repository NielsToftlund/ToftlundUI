using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    ///     <MyNamespace:StatusBar/>
    ///
    /// </summary>
    public class StatusBar : ProgressBar
    {
        static StatusBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StatusBar), new FrameworkPropertyMetadata(typeof(StatusBar)));
        }

        Stopwatch stopwatch = new();
        List<double> Tider = new List<double>();

        ProgressBar? StatusProgressBar;
        RectangleGeometry? Blockbox;
        TextBlock? StatusTextBund, StatusText;

        public string TextOnStatusBar
        {
            get { return (string)GetValue(TextOnStatusBarProperty); }
            set { SetValue(TextOnStatusBarProperty, value); }
        }
        public static readonly DependencyProperty TextOnStatusBarProperty
            = DependencyProperty.Register(
                  "TextOnStatusBar",
                  typeof(string),
                  typeof(StatusBar),
                  new PropertyMetadata("Vent venligst..."));

        public Brush TextColorOnStatusBar
        {
            get { return (Brush)GetValue(TextColorOnStatusBarProperty); }
            set { SetValue(TextColorOnStatusBarProperty, value); }
        }
        public static readonly DependencyProperty TextColorOnStatusBarProperty
            = DependencyProperty.Register(
                "TextColorOnStatusBar",
                typeof(Brush),
                typeof(StatusBar),
                new PropertyMetadata(new SolidColorBrush(Colors.White)));

        public Brush TextColorOnStatusBarBackground
        {
            get { return (Brush)GetValue(TextColorOnStatusBarBackgroundProperty); }
            set { SetValue(TextColorOnStatusBarBackgroundProperty, value); }
        }
        public static readonly DependencyProperty TextColorOnStatusBarBackgroundProperty
            = DependencyProperty.Register(
                "TextColorOnStatusBarBackground",
                typeof(Brush),
                typeof(StatusBar),
                new PropertyMetadata(new SolidColorBrush(Colors.Black)));
        public bool ShowProgressNumber
        {
            get { return (bool)GetValue(ShowProgressNumberProperty); }
            set { SetValue(ShowProgressNumberProperty, value); }
        }
        public static readonly DependencyProperty ShowProgressNumberProperty
            = DependencyProperty.Register(
                "ShowProgressNumber",
                typeof(bool),
                typeof(StatusBar),
                new PropertyMetadata(false));

        public bool ShowTimeEstimate
        {
            get { return (bool)GetValue(ShowTimeEstimateProperty); }
            set { SetValue(ShowTimeEstimateProperty, value); }
        }
        public static readonly DependencyProperty ShowTimeEstimateProperty
            = DependencyProperty.Register(
                "ShowTimeEstimate",
                typeof(bool),
                typeof(StatusBar),
                new PropertyMetadata(false));


        public override void OnApplyTemplate()
        {
            StatusProgressBar = GetTemplateChild("StatusProgressBar") as ProgressBar;
            Blockbox = GetTemplateChild("Blockbox") as RectangleGeometry;
            StatusText = GetTemplateChild("StatusText") as TextBlock;
            StatusTextBund = GetTemplateChild("StatusTextBund") as TextBlock;
            // Events
            StatusProgressBar!.ValueChanged += StatusProgressBar_ValueChanged;
            StatusProgressBar!.Loaded += StatusBar_Loaded;
            StatusProgressBar!.SizeChanged += StatusBar_SizeChanged;

        }

        private void StatusBar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RefreshView();
        }

        private void StatusBar_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshView();
            stopwatch.Reset(); // Stop og nulstil
        }

        private void StatusProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RefreshView();
        }

        private void RefreshView()
        {
            StatusTextBund!.Text = TextOnStatusBar;
            if (ShowProgressNumber)
            {
                StatusTextBund!.Text = StatusTextBund!.Text + " " + StatusProgressBar!.Value + "/" + StatusProgressBar.Maximum;
            }
            if (ShowTimeEstimate)
            {
                if(StatusProgressBar!.Value == 0)
                {
                    stopwatch.Reset();
                }
                if (stopwatch.IsRunning)
                {
                    stopwatch.Stop();
                    Tider.Add(stopwatch.Elapsed.TotalSeconds);
                    stopwatch.Reset();
                    TimeSpan TidTilbage = TimeSpan.FromSeconds(Tider.Average() * (StatusProgressBar!.Maximum - StatusProgressBar!.Value));
                    if (TidTilbage.TotalSeconds > 3600)
                    {
                        StatusTextBund!.Text = StatusTextBund!.Text + " ≈ " + Math.Ceiling(TidTilbage.TotalHours) + " Timer";
                    }
                    else
                    {
                        if (TidTilbage.TotalSeconds > 60)
                        {
                            StatusTextBund!.Text = StatusTextBund!.Text + " ≈ " + Math.Ceiling(TidTilbage.TotalMinutes) + " minutter";
                        }
                        else
                        {
                            if (TidTilbage.TotalSeconds > 0)
                            {
                                StatusTextBund!.Text = StatusTextBund!.Text + " ≈ " + Math.Ceiling(TidTilbage.TotalSeconds) + " sekunder";
                            }
                        }
                    }
                }

                if(StatusProgressBar!.Value < StatusProgressBar.Maximum)
                {    
                    stopwatch.Start();
                }
            }

            StatusText!.Text = StatusTextBund.Text;

            double ProgressX = (Convert.ToDouble(StatusProgressBar!.RenderSize.ToString().Replace(",", ".")) / StatusProgressBar.Maximum) * StatusProgressBar.Value;
            Rect boxstørrelse = new()
            {
                Width = ProgressX,
                Height = 50,
                X = 0
            };
            Blockbox!.Rect = boxstørrelse;
        }
    }
}
