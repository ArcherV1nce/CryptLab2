using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CryptLab2
{
    class RC4
    {
        private readonly byte[] _key;

        private Lazy<byte[]> _state;

        public RC4(byte[] key)
        {
            _key = ExpandKey(key);
            _state = new Lazy<byte[]>(() => {
                var state = new byte[256];
                for (int i = 0; i < 256; i++)
                {
                    state[i] = (byte)i;
                }

                byte j = 0;
                for (int i = 0; i < 256; i++)
                {
                    j = (byte)((state[i] + _key[i] + j) % 256);
                    var temp = state[i];
                    state[i] = state[j];
                    state[j] = temp;

                }
                return state;
            });
        }

        private byte[] ExpandKey(byte[] key)
        {
            var expandedKey = new byte[256];
            int length = key.Length;
            if (length == 256)
            {
                return key;
            }
            else if (length < 256)
            {
                Array.Copy(key, 0, expandedKey, 0, length);
                int position = 0;
                while (length != 256)
                {
                    expandedKey[length] = key[position];
                    position++;
                    length++;
                    if (position == key.Length)
                    {
                        position = 0;
                    }
                }
            }
            else
            {
                throw new Exception("Invalid key");
            }
            return expandedKey;
        }
        byte _i = 0;
        byte _j = 0;
        byte _t = 0;
        public void Encode(Stream stream, Stream writeStream)
        {
            while (true)
            {
                var readedByte = stream.ReadByte();
                if (readedByte == -1) break;
                _i++;
                _j = (byte)((_j + _state.Value[_i]) % 256);
                var temp = _state.Value[_i];
                _state.Value[_i] = _state.Value[_j];
                _state.Value[_j] = temp;
                _t = (byte)((_state.Value[_i] + _state.Value[_j]) % 256);
                var stateByte = _state.Value[_t];
                byte encodedByte = (byte)(readedByte ^ stateByte);
                writeStream.WriteByte(encodedByte);
            }
        }
    }
}