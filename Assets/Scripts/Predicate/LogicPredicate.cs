using System;
using UnityEngine;

namespace Phos.Predicate {
    [Serializable]
	public class LogicPredicate : IPredicate {
        public Operator opr;
        public PredicateHolder[] predicates;

        public bool Evaluate() {
            return opr switch {
                Operator.AND => EvaluateAnd(),
                Operator.OR => EvaluateOr(),
                Operator.NOT => !EvaluateAnd(),
                _ => false
            };
        }

        private bool EvaluateOr() {
            return predicates.Length == 0 || Array.Exists(predicates, predicate => predicate.instance.Evaluate());
        }

        private bool EvaluateAnd() {
            return predicates.Length == 0 || Array.TrueForAll(predicates, predicate => predicate.instance.Evaluate());
        }
    }

    public enum Operator {
        AND, OR, NOT
    }
}