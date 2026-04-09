using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ramsha.Common.Domain
{

    public interface IPageSpecification<T> : ISpecification<T>
    {
        PaginationParams PaginationParams { get; }
    }

    public interface IPageSpecification<T, TResult> : ISpecification<T, TResult>, IPageSpecification<T>
    {

    }

    public class PageSpecification<T> : Specification<T>, IPageSpecification<T>
    {
        public PageSpecification(PaginationParams paginationParams)
        {
            PaginationParams = paginationParams;
            Query
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize);
        }

        public PaginationParams PaginationParams { get; init; }
    }
    public class PageSpecification<T, TResult> : Specification<T, TResult>, IPageSpecification<T, TResult>
    {
        public PageSpecification(PaginationParams paginationParams)
        {
            PaginationParams = paginationParams;
            Query
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize);
        }

        public PaginationParams PaginationParams { get; init; }

    }


}