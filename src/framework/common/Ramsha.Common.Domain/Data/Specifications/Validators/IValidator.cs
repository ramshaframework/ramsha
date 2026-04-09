namespace Ramsha.Common.Domain
{
    public interface IValidator
    {
        bool IsValid<T>(T entity, ISpecification<T> specification);
    }
}