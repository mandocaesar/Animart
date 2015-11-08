using Abp.Domain.Entities;
using Abp.EntityFramework;
using Abp.EntityFramework.Repositories;

namespace Animart.Portal.EntityFramework.Repositories
{
    public abstract class PortalRepositoryBase<TEntity, TPrimaryKey> : EfRepositoryBase<PortalDbContext, TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        protected PortalRepositoryBase(IDbContextProvider<PortalDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //add common methods for all repositories
    }

    public abstract class PortalRepositoryBase<TEntity> : PortalRepositoryBase<TEntity, int>
        where TEntity : class, IEntity<int>
    {
        protected PortalRepositoryBase(IDbContextProvider<PortalDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //do not add any method here, add to the class above (since this inherits it)
    }
}
