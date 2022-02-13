using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;

namespace DeVote.Network.Cryptography
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
                    using (ECDiffieHellman eve = ECDiffieHellman.Create())
                    {
                        var aliceSharedKey = alice.DeriveKeyMaterial(bob.PublicKey);

                        var eveAlice = eve.DeriveKeyMaterial(bob.PublicKey);

                        var bobSharedKey = bob.DeriveKeyMaterial(alice.PublicKey);

                        var eveBob = eve.DeriveKeyMaterial(alice.PublicKey);

                        Console.WriteLine(aliceSharedKey.SequenceEqual(bobSharedKey));

                        Console.WriteLine(eveAlice.SequenceEqual(aliceSharedKey));
                        Console.WriteLine(eveBob.SequenceEqual(bobSharedKey));
                        Console.WriteLine(eveAlice.SequenceEqual(eveBob));

                    }
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
            return Encrypt(data, new ECDHPublicKey(otherPartyPublicKey), out IV);
        }
        public static byte[] Decrypt(byte[] data, ECDiffieHellmanPublicKey otherPartyPublicKey, byte[] IV)
        {
            var _aes = CreateAES(otherPartyPublicKey, IV);
            var decryptor = _aes.CreateDecryptor();
            return decryptor.TransformFinalBlock(data, 0, data.Length);
        }
        public static byte[] Decrypt(byte[] data, byte[] otherPartyPublicKey, byte[] IV)
        {
            return Decrypt(data, new ECDHPublicKey(otherPartyPublicKey), IV);
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
    class ECDHPublicKey : ECDiffieHellmanPublicKey
    {
        public ECDHPublicKey(byte[] t) : base(t)
        {

        }
    }
}
