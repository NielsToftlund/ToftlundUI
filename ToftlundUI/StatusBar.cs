using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
        public class ProgressDataObject
        {
            public double Værdi { get; set; }
            public double Maximum { get; set; }
            public string TekstPåProgressbaren { get; set; } = string.Empty;
            public bool VisVærdier { get; set; } = false;
            public bool VisTidsEstimat { get; set; } = false;

        }
        static StatusBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StatusBar), new FrameworkPropertyMetadata(typeof(StatusBar)));
        }

        readonly Stopwatch _stopwatch = new();
        readonly List<double> _tider = [];

        ProgressBar? _statusProgressBar;
        RectangleGeometry? _blockbox;
        TextBlock? _statusTextBund, _statusText;

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
            _statusProgressBar = GetTemplateChild("StatusProgressBar") as ProgressBar;
            _blockbox = GetTemplateChild("Blockbox") as RectangleGeometry;
            _statusText = GetTemplateChild("StatusText") as TextBlock;
            _statusTextBund = GetTemplateChild("StatusTextBund") as TextBlock;
            // Events
            _statusProgressBar!.ValueChanged += StatusProgressBar_ValueChanged;
            _statusProgressBar!.Loaded += StatusBar_Loaded;
            _statusProgressBar!.SizeChanged += StatusBar_SizeChanged;

        }

        private void StatusBar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RefreshView();
        }

        private void StatusBar_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshView();
            _stopwatch.Reset(); // Stop og nulstil
        }

        private void StatusProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RefreshView();
        }

        private void RefreshView()
        {
            _statusTextBund!.Text = TextOnStatusBar;
            if (ShowProgressNumber)
            {
                _statusTextBund!.Text = _statusTextBund!.Text + " " + _statusProgressBar!.Value + "/" + _statusProgressBar.Maximum;
            }
            if (ShowTimeEstimate)
            {
                if (_statusProgressBar!.Value == 0)
                {
                    _stopwatch.Reset();
                }
                if (_stopwatch.IsRunning)
                {
                    _stopwatch.Stop();
                    _tider.Add(_stopwatch.Elapsed.TotalSeconds);
                    _stopwatch.Reset();
                    TimeSpan TidTilbage = TimeSpan.FromSeconds(_tider.Average() * (_statusProgressBar!.Maximum - _statusProgressBar!.Value));
                    if (TidTilbage.TotalSeconds > 3600)
                    {
                        _statusTextBund!.Text = _statusTextBund!.Text + " ≈ " + Math.Ceiling(TidTilbage.TotalHours) + " Timer";
                    }
                    else
                    {
                        if (TidTilbage.TotalSeconds > 60)
                        {
                            _statusTextBund!.Text = _statusTextBund!.Text + " ≈ " + Math.Ceiling(TidTilbage.TotalMinutes) + " minutter";
                        }
                        else
                        {
                            if (TidTilbage.TotalSeconds > 0)
                            {
                                _statusTextBund!.Text = _statusTextBund!.Text + " ≈ " + Math.Ceiling(TidTilbage.TotalSeconds) + " sekunder";
                            }
                        }
                    }
                }

                if (_statusProgressBar!.Value < _statusProgressBar.Maximum)
                {
                    _stopwatch.Start();
                }
            }

            _statusText!.Text = _statusTextBund.Text;
            //string renderSize = StatusProgressBar!.RenderSize.ToString().Replace(",", ".");
            string renderSize = _statusProgressBar!.RenderSize.ToString();
            if (renderSize.IndexOf(';') > 0)
            {
                //renderSize = renderSize.Replace(';', '.');
                renderSize = renderSize.Substring(0, renderSize.IndexOf(';'));
            }
            else if (renderSize.IndexOf(',') > 0)
            {
                renderSize = renderSize.Substring(0, renderSize.IndexOf(','));
            }

            double ProgressX = (Convert.ToDouble(renderSize) / _statusProgressBar.Maximum) * _statusProgressBar.Value;
            // double ProgressX = (Convert.ToDouble(StatusProgressBar!.RenderSize.ToString().Replace(",", ".")) / StatusProgressBar.Maximum) * StatusProgressBar.Value;
            Rect boxstørrelse = new()
            {
                Width = ProgressX,
                Height = 50,
                X = 0
            };
            _blockbox!.Rect = boxstørrelse;
        }
    }
}
