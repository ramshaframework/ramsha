using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ramsha.Common.Domain
{
    public interface ISpecificationValidator
    {
        bool IsValid<T>(T entity, ISpecification<T> specification);
    }
}