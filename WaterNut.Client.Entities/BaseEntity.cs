//using Core.Common;
//using Core.Common.Data.Contracts;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Runtime.CompilerServices;
//using System.Text;
//using System.Threading.Tasks;
//using TrackableEntities.Client;


//namespace WaterNut.Client.Entities
//{
//    public abstract class BaseEntity<T> : EntityBase, IIdentifiableEntity, ICreateEntityFromString<T> where T : IIdentifiableEntity
//    {



//        public virtual string EntityId
//        {
//            get
//            {
//                throw new NotImplementedException();
//            }
//            set
//            {
//                throw new NotImplementedException();
//            }
//        }

//        public virtual string EntityName
//        {
//            get
//            {
//                return EntityId.ToString();
//            }
//            set
//            {
//                //throw new NotImplementedException();
//            }
//        }

//        public virtual T CreateEntityFromString(string value)
//        {
//            throw new NotImplementedException();
//        }

//        //public static Boolean DoStaticEvents = true;
//        //public static event PropertyChangedEventHandler staticPropertyChanged;
//        //public static void OnStaticPropertyChanged(String info)
//        //{

//        //    if (staticPropertyChanged != null && DoStaticEvents == true)
//        //    {
//        //        staticPropertyChanged(null, new PropertyChangedEventArgs(info));
//        //    }
//        //}

//        //public static void OnStaticPropertyChanged<T>(Expression<Func<T>> selectorExpression)
//        //{
//        //    if (selectorExpression == null)
//        //        throw new ArgumentNullException("selectorExpression");
//        //    var body = selectorExpression.Body as MemberExpression;
//        //    if (body == null)
//        //        throw new ArgumentException("The body must be a member expression");
//        //    OnStaticPropertyChanged(body.Member.Name);
//        //}


//        //event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged { add { PropertyChanged += value; } remove { PropertyChanged -= value; } }
//        public event  PropertyChangedEventHandler MyPropertyChanged;

//        //public void NotifyPropertyChanged(string PropertyName)
//        //{
//        //    _propertyChanged(null, new PropertyChangedEventArgs(PropertyName));
//        //}

//        /// <summary>
//        ///     Notifies listeners that a property value has changed.
//        /// </summary>
//        /// <param name="propertyName">
//        ///     Name of the property used to notify listeners.  This
//        ///     value is optional and can be provided automatically when invoked from compilers
//        ///     that support <see cref="CallerMemberNameAttribute" />.
//        /// </param>


//        protected void NotifyPropertyChanged<T>(Expression<Func<T>> selectorExpression)
//        {
//            if (selectorExpression == null)
//                throw new ArgumentNullException("selectorExpression");
//            var body = selectorExpression.Body as MemberExpression;
//            if (body == null)
//                throw new ArgumentException("The body must be a member expression");
//            NotifyPropertyChanged(body.Member.Name);
//        }

//        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
//        {
//            var eventHandler = MyPropertyChanged;
//            if (eventHandler != null)
//            {
//                eventHandler(this, new PropertyChangedEventArgs(propertyName));
//            }
//        }
//    }
//}
