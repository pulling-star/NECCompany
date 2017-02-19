using System;
using System.Reflection;
using System.Resources;

namespace NSec.Cryptography
{
    internal static class Error
    {
        public static readonly string ArgumentExceptionMessage = new ArgumentException().Message;

        private static ResourceManager s_resourceManager;

        private static ResourceManager ResourceManager
            => s_resourceManager ?? (s_resourceManager = new ResourceManager("NSec.Resources.Strings", typeof(Error).GetTypeInfo().Assembly));

        internal static ArgumentException Argument_CiphertextLength(
            string paramName)
        {
            return new ArgumentException(ResourceManager.GetString("Argument_CiphertextLength"), paramName);
        }

        internal static ArgumentException Argument_DeriveInvalidCount(
            string paramName, 
            string arg0)
        {
            return new ArgumentException(string.Format(ResourceManager.GetString("Argument_DeriveInvalidCount"), arg0), paramName);
        }

        internal static ArgumentException Argument_FormatNotSupported(
            string paramName, 
            string arg0)
        {
            return new ArgumentException(string.Format(ResourceManager.GetString("Argument_FormatNotSupported"), arg0), paramName);
        }

        internal static ArgumentException Argument_HashSize(
            string paramName, 
            string arg0, 
            string arg1, 
            string arg2)
        {
            return new ArgumentException(string.Format(ResourceManager.GetString("Argument_HashSize"), arg0, arg1, arg2), paramName);
        }

        internal static ArgumentException Argument_InvalidPrkLength(
            string paramName, 
            string arg0)
        {
            return new ArgumentException(string.Format(ResourceManager.GetString("Argument_InvalidPrkLength"), arg0), paramName);
        }

        internal static ArgumentException Argument_InvalidPrkLengthExact(
            string paramName, 
            string arg0)
        {
            return new ArgumentException(string.Format(ResourceManager.GetString("Argument_InvalidPrkLengthExact"), arg0), paramName);
        }

        internal static ArgumentException Argument_KeyWrongAlgorithm(
            string paramName, 
            string arg0, 
            string arg1)
        {
            return new ArgumentException(string.Format(ResourceManager.GetString("Argument_KeyWrongAlgorithm"), arg0, arg1), paramName);
        }

        internal static ArgumentException Argument_MacSize(
            string paramName, 
            string arg0, 
            string arg1, 
            string arg2)
        {
            return new ArgumentException(string.Format(ResourceManager.GetString("Argument_MacSize"), arg0, arg1, arg2), paramName);
        }

        internal static ArgumentException Argument_NonceLength(
            string paramName, 
            string arg0)
        {
            return new ArgumentException(string.Format(ResourceManager.GetString("Argument_NonceLength"), arg0), paramName);
        }

        internal static ArgumentException Argument_PlaintextLength(
            string paramName)
        {
            return new ArgumentException(ResourceManager.GetString("Argument_PlaintextLength"), paramName);
        }

        internal static ArgumentException Argument_PlaintextTooLong(
            string paramName)
        {
            return new ArgumentException(ResourceManager.GetString("Argument_PlaintextTooLong"), paramName);
        }

        internal static ArgumentException Argument_SaltNotSupported(
            string paramName)
        {
            return new ArgumentException(ResourceManager.GetString("Argument_SaltNotSupported"), paramName);
        }

        internal static ArgumentException Argument_SharedSecretLength(
            string paramName, 
            string arg0)
        {
            return new ArgumentException(string.Format(ResourceManager.GetString("Argument_SharedSecretLength"), arg0), paramName);
        }

        internal static ArgumentException Argument_SignatureLength(
            string paramName, 
            string arg0)
        {
            return new ArgumentException(string.Format(ResourceManager.GetString("Argument_SignatureLength"), arg0), paramName);
        }

        internal static ArgumentException Argument_SpanBlob(
            string paramName)
        {
            return new ArgumentException(ResourceManager.GetString("Argument_SpanBlob"), paramName);
        }

        internal static ArgumentNullException ArgumentNull_Algorithm(
            string paramName)
        {
            return new ArgumentNullException(paramName, ResourceManager.GetString("ArgumentNull_Algorithm"));
        }

        internal static ArgumentNullException ArgumentNull_Key(
            string paramName)
        {
            return new ArgumentNullException(paramName, ResourceManager.GetString("ArgumentNull_Key"));
        }

