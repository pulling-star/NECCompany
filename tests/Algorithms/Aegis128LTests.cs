using System;
using NSec.Cryptography;
using Xunit;

namespace NSec.Tests.Algorithms
{
    public static class Aegis128LTests
    {
        public static readonly TheoryData<int> PlaintextLengths = Utilities.Primes;

        #region Properties

        [Fact]
        public static void Properties()
        {
            var a = AeadAlgorithm.Aegis128L;

            Assert.Equal(16, a.KeySize);
            Assert.Equal(16, a.NonceSize);
            Assert.Equal(32, a.TagSize);
        }

        #endregion

        #region Encrypt/Decrypt

        [Theory]
        [MemberData(nameof(PlaintextLengths))]
        public static void EncryptDecrypt(int length)
        {
            var a = AeadAlgorithm.Aegis128L;

            using var k = new Key(a);
            var n = Utilities.RandomBytes.Slice(0, a.NonceSize);
            var ad = Utilities.RandomBytes.Slice(0, 100);

            var expected = Utilities.RandomBytes.Slice(0, length).ToArray();

            var ciphertext = a.Encrypt(k, n, ad, expected);
            Assert.NotNull(ciphertext);
            Assert.Equal(length + a.TagSize, ciphertext.Length);

            var actual = a.Decrypt(k, n, ad, ciphertext);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
