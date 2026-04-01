using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace CompanyName.ProjectName.Hosts.WPF.Framework;


/// <summary>
/// Observable dictionary wrapper for XAML binding support
/// </summary>
public class ObservableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, INotifyPropertyChanged
    where TKey : notnull
{
    public new TValue? this[TKey key]
    {
        get => ContainsKey(key) ? base[key] : default;
        set
        {
            base[key] = value!;
            OnPropertyChanged($"Item[{key}]");
            OnPropertyChanged("Item[]"); // Notify for indexer
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
