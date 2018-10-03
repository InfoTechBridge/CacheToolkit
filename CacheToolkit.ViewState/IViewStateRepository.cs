using CacheToolkit.ViewState.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheToolkit.ViewState
{
    public interface IViewStateRepository
    {
        ViewStateInfo GetViewState(string id);
        string SaveViewState(ViewStateInfo info);
    }
}
