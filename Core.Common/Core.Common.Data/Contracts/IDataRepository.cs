using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrackableEntities;


namespace Core.Common.Contracts
{
    public interface IDataRepository
    {
        bool SaveChanges<T>(T obj) where T : ITrackable;
        void Delete(ITrackable obj);
    }
}
