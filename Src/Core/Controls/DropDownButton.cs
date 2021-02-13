using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using FileArchiver.Core.Helpers;

namespace FileArchiver.Core.Controls {
    public class DropDownButton : ContentControl {
        public static readonly DependencyProperty IsPressedProperty;
        public static readonly DependencyProperty DropDownMenuProperty;
        static readonly DependencyPropertyKey IsPressedPropertyKey;

        static DropDownButton() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownButton), new FrameworkPropertyMetadata(typeof(DropDownButton)));
            ControlHelper<DropDownButton> controlHelper = new ControlHelper<DropDownButton>();
            IsPressedPropertyKey = controlHelper.RegisterReadOnlyProperty(nameof(IsPressed), false, (d, e) => d.OnIsPressedChanged(e));
            DropDownMenuProperty = controlHelper.RegisterProperty<ContextMenu>(nameof(DropDownMenu), null, (d, e) => d.OnDropDownMenuChanged(e));
            IsPressedProperty = IsPressedPropertyKey.DependencyProperty;
        }

        public DropDownButton() {
        }

        #region IsPressed

        public bool IsPressed {
            get { return (bool)GetValue(IsPressedProperty); }
        }

        private void OnIsPressedChanged(DependencyPropertyChangedEventArgs e) {
            if(IsPressed)
                ShowDropDownMenu();
            else
                CloseDropDownMenu();
        }

        #endregion

        #region DropDown Menu

        public ContextMenu DropDownMenu {
            get { return (ContextMenu)GetValue(DropDownMenuProperty); }
            set { SetValue(DropDownMenuProperty, value); }
        }

        private void OnDropDownMenuChanged(DependencyPropertyChangedEventArgs e) {
            e.OldValue.CastTo<ContextMenu>().Do(UnsubscribeEvents);
            e.NewValue.CastTo<ContextMenu>().Do(SubscribeEvents);
        }

        #endregion

        #region Handlers

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
            e.Handled = true;
            Focus();
            if(e.ButtonState == MouseButtonState.Pressed) {
                OnLeftButtonPressed(e);
            }
            base.OnMouseLeftButtonDown(e);
        }
        private void OnDropDownMenuClosed(object sender, RoutedEventArgs e) {
            SetIsPressed(false);
        }

        #endregion

        private void SetIsPressed(bool pressed) {
            if(pressed)
                SetValue(IsPressedPropertyKey, true);
            else
                ClearValue(IsPressedPropertyKey);
        }

        private bool IsDropDownMenuInitialized {
            get { return DropDownMenu != null && DropDownMenu.Items.Count != 0; }
        }
        private bool IsDropDownMenuOpen {
            get { return DropDownMenu != null && DropDownMenu.IsOpen; }
        }

        private void ShowDropDownMenu() {
            if(!IsDropDownMenuInitialized) return;
            SetupDropDownMenu(DropDownMenu);
            DropDownMenu.IsOpen = true;
        }
        private void CloseDropDownMenu() {
            if(IsDropDownMenuOpen)
                DropDownMenu.IsOpen = false;
        }
        private void SetupDropDownMenu(ContextMenu dropDownMenu) {
            dropDownMenu.HasDropShadow = true;
            dropDownMenu.PlacementTarget = this;
            dropDownMenu.Placement = PlacementMode.Bottom;
        }

        private void SubscribeEvents(ContextMenu dropDownMenu) {
            dropDownMenu.Closed += OnDropDownMenuClosed;
        }
        private void UnsubscribeEvents(ContextMenu dropDownMenu) {
            dropDownMenu.Closed -= OnDropDownMenuClosed;
        }
        private void OnLeftButtonPressed(MouseButtonEventArgs e) {
            if(!IsPressed)
                SetIsPressed(true);
        }
    }
}