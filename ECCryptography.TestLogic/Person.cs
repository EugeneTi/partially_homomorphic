using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using ECCryptography.Core;
using EllipticCurveCryptography.Common;
using RSACryptography.Core;

namespace ECCryptography.TestLogic
{
    public class Person
    {
        protected Key KeyPair;

        protected CryptoManager Manager;

        public Algorithm KnownAlgorithm { get; }

        public Person(Algorithm knownAlgorithm)
        {
            KnownAlgorithm = knownAlgorithm;
        }

        public void GenerateKeys()
        {
            if (KnownAlgorithm == Algorithm.RSA)
            {
                Manager = new RSAManager();
                KeyPair = Manager.GenerateKey();
            }

            if (KnownAlgorithm == Algorithm.ECC)
            {
                Manager = ECCManager.Default;
                KeyPair = Manager.GenerateKey();
            }
        }

        public BigInteger GenerateSymetricKey(List<Person> allPersons, Person keyOwner)
        {
            switch (KnownAlgorithm)
            {
                case Algorithm.ECC:
                    return GenerateSymetricWithECC(allPersons);
                case Algorithm.RSA:
                    return GenerateSymetricWithRSA();
            }

            return 0;
        }

        /// <summary>
        /// Key = Na x Pa x Pb x ... x Pi
        /// Where Pi = Ni x G
        /// </summary>
        /// <param name="allPersons"></param>
        /// <returns></returns>
        private BigInteger GenerateSymetricWithECC(List<Person> allPersons)
        {
            var keyPair = (ECCKey)KeyPair;
            var symmetricKey = keyPair.Copy();
            //get key = i x G, where i -> count of persons
            symmetricKey.OpenKey = ECCPoint.Multiply(allPersons.Count, keyPair.G);
            //get key = Na x i x G
            symmetricKey.OpenKey = ECCPoint.Multiply(keyPair.PrivateKey, symmetricKey.OpenKey);

            //now each person have to add own key
            foreach (var person in allPersons)
            {
                //Key = Na x i x G x Na x Nb x ... x Ni
                person.AddOwnSecretKey(symmetricKey);
            }

            var left = symmetricKey.OpenKey.X.ToByteArray();
            var right = symmetricKey.OpenKey.Y.ToByteArray();
            var key = left.Concat(right).ToArray();

            return new BigInteger(key);
        }

        private BigInteger GenerateSymetricWithRSA()
        {
            return new BigInteger(0);
        }

        public Key AddOwnSecretKey(Key key)
        {
            if (KnownAlgorithm == Algorithm.ECC)
            {
                var eccKey = (ECCKey)key;
                var keyPair = (ECCKey)KeyPair;
                eccKey.OpenKey = ECCPoint.Multiply(keyPair.PrivateKey, eccKey.OpenKey);
                
                return eccKey;
            }

            if (KnownAlgorithm == Algorithm.RSA)
            {
                
            }

            return key;
        }
    }
}
