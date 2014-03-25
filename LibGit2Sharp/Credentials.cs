namespace LibGit2Sharp
{
    /// <summary>
    /// Class that holds credentials for remote repository access.
    /// </summary>
    public abstract class Credentials
    {
        /// <summary>
        /// Returns the type of credentials to be used for authentication.
        /// </summary>
        public abstract CredentialType Type { get; }
    }
}
