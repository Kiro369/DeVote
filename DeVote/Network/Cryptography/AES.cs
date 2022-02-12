using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace DeVote.Network.Cryptography
{
    class AES
    {
        static Aes _aes;
        static AESKey _key = null;
        public static AESKey Key
        {
            get { return _key; }
            set
            {
                _key = value;
                _aes = Aes.Create();
                _aes.KeySize = 256;
                _aes.BlockSize = 128;
                _aes.Key = _key.Key;
                _aes.IV = _key.IV;
            }
        }
        public static byte[] Encrypt(byte[] data)
        {
            var encryptor = _aes.CreateEncryptor();
            return encryptor.TransformFinalBlock(data, 0, data.Length);
        }
        public static byte[] Decrypt(byte[] data)
        {
            var decryptor = _aes.CreateDecryptor();
            return decryptor.TransformFinalBlock(data, 0, data.Length);
        }
    }
}
