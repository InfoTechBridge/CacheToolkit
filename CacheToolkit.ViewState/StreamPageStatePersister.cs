﻿using System;
using System.IO;
using System.Security.Permissions;
using System.Web;
using System.Web.UI;

namespace CacheToolkit.ViewState
{
    //
    // The StreamPageStatePersister is an example view state
    // persistence mechanism that persists view and control
    // state on the Web server.
    //
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class StreamPageStatePersister : PageStatePersister
    {

        public StreamPageStatePersister(Page page)
            : base(page)
        {
        }
        //
        // Load ViewState and ControlState.
        //
        public override void Load()
        {
            Stream stateStream = GetSecureStream();

            // Read the state string, using the StateFormatter.
            StreamReader reader = new StreamReader(stateStream);

            IStateFormatter formatter = this.StateFormatter;
            string fileContents = reader.ReadToEnd();

            // Deserilize returns the Pair object that is serialized in
            // the Save method.
            Pair statePair = (Pair)formatter.Deserialize(fileContents);

            ViewState = statePair.First;
            ControlState = statePair.Second;
            reader.Close();
            stateStream.Close();
        }
        //
        // Persist any ViewState and ControlState.
        //
        public override void Save()
        {

            if (ViewState != null || ControlState != null)
            {
                if (Page.Session != null)
                {
                    Stream stateStream = GetSecureStream();

                    StreamWriter writer = new StreamWriter(stateStream);

                    IStateFormatter formatter = this.StateFormatter;
                    Pair statePair = new Pair(ViewState, ControlState);

                    // Serialize the statePair object to a string.
                    string serializedState = formatter.Serialize(statePair);

                    writer.Write(serializedState);
                    writer.Close();
                    stateStream.Close();
                }
                else
                    throw new InvalidOperationException("Session needed for StreamPageStatePersister.");
            }
        }
        // Return a secure Stream for your environment.
        private Stream GetSecureStream()
        {
            // You must provide the implementation to build
            // a secure Stream for your environment.
            return null;
        }
    }
}
