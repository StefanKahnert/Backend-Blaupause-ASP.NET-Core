using Backend_Blaupause.Helper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Blaupause_Unit_Test.Mocks
{
    public class UserAuthenticationMock
    {
        public static Mock<UserAuthentication> getInstance()
        {
            return new Mock<UserAuthentication>();
        }
    }
}
