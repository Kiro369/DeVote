using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;

namespace DeVote.Cryptography
{
    class ECDH
    {
        static ECDiffieHellman _ecdh = ECDiffieHellman.Create();
        public static ECDiffieHellmanPublicKey PublicKey { get { return _ecdh.PublicKey; } }
        public static void test()
        {
            using (ECDiffieHellman alice = ECDiffieHellman.Create())
            {
                using (ECDiffieHellman bob = ECDiffieHellman.Create())
                {
                    var aliceSharedKey = alice.DeriveKeyMaterial(bob.PublicKey);
                    var bobSharedKey = bob.DeriveKeyMaterial(alice.PublicKey);
                    var ar = bob.PublicKey.ToByteArray();
                    ECDiffieHellmanPublicKey key = ECDiffieHellmanCngPublicKey.FromByteArray(ar, CngKeyBlobFormat.EccPublicBlob);
                    Encrypt(new byte[] { 1, 1, 1, 1 }, key, out byte[] IV);
                }
            }
        }
    
        public static byte[] Encrypt(byte[] data, ECDiffieHellmanPublicKey otherPartyPublicKey, out byte[] IV)
        {
            var _aes = CreateAES(otherPartyPublicKey, out IV);
            var encryptor = _aes.CreateEncryptor();
            return encryptor.TransformFinalBlock(data, 0, data.Length);
        }
        public static byte[] Encrypt(byte[] data, byte[] otherPartyPublicKey, out byte[] IV)
        {
            return Encrypt(data, ECDiffieHellmanCngPublicKey.FromByteArray(otherPartyPublicKey, CngKeyBlobFormat.EccPublicBlob), out IV);
        }
        public static byte[] Decrypt(byte[] data, ECDiffieHellmanPublicKey otherPartyPublicKey, byte[] IV)
        {
            var _aes = CreateAES(otherPartyPublicKey, IV);
            var decryptor = _aes.CreateDecryptor();
            return decryptor.TransformFinalBlock(data, 0, data.Length);
        }
        public static byte[] Decrypt(byte[] data, byte[] otherPartyPublicKey, byte[] IV)
        {
            return Decrypt(data, ECDiffieHellmanCngPublicKey.FromByteArray(otherPartyPublicKey, CngKeyBlobFormat.EccPublicBlob), IV);
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
