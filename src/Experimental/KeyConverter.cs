using System;
using static Interop.Libsodium;

namespace NSec.Cryptography
{
    public static class KeyConverter
    {
        public static Key ConvertPrivateKey(
            Key key,
            Algorithm algorithm,
            in KeyCreationParameters creationParameters = default)
        {
            if (key == null)
            {
                throw Error.ArgumentNull_Key(nameof(key));
            }
            if (key.Algorithm is not Ed25519)
            {
                throw Error.NotSupported_KeyConversion(key.Algorithm.GetType().Name, algorithm.GetType().Name);
            }
            if (algorithm == null)
            {
                throw Error.ArgumentNull_Algorithm(nameof(algorithm));
            }
            if (algorithm is not X25519)
            {
                throw Error.NotSupported_KeyConversion(key.Algorithm.GetType().Name, algorithm.GetType().Name);
            }

            SecureMemoryHandle? keyHandle = default;
            PublicKey? publicKey = default;
            bool success = false;

            try
            {
                Span<byte> seed = stackalloc byte[crypto_scalarmult_curve25519_BYTES];
                try
                {
                    unsafe
                    {
                        fixed (byte* buf = seed)
                        {
                            int error = crypto_sign_ed25519_sk_to_curve25519(buf, key.Handle);

                            if (error != 0)
                            {
                                throw Error.InvalidOperation_InternalError();
                            }
                        }
                    }

                    algorithm.CreateKey(seed, out keyHandle, out publicKey);
                    success = true;
                }
                finally
                {
                    System.Security.Cryptography.CryptographicOperations.ZeroMemory(seed);
                }
            }
            finally
            {
                if (!success && keyHandle != null)
                {
                    keyHandle.Dispose();
                }
            }

            return new Key(algorithm, in creationParameters, keyHandle, publicKey);
        }

        public static PublicKey ConvertPublicKey(
            PublicKey publicKey,
            Algorithm algorithm)
        {
            if (publicKey == null)
            {
                throw Error.ArgumentNull_Key(nameof(publicKey));
            }
            if (publicKey.Algorithm is not Ed25519)
            {
                throw Error.NotSupported_KeyConversion(publicKey.Algorithm.GetType().Name, algorithm.GetType().Name);
            }
            if (algorithm == null)
            {
                throw Error.ArgumentNull_Algorithm(nameof(algorithm));
            }
            if (algorithm is not X25519)
            {
                throw Error.NotSupported_KeyConversion(publicKey.Algorithm.GetType().Name, algorithm.GetType().Name);
            }

            PublicKey newPublicKey = new(algorithm);

            unsafe
            {
                fixed (PublicKeyBytes* curve25519_pk = newPublicKey)
                fixed (PublicKeyBytes* ed25519_pk = publicKey)
                {
                    int error = crypto_sign_ed25519_pk_to_curve25519(curve25519_pk, ed25519_pk);

                    if (error != 0)
                    {
                        throw Error.InvalidOperation_InternalError();
                    }
                }
            }

            return newPublicKey;
        }
    }
}
