using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Data.Contracts;

namespace Core.Common.Data
{
    public class DataCache<T> where T : class, IIdentifiableEntity, new ()
    {
        private ConcurrentDictionary<string,T> _innerData;
        //private Func<T, T> _cloner;

        public DataCache(IEnumerable<T> source)//, Func<T, T> cloner
        {
            _innerData = new ConcurrentDictionary<string, T>();

            if (source != null)
            {
                source.AsParallel(new ParallelLinqOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }).ForAll(i => { _innerData.AddOrUpdate(i.EntityId, i, (key, oldValue) => i); });
            }
            
          //  _cloner = cloner;
        }

        public T GetSingle(Func<T, bool> predicate)
        {
            try
            {
             if(predicate == null) return null;
             //lock (_innerData)
             //           {
                            return _innerData.Values
                                        .Where(predicate)
                                        //.Select(s => _cloner(s))
                                        .SingleOrDefault();
                        //}
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public void AddItem(T entity)
        {
            //lock (_innerData)
            //{
            if (entity != null) _innerData.AddOrUpdate(entity.EntityId, entity, (key, oldValue) => entity);
            //}
        }

        public List<T> Data
        {
            get { return _innerData.Values.ToList(); }
        }
    }
}
