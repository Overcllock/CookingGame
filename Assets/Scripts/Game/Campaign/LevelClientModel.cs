using System;
using System.Collections.Generic;
using Game.Content.Campaign;

namespace Game.Campaign
{
    public class LevelClientModel
    {
        private readonly Queue<OrderSettings> _ordersQueue;

        private readonly List<OrderClientModel> _currentOrders;
        
        public LevelEntry entry { get; }
        
        public IReadOnlyCollection<OrderClientModel> currentOrders { get { return _currentOrders; } }

        public int levelTime { get { return entry.time; } }
        public int currentTime { get; private set; }
        public int remainingTime { get { return levelTime - currentTime; } }
        
        public int ordersCount { get; }
        public int completedOrdersCount { get; private set; }
        public int failedOrdersCount { get; private set; }
        public int remainingOrdersCount { get { return ordersCount - completedOrdersCount - failedOrdersCount; } }

        public bool isCompleted
        {
            get { return remainingTime <= 0 || remainingOrdersCount <= 0; }
        }

        public bool isWin
        {
            get { return isCompleted && completedOrdersCount >= ordersCount; }
        }

        public event Action<LevelClientModel> timeChanged;
        public event Action<int, OrderClientModel> orderCreated;
        public event Action<int, OrderClientModel> orderRemoved;

        public LevelClientModel(LevelEntry entry)
        {
            this.entry = entry;

            ordersCount = entry.orders.IsNullOrEmpty() ? entry.ordersCount : entry.orders.Length;
            
            if (!entry.orders.IsNullOrEmpty())
            {
                _ordersQueue = new Queue<OrderSettings>(entry.orders);
            }
            else
            {
                var orders = LevelDataFactory.GenerateOrders(entry);
                _ordersQueue = new Queue<OrderSettings>(orders);
            }

            _currentOrders = new List<OrderClientModel>(entry.ordersCapacity);
            
            currentTime = 0;
            completedOrdersCount = 0;
            failedOrdersCount = 0;
        }

        public void Tick()
        {
            currentTime++;

            foreach (var order in _currentOrders)
            {
                order?.Tick();
            }
            
            timeChanged?.Invoke(this);
        }

        private bool CanCreateOrder()
        {
            if (isCompleted)
                return false;

            return !_ordersQueue.IsNullOrEmpty() && 
                   (_currentOrders.Count < entry.ordersCapacity || _currentOrders.FindIndex(model => model == null) != -1);
        }

        public bool TryCreateOrder()
        {
            if (!CanCreateOrder())
                return false;

            var settings = _ordersQueue.Dequeue();
            var order = LevelDataFactory.CreateOrder(settings);

            var index = -1;

            if (_currentOrders.Count < entry.ordersCapacity)
            {
                _currentOrders.Add(order);
                index = _currentOrders.Count - 1;
            }
            else
            {
                index = _currentOrders.FindIndex(model => model == null);
                if (index == -1)
                    return false;

                _currentOrders[index] = order;
            }
            
            orderCreated?.Invoke(index, order);
            return true;
        }

        private bool CanRemoveOrder(int index)
        {
            if (_currentOrders.IsNullOrEmpty() || _currentOrders.Count <= index)
                return false;

            var order = _currentOrders[index];
            return order != null && order.isCompleted;
        }

        public bool TryRemoveOrder(int index)
        {
            if (!CanRemoveOrder(index))
                return false;

            var order = _currentOrders[index];

            if (order.isExpired)
            {
                failedOrdersCount++;
            }
            else
            {
                completedOrdersCount++;
            }
            
            orderRemoved?.Invoke(index, order);

            _currentOrders[index] = null;
            
            return true;
        }

        public bool TryRemoveOrderRecipe(int orderIndex, string recipeId)
        {
            if (isCompleted)
                return false;
            
            if (_currentOrders.IsNullOrEmpty() || _currentOrders.Count <= orderIndex)
                return false;

            var order = _currentOrders[orderIndex];
            if (order == null)
                return false;

            return order.TryRemoveRecipe(recipeId);
        }
    }
}