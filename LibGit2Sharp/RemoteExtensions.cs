using System.IO;
using LibGit2Sharp.Core;
using LibGit2Sharp.Handlers;

namespace LibGit2Sharp
{
    /// <summary>
    ///   Provides helper overloads to a <see cref = "Remote" />.
    /// </summary>
    public static class RemoteExtensions
    {
        /// <summary>
        ///   Fetch from the <see cref = "Remote" />.
        /// </summary>
        /// <param name="remote">The <see cref = "Remote" /> to fetch.</param>
        /// <param name="progressWriter">Writer for server progress messages. Streamed from libgit2 progress callback.</param>
        /// <param name="onCompletion">Completion callback. Corresponds to libgit2 completion callback.</param>
        /// <param name="onUpdateTips">UpdateTips callback. Corresponds to libgit2 update_tips callback.</param>
        /// <param name="onTransferProgress">Callback method that transfer progress will be reported through.
        ///   Reports the client's state regarding the received and processed (bytes, objects) from the server.</param>
        /// <param name="tagFetchMode">Optional parameter indicating what tags to download.</param>
        public static void Fetch(this Remote remote, TextWriter progressWriter, CompletionHandler onCompletion = null, UpdateTipsHandler onUpdateTips = null, TransferProgressHandler onTransferProgress = null, TagFetchMode tagFetchMode = TagFetchMode.Auto)
        {
            Ensure.ArgumentNotNull(remote, "remote");

            var onProgress = progressWriter != null ? progressWriter.Write : default(ProgressHandler);
            remote.Fetch(onProgress, onCompletion, onUpdateTips, onTransferProgress, tagFetchMode);
        }
    }
}