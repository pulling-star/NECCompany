using System;
using static Interop.Libsodium;

namespace NSec.Cryptography
{
    //
    //  Concatenation Key Derivation Function
    //
    //      Single-step key derivation function based on HMAC-SHA-256
    //
    //  References
    //
    //      NIST Special Publication 800-56A, Revision 2, Section 5.8
    //
    //  Parameters
    //
    //      Salt Size - Any.
    //
    //      Shared Info Size - Any.
    //
    //      Output Size - The length of the keying data to be generated must be
    //          less than or equal to HashLen*(2^32-1).
    //
    public sealed class ConcatKdfHmacSha256 : KeyDerivationAlgorithm
    {
        public ConcatKdfHmacSha256() : base(
            supportsSalt: true,
            maxOutputSize: int.MaxValue)
        {
        }

        internal override void DeriveBytesCore(
            ReadOnlySpan<byte> inputKeyingMaterial,
            ReadOnlySpan<byte> salt,
            ReadOnlySpan<byte> info,
            Span<byte> bytes)
        {
            Span<byte> temp;
            try
            {
                unsafe
                {
                    byte* pointer = stackalloc byte[crypto_auth_hmacsha256_BYTES];
                    temp = new Span<byte>(pointer, crypto_auth_hmacsha256_BYTES);
                }

                int offset = 0;
                uint counter = 0;
                int chunkSize;

                while ((chunkSize = bytes.Length - offset) > 0)
                {
                    counter++;

                    uint counterBigEndian = Utilities.ToBigEndian(counter);

                    crypto_auth_hmacsha256_init(out crypto_auth_hmacsha256_state state, ref salt.DangerousGetPinnableReference(), (UIntPtr)salt.Length);
                    crypto_auth_hmacsha256_update(ref state, ref counterBigEndian, sizeof(uint));
                    crypto_auth_hmacsha256_update(ref state, ref inputKeyingMaterial.DangerousGetPinnableReference(), (ulong)inputKeyingMaterial.Length);
                    crypto_auth_hmacsha256_update(ref state, ref info.DangerousGetPinnableReference(), (ulong)info.Length);
                    crypto_auth_hmacsha256_final(ref state, ref temp.DangerousGetPinnableReference());

                    if (chunkSize > crypto_auth_hmacsha256_BYTES)
                        chunkSize = crypto_auth_hmacsha256_BYTES;
                    temp.Slice(0, chunkSize).CopyTo(bytes.Slice(offset));
                    offset += chunkSize;
                }
            }
            finally
            {
                sodium_memzero(ref temp.DangerousGetPinnableReference(), (UIntPtr)temp.Length);
            }
        }
    }
}
