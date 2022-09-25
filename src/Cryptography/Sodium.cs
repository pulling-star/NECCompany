using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using static Interop.Libsodium;

namespace NSec.Cryptography
{
    internal static class Sodium
    {
        private static int s_initialized;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Initialize()
        {
            if (s_initialized == 0)
            {
                InitializeCore();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static unsafe void InitializeCore()
        {
            try
            {
                if (sodium_library_version_major() != SODIUM_LIBRARY_VERSION_MAJOR ||
                    sodium_library_version_minor() != SODIUM_LIBRARY_VERSION_MINOR)
                {
                    string? version = Marshal.PtrToStringAnsi(sodium_version_string());
                    throw (version != null && version != SODIUM_VERSION_STRING)
                        ? Error.InvalidOperation_InitializationFailed_VersionMismatch(SODIUM_VERSION_STRING, version)
                        : Error.InvalidOperation_InitializationFailed();
                }

                if (sodium_set_misuse_handler(&InternalError) != 0)
                {
                    throw Error.InvalidOperation_InitializationFailed();
                }

                // sodium_init() returns 0 on success, -1 on failure, and 1 if the
                // library had already been initialized.
                if (sodium_init() < 0)
                {
                    throw Error.InvalidOperation_InitializationFailed();
                }
            }
            catch (DllNotFoundException e)
            {
                throw Error.PlatformNotSupported_Initialization(e);
            }
            catch (BadImageFormatException e)
            {
                throw Error.PlatformNotSupported_Initialization(e);
            }

            Interlocked.Exchange(ref s_initialized, 1);
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static void InternalError()
        {
            throw Error.InvalidOperation_InternalError();
        }
    }
}
