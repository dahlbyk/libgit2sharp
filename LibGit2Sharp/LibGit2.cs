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
            var registration = new SmartSubtransportRegistration<T>(prefix, priority);

            return RegisterSmartSubtransport(registration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registration"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static SmartSubtransportRegistration<T> RegisterSmartSubtransport<T>(SmartSubtransportRegistration<T> registration)
            where T : SmartSubtransport, new()
        {
            Ensure.ArgumentNotNull(registration, "registration");

            try
            {
                Proxy.git_transport_register(
                    registration.Prefix,
                    (uint)registration.Priority,
                    registration.FunctionPointer,
                    registration.RegistrationPointer);
            }
            catch (Exception)
            {
                registration.Free();
                throw;
            }

            return registration;
        }
    }
}
