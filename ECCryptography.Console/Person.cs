using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Cryptography.Common;
using Cryptography.ECC;
using Cryptography.RSA;
using Mpir.NET;
using RSACryptography.Core;

namespace Cryptography.Console
{
    public class Person
    {
        protected Key KeyPair;

        protected CryptoManager Manager;

        protected mpz_t SymetricKey;

        public Algorithm KnownAlgorithm { get; }

        public Person(Algorithm knownAlgorithm)
        {
            KnownAlgorithm = knownAlgorithm;
        }

        public void GenerateKeys()
        {
            if (KnownAlgorithm == Algorithm.RSA)
            {
                Manager = RSAManager.Default;
                KeyPair = Manager.GenerateKey();
            }

            if (KnownAlgorithm == Algorithm.ECC)
            {
                Manager = ECCManager.Default;
                KeyPair = Manager.GenerateKey();
            }
        }

        public mpz_t GenerateSymetricKey(List<Person> allPersons)
        {
            switch (KnownAlgorithm)
            {
                case Algorithm.ECC:
                    return GenerateSymetricWithECC(allPersons);
                case Algorithm.RSA:
                    return GenerateSymetricWithRSA(allPersons);
            }

            throw new Exception("Unknown algorithm");
        }

        /// <summary>
        /// Key = Na x Pa x Pb x ... x Pi
        /// Where Pi = Ni x G
        /// 
        /// NEW
        /// Ki = Ni x Po x ... x Pn 
        /// Where Pi = Ni x G
        /// </summary>
        private mpz_t GenerateSymetricWithECC(List<Person> allPersons)
        {
            var keyPair = (ECCKey)KeyPair;
            var symmetricKey = keyPair.Copy();
            
            //get key = i x G, where i -> count of persons
            symmetricKey.OpenKey = ECCPoint.Multiply(allPersons.Count, keyPair.G);
            
            //now each person have to add own key
            foreach (var person in allPersons)
            {
                if (person != this)
                    //Key = Na x i x G x Na x Nb x ... x Ni
                    person.AddOwnSecretKey(symmetricKey);
            }

            //add own private key
            symmetricKey.OpenKey = ECCPoint.Multiply(keyPair.PrivateKey, symmetricKey.OpenKey);

            var left = symmetricKey.OpenKey.X.ToByteArray(0);
            var right = symmetricKey.OpenKey.Y.ToByteArray(0);
            var key = left.Concat(right).ToArray();

            SymetricKey = new mpz_t(key, 0);
            return SymetricKey;
        }

        /// <summary>
        /// Key = P ^ (Na x Nb x ... x Ni)
        /// </summary>
        private mpz_t GenerateSymetricWithRSA(List<Person> allPersons)
        {
            var keyPair = (RSAKey)KeyPair;
            var symmetricKey = keyPair.Copy();

            //get key = P ^ Na
            symmetricKey.OpenKey = keyPair.PublicPrime; 

            foreach (var person in allPersons)
            {
                if (person != this)
                    person.AddOwnSecretKey(symmetricKey);
            }

            symmetricKey.OpenKey = symmetricKey.OpenKey.PowerMod(keyPair.PrivateKey, keyPair.P);

            SymetricKey = symmetricKey.OpenKey;
            return SymetricKey;
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
                var rsaKey = (RSAKey)key;
                var keyPair = (RSAKey)KeyPair;
                rsaKey.OpenKey = rsaKey.OpenKey.PowerMod(keyPair.PrivateKey, keyPair.P);

                return rsaKey;
            }

            throw new Exception("Unknown algorithm");
        }
    }
}
