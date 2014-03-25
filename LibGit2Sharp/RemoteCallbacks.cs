﻿using System;
using LibGit2Sharp.Core;
using LibGit2Sharp.Handlers;

namespace LibGit2Sharp
{
    /// <summary>
    /// Class to translate libgit2 callbacks into delegates exposed by LibGit2Sharp.
    /// Handles generating libgit2 git_remote_callbacks datastructure given a set
    /// of LibGit2Sharp delegates and handles propagating libgit2 callbacks into
    /// corresponding LibGit2Sharp exposed delegates.
    /// </summary>
    internal class RemoteCallbacks
    {
        internal RemoteCallbacks(
            ProgressHandler onProgress = null,
            TransferProgressHandler onDownloadProgress = null,
            UpdateTipsHandler onUpdateTips = null,
            Credentials credentials = null)
        {
            Progress = onProgress;
            DownloadTransferProgress = onDownloadProgress;
            UpdateTips = onUpdateTips;
            Credentials = credentials;
        }

        internal RemoteCallbacks(FetchOptions fetchOptions)
        {
            Ensure.ArgumentNotNull(fetchOptions, "fetchOptions");
            Progress = fetchOptions.OnProgress;
            DownloadTransferProgress = fetchOptions.OnTransferProgress;
            UpdateTips = fetchOptions.OnUpdateTips;
            Credentials = fetchOptions.Credentials;
        }

        #region Delegates

        /// <summary>
        /// Progress callback. Corresponds to libgit2 progress callback.
        /// </summary>
        private readonly ProgressHandler Progress;

        /// <summary>
        /// UpdateTips callback. Corresponds to libgit2 update_tips callback.
        /// </summary>
        private readonly UpdateTipsHandler UpdateTips;

        /// <summary>
        /// Managed delegate to be called in response to a git_transfer_progress_callback callback from libgit2.
        /// This will in turn call the user provided delegate.
        /// </summary>
        private readonly TransferProgressHandler DownloadTransferProgress;

        #endregion

        /// <summary>
        /// The credentials to use for the credential callback.
        /// </summary>
        Credentials Credentials;

        internal GitRemoteCallbacks GenerateCallbacks()
        {
            var callbacks = new GitRemoteCallbacks {version = 1};

            if (Progress != null)
            {
                callbacks.progress = GitProgressHandler;
            }

            if (UpdateTips != null)
            {
                callbacks.update_tips = GitUpdateTipsHandler;
            }

            if (Credentials != null)
            {
                callbacks.acquire_credentials = GitCredentialHandler;
            }

            if (DownloadTransferProgress != null)
            {
                callbacks.download_progress = GitDownloadTransferProgressHandler;
            }

            return callbacks;
        }

        #region Handlers to respond to callbacks raised by libgit2

        /// <summary>
        /// Handler for libgit2 Progress callback. Converts values
        /// received from libgit2 callback to more suitable types
        /// and calls delegate provided by LibGit2Sharp consumer.
        /// </summary>
        /// <param name="str">IntPtr to string from libgit2</param>
        /// <param name="len">length of string</param>
        /// <param name="data">IntPtr to optional payload passed back to the callback.</param>
        /// <returns>0 on success; a negative value to abort the process.</returns>
        private int GitProgressHandler(IntPtr str, int len, IntPtr data)
        {
            ProgressHandler onProgress = Progress;

            bool shouldContinue = true;

            if (onProgress != null)
            {
                string message = LaxUtf8Marshaler.FromNative(str, len);
                shouldContinue = onProgress(message);
            }

            return Proxy.ConvertResultToCancelFlag(shouldContinue);
        }

        /// <summary>
        /// Handler for libgit2 update_tips callback. Converts values
        /// received from libgit2 callback to more suitable types
        /// and calls delegate provided by LibGit2Sharp consumer.
        /// </summary>
        /// <param name="str">IntPtr to string</param>
        /// <param name="oldId">Old reference ID</param>
        /// <param name="newId">New referene ID</param>
        /// <param name="data">IntPtr to optional payload passed back to the callback.</param>
        /// <returns>0 on success; a negative value to abort the process.</returns>
        private int GitUpdateTipsHandler(IntPtr str, ref GitOid oldId, ref GitOid newId, IntPtr data)
        {
            UpdateTipsHandler onUpdateTips = UpdateTips;
            bool shouldContinue = true;

            if (onUpdateTips != null)
            {
                string refName = LaxUtf8Marshaler.FromNative(str);
                shouldContinue = onUpdateTips(refName, oldId, newId);
            }

            return Proxy.ConvertResultToCancelFlag(shouldContinue);
        }

        /// <summary>
        /// The delegate with the signature that matches the native git_transfer_progress_callback function's signature.
        /// </summary>
        /// <param name="progress"><see cref="GitTransferProgress"/> structure containing progress information.</param>
        /// <param name="payload">Payload data.</param>
        /// <returns>the result of the wrapped <see cref="TransferProgressHandler"/></returns>
        private int GitDownloadTransferProgressHandler(ref GitTransferProgress progress, IntPtr payload)
        {
            bool shouldContinue = true;

            if (DownloadTransferProgress != null)
            {
                shouldContinue = DownloadTransferProgress(new TransferProgress(progress));
            }

            return Proxy.ConvertResultToCancelFlag(shouldContinue);
        }

        private int GitCredentialHandler(out IntPtr cred, IntPtr url, IntPtr username_from_url, uint types, IntPtr payload)
        {
            switch (Credentials.Type)
            {
                case CredentialType.Default:
                    return NativeMethods.git_cred_default_new(out cred);
                case CredentialType.UsernamePassword:
                    return NativeMethods.git_cred_userpass_plaintext_new(out cred,
                        ((UsernamePasswordCredentials)Credentials).Username, ((UsernamePasswordCredentials)Credentials).Password);
                default:
                    cred = IntPtr.Zero;

                    Proxy.giterr_set_str(GitErrorCategory.Invalid, String.Format("Unknown credential type {0}", Credentials.Type));
                    return -1;
            }
        }

        #endregion
    }
}
