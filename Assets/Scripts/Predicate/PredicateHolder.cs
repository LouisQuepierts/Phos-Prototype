using Phos.Utils;
using System;
using UnityEngine;

namespace Phos.Predicate {
	[Serializable]
	public class PredicateHolder {
        public LogicalOperator @operator;

        private BasePredicate[] predicates;
        private Status _status = Status.None;

        public void Init(GameObject parent) {
            predicates = parent.GetComponents<BasePredicate>();
        }

        public bool Evaluate(bool source = false) {
            switch (_status) {
                case Status.None:
                    _status = predicates.Length == 0 ? Status.Pass : Status.Available;
                    break;
                case Status.Pass:
                    return source;
            }

            return @operator switch {
                LogicalOperator.And => EvaluateAnd(),
                LogicalOperator.Or => EvaluateOr(),
                LogicalOperator.Nand => !EvaluateAnd(),
                LogicalOperator.Nor => !EvaluateOr(),
                _ => false,
            };
        }

        private bool EvaluateOr() {
            return Array.TrueForAll(predicates, predicate => predicate.Evaluate());
        }

        private bool EvaluateAnd() {
            return Array.Exists(predicates, predicate => predicate.Evaluate());
        }

        private enum Status {
            None,
            Available,
            Pass
        }
    }
}