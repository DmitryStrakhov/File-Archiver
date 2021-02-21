using System;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using NUnit.Framework;
using FileArchiver.Core.Helpers;
using System.Linq.Expressions;
using Expression = System.Linq.Expressions.Expression;

namespace FileArchiver.Tests {
    public static class ControlAssert {
        public static void PropertyIsBound<T>(T dependencyObject, Expression<Func<DependencyProperty>> getTargetProperty, string bindingPath)
            where T : DependencyObject {

            Guard.IsNotNull(dependencyObject, nameof(dependencyObject));
            Guard.IsNotNull(getTargetProperty, nameof(getTargetProperty));
            Guard.IsNotNullOrEmpty(bindingPath, nameof(bindingPath));

            Expression body = getTargetProperty?.Body;

            if(body == null || body.NodeType != ExpressionType.MemberAccess) {
                throw new ArgumentException(nameof(getTargetProperty));
            }

            FieldInfo field = (FieldInfo)((MemberExpression)body).Member;
            DependencyProperty dp = (DependencyProperty)field.GetValue(null);
            Binding binding = BindingOperations.GetBinding(dependencyObject, dp);
            Assert.NotNull(binding);
            Assert.AreEqual(bindingPath, binding.Path.Path);
        }
    }
}