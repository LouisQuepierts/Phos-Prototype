using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Phos.Optical {
    [ExecuteAlways]
    public class OpticalDeviceManager : MonoBehaviour {
        public bool debug;
        public int debugTickRate = 20;
        
        private static OpticalDeviceManager _instance;
        
        private readonly HashSet<OpticalDevice> _devices = new();
        private int tick = 0;

        public static OpticalDeviceManager GetInstance() {
            return _instance ??= Create();
        }

        private static OpticalDeviceManager Create() {
            var obj = FindFirstObjectByType<OpticalDeviceManager>() ?? new GameObject("OpticalDeviceManager").AddComponent<OpticalDeviceManager>();
            return obj;
        }
        
        public OpticalDeviceManager() {
            _instance = this;
        }
        
        public void Add(OpticalDevice device) {
            _devices.Add(device);
            OpticalUpdate();
        }
        
        public void Remove(OpticalDevice device) {
            _devices.Remove(device);
            OpticalUpdate();
        }

        private void FixedUpdate() {
            if (!Application.isPlaying) {
                var shouldUpdate = _devices.Aggregate(false, (current, device) => current | device.CheckChanged());

                if (!shouldUpdate) return;
            }

            OpticalUpdate();
        }

        private void OpticalUpdate() {
            // Update
            foreach (var device in _devices) {
                device.OpticalUpdate();
            }
            
            // Last Update
            foreach (var device in _devices) {
                device.LateOpticalUpdate();
            }
        }

        private void OnDestroy() {
            _instance = null;
        }

#if UNITY_EDITOR
        private void Update() {
            if (!debug || Application.isPlaying) return;
            tick++;
            if (tick < debugTickRate) return;
            tick = 0;
            FixedUpdate();
        }
#endif
    }
}