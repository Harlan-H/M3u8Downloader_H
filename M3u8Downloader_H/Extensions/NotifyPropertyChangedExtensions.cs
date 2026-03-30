using M3u8Downloader_H.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace M3u8Downloader_H.Extensions;

internal static class NotifyPropertyChangedExtensions
{
    extension<TOwner>(TOwner owner)
        where TOwner : INotifyPropertyChanged
    {
        public IDisposable WatchProperty<TProperty>(
            Expression<Func<TOwner, TProperty>> propertyExpression,
            Action callback,
            bool watchInitialValue = false
        )
        {
            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression?.Member is not PropertyInfo property)
                throw new ArgumentException("Provided expression must reference a property.");

            void OnPropertyChanged(object? sender, PropertyChangedEventArgs args)
            {
                if (string.Equals(args.PropertyName, property.Name, StringComparison.Ordinal))
                {
                    callback();
                }
            }

            owner.PropertyChanged += OnPropertyChanged;

            if (watchInitialValue)
                callback();

            return new Disposable(() => owner.PropertyChanged -= OnPropertyChanged);
        }

    }
}
