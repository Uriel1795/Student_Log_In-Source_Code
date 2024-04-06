using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dropbox.Api.Common;
using Dropbox.Api;
using System.Configuration;

namespace loginproto
{
    public static class DropboxConnector
    {
        public static async Task<DropboxClient> Connector()
        {
            var accessToken = ConfigurationManager.AppSettings["accessToken"];

            var dbx = new DropboxClient(accessToken);

            var accountInfo = await dbx.Users.GetCurrentAccountAsync();

            var db =
            dbx.WithPathRoot(new PathRoot.NamespaceId(accountInfo.RootInfo.RootNamespaceId));

            return db;
         }
    }
}
