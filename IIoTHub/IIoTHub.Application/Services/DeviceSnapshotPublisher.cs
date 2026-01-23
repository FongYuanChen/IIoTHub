using IIoTHub.Application.Interfaces;
using IIoTHub.Domain.Models.DeviceSnapshots;
using System.Collections.Concurrent;

namespace IIoTHub.Application.Services
{
    public class DeviceSnapshotPublisher : IDeviceSnapshotPublisher
    {
        private readonly ConcurrentDictionary<Guid, Subscription> _subscriptions = new();

        /// <summary>
        /// 發佈指定設備的快照更新
        /// </summary>
        /// <typeparam name="TSnapshot"></typeparam>
        /// <param name="deviceId"></param>
        /// <param name="snapshot"></param>
        public void Publish<TSnapshot>(Guid deviceId, TSnapshot snapshot) where TSnapshot : DeviceSnapshot
        {
            if (!_subscriptions.TryGetValue(deviceId, out var subscription))
                return;

            Action<TSnapshot>[] snapshotHandlers;
            lock (subscription.SyncRoot)
            {
                snapshotHandlers = subscription.Handlers
                    .OfType<Action<TSnapshot>>()
                    .ToArray();
            }

            foreach (var handler in snapshotHandlers)
            {
                handler(snapshot);
            }
        }

        /// <summary>
        /// 訂閱指定設備的快照更新
        /// </summary>
        /// <typeparam name="TSnapshot"></typeparam>
        /// <param name="deviceId"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public IDisposable Subscribe<TSnapshot>(Guid deviceId, Action<TSnapshot> handler) where TSnapshot : DeviceSnapshot
        {
            var subscription = _subscriptions.GetOrAdd(deviceId, _ => new Subscription());

            lock (subscription.SyncRoot)
            {
                subscription.Handlers.Add(handler);
            }

            return new SubscriptionDisposable(() =>
            {
                lock (subscription.SyncRoot)
                {
                    subscription.Handlers.Remove(handler);
                }
            });
        }

        /// <summary>
        /// 訂閱訊息
        /// </summary>
        private class Subscription
        {
            /// <summary>
            /// 鎖物件
            /// </summary>
            public readonly object SyncRoot = new();

            /// <summary>
            /// 訂閱者回調列表
            /// </summary>
            public readonly List<Delegate> Handlers = new();
        }

        /// <summary>
        /// 用於管理訂閱取消的 IDisposable 實作
        /// </summary>
        private class SubscriptionDisposable : IDisposable
        {
            private readonly Action _unsubscribe;

            public SubscriptionDisposable(Action unsubscribe)
            {
                _unsubscribe = unsubscribe;
            }

            public void Dispose() => _unsubscribe();
        }
    }
}
