namespace CryptLab2
{
    internal class Salsa20
    {
        private uint[] _state = new uint[16];
        private readonly byte[] key;
        private readonly byte[] nonce;

        public Salsa20(byte[] key, byte[] nonce)
        {
            this.key = key;
            this.nonce = nonce;
        }

        private void Init()
        {
            _state[0] = 0x61707865;
            _state[5] = 0x3320646e;
            _state[10] = 0x79622d32;
            _state[15] = 0x6b206574;
            //key
            _state[1] = ((uint)((key[3]) << 24)) | ((uint)(key[2] << 16)) | (uint)(key[1] << 8) | key[0];
            _state[2] = ((uint)((key[7]) << 24)) | ((uint)(key[6] << 16)) | (uint)(key[5] << 8) | key[4];
            _state[3] = ((uint)((key[11]) << 24)) | ((uint)(key[10] << 16)) | (uint)(key[9] << 8) | key[8];
            _state[4] = ((uint)((key[15]) << 24)) | ((uint)(key[14] << 16)) | (uint)(key[13] << 8) | key[12];
            _state[11] = ((uint)((key[19]) << 24)) | ((uint)(key[18] << 16)) | (uint)(key[17] << 8) | key[16];
            _state[12] = ((uint)((key[23]) << 24)) | ((uint)(key[22] << 16)) | (uint)(key[21] << 8) | key[20];
            _state[13] = ((uint)((key[27]) << 24)) | ((uint)(key[26] << 16)) | (uint)(key[25] << 8) | key[24];
            _state[14] = ((uint)((key[31]) << 24)) | ((uint)(key[30] << 16)) | (uint)(key[29] << 8) | key[28];
            //nonce
            _state[6] = ((uint)((nonce[3]) << 24)) | ((uint)(nonce[2] << 16)) | (uint)(nonce[1] << 8) | nonce[0];
            _state[7] = ((uint)((nonce[7]) << 24)) | ((uint)(nonce[6] << 16)) | (uint)(nonce[5] << 8) | nonce[4];
        }

        public uint[] Encrypt(byte[] block)
        {
            Init();
            //block
            _state[8] = ((uint)((block[3]) << 24)) | ((uint)(block[2] << 16)) | (uint)(block[1] << 8) | block[0];
            _state[9] = ((uint)((block[7]) << 24)) | ((uint)(block[6] << 16)) | (uint)(block[5] << 8) | block[4];
            for (int i = 1; i <= 20; i++)
            {
                if (i % 2 == 1)
                {
                    QR(0, 4, 8, 12);
                    QR(5, 9, 13, 1);
                    QR(10, 14, 2, 6);
                    QR(15, 3, 7, 11);
                }
                else
                {
                    QR(0, 1, 2, 3);
                    QR(5, 6, 7, 4);
                    QR(10, 11, 8, 9);
                    QR(15, 12, 13, 14);
                }
            }
            uint[] res = new uint[2];
            res[0] = _state[8];
            res[1] = _state[9];
            return res;
        }

        private void QR(byte a, byte b, byte c, byte d)
        {
            _state[b] ^= RotateByNLeft((_state[a] + _state[d]), 7);
            _state[c] ^= RotateByNLeft((_state[b] + _state[a]), 9);
            _state[d] ^= RotateByNLeft((_state[c] + _state[d]), 13);
            _state[a] ^= RotateByNLeft((_state[d] + _state[c]), 18);
        }

        private uint RotateByNLeft(uint x, int n)
        {
            return (x << n) | (x >> (32 - n));
        }
    }
}