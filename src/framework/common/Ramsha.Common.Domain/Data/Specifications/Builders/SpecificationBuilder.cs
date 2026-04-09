namespace Ramsha.Common.Domain
{
    internal class SpecificationBuilder<T>
        : ICacheSpecificationBuilder<T>, IOrderedSpecificationBuilder<T>, ISpecificationBuilder<T>
    {
        public Specification<T> Specification { get; }

        public SpecificationBuilder(Specification<T> specification)
        {
            Specification = specification;
        }
    }

    internal class SpecificationBuilder<T, TResult> : SpecificationBuilder<T>,
    ICacheSpecificationBuilder<T, TResult>, IOrderedSpecificationBuilder<T, TResult>, ISpecificationBuilder<T, TResult>
    {
        public new Specification<T, TResult> Specification { get; }

        public SpecificationBuilder(Specification<T, TResult> specification)
            : base(specification)
        {
            Specification = specification;
        }
    }

    internal class IncludableSpecificationBuilder<T, TResult, TProperty>
       : SpecificationBuilder<T, TResult>, IIncludableSpecificationBuilder<T, TResult, TProperty> where T : class
    {
        public IncludableSpecificationBuilder(Specification<T, TResult> specification) : base(specification)
        {
        }
    }

    internal class IncludableSpecificationBuilder<T, TProperty>
        : SpecificationBuilder<T>, IIncludableSpecificationBuilder<T, TProperty> where T : class
    {
        public IncludableSpecificationBuilder(Specification<T> specification) : base(specification)
        {
        }
    }
}