using System;
using System.Text;
using NSec.Cryptography;
using Xunit;

namespace NSec.Tests.Examples
{
    public static class Incremental
    {
        [Fact]
        public static void Hash()
        {
            #region Incremental Hash

            // select the BLAKE2b-256 algorithm
            var algorithm = HashAlgorithm.Blake2b_256;

            // initialize the state with the algorithm
            IncrementalHash.Initialize(algorithm, out var state);

            // incrementally update the state with some data
            var lines = new[]
            {
                "It is a period of civil war.\n",
                "Rebel spaceships, striking\n",
                "from a hidden base, have won\n",
                "their first victory against\n",
                "the evil Galactic Empire.\n"
            };
            foreach (var line in lines)
            {
                IncrementalHash.Update(ref state, Encoding.UTF8.GetBytes(line));
            }

            // finalize the computation and get the result
            var hash = IncrementalHash.Finalize(ref state);

            #endregion

            Assert.Equal(algorithm.Hash(Encoding.UTF8.GetBytes(string.Concat(lines))), hash);
        }

        [Fact]
        public static void Mac()
        {
            #region Incremental MAC

            // select the BLAKE2b-256 algorithm
            var algorithm = MacAlgorithm.Blake2b_256;

            // create a new key
            using var key = Key.Create(algorithm);

            // initialize the state with the key
            IncrementalMac.Initialize(key, out var state);

            // incrementally update the state with some data
            var lines = new[]
            {
                "It is a dark time for the\n",
                "Rebellion. Although the Death\n",
                "Star has been destroyed,\n",
                "Imperial troops have driven the\n",
                "Rebel forces from their hidden\n",
                "base and pursued them across\n",
                "the galaxy.\n"
            };
            foreach (var line in lines)
            {
                IncrementalMac.Update(ref state, Encoding.UTF8.GetBytes(line));
            }

            // finalize the computation and get the result
            var mac = IncrementalMac.Finalize(ref state);

            #endregion

            Assert.Equal(algorithm.Mac(key, Encoding.UTF8.GetBytes(string.Concat(lines))), mac);
        }

        [Fact]
        public static void Signature()
        {
            #region Incremental Signing

            // define some data to be signed
            var lines = new[]
            {
                "Luke Skywalker has returned to\n",
                "his home planet of Tatooine in\n",
                "an attempt to rescue his\n",
                "friend Han Solo from the\n",
                "clutches of the vile gangster\n",
                "Jabba the Hutt.\n",
            };

            // select the Ed25519ph algorithm
            var algorithm = SignatureAlgorithm.Ed25519ph;

            // create a new key pair
            using var key = Key.Create(algorithm);

            // initialize the state with the private key
            IncrementalSignature.Initialize(key, out var state);

            // incrementally update the state with the data
            foreach (var line in lines)
            {
                IncrementalSignature.Update(ref state, Encoding.UTF8.GetBytes(line));
            }

            // finalize the computation and get the result
            var signature = IncrementalSignature.Finalize(ref state);

            #endregion

            Assert.Equal(algorithm.Sign(key, Encoding.UTF8.GetBytes(string.Concat(lines))), signature);
        }

        [Fact]
        public static void SignatureVerification()
        {
            using var key = Key.Create(SignatureAlgorithm.Ed25519ph);

            #region Incremental Signature Verification

            // define some data to be verified
            var lines = new[]
            {
                "Luke Skywalker has returned to\n",
                "his home planet of Tatooine in\n",
                "an attempt to rescue his\n",
                "friend Han Solo from the\n",
                "clutches of the vile gangster\n",
                "Jabba the Hutt.\n",
            };

            // select the Ed25519ph algorithm
            var algorithm = SignatureAlgorithm.Ed25519ph;

            // obtain the public key
            var publicKey = /*{*/key.PublicKey;/*}*/

            // obtain the signature
            var signature = /*{*/algorithm.Sign(key, Encoding.UTF8.GetBytes(string.Concat(lines)));/*}*/

            // initialize the state with the public key
            IncrementalSignatureVerification.Initialize(publicKey, out var state);

            // incrementally update the state with the data
            foreach (var line in lines)
            {
                IncrementalSignatureVerification.Update(ref state, Encoding.UTF8.GetBytes(line));
            }

            // verify the data using the signature
            if (IncrementalSignatureVerification.FinalizeAndVerify(ref state, signature))
            {
                // verified!
                /*{*//*}*/
            }

            #endregion
        }
    }
}
