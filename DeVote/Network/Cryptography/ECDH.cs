using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;

namespace DeVote.Network.Cryptography
{
    class ECDH
    {
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
    }
}
