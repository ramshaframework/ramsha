namespace Ramsha.Common.Domain
{
    public interface ICacheSpecificationBuilder<T, TResult>
        : ISpecificationBuilder<T, TResult>, ICacheSpecificationBuilder<T>
    {
    }

    public interface ICacheSpecificationBuilder<T>
        : ISpecificationBuilder<T>
    {
    }

    public interface IOrderedSpecificationBuilder<T, TResult>
        : ISpecificationBuilder<T, TResult>, IOrderedSpecificationBuilder<T>
    {
    }

    public interface IOrderedSpecificationBuilder<T>
        : ISpecificationBuilder<T>
    {
    }

    public interface ISpecificationBuilder<T, TResult>
        : ISpecificationBuilder<T>
    {
        new Specification<T, TResult> Specification { get; }
    }

    public interface ISpecificationBuilder<T>
    {
        Specification<T> Specification { get; }
    }




    public interface IIncludableSpecificationBuilder<T, TResult, out TProperty>
       : ISpecificationBuilder<T, TResult>, IIncludableSpecificationBuilder<T, TProperty> where T : class
    {
    }

    public interface IIncludableSpecificationBuilder<T, out TProperty>
       : ISpecificationBuilder<T> where T : class
    {
    }


}