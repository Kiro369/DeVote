using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;

namespace DeVote.Cryptography
{
    class ECDH
    {
        static ECDiffieHellman _ecdh = ECDiffieHellman.Create();
        public static byte[] PublicKey { get { return _ecdh.PublicKey.ExportSubjectPublicKeyInfo(); } }

        public static byte[] Encrypt(byte[] data, ECDiffieHellmanPublicKey otherPartyPublicKey, out byte[] IV)
        {
            var _aes = CreateAES(otherPartyPublicKey, out IV);
            var encryptor = _aes.CreateEncryptor();
            return encryptor.TransformFinalBlock(data, 0, data.Length);
        }
        public static byte[] Encrypt(byte[] data, byte[] otherPartyPublicKey, out byte[] IV)
        {
            var tempECDH = ECDiffieHellman.Create();
            tempECDH.ImportSubjectPublicKeyInfo(otherPartyPublicKey, out int bytesRead);

            if (bytesRead != otherPartyPublicKey.Length)
                throw new CryptographicException("Failed to import subject public key");

            return Encrypt(data, tempECDH.PublicKey, out IV);
        }
        public static byte[] Decrypt(byte[] data, ECDiffieHellmanPublicKey otherPartyPublicKey, byte[] IV)
        {
            var _aes = CreateAES(otherPartyPublicKey, IV);
            var decryptor = _aes.CreateDecryptor();
            return decryptor.TransformFinalBlock(data, 0, data.Length);
        }
        public static byte[] Decrypt(byte[] data, byte[] otherPartyPublicKey, byte[] IV)
        {
            var tempECDH = ECDiffieHellman.Create();
            tempECDH.ImportSubjectPublicKeyInfo(otherPartyPublicKey, out int bytesRead);

            if (bytesRead != otherPartyPublicKey.Length)
                throw new CryptographicException("Failed to import subject public key");

            return Decrypt(data, tempECDH.PublicKey, IV);
        }
        #region AES
        static Aes CreateAES(ECDiffieHellmanPublicKey otherPartyPublicKey, out byte[] IV)
        {
            var sharedKey = _ecdh.DeriveKeyMaterial(otherPartyPublicKey);
            var _aes = Aes.Create();
            _aes.KeySize = 256;
            _aes.BlockSize = 128;
            _aes.Key = sharedKey;
            _aes.GenerateIV();
            IV = _aes.IV;
            return _aes;
        }
        static Aes CreateAES(ECDiffieHellmanPublicKey otherPartyPublicKey, byte[] IV)
        {
            var sharedKey = _ecdh.DeriveKeyMaterial(otherPartyPublicKey);
            var _aes = Aes.Create();
            _aes.KeySize = 256;
            _aes.BlockSize = 128;
            _aes.Key = sharedKey;
            _aes.IV = IV;
            return _aes;
        }
        #endregion
    }
}
