using System;
using NSec.Cryptography;
using Xunit;

namespace NSec.Tests.Algorithms
{
    public static class Blake2bMacTests
    {
        #region Properties

        [Fact]
        public static void Properties()
        {
            var a = new Blake2bMac();

            Assert.Equal(16, a.MinKeySize);
            Assert.Equal(32, a.DefaultKeySize);
            Assert.Equal(64, a.MaxKeySize);

            Assert.Equal(32, a.MacSize);
        }

        #endregion

        #region Export #1

        [Theory]
        [InlineData(16)]
        [InlineData(32)]
        [InlineData(64)]
        public static void ExportImportRaw(int keySize)
        {
            var a = new Blake2bMac();
            var b = Utilities.RandomBytes.Slice(0, keySize);

            using (var k = Key.Import(a, b, KeyBlobFormat.RawSymmetricKey, new KeyCreationParameters { ExportPolicy = KeyExportPolicies.AllowPlaintextArchiving }))
            {
                Assert.Equal(KeyExportPolicies.AllowPlaintextArchiving, k.ExportPolicy);

                var expected = b.ToArray();
                var actual = k.Export(KeyBlobFormat.RawSymmetricKey);

                Assert.Equal(expected, actual);
            }
        }

        [Theory]
        [InlineData(16)]
        [InlineData(32)]
        [InlineData(64)]
        public static void ExportImportNSec(int keySize)
        {
            var a = new Blake2bMac();
            var b = Utilities.RandomBytes.Slice(0, keySize);

            using (var k1 = Key.Import(a, b, KeyBlobFormat.RawSymmetricKey, new KeyCreationParameters { ExportPolicy = KeyExportPolicies.AllowPlaintextArchiving }))
            {
                Assert.Equal(KeyExportPolicies.AllowPlaintextArchiving, k1.ExportPolicy);

                var n = k1.Export(KeyBlobFormat.NSecSymmetricKey);
                Assert.NotNull(n);

                using (var k2 = Key.Import(a, n, KeyBlobFormat.NSecSymmetricKey, new KeyCreationParameters { ExportPolicy = KeyExportPolicies.AllowPlaintextArchiving }))
                {
                    var expected = b.ToArray();
                    var actual = k2.Export(KeyBlobFormat.RawSymmetricKey);

                    Assert.Equal(expected, actual);
                }
            }
        }

        #endregion

        #region Mac #1

        [Fact]
        public static void HashWithNullKey()
        {
            var a = new Blake2bMac();

            Assert.Throws<ArgumentNullException>("key", () => a.Mac(null, ReadOnlySpan<byte>.Empty));
        }

        [Fact]
        public static void HashWithWrongKey()
        {
            var a = new Blake2bMac();

            using (var k = new Key(new Ed25519()))
            {
                Assert.Throws<ArgumentException>("key", () => a.Mac(k, ReadOnlySpan<byte>.Empty));
            }
        }

        [Fact]
        public static void HashWithKeySuccess()
        {
            var a = new Blake2bMac();

            using (var k = new Key(a))
            {
                var b = a.Mac(k, ReadOnlySpan<byte>.Empty);

                Assert.NotNull(b);
                Assert.Equal(a.MacSize, b.Length);
            }
        }

        #endregion

        #region Mac #3

        [Fact]
        public static void HashWithSpanWithNullKey()
        {
            var a = new Blake2bMac();

            Assert.Throws<ArgumentNullException>("key", () => a.Mac(null, ReadOnlySpan<byte>.Empty, Span<byte>.Empty));
        }

        [Fact]
        public static void HashWithSpanWithWrongKey()
        {
            var a = new Blake2bMac();

            using (var k = new Key(new Ed25519()))
            {
                Assert.Throws<ArgumentException>("key", () => a.Mac(k, ReadOnlySpan<byte>.Empty, Span<byte>.Empty));
            }
        }

        [Fact]
        public static void HashWithSpanTooSmall()
        {
            var a = new Blake2bMac();

            using (var k = new Key(a))
            {
                Assert.Throws<ArgumentException>("mac", () => a.Mac(k, ReadOnlySpan<byte>.Empty, new byte[a.MacSize - 1]));
            }
        }

        [Fact]
        public static void HashWithSpanTooLarge()
        {
            var a = new Blake2bMac();

            using (var k = new Key(a))
            {
                Assert.Throws<ArgumentException>("mac", () => a.Mac(k, ReadOnlySpan<byte>.Empty, new byte[a.MacSize + 1]));
            }
        }

        [Fact]
        public static void HashWithSpanSuccess()
        {
            var a = new Blake2bMac();

            using (var k = new Key(a))
            {
                a.Mac(k, ReadOnlySpan<byte>.Empty, new byte[a.MacSize]);
            }
        }

        #endregion

        #region CreateKey

        [Fact]
        public static void CreateKey()
        {
            var a = new Blake2bMac();

            using (var k = new Key(a, new KeyCreationParameters { ExportPolicy = KeyExportPolicies.AllowPlaintextArchiving }))
            {
                var actual = k.Export(KeyBlobFormat.RawSymmetricKey);

                var unexpected = new byte[actual.Length];
                Utilities.Fill(unexpected, actual[0]);

                Assert.NotEqual(unexpected, actual);
            }
        }

        #endregion
    }
}
