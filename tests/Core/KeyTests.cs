using System;
using NSec.Cryptography;
using Xunit;

namespace NSec.Tests.Core
{
    public static class KeyTests
    {
        public static readonly TheoryData<Type> AsymmetricKeyAlgorithms = Registry.AsymmetricAlgorithms;
        public static readonly TheoryData<Type> SymmetricKeyAlgorithms = Registry.SymmetricAlgorithms;
        public static readonly TheoryData<Type> KeylessAlgorithms = Registry.KeylessAlgorithms;

        public static readonly TheoryData<Type, KeyBlobFormat> PublicKeyBlobFormats = Registry.PublicKeyBlobFormats;
        public static readonly TheoryData<Type, KeyBlobFormat> PrivateKeyBlobFormats = Registry.PrivateKeyBlobFormats;
        public static readonly TheoryData<Type, KeyBlobFormat> SymmetricKeyBlobFormats = Registry.SymmetricKeyBlobFormats;

        #region Properties

        [Theory]
        [MemberData(nameof(AsymmetricKeyAlgorithms))]
        public static void PropertiesAsymmetric(Type algorithmType)
        {
            var a = (Algorithm)Activator.CreateInstance(algorithmType);

            using (var k = new Key(a, new KeyCreationParameters { ExportPolicy = KeyExportPolicies.None }))
            {
                Assert.Same(a, k.Algorithm);
                Assert.Equal(KeyExportPolicies.None, k.ExportPolicy);
                Assert.NotNull(k.PublicKey);
                Assert.Same(a, k.PublicKey.Algorithm);
            }
        }

        [Theory]
        [MemberData(nameof(SymmetricKeyAlgorithms))]
        public static void PropertiesSymmetric(Type algorithmType)
        {
            var a = (Algorithm)Activator.CreateInstance(algorithmType);

            using (var k = new Key(a, new KeyCreationParameters { ExportPolicy = KeyExportPolicies.None }))
            {
                Assert.Same(a, k.Algorithm);
                Assert.Equal(KeyExportPolicies.None, k.ExportPolicy);
                Assert.Null(k.PublicKey);
            }
        }

        [Theory]
        [MemberData(nameof(AsymmetricKeyAlgorithms))]
        public static void PropertiesAsymmetricAfterDispose(Type algorithmType)
        {
            var a = (Algorithm)Activator.CreateInstance(algorithmType);

            var k = new Key(a, new KeyCreationParameters { ExportPolicy = KeyExportPolicies.None });
            k.Dispose();
            Assert.Same(a, k.Algorithm);
            Assert.Equal(KeyExportPolicies.None, k.ExportPolicy);
            Assert.NotNull(k.PublicKey);
            Assert.Same(a, k.PublicKey.Algorithm);
        }

        [Theory]
        [MemberData(nameof(SymmetricKeyAlgorithms))]
        public static void PropertiesSymmetricAfterDispose(Type algorithmType)
        {
            var a = (Algorithm)Activator.CreateInstance(algorithmType);

            var k = new Key(a, new KeyCreationParameters { ExportPolicy = KeyExportPolicies.None });
            k.Dispose();
            Assert.Same(a, k.Algorithm);
            Assert.Equal(KeyExportPolicies.None, k.ExportPolicy);
            Assert.Null(k.PublicKey);
        }

        #endregion

        #region Ctor

        [Fact]
        public static void CtorWithNullAlgorithm()
        {
            Assert.Throws<ArgumentNullException>("algorithm", () => new Key(null));
        }

        [Theory]
        [MemberData(nameof(KeylessAlgorithms))]
        public static void CtorWithAlgorithmThatDoesNotUseKeys(Type algorithmType)
        {
            var a = (Algorithm)Activator.CreateInstance(algorithmType);

            Assert.Throws<NotSupportedException>(() => new Key(a));
        }

        #endregion

        #region Create

        [Fact]
        public static void CreateWithNullAlgorithm()
        {
            Assert.Throws<ArgumentNullException>("algorithm", () => Key.Create(null));
        }

        [Theory]
        [MemberData(nameof(KeylessAlgorithms))]
        public static void CreateWithAlgorithmThatDoesNotUseKeys(Type algorithmType)
        {
            var a = (Algorithm)Activator.CreateInstance(algorithmType);

            Assert.Throws<NotSupportedException>(() => Key.Create(a));
        }

        #endregion

        #region Import

