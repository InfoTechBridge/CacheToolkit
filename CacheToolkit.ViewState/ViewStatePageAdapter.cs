using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.Adapters;

namespace CacheToolkit.ViewState
{
    public class ViewStatePageAdapter : PageAdapter
    {
        public override PageStatePersister GetStatePersister()
        {
            //return new CachePageStatePersister(this.Page);
            return new ViewStatePersister(this.Page) { UseDefaultViewStateHiddenField = true };
            //return new SessionPageStatePersister(this.Page);
            //return base.GetStatePersister();
        }

    }
}
