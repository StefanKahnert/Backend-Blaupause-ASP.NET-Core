
using Moq;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Backend_Blaupause.Helper;
using Backend_Blaupause.Models.Interfaces;
using Backend_Blaupause_Unit_Test.Mocks;
using static Backend_Blaupause.Helper.PermissionAttribute;

namespace Schulungstracker.backend.AttributesTests
{
    [TestClass]
    public class PermissionAttributeTest
    {
        private Mock<UserAuthentication> userAuthenticationMock;
        private List<string> roles = new List<string>();
        private List<string> modules = new List<string>();
        private ActionExecutingContext context;
        private ActionExecutionDelegate next;
        private PermissionFilter permissionFilter;

        [TestInitialize]
        public void Setup()
        {
            roles.Add(IPermission.ADMINISTRATOR);
            userAuthenticationMock = UserAuthenticationMock.getInstance();
            permissionFilter = new PermissionFilter(userAuthenticationMock.Object, roles, modules);

            var httpContext = new DefaultHttpContext();

            var actionContext = new ActionContext
            {
                HttpContext = httpContext,
                RouteData = new RouteData(),
                ActionDescriptor = new ActionDescriptor(),
            };
            var metadata = new List<IFilterMetadata>();

            context = new ActionExecutingContext(
                actionContext,
                metadata,
                new Dictionary<string, object>(),
                permissionFilter);

            next = () => {
                var ctx = new ActionExecutedContext(actionContext, metadata, permissionFilter);
                return Task.FromResult(ctx);
            };
        }

        [TestMethod]
        public async Task TestOnActionExecutedWithoutAdminRights()
        {
            userAuthenticationMock.Setup(mock => mock.userHasPermission(roles.FirstOrDefault(), null)).Returns(false);
            await permissionFilter.OnActionExecutionAsync(context, next);

            Assert.AreEqual((int) HttpStatusCode.Forbidden, ((StatusCodeResult)context.Result).StatusCode);
        }

        [TestMethod]
        public async Task TestOnActionExecutedWithAdminRights()
        {
            userAuthenticationMock.Setup(mock => mock.userHasPermission(roles.FirstOrDefault(), null)).Returns(true);
            await permissionFilter.OnActionExecutionAsync(context, next);
            Assert.IsNull(((StatusCodeResult)context.Result));
        }

    }
}