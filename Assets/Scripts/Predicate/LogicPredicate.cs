using System;
using UnityEngine;

namespace Phos.Predicate {
    [CreateAssetMenu(fileName = "LogicPredicate", menuName = "Predicate/LogicPredicate")]
	public class LogicPredicate : BasePredicate {
        [InspectorName("Operator")]
        public Operator opr;

        public BasePredicate[] predicates;

        public override bool Evaluate() {
            return opr switch {
                Operator.AND => EvaluateAnd(),
                Operator.OR => EvaluateOr(),
                Operator.NOT => !EvaluateAnd(),
                _ => false
            };
        }

        private bool EvaluateOr() {
            return predicates.Length == 0 || Array.Exists(predicates, predicate => predicate.Evaluate());
        }

        private bool EvaluateAnd() {
            return predicates.Length == 0 || Array.TrueForAll(predicates, predicate => predicate.Evaluate());
        }
    }

    public enum Operator {
        AND, OR, NOT
    }
}