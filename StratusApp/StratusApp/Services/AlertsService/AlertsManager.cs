using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel;
using Utils.Enums;

namespace StratusApp.Services.AlertsService
{
    public class AlertsManager : ICollection<KeyValuePair<string, AlertTimer>>
    {
        private readonly ConcurrentDictionary<string, AlertTimer> _alertTimers;

        public event EventHandler AlertTimerElapsed;
        
        public AlertsManager() 
        { 
            _alertTimers = new ConcurrentDictionary<string, AlertTimer>();
        }

        public int Count => _alertTimers.Count;

        public bool IsReadOnly => false;

        public void Add(KeyValuePair<string, AlertTimer> item)
        {
            if(!_alertTimers.TryAdd(item.Key, item.Value))
            {
                throw new InvalidOperationException();
            }

            _alertTimers[item.Key].AlertTimerElapsed += alertTimer_Elapsed;
            _alertTimers[item.Key].StartTimer();
        }

        private void alertTimer_Elapsed(object? sender, EventArgs e)
        {
            AlertTimerElapsed?.Invoke(sender, e);
        }

        public void Clear()
        {
            _alertTimers.Clear();
        }

        public bool Contains(KeyValuePair<string, AlertTimer> item)
        {
            return _alertTimers.ContainsKey(item.Key);
        }

        public void CopyTo(KeyValuePair<string, AlertTimer>[] array, int arrayIndex)
        {
            foreach(var  item in _alertTimers)
            {
                array[arrayIndex++] = item;
            }
        }

        public IEnumerator<KeyValuePair<string, AlertTimer>> GetEnumerator()
        {
            return _alertTimers.GetEnumerator();
        }

        public bool Remove(KeyValuePair<string, AlertTimer> item)
        {
            return _alertTimers.TryRemove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void UpdateIntervalTime(string userEmail, long intervalTimeMilisec)
        {
            if(_alertTimers.ContainsKey(userEmail))
            {
                _alertTimers[userEmail].UpdateIntervalTime(intervalTimeMilisec);
            }
        }

        public void UpdateThresholdValue(string userEmail, int value, eAlertType type)
        {
            if (_alertTimers.ContainsKey(userEmail))
            {
                _alertTimers[userEmail].UpdateThresholdValue(value, type);
            }
        }

        public double GetThresholdValue(string userEmail, eAlertType type)
        {
            if (_alertTimers.ContainsKey(userEmail))
            {
                return _alertTimers[userEmail].GetThresholdValueByType(type);
            }

            throw new InvalidProgramException();
        }
    }
}
