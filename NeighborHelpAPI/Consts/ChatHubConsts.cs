using System;
using System.Collections.Generic;
using System.Text;

namespace NeighborHelpAPI.Consts
{
    public static class ChatHubConsts
    {
        public const string Path = "/chat";

        //From server to Clients
        public const string NotifyClients = "Notify";
        public const string ReceiveClientsMesage = "Receive";

        //From clients to server
        public const string SendMessage = "Send";
        public const string SendToGroup = "SendToGroup";
        public const string EnderToGroup = "Enter";
    }
}
