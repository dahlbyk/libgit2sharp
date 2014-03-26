using System;

namespace LibGit2Sharp
{
    /// <summary>
    /// Class that holds credentials for remote repository access.
    /// </summary>
    public abstract class Credentials
    {
        protected internal abstract int GitCredentialHandler(out IntPtr cred, IntPtr url, IntPtr usernameFromUrl, uint types, IntPtr payload);
    }
}
