using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CacheToolkit.ViewState.Model;

namespace CacheToolkit.ViewState.Repository
{
    public class SqlViewStateRepository : IViewStateRepository
    {
        public ViewStateInfo GetViewState(string id)
        {
            throw new NotImplementedException();
        }

        public string SaveViewState(ViewStateInfo info)
        {
            throw new NotImplementedException();
        }
    }
}
