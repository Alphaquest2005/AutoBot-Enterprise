using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Data
{
    public static class DbContextExtentions
    {
        public static IQueryable<TResult> CreateNavigationQuery<TSource, TResult>(this DbContext ctx, TSource entity, Expression<Func<TSource, TResult>> navigationPath)
            where TSource : class,IEntityWithKey
        {
            return CreateNavigationQuery<TResult,TSource>(ctx, entity, (MemberExpression)navigationPath.Body);
        }

        public static IQueryable<TResult> CreateNavigationQuery<TSource, TResult>(this DbContext ctx, TSource entity, Expression<Func<TSource, ICollection<TResult>>> navigationPath) 
            where TSource : class, IEntityWithKey
        {
            return CreateNavigationQuery<TResult,TSource>(ctx, entity, (MemberExpression)navigationPath.Body);
        }

        public static IQueryable<TResult> CreateNavigationQuery<TContext, TSource, TResult>(TSource entity, Expression<Func<TSource, ICollection<TResult>>> navigationPath)
            where TContext : DbContext, new()
            where TSource : class, IEntityWithKey
        {
            var ctx = new TContext();
            return CreateNavigationQuery<TResult,TSource>(ctx, entity, (MemberExpression)navigationPath.Body);
        }

        public static IQueryable<TResult> CreateNavigationQuery<TResult, TSource>(this DbContext ctx, TSource entity, MemberExpression navigationPath) where TSource:class,IEntityWithKey
        {
            var navigationName = navigationPath.Member.Name;

            //var ts = ctx.Set<TSource>().Create();
            //var rs = ctx.Set<TSource>().Attach(entity);
            
            
           var ose = ((IObjectContextAdapter)ctx).ObjectContext.ObjectStateManager.GetObjectStateEntry(entity);

           var rm = ((IObjectContextAdapter)ctx).ObjectContext.ObjectStateManager.GetRelationshipManager(entity);

            var entityType = (EntityType)ose.EntitySet.ElementType;
            var navigation = entityType.NavigationProperties[navigationName];

                var relatedEnd = rm.GetRelatedEnd(navigation.RelationshipType.FullName, navigation.ToEndMember.Name);

                return ((dynamic)relatedEnd).CreateSourceQuery();
        }
    }
}