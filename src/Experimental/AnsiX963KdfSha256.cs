using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using NSec.Cryptography;
using static Interop.Libsodium;

namespace NSec.Experimental
{
    //
    //  ANSI X9.63 Key Derivation Function
    //
    //  References
    //
    //      SEC 1: Elliptic Curve Cryptography, Section 3.6.1
    //
    //  Parameters
    //
    //      Salt Size - No salt is used.
    //
    //      Shared Info Size - Any.
    //
    //      Output Size - The length of the keying data to be generated must be
    //          less than HashLen*(2^32-1).
    //
    public sealed class AnsiX963KdfSha256 : KeyDerivationAlgorithm
    {
        public AnsiX963KdfSha256() : base(
            supportsSalt: false,
            maxCount: int.MaxValue)
        {
        }

        private protected unsafe override void DeriveBytesCore(
            ReadOnlySpan<byte> ikm,
            ReadOnlySpan<byte> salt,
            ReadOnlySpan<byte> info,
            Span<byte> bytes)
        {
            Debug.Assert(salt.IsEmpty);

            byte* temp = stackalloc byte[crypto_hash_sha256_BYTES];

            try
            {
                fixed (byte* key = ikm)
                fixed (byte* @in = info)
                fixed (byte* @out = bytes)
                {
                    int offset = 0;
                    uint counter = 0;
                    int chunkSize;

                    while ((chunkSize = bytes.Length - offset) > 0)
                    {
                        counter++;

                        uint counterBigEndian = BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(counter) : counter;

                        crypto_hash_sha256_state state;
                        crypto_hash_sha256_init(&state);
                        crypto_hash_sha256_update(&state, key, (ulong)ikm.Length);
                        crypto_hash_sha256_update(&state, (byte*)&counterBigEndian, sizeof(uint));
                        crypto_hash_sha256_update(&state, @in, (ulong)info.Length);
                        crypto_hash_sha256_final(&state, temp);

                        if (chunkSize > crypto_hash_sha256_BYTES)
                        {
                            chunkSize = crypto_hash_sha256_BYTES;
                        }

                        Unsafe.CopyBlockUnaligned(@out + offset, temp, (uint)chunkSize);
                        offset += chunkSize;
                    }
                }
            }
            finally
            {
                Unsafe.InitBlockUnaligned(temp, 0, crypto_hash_sha256_BYTES);
            }
        }
    }
}
