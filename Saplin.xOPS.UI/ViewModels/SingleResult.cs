using System;
using Xamarin.Forms;

namespace Saplin.xOPS.UI.ViewModels
{
    public class SingleResult : BindableObject
    {
        public static readonly BindableProperty ValueProperty =
            BindableProperty.Create(
                propertyName: nameof(Value),
                returnType: typeof(double),
                declaringType: typeof(SingleResult),
                defaultValue: -1.0d,
                defaultBindingMode: BindingMode.OneWay
             );

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }


        public bool Int { get; set; }
        public bool MultiThreaded { get; set; }
    }
}
