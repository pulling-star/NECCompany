using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NSec.Cryptography;

namespace NSec.Experimental
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct IncrementalHash
    {
        private readonly IncrementalHashState _state;
        private readonly HashAlgorithm _algorithm;

        public HashAlgorithm Algorithm => _algorithm;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static new bool Equals(
            object objA,
            object objB)
        {
            return object.Equals(objA, objB);
        }

        public static byte[] Finalize(
            ref IncrementalHash state)
        {
            if (state._algorithm == null)
            {
                throw Error.InvalidOperation_UninitializedState();
            }

            try
            {
                byte[] hash = new byte[state._algorithm.HashSize];
                state._algorithm.FinalizeCore(ref Unsafe.AsRef(in state._state), hash);
                return hash;
            }
            finally
            {
                Unsafe.AsRef(in state._algorithm) = null;
            }
        }

        public static void Finalize(
            ref IncrementalHash state,
            Span<byte> hash)
        {
            if (state._algorithm == null)
            {
                throw Error.InvalidOperation_UninitializedState();
            }
            if (hash.Length != state._algorithm.HashSize)
            {
                throw Error.Argument_HashLength(nameof(hash), state._algorithm.HashSize.ToString());
            }

            try
            {
                state._algorithm.FinalizeCore(ref Unsafe.AsRef(in state._state), hash);
            }
            finally
            {
                Unsafe.AsRef(in state._algorithm) = null;
            }
        }

        public static bool FinalizeAndVerify(
            ref IncrementalHash state,
            ReadOnlySpan<byte> hash)
        {
            if (state._algorithm == null)
            {
                throw Error.InvalidOperation_UninitializedState();
            }

            try
            {
                return hash.Length == state._algorithm.HashSize && state._algorithm.FinalizeAndVerifyCore(ref Unsafe.AsRef(in state._state), hash);
            }
            finally
            {
                Unsafe.AsRef(in state._algorithm) = null;
            }
        }

        public static void Initialize(
            HashAlgorithm algorithm,
            out IncrementalHash state)
        {
            if (algorithm == null)
            {
                throw Error.ArgumentNull_Algorithm(nameof(algorithm));
            }

            bool success = false;
            try
            {
                state = default;
                algorithm.InitializeCore(algorithm.HashSize, out Unsafe.AsRef(in state._state));
                Unsafe.AsRef(in state._algorithm) = algorithm;
                success = true;
            }
            finally
            {
                if (!success)
                {
                    state = default;
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static new bool ReferenceEquals(
            object objA,
            object objB)
        {
            return object.ReferenceEquals(objA, objB);
        }

        public static void Update(
            ref IncrementalHash state,
            ReadOnlySpan<byte> data)
        {
            if (state._algorithm == null)
            {
                throw Error.InvalidOperation_UninitializedState();
            }

            state._algorithm.UpdateCore(ref Unsafe.AsRef(in state._state), data);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(
            object obj)
        {
            throw Error.NotSupported_Operation();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode()
        {
            throw Error.NotSupported_Operation();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString()
        {
            return typeof(IncrementalHash).ToString();
        }
    }
}
