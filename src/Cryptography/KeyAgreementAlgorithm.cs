using System;
using System.Diagnostics;
using System.Threading;
using static Interop.Libsodium;

namespace NSec.Cryptography
{
    //
    //  A key agreement algorithm
    //
    //  Candidates
    //
    //      | Algorithm  | Reference  | Key Size | Shared Secret Size |
    //      | ---------- | ---------- | -------- | ------------------ |
    //      | ECDH P-256 | FIPS 186-3 | 32       | 32                 |
    //      | ECDH P-521 | FIPS 186-3 | 66       | 66                 |
    //      | X25519     | RFC 7748   | 32       | 32                 |
    //      | X448       | RFC 7748   | 56       | 56                 |
    //
    public abstract class KeyAgreementAlgorithm : Algorithm
    {
        private static X25519 s_X25519;

        private readonly int _privateKeySize;
        private readonly int _publicKeySize;
        private readonly int _sharedSecretSize;

        private protected KeyAgreementAlgorithm(
            int privateKeySize,
            int publicKeySize,
            int sharedSecretSize)
        {
            Debug.Assert(privateKeySize > 0);
            Debug.Assert(publicKeySize > 0);
            Debug.Assert(sharedSecretSize > 0);

            _privateKeySize = privateKeySize;
            _publicKeySize = publicKeySize;
            _sharedSecretSize = sharedSecretSize;
        }

        public static X25519 X25519
        {
            get
            {
                X25519 instance = s_X25519;
                if (instance == null)
                {
                    Interlocked.CompareExchange(ref s_X25519, new X25519(), null);
                    instance = s_X25519;
                }
                return instance;
            }
        }

        public int PrivateKeySize => _privateKeySize;

        public int PublicKeySize => _publicKeySize;

        public int SharedSecretSize => _sharedSecretSize;

        public SharedSecret Agree(
            Key key,
            PublicKey otherPartyPublicKey)
        {
            if (key == null)
                throw Error.ArgumentNull_Key(nameof(key));
            if (key.Algorithm != this)
                throw Error.Argument_KeyWrongAlgorithm(nameof(key), key.Algorithm.GetType().FullName, GetType().FullName);
            if (otherPartyPublicKey == null)
                throw Error.ArgumentNull_Key(nameof(otherPartyPublicKey));
            if (otherPartyPublicKey.Algorithm != this)
                throw Error.Argument_KeyWrongAlgorithm(nameof(otherPartyPublicKey), key.Algorithm.GetType().FullName, GetType().FullName);

            SecureMemoryHandle sharedSecretHandle = null;
            bool success = false;

            try
            {
                success = TryAgreeCore(key.Handle, otherPartyPublicKey.Bytes, out sharedSecretHandle);
            }
            finally
            {
                if (!success && sharedSecretHandle != null)
                {
                    sharedSecretHandle.Dispose();
                }
            }

            if (!success)
            {
                throw Error.Cryptographic_KeyAgreementFailed();
            }

            return new SharedSecret(sharedSecretHandle);
        }

        public bool TryAgree(
            Key key,
            PublicKey otherPartyPublicKey,
            out SharedSecret result)
        {
            if (key == null)
                throw Error.ArgumentNull_Key(nameof(key));
            if (key.Algorithm != this)
                throw Error.Argument_KeyWrongAlgorithm(nameof(key), key.Algorithm.GetType().FullName, GetType().FullName);
            if (otherPartyPublicKey == null)
                throw Error.ArgumentNull_Key(nameof(otherPartyPublicKey));
            if (otherPartyPublicKey.Algorithm != this)
                throw Error.Argument_KeyWrongAlgorithm(nameof(otherPartyPublicKey), key.Algorithm.GetType().FullName, GetType().FullName);

            SecureMemoryHandle sharedSecretHandle = null;
            bool success = false;

            try
            {
                success = TryAgreeCore(key.Handle, otherPartyPublicKey.Bytes, out sharedSecretHandle);
            }
            finally
            {
                if (!success && sharedSecretHandle != null)
                {
                    sharedSecretHandle.Dispose();
                }
            }

            result = success ? new SharedSecret(sharedSecretHandle) : null;
            return success;
        }

        internal sealed override int GetKeySize()
        {
            return _privateKeySize;
        }

        internal sealed override int GetPublicKeySize()
        {
            return _publicKeySize;
        }

        internal abstract override int GetSeedSize();

        private protected abstract bool TryAgreeCore(
            SecureMemoryHandle keyHandle,
            in PublicKeyBytes otherPartyPublicKey,
            out SecureMemoryHandle sharedSecretHandle);
    }
}
