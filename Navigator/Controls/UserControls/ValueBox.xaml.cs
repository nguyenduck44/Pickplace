using NavigatorMachine.Models;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace NavigatorMachine.Controls
{
    /// <summary>
    /// Interaction logic for ValueBox.xaml
    /// </summary>
    public partial class ValueBox : UserControl, INotifyPropertyChanged
    {
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(ValueBox), new PropertyMetadata("Header"));

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set 
            { 
                SetValue(ValueProperty, value);
                OnPropertyChanged(nameof(ValueToString));
            }
        }
        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(ValueBox), new PropertyMetadata(0.0));

        public double MinValue
        {
            get { return (double)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, MinValue); }
        }
        // Using a DependencyProperty as the backing store for MinValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(double), typeof(ValueBox), new PropertyMetadata(-10000.0));

        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, MaxValue); }
        }
        // Using a DependencyProperty as the backing store for MaxValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double), typeof(ValueBox), new PropertyMetadata(10000.0));


        public string Unit
        {
            get { return (string)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register("Unit", typeof(string), typeof(ValueBox), new PropertyMetadata(""));

        public bool ReverseUnitPlace
        {
            get { return (bool)GetValue(ReverseUnitPlaceProperty); }
            set { SetValue(ReverseUnitPlaceProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReverseUnitPlaceProperty =
            DependencyProperty.Register("ReverseUnitPlace", typeof(bool), typeof(ValueBox), new PropertyMetadata(false));

        public string ValueToString => ReverseUnitPlace && Unit != "" ? $"{Unit} " + Value  : Value.ToString() + $" {Unit} ";

        

        public ValueBox()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            CData data = new CData()
            {
                Header = Header,

                OldValue = Value,
                Value = Value,

                MinValue = MinValue,
                MaxValue = MaxValue,
            };

            ValueEditor valueEditor = new ValueEditor(data);

            if (valueEditor.ShowDialog() == true)
            {
                Value = data.Value;

                OnPropertyChanged(nameof(ValueToString));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
