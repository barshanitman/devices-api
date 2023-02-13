using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Vnext.Function
{

    public interface IGenericRepo<TEntity>
    {

        Task<IEnumerable<TEntity>> GetAll();

        Task<TEntity> GetById(int Id);

        TEntity Add(TEntity entity);

        IEnumerable<TEntity> AddMultiple(IEnumerable<TEntity> entities);

        TEntity UpdateEntity(TEntity entity);

        void SaveChanges();

        Task<TEntity> FindByCondition(Expression<Func<TEntity, bool>> predicate);
        Task<bool> RemoveEntity(int id);

        public bool EntityExists(int Id);


    }



}