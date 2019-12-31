using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FileArchiver.Base;
using FileArchiver.Controls;
using Microsoft.Xaml.Behaviors;

namespace FileArchiver.Behaviors {
    public abstract class FileSystemBehavior : Behavior<DropDownButton> {
        static readonly ControlHelper<FileSystemBehavior> helper = new ControlHelper<FileSystemBehavior>();
        MenuItem menuItem;

        public FileSystemBehavior() {
            this.menuItem = null;
        }

        #region Command

        public ICommand Command {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
        public static readonly DependencyProperty CommandProperty = helper.RegisterProperty<ICommand>(nameof(Command));

        #endregion

        private MenuItem MenuItem {
            get { return menuItem ?? (menuItem = CreateMenuItemInstance()); }
        }

        protected sealed override void OnAttached() {
            EnsureDropDownMenu();
            AssociatedObject.DropDownMenu.Items.Add(MenuItem);
        }
        protected sealed override void OnDetaching() {
            AssociatedObject.DropDownMenu?.Items.Remove(MenuItem);
        }
        protected void ExecuteCommand(string path) {
            Command?.Execute(path);
        }

        private void EnsureDropDownMenu() {
            if(AssociatedObject.DropDownMenu == null)
                AssociatedObject.DropDownMenu = new ContextMenu();
        }
        private MenuItem CreateMenuItemInstance() {
            MenuItem item = new MenuItem();
            item.Header = CommandName;
            item.Click += (s, e) => HandleMenuItemClick();
            return item;
        }
        protected abstract string CommandName { get; }
        protected abstract void HandleMenuItemClick();
    }
}
