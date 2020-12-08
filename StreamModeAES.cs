using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CryptLab2
{
    class StreamModeAES
    {
        byte[] data = { 0x32, 0x43, 0xf6, 0xa8, 0x88, 0x5a, 0x30, 0x8d, 0x31, 0x31, 0x98, 0xa2, 0xe0, 0x37, 0x07, 0x34 };
        byte[] ckey = { 0x2b, 0x7e, 0x15, 0x16, 0x28, 0xae, 0xd2, 0xa6, 0xab, 0xf7, 0x15, 0x88, 0x09, 0xcf, 0x4f, 0x3c };

        public StreamModeAES()
        {

        }

        public void ModeECB(Stream ins, Stream outs)
        {
            AES encryption = new AES();
            byte[] small = new byte[16];
            while (true)
            {
                var count = ins.Read(small);
                var res = encryption.Encrypt(small, ckey);
                outs.Write(res);
                if (count < 16)
                {
                    break;
                }
            }
        }

        public void ModeCBC(Stream ins, Stream outs, byte[] IV)
        {
            AES encryption = new AES();
            byte[] small = new byte[16];
            while (true)
            {
                var count = ins.Read(small);
                for (int i = 0; i < 16; i++)
                {
                    small[i] ^= IV[i];
                }
                IV = encryption.Encrypt(small, ckey);
                outs.Write(IV);
                if (count < 16)
                {
                    break;
                }
            }
        }

        public void ModeCFB(Stream ins, Stream outs, byte[] IV, byte s)
        {
            AES encryption = new AES();
            byte[] small = new byte[s];
            while (true)
            {
                var count = ins.Read(small);
                var IVe = encryption.Encrypt(IV, ckey);
                for (int i = 0; i < s; i++)
                {
                    small[i] ^= IVe[i];
                }
                for (int i = s; i < 16; i++)
                {
                    IV[i - s] = IV[i];
                }
                for (int i = 0; i < s; i++)
                {
                    IV[16 - s + i] = small[i];
                }
                outs.Write(small);
                if (count < 128)
                {
                    break;
                }
            }
        }

        public void ModeOFB(Stream ins, Stream outs, byte[] IV)
        {
            AES encryption = new AES();
            while (true)
            {
                int count = 0;
                IV = encryption.Encrypt(IV, ckey);
                while (count < IV.Length)
                {
                    var readedByte = ins.ReadByte();

                    if (readedByte == -1)
                    {
                        break;
                    }
                    outs.WriteByte((byte)((byte)(readedByte) ^ IV[count]));
                    count++;
                }
            }
        }

        public void ModeCounter(Stream ins, Stream outs)
        {
            AES encryption = new AES();
            byte[] N = new byte[16];
            N[15] = 1;
            while (true)
            {
                int count = 0;
                var enc = encryption.Encrypt(N, ckey);
                while (count < enc.Length)
                {
                    var readedByte = ins.ReadByte();

                    if (readedByte == -1)
                    {
                        break;
                    }
                    outs.WriteByte((byte)((byte)(readedByte) ^ enc[count]));
                    count++;
                }
                IncreaseN(N);
            }
        }

        private void IncreaseN(byte[] N)
        {
            bool F = true;
            int pos = N.Length - 1;
            while (F)
            {
                if (pos < 0)
                {
                    pos = N.Length - 1;
                }
                if (N[pos] < 255)
                {
                    N[pos] += 1;
                    F = true;
                }
                else
                {
                    N[pos] = 0;
                    pos--;
                }

            }
        }
    }
}
