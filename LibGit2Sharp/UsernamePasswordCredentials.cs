using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibGit2Sharp
{
    /// <summary>
    /// Class that holds username and password credentials for remote repository access.
    /// </summary>
    public sealed class UsernamePasswordCredentials : Credentials
    {
        /// <summary>
        /// Returns the type of credentials to be used for authentication.
        /// </summary>
        public override CredentialType Type
        {
            get
            {
                return CredentialType.UsernamePassword;
            }
        }

        /// <summary>
        /// Username for username/password authentication (as in HTTP basic auth).
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password for username/password authentication (as in HTTP basic auth).
        /// </summary>
        public string Password { get; set; }
    }
}
