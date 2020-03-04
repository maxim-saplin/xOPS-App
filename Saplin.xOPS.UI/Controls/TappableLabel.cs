using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace Saplin.xOPS.UI.Controls
{
    public class TappableLabel : Label
    {
        public TappableLabel()
        {
            var g = new TapGestureRecognizer();
            g.Tapped += (s, e) => DoClick();
            GestureRecognizers.Add(g);
        }

        private event EventHandler click;
        public void DoClick()
        {
            if (AnimateOnTap) Animate();

            click?.Invoke(this, null);
            if (Command != null) Command.Execute(CommandParameter);
        }

        private async void Animate()
        {
            await this.FadeTo(0.5, 150);
            await this.FadeTo(1.0, 100);
        }

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

        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(
                propertyName: nameof(Command),
                returnType: typeof(ICommand),
                declaringType: typeof(TappableLabel),
                defaultValue: null,
                defaultBindingMode: BindingMode.OneWay
             );

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        private void ExecuteCommand(object sender, EventArgs e)
        {
            if (IsEnabled)
                Command.Execute(CommandParameter);
        }

        public static readonly BindableProperty CommandParameterProperty =
            BindableProperty.Create(
                propertyName: nameof(CommandParameter),
                returnType: typeof(Object),
                declaringType: typeof(TappableLabel),
                defaultValue: null,
                defaultBindingMode: BindingMode.OneWay
             );

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public static readonly BindableProperty SelectedProperty =
            BindableProperty.Create(
                propertyName: nameof(CommandParameter),
                returnType: typeof(bool),
                declaringType: typeof(TappableLabel),
                defaultValue: false,
                defaultBindingMode: BindingMode.OneWay
             );

        public bool Selected
        {
            get { return (bool)GetValue(SelectedProperty); }
            set { SetValue(SelectedProperty, value); }
        }

        public static readonly BindableProperty AnimateOnTapProperty =
            BindableProperty.Create(
                propertyName: nameof(AnimateOnTap),
                returnType: typeof(bool),
                declaringType: typeof(TappableLabel),
                defaultValue: false,
                defaultBindingMode: BindingMode.OneWay
             );

        public bool AnimateOnTap
        {
            get { return (bool)GetValue(AnimateOnTapProperty); }
            set { SetValue(AnimateOnTapProperty, value); }
        }
    }
}
