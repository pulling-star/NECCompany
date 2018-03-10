using System;
using System.Runtime.InteropServices;
using static NSec.Cryptography.Experimental.Keccak.KeccakTiny;

namespace NSec.Cryptography.Experimental
{
    //
    //  SHA3-512
    //
    //      SHA-3 Permutation-Based Hash with a 512-bit message digest
    //
    //  References:
    //
    //      FIPS 202 - SHA-3 Standard: Permutation-Based Hash and
    //          Extendable-Output Functions
    //
    //  Parameters:
    //
    //      Input Size - The SHA-3 functions are defined on messages of any bit
    //          length, including the empty string.
    //
    //      Hash Size - 64 bytes (256 bits of security).
    //
    public sealed class Sha3_512 : HashAlgorithm
    {
        public Sha3_512() : base(
            hashSize: 64)
        {
        }

        internal override bool FinalizeAndTryVerifyCore(
            ref IncrementalHash.State state,
            ReadOnlySpan<byte> hash)
        {
            throw new NotImplementedException();
        }

        internal override void FinalizeCore(
            ref IncrementalHash.State state,
            Span<byte> hash)
        {
            throw new NotImplementedException();
        }

        internal override void InitializeCore(
            out IncrementalHash.State state)
        {
            throw new NotImplementedException();
        }

        internal override void UpdateCore(
            ref IncrementalHash.State state,
            ReadOnlySpan<byte> data)
        {
            throw new NotImplementedException();
        }

        private protected override void HashCore(
            ReadOnlySpan<byte> data,
            Span<byte> hash)
        {
            sha3_512(
                ref MemoryMarshal.GetReference(hash),
                (ulong)hash.Length,
                ref MemoryMarshal.GetReference(data),
                (ulong)data.Length);
        }

        private protected override bool TryVerifyCore(
            ReadOnlySpan<byte> data,
            ReadOnlySpan<byte> hash)
        {
            throw new NotImplementedException();
        }
    }
}
