using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Saplin.xOPS.UI.Controls
{
    public class LabelRepeater : StackLayout
    {
        private Label label;

        public LabelRepeater()
        {
            label = new Label();

            var g = new TapGestureRecognizer();
            g.Tapped += (s, e) => DoClick();
            label.GestureRecognizers.Add(g);
            label.FormattedText = new FormattedString();

            Children.Add(label);
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

        public event EventHandler Tapped
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

            if (control.label.Style == null) control.label.Style = control.LabelStyle;

            control.Animate();

            control.label.FormattedText?.Spans?.Clear();

            var collection = newValue as IEnumerable<string>;

            if (collection != null)
            {
                foreach (var i in collection)
                {
                    control.label.FormattedText?.Spans?.Add(new Span() { Text = i+"\n" });
                }
            }

        }

        public Style LabelStyle
        {
            get;set;
        }
    }
}