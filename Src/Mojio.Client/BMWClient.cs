using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mojio.Client
{
    public class BMWClient : MojioClient
    {
        /// <summary>
        /// Initiate a new BMWClient.  No session token has been created yet.
        /// Client must call BeginAsync, or set a valid Token.
        /// </summary>
        /// <param name="Url">API endpoint URL</param>
        public BMWClient(string Url = Live) : base(Url)
        {

        }
    }
}
