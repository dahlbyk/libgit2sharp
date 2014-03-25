using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibGit2Sharp
{
    /// <summary>
    /// Type type of a Credentials object.
    /// </summary>
    [Flags]
    public enum CredentialType
    {
        /// <summary>
        /// Username and password should be provided.
        /// </summary>
        UsernamePassword = (1 << 0),

        /// <summary>
        /// Default credentials (the credentials of the logged-in user)
        /// should be presented if NTLM or Negotiate authentication is
        /// available.
        /// </summary>
        Default = (1 << 3),
    };
}

