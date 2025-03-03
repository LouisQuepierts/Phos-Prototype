namespace Phos.Predicate {
	public interface IPredicate {
        public abstract bool Evaluate(bool source = false);
    }
}