using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using LibGit2Sharp.Core;

namespace LibGit2Sharp
{
    /// <summary>
    ///   Exposed git_remote_callbacks callbacks as events
    /// </summary>
    public class RemoteCallbacks
    {
        /// <summary>
        ///   Constructor.
        /// </summary>
        public RemoteCallbacks()
        { }

        /// <summary>
        ///   Delegate definition to handle Progress callbacks.
        /// </summary>
        /// <param name="message">Progress message.</param>
        public delegate void ProgressHandler(string message);

        /// <summary>
        ///   Delegate definition to handle UpdateTips callbacks
        /// </summary>
        /// <param name="referenceName">Name of the updated reference.</param>
        /// <param name="oldId">Old id of the reference.</param>
        /// <param name="newId">New id of the reference.</param>
        /// <returns>Return negative integer 0 to cancel.</returns>
        public delegate int UpdateTipsHandler(string referenceName, ObjectId oldId, ObjectId newId);

        /// <summary>
        ///   Delegate definition to handle Completion callbacks.
        /// </summary>
        /// <param name="RemoteCompletionType"></param>
        /// <returns></returns>
        public delegate int CompletionHandler(RemoteCompletionType RemoteCompletionType);

        #region Delegates

        /// <summary>
        ///   Progress callback. Corresponds to libgit2 progress callback.
        /// </summary>
        public ProgressHandler Progress;
       
        /// <summary>
        ///   UpdateTips callback. Corresponds to libgit2 update_tips callback.
        /// </summary>
        public UpdateTipsHandler UpdateTips;
        
        /// <summary>
        ///   Completion callback. Corresponds to libgit2 Completion callback.
        /// </summary>
        public CompletionHandler Completion;
        
        #endregion

        internal GitRemoteCallbacks GenerateCallbacks()
        {
            GitRemoteCallbacks callbacks = new GitRemoteCallbacks();

            if (Progress != null)
            {
                callbacks.progress = GitProgressHandler;
            }

            if (UpdateTips != null)
            {
                callbacks.update_tips = GitUpdateTipsHandler;
            }

            if (Completion != null)
            {
                callbacks.completion = GitCompletionHandler;
            }

            return callbacks;
        }

        #region Handlers to respond to callbacks raised by libgit2

        /// <summary>
        ///   Handler for libgit2 Progress callback. Converts values
        ///   received from libgit2 callback to more suitable types
        ///   and calls delegate provided by LibGit2Sharp consumer.
        /// </summary>
        /// <param name="str">IntPtr to string from libgit2</param>
        /// <param name="len">length of string</param>
        /// <param name="data"></param>
        internal void GitProgressHandler(IntPtr str, int len, IntPtr data)
        {
            ProgressHandler onProgress = Progress;

            if (onProgress != null)
            {
                string message = Utf8Marshaler.FromNative(str, (uint) len);
                onProgress(message);
            }
        }

        /// <summary>
        ///   Handler for libgit2 update_tips callback. Converts values
        ///   received from libgit2 callback to more suitable types
        ///   and calls delegate provided by LibGit2Sharp consumer.
        /// </summary>
        /// <param name="str">IntPtr to string</param>
        /// <param name="oldId">Old reference ID</param>
        /// <param name="newId">New referene ID</param>
        /// <param name="data"></param>
        /// <returns></returns>
        internal int GitUpdateTipsHandler(IntPtr str, ref GitOid oldId, ref GitOid newId, IntPtr data)
        {
            UpdateTipsHandler onUpdateTips = UpdateTips;
            int result = 0;

            if (onUpdateTips != null)
            {
                string refName = Utf8Marshaler.FromNative(str);
                result = onUpdateTips(refName, new ObjectId(oldId), new ObjectId(newId));
            }

            return result;
        }

        /// <summary>
        ///   Handler for libgit2 completion callback. Converts values
        ///   received from libgit2 callback to more suitable types
        ///   and calls delegate provided by LibGit2Sharp consumer.
        /// </summary>
        /// <param name="remoteCompletionType"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        internal int GitCompletionHandler(RemoteCompletionType remoteCompletionType, IntPtr data)
        {
            CompletionHandler completion = Completion;
            int result = 0;

            if (completion != null)
            {
                result = completion(remoteCompletionType);
            }

            return result;
        }

        #endregion
    }
}
