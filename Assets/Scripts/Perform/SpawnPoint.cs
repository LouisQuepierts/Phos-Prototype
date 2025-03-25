using System;
using Phos.Navigate;
using Phos.Operation;
using UnityEngine;

namespace Phos.Perform {
    public class SpawnPoint : MonoBehaviour {
        public GameObject operationGroup;

        public void Spawn(PlayerController player) {
            player.Spawn(GetPosition());
            BaseBiOperation[] operations = (operationGroup ? operationGroup : gameObject).GetComponents<BaseBiOperation>();
            foreach (var operation in operations) {
                operation.Execute(true);
            }
        }

        private Vector3 GetPosition() {
            if (TryGetComponent(out NavigateNode node)) {
                Debug.Log($"SpawnPoint: {node}");
                return node.GetNodePosition();
            }

            return transform.position;
        }
    }
}