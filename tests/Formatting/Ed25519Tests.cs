using System;
using System.Text;
using NSec.Cryptography;
using NSec.Cryptography.Formatting;
using Xunit;

namespace NSec.Tests.Formatting
{
    public static class Ed25519Tests
    {
        private static readonly byte[] s_oid = { 0x2B, 0x65, 0x70 };

        [Fact]
        public static void PkixPrivateKey()
        {
            var a = new Ed25519();
            var b = Utilities.RandomBytes.Slice(0, a.PrivateKeySize);

            using (var k = Key.Import(a, b, KeyBlobFormat.RawPrivateKey, KeyFlags.AllowExport))
            {
                var blob = new ReadOnlySpan<byte>(k.Export(KeyBlobFormat.PkixPrivateKey));

                var reader = new Asn1Reader(ref blob);
                reader.BeginSequence();
                Assert.Equal(0, reader.Integer32());
                reader.BeginSequence();
                Assert.Equal(s_oid, reader.ObjectIdentifier().ToArray());
                reader.End();
                var edPrivateKey = reader.OctetString();
                reader.End();
                Assert.True(reader.SuccessComplete);

                var reader2 = new Asn1Reader(ref edPrivateKey);
                Assert.Equal(b.ToArray(), reader2.OctetString().ToArray());
                Assert.True(reader2.SuccessComplete);
            }
        }

        [Fact]
        public static void PkixPrivateKeyText()
        {
            var a = new Ed25519();
            var b = Utilities.RandomBytes.Slice(0, a.PrivateKeySize);

            using (var k = Key.Import(a, b, KeyBlobFormat.RawPrivateKey, KeyFlags.AllowExport))
            {
                var expected = Encoding.UTF8.GetBytes(
                    "-----BEGIN PRIVATE KEY-----\r\n" +
                    Convert.ToBase64String(k.Export(KeyBlobFormat.PkixPrivateKey)) + "\r\n" +
                    "-----END PRIVATE KEY-----\r\n");

                var actual = k.Export(KeyBlobFormat.PkixPrivateKeyText);

                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public static void PkixPublicKey()
        {
            var a = new Ed25519();
            var b = Utilities.RandomBytes.Slice(0, a.PrivateKeySize);

            using (var k = Key.Import(a, b, KeyBlobFormat.RawPrivateKey))
            {
                var publicKeyBytes = k.Export(KeyBlobFormat.RawPublicKey);
                var blob = new ReadOnlySpan<byte>(k.Export(KeyBlobFormat.PkixPublicKey));

                var reader = new Asn1Reader(ref blob);
                reader.BeginSequence();
                reader.BeginSequence();
                Assert.Equal(s_oid, reader.ObjectIdentifier().ToArray());
                reader.End();
                Assert.Equal(publicKeyBytes, reader.BitString().ToArray());
                reader.End();
                Assert.True(reader.SuccessComplete);
            }
        }

        [Fact]
        public static void PkixPublicKeyText()
        {
            var a = new Ed25519();
            var b = Utilities.RandomBytes.Slice(0, a.PrivateKeySize);

            using (var k = Key.Import(a, b, KeyBlobFormat.RawPrivateKey))
            {
                var expected = Encoding.UTF8.GetBytes(
                    "-----BEGIN PUBLIC KEY-----\r\n" +
                    Convert.ToBase64String(k.Export(KeyBlobFormat.PkixPublicKey)) + "\r\n" +
                    "-----END PUBLIC KEY-----\r\n");

                var actual = k.Export(KeyBlobFormat.PkixPublicKeyText);

                Assert.Equal(expected, actual);
            }
        }
    }
}
