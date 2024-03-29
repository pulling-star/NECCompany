using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using static Interop.Libsodium;

namespace NSec.Cryptography
{
    //
    //  BLAKE2b (unkeyed)
    //
    //  References:
    //
    //      RFC 7693 - The BLAKE2 Cryptographic Hash and Message Authentication
    //          Code (MAC)
    //
    //  Parameters:
    //
    //      Input Size - Between 0 and 2^128-1 bytes. (A Span<byte> can hold
    //          only up to 2^31-1 bytes.)
    //
    //      Hash Size - Between 1 and 64 bytes. For 128 bits of security, the
    //          output length should not be less than 32 bytes (BLAKE2b-256).
    //
    public sealed class Blake2b : HashAlgorithm
    {
        public static readonly int MinHashSize = 32;
        public static readonly int MaxHashSize = crypto_generichash_blake2b_BYTES_MAX;

        private static int s_selfTest;

        public Blake2b() : this(
            hashSize: crypto_generichash_blake2b_BYTES)
        {
        }

        public Blake2b(int hashSize) : base(
            hashSize: hashSize)
        {
            if (hashSize < MinHashSize || hashSize > MaxHashSize)
            {
                throw Error.ArgumentOutOfRange_HashSize(nameof(hashSize), hashSize, MinHashSize, MaxHashSize);
            }
            if (s_selfTest == 0)
            {
                SelfTest();
                Interlocked.Exchange(ref s_selfTest, 1);
            }
        }

        internal unsafe override void FinalizeCore(
            ref IncrementalHashState state,
            Span<byte> hash)
        {
            Debug.Assert(hash.Length >= crypto_generichash_blake2b_BYTES_MIN);
            Debug.Assert(hash.Length <= crypto_generichash_blake2b_BYTES_MAX);

            fixed (crypto_generichash_blake2b_state* state_ = &state.blake2b)
            fixed (byte* @out = hash)
            {
                int error = crypto_generichash_blake2b_final(
                    state_,
                    @out,
                    (nuint)hash.Length);

                Debug.Assert(error == 0);
            }
        }

        internal unsafe override void InitializeCore(
            out IncrementalHashState state)
        {
            Debug.Assert(HashSize >= crypto_generichash_blake2b_BYTES_MIN);
            Debug.Assert(HashSize <= crypto_generichash_blake2b_BYTES_MAX);

            fixed (crypto_generichash_blake2b_state* state_ = &state.blake2b)
            {
                int error = crypto_generichash_blake2b_init(
                    state_,
                    IntPtr.Zero,
                    0,
                    (nuint)HashSize);

                Debug.Assert(error == 0);
            }
        }

        internal unsafe override void UpdateCore(
            ref IncrementalHashState state,
            ReadOnlySpan<byte> data)
        {
            fixed (crypto_generichash_blake2b_state* state_ = &state.blake2b)
            fixed (byte* @in = data)
            {
                int error = crypto_generichash_blake2b_update(
                    state_,
                    @in,
                    (ulong)data.Length);

                Debug.Assert(error == 0);
            }
        }

        private protected unsafe override void HashCore(
            ReadOnlySpan<byte> data,
            Span<byte> hash)
        {
            Debug.Assert(hash.Length >= crypto_generichash_blake2b_BYTES_MIN);
            Debug.Assert(hash.Length <= crypto_generichash_blake2b_BYTES_MAX);

            fixed (byte* @out = hash)
            fixed (byte* @in = data)
            {
                int error = crypto_generichash_blake2b(
                    @out,
                    (nuint)hash.Length,
                    @in,
                    (ulong)data.Length,
                    IntPtr.Zero,
                    0);

                Debug.Assert(error == 0);
            }
        }

        private static void SelfTest()
        {
            if ((crypto_generichash_blake2b_bytes() != crypto_generichash_blake2b_BYTES) ||
                (crypto_generichash_blake2b_bytes_max() != crypto_generichash_blake2b_BYTES_MAX) ||
                (crypto_generichash_blake2b_bytes_min() != crypto_generichash_blake2b_BYTES_MIN) ||
                (crypto_generichash_blake2b_statebytes() != (nuint)Unsafe.SizeOf<crypto_generichash_blake2b_state>()))
            {
                throw Error.InvalidOperation_InitializationFailed();
            }
        }
    }
}
