using System;
using System.Windows;

namespace FileArchiver.Helpers {
    public sealed class ControlHelper<TOwner>
        where TOwner : DependencyObject {

        public ControlHelper() {

        }
        public DependencyProperty RegisterProperty<T>(string name, T defaultValue = default(T), PropertyChangedCallback propertyChanged = null) {
            FrameworkPropertyMetadata metadata = new FrameworkPropertyMetadata(defaultValue, (d, e) => propertyChanged?.Invoke((TOwner)d, e));
            return DependencyProperty.Register(name, typeof(T), typeof(TOwner), metadata);
        }
        public DependencyPropertyKey RegisterReadOnlyProperty<T>(string name, T defaultValue = default(T), PropertyChangedCallback propertyChanged = null) {
            FrameworkPropertyMetadata metadata = new FrameworkPropertyMetadata(defaultValue, (d, e) => propertyChanged?.Invoke((TOwner)d, e));
            return DependencyProperty.RegisterReadOnly(name, typeof(T), typeof(TOwner), metadata);
        }

        public delegate void PropertyChangedCallback(TOwner owner, DependencyPropertyChangedEventArgs e);
    }
}
