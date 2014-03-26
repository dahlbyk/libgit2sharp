using System;
using LibGit2Sharp.Core;

namespace LibGit2Sharp
{
    /// <summary>
    /// Class that holds username and password credentials for remote repository access.
    /// </summary>
    public sealed class UsernamePasswordCredentials : Credentials
    {
        protected internal override int GitCredentialHandler(out IntPtr cred, IntPtr url, IntPtr usernameFromUrl, uint types, IntPtr payload)
        {
            return NativeMethods.git_cred_userpass_plaintext_new(out cred, Username, Password);
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
