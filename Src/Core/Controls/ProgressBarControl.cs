using System.Windows;
using System.Windows.Controls;
using FileArchiver.Core.Helpers;

namespace FileArchiver.Core.Controls {
    public class ProgressBarControl : ProgressBar {
        public static readonly DependencyProperty IndeterminateValueProperty;
        public static readonly DependencyProperty ProgressValueProperty;

        static ProgressBarControl() {
            ControlHelper<ProgressBarControl> controlHelper = new ControlHelper<ProgressBarControl>();
            IndeterminateValueProperty = controlHelper.RegisterProperty(nameof(IndeterminateValue), -1.0d);
            ProgressValueProperty = controlHelper.RegisterProperty(nameof(ProgressValue), 0.0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal, OnProgressValueChanged, CoerceProgressValue);
        }

        public ProgressBarControl() {
        }

        public double IndeterminateValue {
            get { return (double)GetValue(IndeterminateValueProperty); }
            set { SetValue(IndeterminateValueProperty, value); }
        }
        public double ProgressValue {
            get { return (double)GetValue(ProgressValueProperty); }
            set { SetValue(ProgressValueProperty, value); }
        }
        static object CoerceProgressValue(DependencyObject d, object baseValue) {
            ProgressBarControl control = (ProgressBarControl)d;
            double value = (double)baseValue;

            if(MathHelper.AreEqual(control.IndeterminateValue, value)) {
                return baseValue;
            }
            if(value < control.Minimum) return control.Minimum;
            if(value > control.Maximum) return control.Maximum;
            return baseValue;

        }
        static void OnProgressValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ProgressBarControl control = (ProgressBarControl)d;
            control.OnProgressValueChanged(e);
        }
        private void OnProgressValueChanged(DependencyPropertyChangedEventArgs e) {
            double value = (double)e.NewValue;

            if(MathHelper.AreEqual(IndeterminateValue, value)) {
                EnsureIndeterminate();
            }
            else {
                EnsureDeterminate();
                Value = value;
            }
            if(MathHelper.AreEqual(value, IndeterminateValue)) {
                if(!IsIndeterminate) IsIndeterminate = true;
            }
            else {
                if(IsIndeterminate) IsIndeterminate = false;
                Value = (double)e.NewValue;
            }
        }
        private void EnsureIndeterminate() {
            if(!IsIndeterminate) IsIndeterminate = true;
        }
        private void EnsureDeterminate() {
            if(IsIndeterminate) IsIndeterminate = false;
        }
    }
}