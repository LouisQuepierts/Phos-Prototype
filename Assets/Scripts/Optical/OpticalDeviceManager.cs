using System.Collections.Generic;
using UnityEngine;

namespace Phos.Optical {
    public class OpticalDeviceManager : MonoBehaviour {
        private static OpticalDeviceManager _instance;
        
        private readonly List<OpticalDevice> _devices = new();

        public static OpticalDeviceManager GetInstance() {
            return _instance ??= Create();
        }

        private static OpticalDeviceManager Create() {
            var obj = new GameObject("OpticalDeviceManager");
            return obj.AddComponent<OpticalDeviceManager>();
        }
        
        public OpticalDeviceManager() {
            _instance = this;
        }
        
        public void Add(OpticalDevice device) {
            _devices.Add(device);
        }
        
        public void Remove(OpticalDevice device) {
            _devices.Remove(device);
        }
    }
}