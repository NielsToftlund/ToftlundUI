using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
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
    ///     <MyNamespace:IPv4Address/>
    ///
    /// </summary>
    public class IPv4Address : Control
    {
        static IPv4Address()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IPv4Address), new FrameworkPropertyMetadata(typeof(IPv4Address)));
        }

        TextBox? _segment1, _segment2, _segment3, _segment4, _dot1, _dot2, _dot3;
        private static readonly List<Key> _digitKeys = [Key.D0, Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9, Key.NumPad0, Key.NumPad1, Key.NumPad2, Key.NumPad3, Key.NumPad4, Key.NumPad5, Key.NumPad6, Key.NumPad7, Key.NumPad8, Key.NumPad9];
        private static readonly List<Key> _moveForwardKeys = [Key.Right];
        private static readonly List<Key> _moveBackwardKeys = [Key.Left];
        private static readonly List<Key> _otherAllowedKeys = [Key.Tab, Key.Delete];

        Border? _iPv4AddressBorder;
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
            set { SetValue(IPaddressProperty, value); }
        }
        public static DependencyProperty IPaddressProperty
            = DependencyProperty.Register(
                "IPaddress",
                typeof(string),
                typeof(IPv4Address),
                new PropertyMetadata(""));

        public override void OnApplyTemplate()
        {
            _segments.Add(_segment1!);
            _segments.Add(_segment2!);
            _segments.Add(_segment3!);
            _segments.Add(_segment4!);

            _segment1 = GetTemplateChild("Segment1") as TextBox;
            _segment2 = GetTemplateChild("Segment2") as TextBox;
            _segment3 = GetTemplateChild("Segment3") as TextBox;
            _segment4 = GetTemplateChild("Segment4") as TextBox;
            _dot1 = GetTemplateChild("Dot1") as TextBox;
            _dot2 = GetTemplateChild("Dot2") as TextBox;
            _dot3 = GetTemplateChild("Dot3") as TextBox;
            _iPv4AddressBorder = GetTemplateChild("IPv4AddressBorder") as Border;

            // Events
            _segment1!.Loaded += IPv4Address_Loaded;
            _iPv4AddressBorder!.SizeChanged += IPv4Address_SizeChanged;

            _segment1!.PreviewKeyDown += IPv4Address_PreviewKeyDown;
            _segment2!.PreviewKeyDown += IPv4Address_PreviewKeyDown;
            _segment3!.PreviewKeyDown += IPv4Address_PreviewKeyDown;
            _segment4!.PreviewKeyDown += IPv4Address_PreviewKeyDown;

            _segment1!.LostFocus += IPv4Address_LostFocus;
            _segment2!.LostFocus += IPv4Address_LostFocus;
            _segment3!.LostFocus += IPv4Address_LostFocus;
            _segment4!.LostFocus += IPv4Address_LostFocus;
        }

        private void IPv4Address_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateIP();
        }

        private void IPv4Address_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (_digitKeys.Contains(e.Key))
            {
                e.Handled = ShouldCancelDigitKeyPress();
                HandleDigitPress();
            }
            else if (_moveBackwardKeys.Contains(e.Key))
            {
                e.Handled = ShouldCancelBackwardKeyPress();
                HandleBackwardKeyPress();
            }
            else if (_moveForwardKeys.Contains(e.Key))
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

            Border NoColor = new()
            {
                BorderBrush = new SolidColorBrush(Colors.Red)
            };

            _segment2!.SetValue(TemplateProperty, GetRoundedTextBoxTemplate(LeftSideCornerRadius));
            _segment3!.SetValue(TemplateProperty, GetRoundedTextBoxTemplate(LeftSideCornerRadius));
            _dot1!.SetValue(TemplateProperty, GetRoundedTextBoxTemplate(LeftSideCornerRadius));
            _dot2!.SetValue(TemplateProperty, GetRoundedTextBoxTemplate(LeftSideCornerRadius));
            _dot3!.SetValue(TemplateProperty, GetRoundedTextBoxTemplate(LeftSideCornerRadius));

            _segment1!.SetValue(TemplateProperty, GetRoundedTextBoxTemplate(LeftSideCornerRadius));
            _segment4!.SetValue(TemplateProperty, GetRoundedTextBoxTemplate(RightSideCornerRadius));

            if (IPaddress != "")
            {
                string[] SegmentIP = IPaddress.Split(".");
                if (SegmentIP[0].IsNumeric() && SegmentIP[1].IsNumeric() && SegmentIP[2].IsNumeric() && SegmentIP[3].IsNumeric())
                {
                    _segment1!.Text = SegmentIP[0];
                    _segment2!.Text = SegmentIP[1];
                    _segment3!.Text = SegmentIP[2];
                    _segment4!.Text = SegmentIP[3];
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
                new Typeface(_segment1!.FontFamily, _segment1!.FontStyle, _segment1.FontWeight, _segment1.FontStretch),
                _segment1.FontSize,
                Brushes.Black,
                new NumberSubstitution(),
                VisualTreeHelper.GetDpi(_segment1).PixelsPerDip);

            _segment1!.Width = formattedText.Width + 5;
            _segment2!.Width = formattedText.Width + 5;
            _segment3!.Width = formattedText.Width + 5;
            _segment4!.Width = formattedText.Width + 5;
        }

        //public static bool IsNumeric(this string text) => double.TryParse(text, out _);

        private void ValidateIP()
        {
            string TempIPaddress = _segment1!.Text + "." + _segment2!.Text + "." + _segment3!.Text + "." + _segment4!.Text;
            string[] SegmentIP = TempIPaddress.Split(".");
            if (SegmentIP[0].IsNumeric() && SegmentIP[1].IsNumeric() && SegmentIP[2].IsNumeric() && SegmentIP[3].IsNumeric())
            {
                if (Convert.ToInt32(SegmentIP[0]) > 255 || (Convert.ToInt32(SegmentIP[0]) < 0))
                {
                    _segment1!.Background = new SolidColorBrush(Colors.Red);
                    IPaddress = "";
                }
                else
                {
                    _segment1!.Background = new SolidColorBrush(Colors.Transparent);
                }
                if (Convert.ToInt32(SegmentIP[1]) > 255 || (Convert.ToInt32(SegmentIP[1]) < 0))
                {
                    _segment2!.Background = new SolidColorBrush(Colors.Red);
                    IPaddress = "";
                }
                else
                {
                    _segment2!.Background = new SolidColorBrush(Colors.Transparent);
                }
                if (Convert.ToInt32(SegmentIP[2]) > 255 || (Convert.ToInt32(SegmentIP[2]) < 0))
                {
                    _segment3!.Background = new SolidColorBrush(Colors.Red);
                    IPaddress = "";
                }
                else
                {
                    _segment3!.Background = new SolidColorBrush(Colors.Transparent);
                }
                if (Convert.ToInt32(SegmentIP[3]) > 255 || (Convert.ToInt32(SegmentIP[3]) < 0))
                {
                    _segment4!.Background = new SolidColorBrush(Colors.Red);
                    IPaddress = "";
                }
                else
                {
                    _segment4!.Background = new SolidColorBrush(Colors.Transparent);
                }
            }
            else
            {
                IPaddress = "";
            }
        }

        public static ControlTemplate GetRoundedTextBoxTemplate(CornerRadius RadiusData)
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

        private readonly List<TextBox> _segments = [];

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
            if (!ReferenceEquals(currentTextBox, _segment1!))
            {
                var previousSegmentIndex = _segments.FindIndex(box => ReferenceEquals(box, currentTextBox)) - 1;
                currentTextBox.SelectionLength = 0;
                currentTextBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
                _segments[previousSegmentIndex].CaretIndex = _segments[previousSegmentIndex].Text.Length;
            }
        }



        private void MoveFocusToNextSegment(TextBox currentTextBox)
        {
            if (!ReferenceEquals(currentTextBox, _segment4!))
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
                   _otherAllowedKeys.Contains(e.Key);
        }











    }

    public static class StringExt
    {
        public static bool IsNumeric(this string text) => double.TryParse(text, out _);
    }
}
