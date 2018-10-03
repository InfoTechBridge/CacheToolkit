using CacheToolkit.ViewState.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace CacheToolkit.ViewState
{
    public class ViewStateManager
    {
        //private IViewStateRepository Repository;
        private static ViewStateManager DefaultInstance;
        public ViewStateManager()
        {

        }
        public static ViewStateManager Default
        {
            get
            {
                if (DefaultInstance == null)
                    DefaultInstance = new ViewStateManager();

                return DefaultInstance;
            }
        }
                
        public void Load(Page page)
        {
            
        }
        public void Save(Page page)
        {
           
        }
    }
}