        internal static ArgumentNullException ArgumentNull_SharedSecret(
            string paramName)
        {
            return new ArgumentNullException(paramName, ResourceManager.GetString("ArgumentNull_SharedSecret"));
        }

        internal static ArgumentOutOfRangeException ArgumentOutOfRange_DeriveInvalidCount(
            string paramName, 
            string arg0)
        {
            return new ArgumentOutOfRangeException(paramName, string.Format(ResourceManager.GetString("ArgumentOutOfRange_DeriveInvalidCount"), arg0));
        }

        internal static ArgumentOutOfRangeException ArgumentOutOfRange_DeriveNegativeCount(
            string paramName)
        {
            return new ArgumentOutOfRangeException(paramName, ResourceManager.GetString("ArgumentOutOfRange_DeriveNegativeCount"));
        }

        internal static ArgumentOutOfRangeException ArgumentOutOfRange_GenerateNegativeCount(
            string paramName)
        {
            return new ArgumentOutOfRangeException(paramName, ResourceManager.GetString("ArgumentOutOfRange_GenerateNegativeCount"));
        }

        internal static ArgumentOutOfRangeException ArgumentOutOfRange_HashSize(
            string paramName, 
            string arg0, 
            string arg1, 
            string arg2)
        {
            return new ArgumentOutOfRangeException(paramName, string.Format(ResourceManager.GetString("ArgumentOutOfRange_HashSize"), arg0, arg1, arg2));
        }

        internal static ArgumentOutOfRangeException ArgumentOutOfRange_MacSize(
            string paramName, 
            string arg0, 
            string arg1, 
            string arg2)
        {
            return new ArgumentOutOfRangeException(paramName, string.Format(ResourceManager.GetString("ArgumentOutOfRange_MacSize"), arg0, arg1, arg2));
        }

        internal static CryptographicException Cryptographic_DecryptionFailed()
        {
            return new CryptographicException(ResourceManager.GetString("Cryptographic_DecryptionFailed"));
        }

        internal static CryptographicException Cryptographic_InitializationFailed()
        {
            return new CryptographicException(ResourceManager.GetString("Cryptographic_InitializationFailed"));
        }

        internal static CryptographicException Cryptographic_KeyAgreementFailed()
        {
            return new CryptographicException(ResourceManager.GetString("Cryptographic_KeyAgreementFailed"));
        }

        internal static CryptographicException Cryptographic_LibsodiumOutdated(
            string arg0, 
            string arg1)
        {
            return new CryptographicException(string.Format(ResourceManager.GetString("Cryptographic_LibsodiumOutdated"), arg0, arg1));
        }

        internal static CryptographicException Cryptographic_VerificationFailed()
        {
            return new CryptographicException(ResourceManager.GetString("Cryptographic_VerificationFailed"));
        }

        internal static FormatException Format_InvalidBlob()
        {
            return new FormatException(ResourceManager.GetString("Format_InvalidBlob"));
        }

        internal static InvalidOperationException InvalidOperation_AlreadyArchived()
        {
            return new InvalidOperationException(ResourceManager.GetString("InvalidOperation_AlreadyArchived"));
        }

        internal static InvalidOperationException InvalidOperation_ExportNotAllowed()
        {
            return new InvalidOperationException(ResourceManager.GetString("InvalidOperation_ExportNotAllowed"));
        }

        internal static NotSupportedException NotSupported_CreateKey()
        {
            return new NotSupportedException(ResourceManager.GetString("NotSupported_CreateKey"));
        }

        internal static NotSupportedException NotSupported_ExportKey()
        {
            return new NotSupportedException(ResourceManager.GetString("NotSupported_ExportKey"));
        }

        internal static NotSupportedException NotSupported_ImportKey()
        {
            return new NotSupportedException(ResourceManager.GetString("NotSupported_ImportKey"));
        }

        internal static NotSupportedException NotSupported_Operation()
        {
            return new NotSupportedException(ResourceManager.GetString("NotSupported_Operation"));
        }

        internal static ObjectDisposedException ObjectDisposed_Key()
        {
            return new ObjectDisposedException(ResourceManager.GetString("ObjectDisposed_Key"));
        }

        internal static PlatformNotSupportedException PlatformNotSupported_Algorithm()
        {
            return new PlatformNotSupportedException(ResourceManager.GetString("PlatformNotSupported_Algorithm"));
        }
    }
}