        [Fact]
        public static void ImportWithNullAlgorithm()
        {
            Assert.Throws<ArgumentNullException>("algorithm", () => Key.Import(null, ReadOnlySpan<byte>.Empty, 0));
        }

        [Theory]
        [MemberData(nameof(AsymmetricKeyAlgorithms))]
        [MemberData(nameof(SymmetricKeyAlgorithms))]
        public static void ImportWithFormatMin(Type algorithmType)
        {
            var a = (Algorithm)Activator.CreateInstance(algorithmType);

            Assert.Throws<ArgumentException>("format", () => Key.Import(a, ReadOnlySpan<byte>.Empty, (KeyBlobFormat)int.MinValue));
        }

        [Theory]
        [MemberData(nameof(AsymmetricKeyAlgorithms))]
        [MemberData(nameof(SymmetricKeyAlgorithms))]
        public static void ImportWithFormatMax(Type algorithmType)
        {
            var a = (Algorithm)Activator.CreateInstance(algorithmType);

            Assert.Throws<ArgumentException>("format", () => Key.Import(a, ReadOnlySpan<byte>.Empty, (KeyBlobFormat)int.MaxValue));
        }

        [Theory]
        [MemberData(nameof(PrivateKeyBlobFormats))]
        public static void ImportPrivateKeyEmpty(Type algorithmType, KeyBlobFormat format)
        {
            var a = (Algorithm)Activator.CreateInstance(algorithmType);

            Assert.Throws<FormatException>(() => Key.Import(a, ReadOnlySpan<byte>.Empty, format));
        }

        [Theory]
        [MemberData(nameof(SymmetricKeyBlobFormats))]
        public static void ImportSymmetricKeyEmpty(Type algorithmType, KeyBlobFormat format)
        {
            var a = (Algorithm)Activator.CreateInstance(algorithmType);

            Assert.Throws<FormatException>(() => Key.Import(a, ReadOnlySpan<byte>.Empty, format));
        }

        #endregion

        #region TryImport

        [Fact]
        public static void TryImportWithNullAlgorithm()
        {
            Assert.Throws<ArgumentNullException>("algorithm", () => Key.TryImport(null, ReadOnlySpan<byte>.Empty, 0, out var k, new KeyCreationParameters { ExportPolicy = KeyExportPolicies.None }));
        }

        [Theory]
        [MemberData(nameof(AsymmetricKeyAlgorithms))]
        [MemberData(nameof(SymmetricKeyAlgorithms))]
        public static void TryImportWithFormatMin(Type algorithmType)
        {
            var a = (Algorithm)Activator.CreateInstance(algorithmType);

            Assert.Throws<ArgumentException>("format", () => Key.TryImport(a, ReadOnlySpan<byte>.Empty, (KeyBlobFormat)int.MinValue, out var k, new KeyCreationParameters { ExportPolicy = KeyExportPolicies.None }));
        }

        [Theory]
        [MemberData(nameof(AsymmetricKeyAlgorithms))]
        [MemberData(nameof(SymmetricKeyAlgorithms))]
        public static void TryImportWithFormatMax(Type algorithmType)
        {
            var a = (Algorithm)Activator.CreateInstance(algorithmType);

            Assert.Throws<ArgumentException>("format", () => Key.TryImport(a, ReadOnlySpan<byte>.Empty, (KeyBlobFormat)int.MaxValue, out var k, new KeyCreationParameters { ExportPolicy = KeyExportPolicies.None }));
        }

        [Theory]
        [MemberData(nameof(PrivateKeyBlobFormats))]
        public static void TryImportPrivateKeyEmpty(Type algorithmType, KeyBlobFormat format)
        {
            var a = (Algorithm)Activator.CreateInstance(algorithmType);

            Assert.False(Key.TryImport(a, ReadOnlySpan<byte>.Empty, format, out var k, new KeyCreationParameters { ExportPolicy = KeyExportPolicies.None }));
        }

        [Theory]
        [MemberData(nameof(SymmetricKeyBlobFormats))]
        public static void TryImportSymmetricKeyEmpty(Type algorithmType, KeyBlobFormat format)
        {
            var a = (Algorithm)Activator.CreateInstance(algorithmType);

            Assert.False(Key.TryImport(a, ReadOnlySpan<byte>.Empty, format, out var k, new KeyCreationParameters { ExportPolicy = KeyExportPolicies.None }));
        }

