using System;
using System.Diagnostics;

namespace NSec.Experimental.Asn1
{
    // ITU-T X.690 5.0 DER
    internal readonly struct Asn1Oid
    {
        private readonly byte[] _bytes;

        public Asn1Oid(
            uint first,
            uint second,
            params uint[] rest)
        {
            int length = GetLength(first * 40 + second);
            for (int i = 0; i < rest.Length; i++)
            {
                length += GetLength(rest[i]);
            }
            byte[] bytes = new byte[length];
            int pos = Encode(first * 40 + second, bytes, 0);
            for (int i = 0; i < rest.Length; i++)
            {
                pos += Encode(rest[i], bytes, pos);
            }
            _bytes = bytes;
        }

        public ReadOnlySpan<byte> Bytes => _bytes;

        private static int Encode(
            uint value,
            byte[] buffer,
            int pos)
        {
            int start = pos;
            Debug.Assert((value & 0xF0000000) == 0);
            if ((value & 0xFFE00000) != 0)
            {
                buffer[pos++] = (byte)((value >> 21) & 0x7F | 0x80);
            }
            if ((value & 0xFFFFC000) != 0)
            {
                buffer[pos++] = (byte)((value >> 14) & 0x7F | 0x80);
            }
            if ((value & 0xFFFFFF80) != 0)
            {
                buffer[pos++] = (byte)((value >> 7) & 0x7F | 0x80);
            }
            buffer[pos++] = (byte)(value & 0x7F);
            return pos - start;
        }

        private static int GetLength(
            uint value)
        {
            int length = 0;
            Debug.Assert((value & 0xF0000000) == 0);
            if ((value & 0xFFE00000) != 0)
            {
                length++;
            }
            if ((value & 0xFFFFC000) != 0)
            {
                length++;
            }
            if ((value & 0xFFFFFF80) != 0)
            {
                length++;
            }
            length++;
            return length;
        }
    }
}
