using Core.Interfaces;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class BaseSpecification<T> : ISpecification<T> where T : class
    {
        private readonly Expression<Func<T, bool>> _criteria;

        public BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            _criteria = criteria;
        }
        public Expression<Func<T, bool>> Criteria => _criteria;
    }
}
