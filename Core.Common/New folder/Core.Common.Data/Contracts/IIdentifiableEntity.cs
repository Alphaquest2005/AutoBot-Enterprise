using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Data.Contracts
{
    public interface IIdentifiableEntity
    {
         string EntityId { get; set; }
         string EntityName { get; set; }
    }
}
