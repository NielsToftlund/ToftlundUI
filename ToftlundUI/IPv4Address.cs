using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    ///     <MyNamespace:IPv4Address/>
    ///
    /// </summary>
    public class IPv4Address : Control
    {
        static IPv4Address()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IPv4Address), new FrameworkPropertyMetadata(typeof(IPv4Address)));
        }

        TextBox? Segment1, Segment2, Segment3, Segment4;
        private static readonly List<Key> DigitKeys = new() { Key.D0, Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9, Key.NumPad0, Key.NumPad1, Key.NumPad2, Key.NumPad3, Key.NumPad4, Key.NumPad5, Key.NumPad6, Key.NumPad7, Key.NumPad8, Key.NumPad9 };
        private static readonly List<Key> MoveForwardKeys = new() { Key.Right };
        private static readonly List<Key> MoveBackwardKeys = new() { Key.Left };
        private static readonly List<Key> OtherAllowedKeys = new() { Key.Tab, Key.Delete };

        Border? IPv4AddressBorder;
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        public static readonly DependencyProperty CornerRadiusProperty
            = DependencyProperty.Register(
                "CornerRadius",
                typeof(CornerRadius),
                typeof(IPv4Address),
                new PropertyMetadata(new CornerRadius(0)));

        public string IPaddress
        {
            get { return (string)GetValue(IPaddressProperty); }
            set { SetValue(IPaddressProperty, value);}
        }
        public static readonly DependencyProperty IPaddressProperty
            = DependencyProperty.Register(
                "IPaddress",
                typeof(string),
                typeof(IPv4Address),
                new PropertyMetadata(""));

        public override void OnApplyTemplate()
        {
            _segments.Add(Segment1!);
            _segments.Add(Segment2!);
            _segments.Add(Segment3!);
            _segments.Add(Segment4!);

            Segment1 = GetTemplateChild("Segment1") as TextBox;
            Segment2 = GetTemplateChild("Segment2") as TextBox;
            Segment3 = GetTemplateChild("Segment3") as TextBox;
            Segment4 = GetTemplateChild("Segment4") as TextBox;
            IPv4AddressBorder = GetTemplateChild("IPv4AddressBorder") as Border;
            
            // Events
            Segment1!.Loaded += IPv4Address_Loaded;
            IPv4AddressBorder!.SizeChanged += IPv4Address_SizeChanged;
            
            Segment1!.PreviewKeyDown += IPv4Address_PreviewKeyDown;
            Segment2!.PreviewKeyDown += IPv4Address_PreviewKeyDown;
            Segment3!.PreviewKeyDown += IPv4Address_PreviewKeyDown;
            Segment4!.PreviewKeyDown += IPv4Address_PreviewKeyDown;

            Segment1!.LostFocus += IPv4Address_LostFocus;
            Segment2!.LostFocus += IPv4Address_LostFocus;
            Segment3!.LostFocus += IPv4Address_LostFocus;
            Segment4!.LostFocus += IPv4Address_LostFocus;
        }

        private void IPv4Address_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateIP();
        }

        private void IPv4Address_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (DigitKeys.Contains(e.Key))
            {
                e.Handled = ShouldCancelDigitKeyPress();
                HandleDigitPress();
            }
            else if (MoveBackwardKeys.Contains(e.Key))
            {
                e.Handled = ShouldCancelBackwardKeyPress();
                HandleBackwardKeyPress();
            }
            else if (MoveForwardKeys.Contains(e.Key))
            {
                e.Handled = ShouldCancelForwardKeyPress();
                HandleForwardKeyPress();
            }
            else if (e.Key == Key.Back)
            {
                HandleBackspaceKeyPress();
            }
            else if (e.Key == Key.OemPeriod)
            {
                e.Handled = true;
                HandlePeriodKeyPress();
            }
            else
            {
                e.Handled = !AreOtherAllowedKeysPressed(e);
            }
        }

        private void IPv4Address_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetSegmentWidth("255");
        }

        private void IPv4Address_Loaded(object sender, RoutedEventArgs e)
        {
            SetSegmentWidth("255");
            CornerRadius LeftSideCornerRadius = new()
            {
                TopLeft = CornerRadius.TopLeft,
                BottomLeft = CornerRadius.TopLeft
            };
            CornerRadius RightSideCornerRadius = new()
            {
                TopRight = CornerRadius.TopLeft,
                BottomRight = CornerRadius.TopLeft
            };
            Segment1!.SetValue(TemplateProperty, GetRoundedTextBoxTemplate(LeftSideCornerRadius));
            Segment4!.SetValue(TemplateProperty, GetRoundedTextBoxTemplate(RightSideCornerRadius));

            if (IPaddress != "")
            {
                string[] SegmentIP = IPaddress.Split(".");
                if (SegmentIP[0].IsNumeric() && SegmentIP[1].IsNumeric() && SegmentIP[2].IsNumeric() && SegmentIP[3].IsNumeric())
                {
                    Segment1!.Text = SegmentIP[0];
                    Segment2!.Text = SegmentIP[1];
                    Segment3!.Text = SegmentIP[2];
                    Segment4!.Text = SegmentIP[3];
                }
             
            }
            ValidateIP();
        }

        private void SetSegmentWidth(string candidate)
        {
            var formattedText = new FormattedText(
                candidate,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(Segment1!.FontFamily, Segment1!.FontStyle, Segment1.FontWeight, Segment1.FontStretch),
                Segment1.FontSize,
                Brushes.Black,
                new NumberSubstitution(),
                VisualTreeHelper.GetDpi(Segment1).PixelsPerDip);

            Segment1!.Width = formattedText.Width + 5;
            Segment2!.Width = formattedText.Width + 5;
            Segment3!.Width = formattedText.Width + 5;
            Segment4!.Width = formattedText.Width + 5;
        }

        //public static bool IsNumeric(this string text) => double.TryParse(text, out _);

        private void ValidateIP()
        {
            string TempIPaddress = Segment1!.Text + "." + Segment2!.Text + "." + Segment3!.Text + "." + Segment4!.Text;
            string[] SegmentIP = TempIPaddress.Split(".");
            if (SegmentIP[0].IsNumeric() && SegmentIP[1].IsNumeric() && SegmentIP[2].IsNumeric() && SegmentIP[3].IsNumeric())
            {
                if (Convert.ToInt32(SegmentIP[0]) > 255 || (Convert.ToInt32(SegmentIP[0]) < 0))
                {
                    Segment1!.Background = new SolidColorBrush(Colors.Red);
                    IPaddress = "";
                }
                else
                {
                    Segment1!.Background = new SolidColorBrush(Colors.Transparent);
                }
                if (Convert.ToInt32(SegmentIP[1]) > 255 || (Convert.ToInt32(SegmentIP[1]) < 0))
                {
                    Segment2!.Background = new SolidColorBrush(Colors.Red);
                    IPaddress = "";
                }
                else
                {
                    Segment2!.Background = new SolidColorBrush(Colors.Transparent);
                }
                if (Convert.ToInt32(SegmentIP[2]) > 255 || (Convert.ToInt32(SegmentIP[2]) < 0))
                {
                    Segment3!.Background = new SolidColorBrush(Colors.Red);
                    IPaddress = "";
                }
                else
                {
                    Segment3!.Background = new SolidColorBrush(Colors.Transparent);
                }
                if (Convert.ToInt32(SegmentIP[3]) > 255 || (Convert.ToInt32(SegmentIP[3]) < 0))
                {
                    Segment4!.Background = new SolidColorBrush(Colors.Red);
                    IPaddress = "";
                }
                else
                {
                    Segment4!.Background = new SolidColorBrush(Colors.Transparent);
                }
            }
            else
            {
                IPaddress = "";
            }
        }

        public ControlTemplate GetRoundedTextBoxTemplate(CornerRadius RadiusData)
        {
            ControlTemplate template = new(typeof(TextBoxBase));
            FrameworkElementFactory elemFactory = new(typeof(Border))
            {
                Name = "Border"
            };
            elemFactory.SetValue(Border.CornerRadiusProperty, RadiusData);
            elemFactory.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(TextBox.BackgroundProperty));
            template.VisualTree = elemFactory;
            FrameworkElementFactory scrollViewerElementFactory = new(typeof(ScrollViewer))
            {
                Name = "PART_ContentHost"
            };
            elemFactory.AppendChild(scrollViewerElementFactory);
            return template;
        }

        private readonly List<TextBox> _segments = new();

        private bool ShouldCancelBackwardKeyPress()
        {
            var currentTextBox = FocusManager.GetFocusedElement(this) as TextBox;
            return currentTextBox != null && currentTextBox.CaretIndex == 0;
        }

        private void HandleBackspaceKeyPress()
        {
            var currentTextBox = FocusManager.GetFocusedElement(this) as TextBox;

            if (currentTextBox != null && currentTextBox.CaretIndex == 0 && currentTextBox.SelectedText.Length == 0)
            {
                MoveFocusToPreviousSegment(currentTextBox);
            }
        }

        private void HandleBackwardKeyPress()
        {
            var currentTextBox = FocusManager.GetFocusedElement(this) as TextBox;

            if (currentTextBox != null && currentTextBox.CaretIndex == 0)
            {
                MoveFocusToPreviousSegment(currentTextBox);
            }
        }

        private bool ShouldCancelForwardKeyPress()
        {
            var currentTextBox = FocusManager.GetFocusedElement(this) as TextBox;
            return currentTextBox != null && currentTextBox.CaretIndex == 3;
        }

        private void HandleForwardKeyPress()
        {
            var currentTextBox = FocusManager.GetFocusedElement(this) as TextBox;

            if (currentTextBox != null && currentTextBox.CaretIndex == currentTextBox.Text.Length)
            {
                MoveFocusToNextSegment(currentTextBox);
            }
        }

        private void HandlePeriodKeyPress()
        {
            var currentTextBox = FocusManager.GetFocusedElement(this) as TextBox;

            if (currentTextBox != null && currentTextBox.Text.Length > 0 && currentTextBox.CaretIndex == currentTextBox.Text.Length)
            {
                MoveFocusToNextSegment(currentTextBox);
            }
        }

        private void MoveFocusToPreviousSegment(TextBox currentTextBox)
        {
            if (!ReferenceEquals(currentTextBox, Segment1!))
            {
                var previousSegmentIndex = _segments.FindIndex(box => ReferenceEquals(box, currentTextBox)) - 1;
                currentTextBox.SelectionLength = 0;
                currentTextBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
                _segments[previousSegmentIndex].CaretIndex = _segments[previousSegmentIndex].Text.Length;
            }
        }



        private void MoveFocusToNextSegment(TextBox currentTextBox)
        {
            if (!ReferenceEquals(currentTextBox, Segment4!))
            {
                currentTextBox.SelectionLength = 0;
                currentTextBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        //private void DataObject_OnPasting(object sender, DataObjectPastingEventArgs e)
        //{
        //    var isText = e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText, true);
        //    if (!isText)
        //    {
        //        e.CancelCommand();
        //        return;
        //    }

        //    var text = e.SourceDataObject.GetData(DataFormats.UnicodeText) as string;
        //    if (!int.TryParse(text, out _))
        //    {
        //        e.CancelCommand();
        //    }
        //}

        private bool ShouldCancelDigitKeyPress()
        {
            var currentTextBox = FocusManager.GetFocusedElement(this) as TextBox;
            return currentTextBox != null &&
                   currentTextBox.Text.Length == 3 &&
                   currentTextBox.CaretIndex == 3 &&
                   currentTextBox.SelectedText.Length == 0;
        }

        private void HandleDigitPress()
        {
            var currentTextBox = FocusManager.GetFocusedElement(this) as TextBox;

            if (currentTextBox != null && currentTextBox.Text.Length == 3 &&
                currentTextBox.CaretIndex == 3 && currentTextBox.SelectedText.Length == 0)
            {
                MoveFocusToNextSegment(currentTextBox);
            }
        }

        private static bool AreOtherAllowedKeysPressed(KeyEventArgs e)
        {
            return e.Key == Key.C && ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) != 0) ||
                   e.Key == Key.V && ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) != 0) ||
                   e.Key == Key.A && ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) != 0) ||
                   e.Key == Key.X && ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) != 0) ||
                   OtherAllowedKeys.Contains(e.Key);
        }











    }

    public static class StringExt
    {
        public static bool IsNumeric(this string text) => double.TryParse(text, out _);
    }
}
