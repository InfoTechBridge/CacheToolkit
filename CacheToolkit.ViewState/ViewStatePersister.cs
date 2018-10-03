using CacheToolkit.ViewState.Model;
using CacheToolkit.ViewState.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace CacheToolkit.ViewState
{
    //[AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class ViewStatePersister : PageStatePersister
    {
        private const string ViewStateHiddenFieldName = "__VIEWSTATEKEY"; // Custom hidden field

        private IViewStateRepository Repository;
        public bool UseDefaultViewStateHiddenField { get; set; } // __VIEWSTATE hidden field

        public ViewStatePersister(Page page)
            : base(page)
        {
            //this.Repository = CacheViewStateRepository.Default;
            this.Repository = new FileViewStateRepository(@"D:\MyProjects\SmsPanel\CacheToolkit.ViewState.WebUI\App_Data\ViewState");
        }

        public ViewStatePersister(Page page, IViewStateRepository repository)
            : base(page)
        {
            this.Repository = repository;
        }
                
        //
        // Summary:
        //     Deserializes and loads persisted state from the repository when
        //     a System.Web.UI.Page object initializes its control hierarchy.
        //
        // Exceptions:
        //   T:System.Web.HttpException:
        //     The System.Web.UI.SessionPageStatePersister.Load method could not successfully
        //     deserialize the state contained in the request to the Web server.
        public override void Load()
        {
            string viewStateKey;
            if (UseDefaultViewStateHiddenField)
            {
                // Get key from __VIEWSTATE hidden field
                HiddenFieldPageStatePersister _hiddenFiledPagePersister = new HiddenFieldPageStatePersister(Page);
                _hiddenFiledPagePersister.Load();
                viewStateKey = _hiddenFiledPagePersister.ViewState as string;
            }
            else
            {
                // Get key from custom hidden field
                //string viewStateKey = Page.Request.Params[ViewStateHiddenFieldName] as string;
                viewStateKey = Page.Request.Form[ViewStateHiddenFieldName] as string;
            }
                        
            if (!string.IsNullOrEmpty(viewStateKey))
            {
                ViewStateInfo info = (ViewStateInfo)Repository.GetViewState(viewStateKey);

                IStateFormatter formatter = this.StateFormatter;
                Pair statePair = (Pair)formatter.Deserialize((string)info.Value);

                ViewState = statePair.First;
                ControlState = statePair.Second;

                // Remove from repository after serve
            }
        }
        //
        // Summary:
        //     Serializes any object state contained in the System.Web.UI.PageStatePersister.ViewState
        //     or the System.Web.UI.PageStatePersister.ControlState property and writes the
        //     state to the repository.
        public override void Save()
        {
            if (base.ViewState == null && base.ControlState == null)
                return;

            IStateFormatter formatter = this.StateFormatter;
            Pair statePair = new Pair(ViewState, ControlState);

            // Serialize the statePair object to a string.
            string serializedState = formatter.Serialize(statePair);

            ViewStateInfo info = new ViewStateInfo(serializedState);

            string viewStateKey = Page.Request.Form[ViewStateHiddenFieldName] as string;
            if (!string.IsNullOrEmpty(viewStateKey))
                info.Id = viewStateKey;

            Repository.SaveViewState(info);
            
            if (UseDefaultViewStateHiddenField)
            {
                // Save key in __VIEWSTATE hidden field
                HiddenFieldPageStatePersister _hiddenFiledPagePersister = new HiddenFieldPageStatePersister(Page);
                _hiddenFiledPagePersister.ViewState = info.Id;
                _hiddenFiledPagePersister.Save();
            }
            else
            {
                // Register hidden field to store cache key in
                Page.ClientScript.RegisterHiddenField(ViewStateHiddenFieldName, info.Id);
            }
        }
    }
}
