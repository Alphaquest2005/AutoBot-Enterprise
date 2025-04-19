using Core.Common.Data.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Data
{
    public abstract class BaseEntity<T> : IIdentifiableEntity, ICreateEntityFromString<T> where T : IIdentifiableEntity
    {
        
        
       
        public virtual string EntityId
        {
            get
            {
throw new NotImplementedException();
            }
            set
            {
throw new NotImplementedException();
            }
        }

        public virtual string EntityName
        {
            get
            {
throw new NotImplementedException();
            }
            set
            {
throw new NotImplementedException();
            }
        }

        public virtual T CreateEntityFromString(string value)
        {
            throw new NotImplementedException();
        }

        public static Boolean DoStaticEvents = true;
        public static event PropertyChangedEventHandler staticPropertyChanged;
        public static void OnStaticPropertyChanged(String info)
        {

            if (staticPropertyChanged != null && DoStaticEvents == true)
            {
staticPropertyChanged(null, new PropertyChangedEventArgs(info));
            }
        }

        public static void OnStaticPropertyChanged<T>(Expression<Func<T>> selectorExpression)
        {
            if (selectorExpression == null)
throw new ArgumentNullException("selectorExpression");
            MemberExpression body = selectorExpression.Body as MemberExpression;
            if (body == null)
throw new ArgumentException("The body must be a member expression");
            OnStaticPropertyChanged(body.Member.Name);
        }

    }
}
