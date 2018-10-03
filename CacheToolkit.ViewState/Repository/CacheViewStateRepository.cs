using CacheToolkit.ViewState.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace CacheToolkit.ViewState.Repository
{
    public class CacheViewStateRepository : IViewStateRepository
    {
        ObjectCache Cache;
        private static CacheViewStateRepository DefaultInstance;
        public CacheViewStateRepository()
            : this(MemoryCache.Default)
        {

        }
        public CacheViewStateRepository(ObjectCache cache)
        {
            this.Cache = cache;
        }
        public static CacheViewStateRepository Default
        {
            get
            {
                if (DefaultInstance == null)
                    DefaultInstance = new CacheViewStateRepository();

                return DefaultInstance;
            }
        }
        public ViewStateInfo GetViewState(string id)
        {
            return (ViewStateInfo)Cache.Get(id);
        }
        public string SaveViewState(ViewStateInfo info)
        {
            Cache.Set(info.Id, info, null);
            return info.Id;
        }
    }
}
