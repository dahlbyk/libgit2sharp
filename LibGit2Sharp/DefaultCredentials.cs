using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibGit2Sharp
{
    /// <summary>
    /// A credential object that will provide the "default" credentials
    /// (logged-in user information) via NTLM or SPNEGO authentication.
    /// </summary>
    public sealed class DefaultCredentials : Credentials
    {
        /// <summary>
        /// Returns the type of credentials to be used for authentication.
        /// </summary>
        public override CredentialType Type
        {
            get
            {
                return CredentialType.Default;
            }
        }
    }
}
