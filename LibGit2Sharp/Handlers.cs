﻿using System;
using LibGit2Sharp.Core;

namespace LibGit2Sharp.Handlers
{
    /// <summary>
    ///   Delegate definition to handle Progress callback.
    /// </summary>
    /// <param name="message">Progress message.</param>
    public delegate void ProgressHandler(string message);

    /// <summary>
    ///   Delegate definition to handle UpdateTips callback.
    /// </summary>
    /// <param name="referenceName">Name of the updated reference.</param>
    /// <param name="oldId">Old ID of the reference.</param>
    /// <param name="newId">New ID of the reference.</param>
    /// <returns>Return negative integer to cancel.</returns>
    public delegate int UpdateTipsHandler(string referenceName, ObjectId oldId, ObjectId newId);

    /// <summary>
    ///   Delegate definition to handle Completion callback.
    /// </summary>
    /// <param name="RemoteCompletionType"></param>
    /// <returns></returns>
    public delegate int CompletionHandler(RemoteCompletionType RemoteCompletionType);

    /// <summary>
    ///   Delegate definition for transfer progress callback.
    /// </summary>
    /// <param name="progress">The <see cref = "TransferProgress" /> object containing progress information.</param>
    public delegate void TransferProgressHandler(TransferProgress progress);
}