        #endregion

        #region Export

        [Theory]
        [MemberData(nameof(AsymmetricKeyAlgorithms))]
        [MemberData(nameof(SymmetricKeyAlgorithms))]
        public static void ExportWithFormatMin(Type algorithmType)
        {
            var a = (Algorithm)Activator.CreateInstance(algorithmType);

            using (var k = new Key(a, new KeyCreationParameters { ExportPolicy = KeyExportPolicies.AllowPlaintextExport }))
            {
                Assert.Equal(KeyExportPolicies.AllowPlaintextExport, k.ExportPolicy);

                Assert.Throws<ArgumentException>("format", () => k.Export((KeyBlobFormat)int.MinValue));
            }
        }

        [Theory]
        [MemberData(nameof(AsymmetricKeyAlgorithms))]
        [MemberData(nameof(SymmetricKeyAlgorithms))]
        public static void ExportWithFormatMax(Type algorithmType)
        {
            var a = (Algorithm)Activator.CreateInstance(algorithmType);

            using (var k = new Key(a, new KeyCreationParameters { ExportPolicy = KeyExportPolicies.None }))
            {
                Assert.Equal(KeyExportPolicies.None, k.ExportPolicy);

                Assert.Throws<ArgumentException>("format", () => k.Export((KeyBlobFormat)int.MaxValue));
            }
        }

        [Theory]
        [MemberData(nameof(PublicKeyBlobFormats))]
        public static void ExportPublicKey(Type algorithmType, KeyBlobFormat format)
        {
            var a = (Algorithm)Activator.CreateInstance(algorithmType);

            using (var k = new Key(a, new KeyCreationParameters { ExportPolicy = KeyExportPolicies.None }))
            {
                Assert.Equal(KeyExportPolicies.None, k.ExportPolicy);

                Assert.NotNull(k.Export(format));
                Assert.NotNull(k.Export(format));
                Assert.NotNull(k.Export(format));
            }
        }

        [Theory]
        [MemberData(nameof(PrivateKeyBlobFormats))]
        public static void ExportPrivateKeyNotAllowed(Type algorithmType, KeyBlobFormat format)
        {
            var a = (Algorithm)Activator.CreateInstance(algorithmType);

            using (var k = new Key(a, new KeyCreationParameters { ExportPolicy = KeyExportPolicies.None }))
            {
                Assert.Equal(KeyExportPolicies.None, k.ExportPolicy);

                Assert.Throws<InvalidOperationException>(() => k.Export(format));
                Assert.Throws<InvalidOperationException>(() => k.Export(format));
                Assert.Throws<InvalidOperationException>(() => k.Export(format));
            }
        }

        [Theory]
        [MemberData(nameof(SymmetricKeyBlobFormats))]
        public static void ExportSymmetricKeyNotAllowed(Type algorithmType, KeyBlobFormat format)
        {
            var a = (Algorithm)Activator.CreateInstance(algorithmType);

            using (var k = new Key(a, new KeyCreationParameters { ExportPolicy = KeyExportPolicies.None }))
            {
                Assert.Equal(KeyExportPolicies.None, k.ExportPolicy);

                Assert.Throws<InvalidOperationException>(() => k.Export(format));
                Assert.Throws<InvalidOperationException>(() => k.Export(format));
                Assert.Throws<InvalidOperationException>(() => k.Export(format));
            }
        }

        [Theory]
        [MemberData(nameof(PrivateKeyBlobFormats))]
        public static void ExportPrivateKeyExportAllowed(Type algorithmType, KeyBlobFormat format)
        {
            var a = (Algorithm)Activator.CreateInstance(algorithmType);

            using (var k = new Key(a, new KeyCreationParameters { ExportPolicy = KeyExportPolicies.AllowPlaintextExport }))
            {
                Assert.Equal(KeyExportPolicies.AllowPlaintextExport, k.ExportPolicy);

                Assert.NotNull(k.Export(format));
                Assert.NotNull(k.Export(format));
                Assert.NotNull(k.Export(format));
            }
        }

        [Theory]
        [MemberData(nameof(SymmetricKeyBlobFormats))]
        public static void ExportSymmetricKeyExportAllowed(Type algorithmType, KeyBlobFormat format)
        {
            var a = (Algorithm)Activator.CreateInstance(algorithmType);

            using (var k = new Key(a, new KeyCreationParameters { ExportPolicy = KeyExportPolicies.AllowPlaintextExport }))
            {
                Assert.Equal(KeyExportPolicies.AllowPlaintextExport, k.ExportPolicy);

                Assert.NotNull(k.Export(format));
                Assert.NotNull(k.Export(format));
                Assert.NotNull(k.Export(format));
            }
        }

