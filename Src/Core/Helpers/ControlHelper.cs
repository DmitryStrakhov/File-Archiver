using System;
using System.Windows;

namespace FileArchiver.Core.Helpers {
    public sealed class ControlHelper<TOwner>
        where TOwner : DependencyObject {

        public ControlHelper() {
        }
        public DependencyProperty RegisterProperty<T>(
            string name,
            T defaultValue = default(T),
            PropertyChangedCallback propertyChanged = null) {

            FrameworkPropertyMetadata metadata = new FrameworkPropertyMetadata(
                defaultValue,
                (d, e) => propertyChanged?.Invoke((TOwner)d, e));

            return DependencyProperty.Register(name, typeof(T), typeof(TOwner), metadata);
        }
        public DependencyProperty RegisterProperty<T>(
            string name,
            T defaultValue,
            FrameworkPropertyMetadataOptions flags,
            PropertyChangedCallback propertyChanged = null,
            CoerceValueCallback coerceValue = null) {

            FrameworkPropertyMetadata metadata = new FrameworkPropertyMetadata(
                defaultValue,
                flags,
                (d, e) => propertyChanged?.Invoke((TOwner)d, e),
                (d, e) => coerceValue?.Invoke(d, e));
            return DependencyProperty.Register(name, typeof(T), typeof(TOwner), metadata);
        }
        public DependencyPropertyKey RegisterReadOnlyProperty<T>(
            string name,
            T defaultValue = default(T),
            PropertyChangedCallback propertyChanged = null) {

            FrameworkPropertyMetadata metadata = new FrameworkPropertyMetadata(
                defaultValue,
                (d, e) => propertyChanged?.Invoke((TOwner)d, e));
            return DependencyProperty.RegisterReadOnly(name, typeof(T), typeof(TOwner), metadata);
        }

        public delegate void PropertyChangedCallback(TOwner owner, DependencyPropertyChangedEventArgs e);
    }
}