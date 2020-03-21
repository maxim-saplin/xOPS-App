using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Saplin.xOPS.UI.Controls
{
    public class LabelRepeater : StackLayout
    {
        private BoxView box;

        public LabelRepeater()
        {
            box = new BoxView();

            var g = new TapGestureRecognizer();
            g.Tapped += (s, e) => DoClick();
            box.GestureRecognizers.Add(g);
            box.HorizontalOptions = LayoutOptions.Fill;
            box.VerticalOptions = LayoutOptions.Fill;
            box.BackgroundColor = Color.Transparent;
        }

        public void DoClick()
        {
            click?.Invoke(this, null);
        }

        private async void Animate(View view = null)
        {
            if (view == null) view = this;

            await view.FadeTo(0.5, 150);
            await view.FadeTo(1.0, 100);
        }

        private event EventHandler click;

        public event EventHandler Clicked
        {
            add
            {
                lock (this) { click += value; }
            }
            remove
            {
                lock (this) { click -= value; }
            }
        }

        private async void Animate()
        {
            if (Opacity == 0) return;
            await this.FadeTo(0.0, 100);
            await this.FadeTo(1.0, 1500);
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

            control.Children.Add(control.box);
        }

        public Style LabelStyle
        {
            get;set;
        }
    }
}