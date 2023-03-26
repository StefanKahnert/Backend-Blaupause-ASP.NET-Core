using Backend_Blaupause.Helper;
using NUnit.Framework;

namespace Backend_Blaupause_Unit_Test
{
    public class SHA512GeneratorTest
    {
        [Test]
        public void GenerateHash()
        {
            //1. Define Input, and Expected results
            string plain = "waddeHaddeDuddeDa";

            //By https://hashgenerator.de/ SHA512
            string expectedResult = "1666d1f9c01b5d0c162c9c0c397497f15084f84f62ba1e1b0dac51acad2bc5ded4dec127257088ba14b7c9a188c1998880a97b484d3af1a14b4a3aa0ebb9f1eb";

            //Some other Hash
            string expectedFailResult = "39b0f2b1b85f2d4ae54b0fa9cf6d6489b6718852d8d32b9f752f990b446f739dbffd5bd710ea82d58e0e02d5f98c76562aa485520621357b4ca7fc600b788ed9";

            //2. Execute Method with Input
            string result = SHA512Generator.generateSha512Hash(plain);

            //3. Compare results 
            Assert.AreEqual(result, expectedResult);
            Assert.AreNotEqual(result, expectedFailResult);
        }
    }
}
