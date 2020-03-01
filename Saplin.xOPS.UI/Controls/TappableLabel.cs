using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace Saplin.xOPS.UI.Controls
{
    public class TappableLabel : Label
    {
        private event EventHandler click;

        public void DoClick()
        {
            click.Invoke(this, null);
        }

        public event EventHandler Clicked
        {
            add
            {
                lock (this)
                {
                    click += value;

                    var g = new TapGestureRecognizer();
                    g.Tapped += (s, e) => click?.Invoke(s, e);
                    GestureRecognizers.Add(g);
                }
            }
            remove
            {
                lock (this)
                {
                    click -= value;
                    GestureRecognizers.Clear();
                }
            }
        }

        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(
                propertyName: nameof(Command),
                returnType: typeof(ICommand),
                declaringType: typeof(TappableLabel),
                defaultValue: null,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: CommandChanged
             );

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        private static void CommandChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (TappableLabel)bindable;
            var command = newValue as ICommand;

            if (command == null) control.Clicked -= control.ExecuteCommand;
            else control.Clicked += control.ExecuteCommand;
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
    }
}
