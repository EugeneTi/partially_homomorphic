using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mpir.NET;

namespace Cryptography.Paillier
{
    public class PaillierKey
    {
        public mpz_t N { get; set; }
        public mpz_t G { get; set; }

        public mpz_t Lambda { get; set; }

        public mpz_t Mu { get; set; }

    }
}
