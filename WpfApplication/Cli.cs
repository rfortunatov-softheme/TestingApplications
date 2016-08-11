// <copyright company="Dell Inc.">
//     Confidential and Proprietary
//     Copyright © 2015 Dell Inc. 
//     ALL RIGHTS RESERVED.
// </copyright>

using Replay.Core.Client;
using Replay.Core.Web.Helpers;

namespace WpfApplication
{
    public static class Cli
    {
        public static ICoreClient Core { get { return TheCore.Client ; } }
    }
}