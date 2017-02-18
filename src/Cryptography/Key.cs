using System;
using System.Diagnostics;
using static Interop.Libsodium;

namespace NSec.Cryptography
{
    [DebuggerDisplay("Algorithm = {Algorithm}")]
    public sealed class Key : IDisposable
    {
        private readonly Algorithm _algorithm;
        private readonly KeyFlags _flags;
        private readonly SecureMemoryHandle _handle;
        private readonly PublicKey _publicKey;

        private bool _exported;

        public Key(
            Algorithm algorithm,
            KeyFlags flags = KeyFlags.None)
        {
            if (algorithm == null)
                throw new ArgumentNullException(nameof(algorithm));

            int keySize = algorithm.GetDefaultKeySize();

            SecureMemoryHandle keyHandle = null;
            byte[] publicKeyBytes = null;
            bool success = false;

            try
            {
                SecureMemoryHandle.Alloc(keySize, out keyHandle);
                SecureRandom.GenerateKeyCore(keyHandle);
                algorithm.CreateKey(keyHandle, out publicKeyBytes);
                success = true;
            }
            finally
            {
                if (!success && keyHandle != null)
                {
                    keyHandle.Dispose();
                }
            }

            keyHandle.MakeReadOnly();

            _algorithm = algorithm;
            _flags = flags;
            _handle = keyHandle;
            _publicKey = (publicKeyBytes) != null ? new PublicKey(algorithm, publicKeyBytes) : null;
        }

        internal Key(
            Algorithm algorithm,
            KeyFlags flags,
            SecureMemoryHandle keyHandle,
            byte[] publicKeyBytes)
        {
            Debug.Assert(algorithm != null);
            Debug.Assert(keyHandle != null);

            keyHandle.MakeReadOnly();

            _algorithm = algorithm;
            _flags = flags;
            _handle = keyHandle;
            _publicKey = (publicKeyBytes) != null ? new PublicKey(algorithm, publicKeyBytes) : null;
        }

        public Algorithm Algorithm => _algorithm;

        public KeyFlags Flags => _flags;

        public PublicKey PublicKey => _publicKey;

        internal SecureMemoryHandle Handle => _handle;

        public static Key Create(
            Algorithm algorithm,
            KeyFlags flags = KeyFlags.None)
        {
            return new Key(algorithm, flags);
        }

        public static int GetKeyBlobSize(
            Algorithm algorithm,
            KeyBlobFormat format)
        {
            if (algorithm == null)
                throw new ArgumentNullException(nameof(algorithm));

            return algorithm.GetKeyBlobSize(format);
        }

        public static ReadOnlySpan<KeyBlobFormat> GetSupportedKeyBlobFormats(
            Algorithm algorithm)
        {
            if (algorithm == null)
                throw new ArgumentNullException(nameof(algorithm));

            return algorithm.GetSupportedKeyBlobFormats();
        }

        public static Key Import(
           Algorithm algorithm,
           ReadOnlySpan<byte> blob,
           KeyBlobFormat format,
           KeyFlags flags = KeyFlags.None)
        {
            if (algorithm == null)
                throw new ArgumentNullException(nameof(algorithm));

            SecureMemoryHandle keyHandle = null;
            byte[] publicKeyBytes = null;
            bool success = false;

            try
            {
                success = algorithm.TryImportKey(blob, format, out keyHandle, out publicKeyBytes);
            }
            finally
            {
                if (!success && keyHandle != null)
                {
                    keyHandle.Dispose();
                }
            }

            if (!success)
            {
                throw new FormatException();
            }

            return new Key(algorithm, flags, keyHandle, publicKeyBytes);
        }

        public static bool TryImport(
            Algorithm algorithm,
            ReadOnlySpan<byte> blob,
            KeyBlobFormat format,
            KeyFlags flags,
            out Key result)
        {
            if (algorithm == null)
                throw new ArgumentNullException(nameof(algorithm));

            SecureMemoryHandle keyHandle = null;
            byte[] publicKeyBytes = null;
            bool success = false;

            try
            {
                success = algorithm.TryImportKey(blob, format, out keyHandle, out publicKeyBytes);
            }
            finally
            {
                if (!success && keyHandle != null)
                {
                    keyHandle.Dispose();
                }
            }

            result = success ? new Key(algorithm, flags, keyHandle, publicKeyBytes) : null;
            return success;
        }

        public void Dispose()
        {
            _handle.Dispose();
        }

        public byte[] Export(
            KeyBlobFormat format)
        {
            int maxBlobSize = GetKeyBlobSize(_algorithm, format);
            byte[] blob = new byte[maxBlobSize];
            int blobSize = Export(format, blob);
            Array.Resize(ref blob, blobSize);
            return blob;
        }

        public int Export(
            KeyBlobFormat format,
            Span<byte> blob)
        {
            if (_handle.IsClosed)
                throw new ObjectDisposedException(GetType().FullName);

            if (format < 0)
            {
                bool allowExport = (_flags & KeyFlags.AllowExport) != 0;
                bool allowArchiving = (_flags & KeyFlags.AllowArchiving) != 0;

                if (!allowExport)
                {
                    if (!allowArchiving || _exported)
                    {
                        throw new InvalidOperationException();
                    }
                }

                _exported = true;

                return _algorithm.ExportKey(_handle, format, blob);
            }
            else
            {
                return _algorithm.ExportPublicKey(_publicKey.Bytes, format, blob);
            }
        }
    }
}
