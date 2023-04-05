using System;
using UnityEngine;

namespace Game.Events
{
    public class EventSubscriptionsContainer
    {
        private Action[] _subscriptions;

        public EventsType type { get; private set; }

        public Action this[int index]
        {
            get { return _subscriptions[index]; }
        }

        public int Count { get { return _subscriptions != null ? _subscriptions.Length : 0; } }

        public EventSubscriptionsContainer(EventsType type)
        {
            this.type = type;
        }

        public void Update()
        {
            if (_subscriptions == null) return;

            for (int i = 0; i < _subscriptions.Length; i++)
            {
                Action subscription = _subscriptions[i];
                if (subscription != null)
                {
                    subscription.Invoke();
                }
                else
                {
                    Debug.LogWarning(
                        $"Trying to execute null subscription " +
                        $"for engine event: {type} index: {i}");
                }
            }
        }

        public void Add(Action subscription)
        {
            if (_subscriptions == null)
            {
                _subscriptions = new Action[] { subscription };
            }
            else
            {
                _subscriptions = _subscriptions.AddElement(subscription);
            }
        }

        public void TryRemove(Action subscription)
        {
            if (_subscriptions == null)
            {
                Debug.LogWarning($"Trying to remove subscription of type {type} from empty container");
                return;
            }

            if (TryFindIndex(subscription, out int index))
            {
                _subscriptions = _subscriptions.RemoveAt(index);
            }
        }

        public void ClearAll()
        {
            _subscriptions = null;
        }

        private bool TryFindIndex(Action subscription, out int index)
        {
            if (_subscriptions != null)
            {
                for (int i = 0; i < _subscriptions.Length; i++)
                {
                    Action nextSubscription = _subscriptions[i];

                    if (nextSubscription == null)
                    {
                        Debug.LogWarning($"Detected null element in event type container of type: {type} index: {i}");
                        continue;
                    }

                    if (ReferenceEquals(subscription.Target, nextSubscription.Target))
                    {
                        index = i;
                        return true;
                    }
                }
            }

            index = 0;
            return false;
        }
    }
}