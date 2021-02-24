using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Extensions
{
    public class BetterExpando : DynamicObject, IDictionary<string, object>
    {
        private Dictionary<string, object> _dict;
        private bool _ignoreCase;
        private bool _returnEmptyStringForMissingProperties;

        /// <summary>
        /// Creates a BetterExpando object/
        /// </summary>
        /// <param name="ignoreCase">Don't be strict about property name casing.</param>
        /// <param name="returnEmptyStringForMissingProperties">If true, returns String.Empty for missing properties.</param>
        /// <param name="root">An ExpandoObject to consume and expose.</param>
        public BetterExpando(bool ignoreCase = false,
          bool returnEmptyStringForMissingProperties = true,
          ExpandoObject root = null)
        {
            if (root == null) root = new ExpandoObject();
            _dict = new Dictionary<string, object>();
            _ignoreCase = ignoreCase;
            _returnEmptyStringForMissingProperties = returnEmptyStringForMissingProperties;
            if (root != null)
            {
                Augment(root);
            }
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            UpdateDictionary(binder.Name, value);
            return true;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            if (indexes[0] is string)
            {
                var key = indexes[0] as string;
                UpdateDictionary(NormalisePropertyName(key), value);
                return true;
            }
            else
            {
                return base.TrySetIndex(binder, indexes, value);
            }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var key = NormalisePropertyName(binder.Name);
            if (_dict.ContainsKey(key))
            {
                result = _dict[key];
                return true;
            }
            if (_returnEmptyStringForMissingProperties)
            {
                result = null;
                return true;
            }
            return base.TryGetMember(binder, out result);
        }

        /// <summary>
        /// Combine two instances together to get a union.
        /// </summary>
        /// <returns>This instance but with additional properties</returns>
        /// <remarks>Existing properties are not overwritten.</remarks>
        public dynamic Augment(BetterExpando obj)
        {
            obj._dict
              .Where(pair => !_dict.ContainsKey(NormalisePropertyName(pair.Key)))
              .ToList()
              .ForEach(pair => UpdateDictionary(pair.Key, pair.Value));
            return this;
        }

        public dynamic Augment(ExpandoObject obj)
        {
            ((IDictionary<string, object>)obj)
              .Where(pair => !_dict.ContainsKey(NormalisePropertyName(pair.Key)))
              .ToList()
              .ForEach(pair => UpdateDictionary(pair.Key, pair.Value));
            return this;
        }

        public T ValueOrDefault<T>(string propertyName, T defaultValue)
        {
            propertyName = NormalisePropertyName(propertyName);
            return _dict.ContainsKey(propertyName)
              ? (T)_dict[propertyName]
              : defaultValue;
        }

        /// <summary>
        /// Check if BetterExpando contains a property.
        /// </summary>
        /// <remarks>Respects the case sensitivity setting</remarks>
        public bool HasProperty(string name)
        {
            return _dict.ContainsKey(NormalisePropertyName(name));
        }

        /// <summary>
        /// Returns this object as comma-separated name-value pairs.
        /// </summary>
        public override string ToString()
        {
            return String.Join(", ", _dict.Select(pair => pair.Key + " = " + pair.Value ?? "(null)").ToArray());
        }

        private void UpdateDictionary(string name, object value)
        {
            var key = NormalisePropertyName(name);
            if (_dict.ContainsKey(key))
            {
                _dict[key] = value;
            }
            else
            {
                _dict.Add(key, value);
            }
        }

        private string NormalisePropertyName(string propertyName)
        {
            return _ignoreCase ? propertyName.ToLower() : propertyName;
        }


        #region IDictionary<string,object> members
        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            return _dict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_dict).GetEnumerator();
        }

        void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
        {
            ((ICollection<KeyValuePair<string, object>>)_dict).Add(item);
        }

        void ICollection<KeyValuePair<string, object>>.Clear()
        {
            _dict.Clear();
        }

        bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
        {
            return ((ICollection<KeyValuePair<string, object>>)_dict).Contains(item);
        }

        void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, object>>)_dict).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
        {
            return ((ICollection<KeyValuePair<string, object>>)_dict).Remove(item);
        }

        int ICollection<KeyValuePair<string, object>>.Count
        {
            get { return _dict.Count; }
        }

        bool ICollection<KeyValuePair<string, object>>.IsReadOnly
        {
            get { return ((ICollection<KeyValuePair<string, object>>)_dict).IsReadOnly; }
        }

        bool IDictionary<string, object>.ContainsKey(string key)
        {
            return _dict.ContainsKey(key);
        }

        void IDictionary<string, object>.Add(string key, object value)
        {
            _dict.Add(key, value);
        }

        bool IDictionary<string, object>.Remove(string key)
        {
            return _dict.Remove(key);
        }

        bool IDictionary<string, object>.TryGetValue(string key, out object value)
        {
            return _dict.TryGetValue(key, out value);
        }

        object IDictionary<string, object>.this[string key]
        {
            get
            {
                try
                {
                   
                    return  _dict.ContainsKey(key)? _dict[key] : null;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            set
            {
                try
                {
                    if(_dict.ContainsKey(key))
                    _dict[key] = value;
                    else
                    {
                        _dict.Add(key, value);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        ICollection<string> IDictionary<string, object>.Keys
        {
            get { return _dict.Keys; }
        }

        ICollection<object> IDictionary<string, object>.Values
        {
            get { return _dict.Values; }
        }
        #endregion

    }

    //    1: /// <summary>
    //    2: /// Extension method that turns a dictionary of string and object to an ExpandoObject
    //    3: /// </summary>
    //    4: public static ExpandoObject ToExpando(this IDictionary<string, object> dictionary)
    //    5: {
    //    6:     var expando = new ExpandoObject();
    //    7:     var expandoDic = (IDictionary<string, object>)expando;
    //    8:
    //    9:     // go through the items in the dictionary and copy over the key value pairs)
    //    10:     foreach (var kvp in dictionary)
    //    11:     {
    //    12:         // if the value can also be turned into an ExpandoObject, then do it!
    //    13:         if (kvp.Value is IDictionary<string, object>)
    //    14:         {
    //    15:             var expandoValue = ((IDictionary<string, object>)kvp.Value).ToExpando();
    //    16:             expandoDic.Add(kvp.Key, expandoValue);
    //    17:         }
    //18:         else if (kvp.Value is ICollection)
    //19:         {
    //20:             // iterate through the collection and convert any strin-object dictionaries
    //21:             // along the way into expando objects
    //22:             var itemList = new List<object>();
    //23:             foreach (var item in (ICollection) kvp.Value)
    //24:             {
    //25:                 if (item is IDictionary<string, object>)
    //26:                 {
    //27:                     var expandoItem = ((IDictionary<string, object>)item).ToExpando();
    //28:                     itemList.Add(expandoItem);
    //29:                 }
    //30:                 else
    //31:                 {
    //32:                     itemList.Add(item);
    //33:                 }
    //34:             }
    //35:
    //36:             expandoDic.Add(kvp.Key, itemList);
    //37:         }
    //38:         else
    //39:         {
    //40:             expandoDic.Add(kvp);
    //41:         }
    //42:     }
    //43:
    //44:     return expando;
    //45: }
}
