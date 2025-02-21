using Phos.Interact;
using Phos.Structure;
using Phos.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Phos.Predicate {
	[Serializable]
	public class PredicateHolder : ISerializationCallbackReceiver {
        public static readonly InstanceFactory<PredicateType, IPredicate> Factory;

        [SerializeReference]
        public IPredicate instance = new TogglePredicate();
        public PredicateType type;

        [SerializeField]
        private string _json;

        public void OnAfterDeserialize() {
            instance = Factory.GetInstance(type);
            JsonUtility.FromJsonOverwrite(_json, instance);
        }

        public void OnBeforeSerialize() {
            _json = JsonUtility.ToJson(instance);
            instance = null;
        }

        static PredicateHolder() {
            Factory = new InstanceFactory<PredicateType, IPredicate>(() => new TogglePredicate(), Enum.GetValues(typeof(PredicateType)).Length);
            Factory.Register(PredicateType.Toggle, () => new TogglePredicate());
            Factory.Register(PredicateType.Structure, () => new StructurePredicate());
            Factory.Register(PredicateType.Interact, () => new InteractionPredicate());
            Factory.Register(PredicateType.Position, () => new TogglePredicate());
            Factory.Register(PredicateType.Logic, () => new LogicPredicate());
            Factory.Freeze();
        }
    }

    [Serializable]
    public enum PredicateType {
        Toggle,
        Structure,
        Interact,
        Position,
        Logic
    }
}