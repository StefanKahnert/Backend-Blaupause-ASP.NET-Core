using Backend_Blaupause.Controllers;
using Backend_Blaupause.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace Backend_Blaupause_Unit_Test
{
    [TestClass]
    public class AuthenticationEndpointTest
    {
        [TestMethod]
        public async Task Login()
        {
            //1. Mock everything you need
            int expirationTime = 20;

            User mockUser = new User();
            mockUser.id = 1;
            mockUser.active = true;
            mockUser.createdAt = DateTime.UtcNow;
            mockUser.firstName = "";
            mockUser.lastName = "";
            mockUser.password = "ee26b0dd4af7e749aa1a8ee3c10ae9923f618980772e473f8819a5d4940e0db27ac185f8a0e1d5f84f88bc887fd67b143732c304cc5fa9ad8e6f57f50028a8ff"; //SHA512 Hash of 'test'
            mockUser.username = "user";

            JWTConfiguration jwtConfiguration = new JWTConfiguration();
            jwtConfiguration.SecretKey = "6ba028366ebcdcef1ce3d73883c3475def4b7925f80e800bc82d91aa4430093622f9f95d20894022864dfa55d9c901ff520e070132eafabdae62a73e5ceeaed1";
            jwtConfiguration.TokenExpirationTime = expirationTime;
            jwtConfiguration.ValidIssuer = "6ba028366ebcdcef1ce3d73883c3475def4b7925f80e800bc82d91aa4430093622f9f95d20894022864dfa55d9c901ff520e070132eafabdae62a73e5ceeaed1";
            jwtConfiguration.ValidAudience = "6ba028366ebcdcef1ce3d73883c3475def4b7925f80e800bc82d91aa4430093622f9f95d20894022864dfa55d9c901ff520e070132eafabdae62a73e5ceeaed1";

            UserIdentity mockUserIdentity = new UserIdentity();
            mockUserIdentity.Password = "test";
            mockUserIdentity.Login = "user";

            var mockIUser = new Mock<IUser>();
            mockIUser.Setup(iUser => iUser.getUserByName(mockUserIdentity.Login)).Returns(Task.FromResult(mockUser));

            var mockResponse = new Mock<HttpResponse>();

            var mockLogger = new Mock<ILogger<AuthenticationEndpoint>>();

            //2. Execute Tested Unit with right Data
            mockIUser.Setup(iUser => iUser.getUserByName(mockUserIdentity.Login)).Returns(Task.FromResult(mockUser));
            AuthenticationEndpoint authenticationEndpoint = new AuthenticationEndpoint(mockIUser.Object, jwtConfiguration, mockLogger.Object)
            {
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext()
                {
                    HttpContext = Mock.Of<HttpContext>(_ => _.Response == mockResponse.Object)
                }
            };

            AccessToken token = await authenticationEndpoint.generateToken(mockUserIdentity);

            //3. Check if result is correct
            Assert.AreEqual(token.Success, true);
            Assert.AreEqual(token.ExpiryIn, expirationTime);

            //4. Execute Tested Unit with wrong data
            mockIUser.Setup(iUser => iUser.getUserByName(mockUserIdentity.Login)).Returns(Task.FromResult(new User()));
            authenticationEndpoint = new AuthenticationEndpoint(mockIUser.Object, jwtConfiguration, mockLogger.Object)
            {
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext()
                {
                    HttpContext = Mock.Of<HttpContext>(_ => _.Response == mockResponse.Object)
                }
            };
            token = await authenticationEndpoint.generateToken(mockUserIdentity);

            //5. Check if result is correct
            Assert.AreNotEqual(token.Success, true);
            Assert.AreNotEqual(token.ExpiryIn, expirationTime);
        }


    }
}
