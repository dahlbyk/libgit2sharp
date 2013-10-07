using System;
using LibGit2Sharp.Core;

namespace LibGit2Sharp
{
    /// <summary>
    /// Methods that affect the entirety of the Git implementation in libgit2 and
    /// LibGit2Sharp.
    /// </summary>
    public sealed class LibGit2
    {
        /// <summary>
        /// Registers a new <see cref="SmartSubtransport"/> as a custom
        /// smart-protocol transport with libgit2.
        /// </summary>
        /// <typeparam name="T">The type of SmartSubtransport to register</typeparam>
        /// <param name="prefix">The prefix (i.e. "http://") to register</param>
        /// <param name="priority">The priority of the transport; the value must be 2 or larger to override a built-in transport</param>
        public static SmartSubtransportRegistration<T> RegisterSmartSubtransport<T>(string prefix, int priority)
            where T : SmartSubtransport, new()
        {
            Ensure.ArgumentNotNull(prefix, "prefix");
            Ensure.ArgumentConformsTo<int>(priority, s => s >= 0, "priority");

            var registration = new SmartSubtransportRegistration<T>(prefix, priority);

            try
            {
                Proxy.git_transport_register(
                    registration.Prefix,
                    (uint)registration.Priority,
                    registration.FunctionPointer,
                    registration.RegistrationPointer);
            }
            catch(Exception)
            {
                UnregisterSmartSubtransport(registration);
                throw;
            }

            return registration;
        }

        /// <summary>
        /// Unregisters a previously registered <see cref="SmartSubtransport"/>
        /// as a custom smart-protocol transport with libgit2.
        /// </summary>
        /// <typeparam name="T">The type of SmartSubtransport to register</typeparam>
        /// <param name="registration">The previous registration</param>
        public static void UnregisterSmartSubtransport<T>(SmartSubtransportRegistration<T> registration)
            where T : SmartSubtransport, new()
        {
            Ensure.ArgumentNotNull(registration, "registration");

            Proxy.git_transport_unregister(registration.Prefix, (uint)registration.Priority);
            registration.Free();
        }
    }
}
