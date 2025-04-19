using System.ComponentModel.DataAnnotations.Schema;
using Core.Common.Data.Contracts;
//using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using TrackableEntities.Client;


namespace WaterNut.Client.DTO
{
   // [JsonObject(IsReference = true)]
    [DataContract(IsReference = true)]
    public abstract class BaseEntity<T> :EntityBase,  IIdentifiableEntity, ICreateEntityFromString<T> where T : IIdentifiableEntity
    {


        //[NotMapped]
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        //[IgnoreDataMember]
        [DataMember]
        public virtual string EntityId { get; set; }
        //[NotMapped]
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        //[IgnoreDataMember]
        [DataMember]
        public virtual string EntityName
        {
            get
            {
                return EntityId.ToString();
            }
            set
            {
               // throw new NotImplementedException();
            }
        }

        public virtual void UpdateEntityName(object sender, PropertyChangedEventArgs e)
        {

        }
        public virtual T CreateEntityFromString(string value)
        {
            throw new NotImplementedException();
        }


        //event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged { add { _propertyChanged += value; } remove { _propertyChanged -= value; } }
        //event PropertyChangedEventHandler _propertyChanged;

        //public void NotifyPropertyChanged(string PropertyName)
        //{
        //    _propertyChanged(null, new PropertyChangedEventArgs(PropertyName));
        //}


       
    }
}
