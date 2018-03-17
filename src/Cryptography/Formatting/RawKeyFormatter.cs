using System;
using System.Diagnostics;
using static Interop.Libsodium;

namespace NSec.Cryptography.Formatting
{
    internal sealed class RawKeyFormatter
    {
        private readonly int _maxKeySize;
        private readonly int _minKeySize;

        public RawKeyFormatter(
            int keySize)
        {
            Debug.Assert(keySize >= 0);

            _minKeySize = keySize;
            _maxKeySize = keySize;
        }

        public RawKeyFormatter(
            int minKeySize,
            int maxKeySize)
        {
            Debug.Assert(minKeySize >= 0);
            Debug.Assert(maxKeySize >= minKeySize);

            _minKeySize = minKeySize;
            _maxKeySize = maxKeySize;
        }

        public bool TryExport(
            SecureMemoryHandle keyHandle,
            Span<byte> blob,
            out int blobSize)
        {
            Debug.Assert(keyHandle != null);
            Debug.Assert(keyHandle.Length >= _minKeySize);
            Debug.Assert(keyHandle.Length <= _maxKeySize);

            blobSize = keyHandle.Length;

            if (blob.Length < blobSize)
            {
                return false;
            }

            keyHandle.Export(blob);
            return true;
        }

        public bool TryImport(
            ReadOnlySpan<byte> blob,
            out SecureMemoryHandle keyHandle)
        {
            if (blob.Length < _minKeySize ||
                blob.Length > _maxKeySize)
            {
                keyHandle = null;
                return false;
            }

            SecureMemoryHandle.Import(blob, out keyHandle);
            return true;
        }
    }
}
