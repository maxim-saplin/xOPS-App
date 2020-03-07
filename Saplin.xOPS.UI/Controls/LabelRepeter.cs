using System.Collections.Generic;
using Xamarin.Forms;

namespace Saplin.xOPS.UI.Controls
{
    public class LabelRepeater : StackLayout
    {

        private async void Animate()
        {
            await this.FadeTo(0.3, 400);
            await this.FadeTo(1.0, 400);
        } 
        public static readonly BindableProperty ItemsProperty =
            BindableProperty.Create(
                propertyName: nameof(Items),
                returnType: typeof(IEnumerable<string>),
                declaringType: typeof(LabelRepeater),
                defaultValue: null,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: ItemsChanged
        );

        public IEnumerable<string> Items
        {
            get { return (IEnumerable<string>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        private static void ItemsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = bindable as LabelRepeater;

            if (control == null) return;

            control.Animate();

            control.Children.Clear();

            var collection = newValue as IEnumerable<string>;

            if (collection != null)
            {
                foreach (var i in collection)
                {
                    control.Children.Add(new Label() { Text = i, Style = control.LabelStyle });
                }
            }
        }

        public Style LabelStyle
        {
            get;set;
        }
    }
}