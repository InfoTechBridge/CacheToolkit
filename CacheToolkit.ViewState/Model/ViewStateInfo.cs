using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheToolkit.ViewState.Model
{
    [Serializable]
    public class ViewStateInfo
    {        
        public string Id { get; set; }
        public object Value { get; set; }
        public DateTimeOffset CreateTime { get; set; }
        public DateTimeOffset LastVisitTime { get; set; }

        public ViewStateInfo(object value)
        {
            this.Id = Guid.NewGuid().ToString().Replace("-", string.Empty);
            this.Value = value;
            this.CreateTime = LastVisitTime = DateTimeOffset.Now;
        }
    }
}
