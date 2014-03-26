using System;
using LibGit2Sharp.Core;

namespace LibGit2Sharp
{
    /// <summary>
    /// A credential object that will provide the "default" credentials
    /// (logged-in user information) via NTLM or SPNEGO authentication.
    /// </summary>
    public sealed class DefaultCredentials : Credentials
    {
        protected internal override int GitCredentialHandler(out IntPtr cred, IntPtr url, IntPtr usernameFromUrl, uint types, IntPtr payload)
        {
            return NativeMethods.git_cred_default_new(out cred);
        }
    }
}
