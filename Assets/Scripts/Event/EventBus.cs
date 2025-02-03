using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Phos.Event {
    public interface IEvent { };
    public class EventBus : MonoBehaviour {
        private static EventBus m_instance;
        private readonly Dictionary<Type, Action<IEvent>> m_listeners = new();
        private readonly Queue<IEvent> m_events = new();

        public static EventBus Instance {
            get {
                if (m_instance == null) {
                    m_instance = FindFirstObjectByType<EventBus>();

                    if (m_instance == null) {
                        GameObject go = new GameObject("EventBus");
                        m_instance = go.AddComponent<EventBus>();
                    }
                }
                return m_instance;
            }
        }

        private void FixedUpdate() {
            while (m_events.Count > 0) {
                IEvent e = m_events.Dequeue();
                Type type = e.GetType();
                if (m_listeners.TryGetValue(type, out Action<IEvent> action)) {
                    action?.Invoke(e);
                }
            }
        }

        /// <summary>
        /// Subscribe to an event
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        public void Subscribe<T>(Action<IEvent> action) where T : IEvent {
            Type type = typeof(T);

            lock (m_listeners) {
                if (!m_listeners.ContainsKey(type)) {
                    m_listeners[type] = action;
                } else {
                    m_listeners[type] += action;
                }
            }
        }

        /// <summary>
        /// Unsubscribe from an event
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        public void Unsubscribe<T>(Action<IEvent> action) where T : IEvent {
            Type type = typeof(T);

            lock (m_listeners) {
                if (m_listeners.ContainsKey(type)) {
                    m_listeners[type] -= action;

                    if (m_listeners[type] == null) {
                        m_listeners.Remove(type);
                    }
                }
            }
        }

        /// <summary>
        /// Publish an event to the event bus
        /// Event will be processed in the next physical update
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        public void Publish<T>(T e) where T : IEvent {
            lock (m_events) {
                m_events.Enqueue(e);
            }
        }
    }
}
