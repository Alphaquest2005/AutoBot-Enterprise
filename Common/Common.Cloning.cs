using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity.Core.Objects;
using System.Data.Common;
using System.Data;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Reflection;
using System.Runtime.Serialization;

namespace InsightPresentationLayer
{
    public static partial class Common
    {

        /// <summary>
        /// Extension method to Enitity Object .Cloning 
        /// Cloning the Object .If required this need to be followed by CleatEntity Objects
        /// 
        /// </summary>
        /// <param name="source">Entity Object to be cloned </param>
        /// <returns></returns>
        /// 
        //public static T EntityClone<T>(this T source) where T:EntityObject  { 
        //    var obj = new System.Runtime.Serialization.DataContractSerializer(typeof(T)); 
        //    using (var stream = new System.IO.MemoryStream())
        //    {
        //        obj.WriteObject(stream, source);
        //        stream.Seek(0, System.IO.SeekOrigin.Begin);
        //        return (T)obj.ReadObject(stream); 
        //    } 
        //}

        public static T EntityClone<T>(T source) where T : EntityObject
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }

            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// Extension method of Entity Object .
        /// This will be used  to load all the Releated Child Objects using load (LazyLoad)
        /// 
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        /// 
        public static EntityObject LoadAllChild(this EntityObject source)
        {
            List<PropertyInfo> PropList = (from a in source.GetType().GetProperties()
                                           where a.PropertyType.Name == "EntityCollection`1"
                                           select a).ToList();
            foreach (PropertyInfo prop in PropList)
            {
                object instance = prop.GetValue(source, null);
                bool isLoad = (bool)instance.GetType().GetProperty("IsLoaded").GetValue(instance, null);
                if (!isLoad)
                {

                    MethodInfo mi = (from a in instance.GetType().GetMethods()
                                     where a.Name == "Load" && a.GetParameters().Length == 0
                                     select a).FirstOrDefault();

                    mi.Invoke(instance, null);
                }
            }
            return (EntityObject)source;
        }



        /// <summary>
        ///  Extension method of Entity Object This will clear the Entity of Object and all related CHild Objects 
        /// 
        /// </summary>
        /// <param name="source">Entity Object on which audit property will be applied</param>
        /// <param name="bcheckHierarchy">This Parameter will define to clear enitty of all Child Object or not to set for child objects</param>
        /// <param name="sModifiedby">This will be used setting Createdby/Modifiedby Attribute</param>
        /// is to be changed </param>
        /// <returns></returns>
        public static EntityObject ClearEntityReference(this  EntityObject source, bool bcheckHierarchy)
        {
            return source.ClearEntityObject(bcheckHierarchy);
        }

        static List<object> parentlst = new List<object>();
        private static T ClearEntityObject<T>(this  T source, bool bcheckHierarchy) //where T : class
        {
            if (source == null || parentlst.Contains(source) == true) return source;

            parentlst.Add(source);
            //Throw if passed object has nothing
            if (source == null) { throw new Exception("Null Object cannot be cloned"); }
            // get the TYpe of passed object 
            Type tObj = source.GetType();
            // check object Passed does not have entity key Attribute 
            PropertyInfo pi = tObj.GetProperty("EntityKey");
            if (pi != null)
            {
                try
                {
                    pi.SetValue(source, null, null);
                    
                }
                catch (Exception ex)
                {

                }

            }


            List<PropertyInfo> nPropList = (from a in source.GetType().GetProperties()
                                            where a.PropertyType.IsClass == true
                                                  && a.PropertyType.FullName.Contains("WaterNut.DataLayer") == true
                                                  && a.PropertyType.FullName.ToUpper().Contains("ENTITYCOLLECTION`1") == false
                                                  && a.PropertyType.FullName.Contains("EntityReference`1") == false
                                                  && a.PropertyType.FullName.Contains("System") == false
                                            select a).ToList();

            foreach (PropertyInfo prop in nPropList)
            {
                var val = prop.GetValue(source, null);

                val.ClearEntityObject(true);
                
            }


            //bcheckHierarchy this flag is used to check and clear child object releation keys 
            if (!bcheckHierarchy)
            {
                parentlst.Remove(source);
                return (T)source;
            }
            // Clearing the Entity for Child Objects 
            // Using the Linq get only Child Reference objects   from source object 
            List<PropertyInfo> PropList = (from a in source.GetType().GetProperties()
                                           where a.PropertyType.Name.ToUpper() == "ENTITYCOLLECTION`1"
                                           select a).ToList();

            // Loop thorough List of Child Object and Clear the Entity Reference 
            foreach (PropertyInfo prop in PropList)
            {


                IEnumerable keys = (IEnumerable)tObj.GetProperty(prop.Name).GetValue(source, null);

                foreach (object key in keys)
                {
                    //Clearing Entity Reference from Parnet Object

                    var ochildprop = (from a in key.GetType().GetProperties()
                                      where a.PropertyType.Name == "EntityReference`1"
                                      select a);//.SingleOrDefault();
                    if (ochildprop.Count() == 1)
                    {
                        ochildprop.First().GetValue(key, null).ClearEntityObject(true);
                    }
                    else
                    {
                        foreach (var item in ochildprop)
                        {
                          //  if(item != null && parentlst.Contains(item) == false)
                            item.GetValue(key, null).ClearEntityObject(true);
                        }
                    }
                    //Clearing the the Entity Reference from Child object .This will recrusive action
                    key.ClearEntityObject(true);
                }
            }
            parentlst.Remove(source);
            return (T)source;
        }


       

       

    }




}

    

