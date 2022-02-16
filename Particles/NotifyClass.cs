using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Particles
{
    public abstract class NotifyClass : INotifyPropertyChanged
    {
        /// <summary>
        /// sets a property. If values aren't equal, a new value is replaced
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field">a reference of a private field of a property</param>
        /// <param name="value">the current value </param>
        /// <param name="propertyName">a name of a selected property [optional]</param>
        /// <returns></returns>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        /// <summary>
        /// an event that says us about changes 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