        [Theory]
        [MemberData(nameof(PrivateKeyBlobFormats))]
        public static void ExportPrivateKeyArchivingAllowed(Type algorithmType, KeyBlobFormat format)
        {
            var a = (Algorithm)Activator.CreateInstance(algorithmType);

            using (var k = new Key(a, new KeyCreationParameters { ExportPolicy = KeyExportPolicies.AllowPlaintextArchiving }))
            {
                Assert.Equal(KeyExportPolicies.AllowPlaintextArchiving, k.ExportPolicy);

                Assert.NotNull(k.Export(format));
                Assert.Throws<InvalidOperationException>(() => k.Export(format));
                Assert.Throws<InvalidOperationException>(() => k.Export(format));
            }
        }

        [Theory]
        [MemberData(nameof(SymmetricKeyBlobFormats))]
        public static void ExportSymmetricKeyArchivingAllowed(Type algorithmType, KeyBlobFormat format)
        {
            var a = (Algorithm)Activator.CreateInstance(algorithmType);

            using (var k = new Key(a, new KeyCreationParameters { ExportPolicy = KeyExportPolicies.AllowPlaintextArchiving }))
            {
                Assert.Equal(KeyExportPolicies.AllowPlaintextArchiving, k.ExportPolicy);

                Assert.NotNull(k.Export(format));
                Assert.Throws<InvalidOperationException>(() => k.Export(format));
                Assert.Throws<InvalidOperationException>(() => k.Export(format));
            }
        }

        [Theory]
        [MemberData(nameof(AsymmetricKeyAlgorithms))]
        public static void ExportPublicKeyAfterDispose(Type algorithmType)
        {
            var a = (Algorithm)Activator.CreateInstance(algorithmType);

            var k = new Key(a, new KeyCreationParameters { ExportPolicy = KeyExportPolicies.None });
            k.Dispose();

            k.Export(KeyBlobFormat.RawPublicKey);
            k.Export(KeyBlobFormat.RawPublicKey);
            k.Export(KeyBlobFormat.RawPublicKey);
        }

        [Theory]
        [MemberData(nameof(AsymmetricKeyAlgorithms))]
        public static void ExportPrivateKeyExportAllowedAfterDispose(Type algorithmType)
        {
            var a = (Algorithm)Activator.CreateInstance(algorithmType);

            var k = new Key(a, new KeyCreationParameters { ExportPolicy = KeyExportPolicies.AllowPlaintextExport });
            k.Dispose();

            Assert.Throws<ObjectDisposedException>(() => k.Export(KeyBlobFormat.RawPrivateKey));
            Assert.Throws<ObjectDisposedException>(() => k.Export(KeyBlobFormat.RawPrivateKey));
            Assert.Throws<ObjectDisposedException>(() => k.Export(KeyBlobFormat.RawPrivateKey));
        }

        [Theory]
        [MemberData(nameof(SymmetricKeyAlgorithms))]
        public static void ExportSymmetricKeyExportAllowedAfterDispose(Type algorithmType)
        {
            var a = (Algorithm)Activator.CreateInstance(algorithmType);

            var k = new Key(a, new KeyCreationParameters { ExportPolicy = KeyExportPolicies.AllowPlaintextExport });
            k.Dispose();

            Assert.Throws<ObjectDisposedException>(() => k.Export(KeyBlobFormat.RawSymmetricKey));
            Assert.Throws<ObjectDisposedException>(() => k.Export(KeyBlobFormat.RawSymmetricKey));
            Assert.Throws<ObjectDisposedException>(() => k.Export(KeyBlobFormat.RawSymmetricKey));
        }

        #endregion

        #region Dispose

        [Theory]
        [MemberData(nameof(AsymmetricKeyAlgorithms))]
        [MemberData(nameof(SymmetricKeyAlgorithms))]
        public static void DisposeMoreThanOnce(Type algorithmType)
        {
            var a = (Algorithm)Activator.CreateInstance(algorithmType);

            var k = new Key(a);
            k.Dispose();
            k.Dispose();
            k.Dispose();
        }

        #endregion
    }
}
