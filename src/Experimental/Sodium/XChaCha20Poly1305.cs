using System;
using System.Buffers;
using System.Diagnostics;
using System.Threading;
using NSec.Cryptography;
using NSec.Cryptography.Formatting;
using static Interop.Libsodium;

namespace NSec.Experimental.Sodium
{
    //
    //  XChaCha20-Poly1305
    //
    //      Authenticated Encryption with Associated Data (AEAD) algorithm
    //      based the XChaCha20 stream cipher and the Poly1305 authenticator
    //
    //  References:
    //
    //      draft-arciszewski-xchacha-03 - XChaCha: eXtended-nonce ChaCha and
    //          AEAD_XChaCha20_Poly1305
    //
    //      RFC 5116 - An Interface and Algorithms for Authenticated Encryption
    //
    //  Parameters:
    //
    //      Key Size - 32 bytes.
    //
    //      Nonce Size - 24 bytes.
    //
    //      Tag Size - 16 bytes.
    //
    //      Plaintext Size - Between 0 and 2^38-64 bytes. (A Span<byte> can only
    //          hold up to 2^31-1 bytes.)
    //
    //      Associated Data Size - Between 0 and 2^64-1 bytes.
    //
    //      Ciphertext Size - The ciphertext always has the size of the
    //          plaintext plus the tag size.
    //
    public sealed class XChaCha20Poly1305 : AeadAlgorithm
    {
        private const uint NSecBlobHeader = 0xDE6148DE;

        private static int s_selfTest;

        public XChaCha20Poly1305() : base(
            keySize: crypto_aead_xchacha20poly1305_ietf_KEYBYTES,
            nonceSize: crypto_aead_xchacha20poly1305_ietf_NPUBBYTES,
            tagSize: crypto_aead_xchacha20poly1305_ietf_ABYTES)
        {
            if (s_selfTest == 0)
            {
                SelfTest();
                Interlocked.Exchange(ref s_selfTest, 1);
            }
        }

        internal override void CreateKey(
            ReadOnlySpan<byte> seed,
            MemoryPool<byte> memoryPool,
            out ReadOnlyMemory<byte> memory,
            out IMemoryOwner<byte> owner,
            out PublicKey publicKey)
        {
            Debug.Assert(seed.Length == crypto_aead_xchacha20poly1305_ietf_KEYBYTES);

            publicKey = null;
            owner = memoryPool.Rent(seed.Length);
            memory = owner.Memory.Slice(0, seed.Length);
            seed.CopyTo(owner.Memory.Span);
        }

        private protected unsafe override void EncryptCore(
            ReadOnlySpan<byte> key,
            in Nonce nonce,
            ReadOnlySpan<byte> associatedData,
            ReadOnlySpan<byte> plaintext,
            Span<byte> ciphertext)
        {
            Debug.Assert(key.Length == crypto_aead_xchacha20poly1305_ietf_KEYBYTES);
            Debug.Assert(nonce.Size == crypto_aead_xchacha20poly1305_ietf_NPUBBYTES);
            Debug.Assert(ciphertext.Length == plaintext.Length + crypto_aead_xchacha20poly1305_ietf_ABYTES);

            fixed (byte* c = ciphertext)
            fixed (byte* m = plaintext)
            fixed (byte* ad = associatedData)
            fixed (Nonce* n = &nonce)
            fixed (byte* k = key)
            {
                int error = crypto_aead_xchacha20poly1305_ietf_encrypt(
                    c,
                    out ulong clen_p,
                    m,
                    (ulong)plaintext.Length,
                    ad,
                    (ulong)associatedData.Length,
                    null,
                    n,
                    k);

                Debug.Assert(error == 0);
                Debug.Assert((ulong)ciphertext.Length == clen_p);
            }
        }

        internal override int GetSeedSize()
        {
            return crypto_aead_xchacha20poly1305_ietf_KEYBYTES;
        }

        private protected unsafe override bool DecryptCore(
            ReadOnlySpan<byte> key,
            in Nonce nonce,
            ReadOnlySpan<byte> associatedData,
            ReadOnlySpan<byte> ciphertext,
            Span<byte> plaintext)
        {
            Debug.Assert(key.Length == crypto_aead_xchacha20poly1305_ietf_KEYBYTES);
            Debug.Assert(nonce.Size == crypto_aead_xchacha20poly1305_ietf_NPUBBYTES);
            Debug.Assert(plaintext.Length == ciphertext.Length - crypto_aead_xchacha20poly1305_ietf_ABYTES);

            fixed (byte* m = plaintext)
            fixed (byte* c = ciphertext)
            fixed (byte* ad = associatedData)
            fixed (Nonce* n = &nonce)
            fixed (byte* k = key)
            {
                int error = crypto_aead_xchacha20poly1305_ietf_decrypt(
                    m,
                    out ulong mlen_p,
                    null,
                    c,
                    (ulong)ciphertext.Length,
                    ad,
                    (ulong)associatedData.Length,
                    n,
                    k);

                // libsodium clears plaintext if decryption fails

                Debug.Assert(error != 0 || (ulong)plaintext.Length == mlen_p);
                return error == 0;
            }
        }

        internal override bool TryExportKey(
            ReadOnlySpan<byte> key,
            KeyBlobFormat format,
            Span<byte> blob,
            out int blobSize)
        {
            switch (format)
            {
            case KeyBlobFormat.RawSymmetricKey:
                return RawKeyFormatter.TryExport(key, blob, out blobSize);
            case KeyBlobFormat.NSecSymmetricKey:
                return NSecKeyFormatter.TryExport(NSecBlobHeader, KeySize, TagSize, key, blob, out blobSize);
            default:
                throw Error.Argument_FormatNotSupported(nameof(format), format.ToString());
            }
        }

        internal override bool TryImportKey(
            ReadOnlySpan<byte> blob,
            KeyBlobFormat format,
            MemoryPool<byte> memoryPool,
            out ReadOnlyMemory<byte> memory,
            out IMemoryOwner<byte> owner,
            out PublicKey publicKey)
        {
            publicKey = null;

            switch (format)
            {
            case KeyBlobFormat.RawSymmetricKey:
                return RawKeyFormatter.TryImport(KeySize, blob, memoryPool, out memory, out owner);
            case KeyBlobFormat.NSecSymmetricKey:
                return NSecKeyFormatter.TryImport(NSecBlobHeader, KeySize, TagSize, blob, memoryPool, out memory, out owner);
            default:
                throw Error.Argument_FormatNotSupported(nameof(format), format.ToString());
            }
        }

        private static void SelfTest()
        {
            if ((crypto_aead_xchacha20poly1305_ietf_abytes() != (UIntPtr)crypto_aead_xchacha20poly1305_ietf_ABYTES) ||
                (crypto_aead_xchacha20poly1305_ietf_keybytes() != (UIntPtr)crypto_aead_xchacha20poly1305_ietf_KEYBYTES) ||
                (crypto_aead_xchacha20poly1305_ietf_npubbytes() != (UIntPtr)crypto_aead_xchacha20poly1305_ietf_NPUBBYTES) ||
                (crypto_aead_xchacha20poly1305_ietf_nsecbytes() != (UIntPtr)crypto_aead_xchacha20poly1305_ietf_NSECBYTES))
            {
                throw Error.InvalidOperation_InitializationFailed();
            }
        }
    }
}
