using Backend_Blaupause.Models;
using HotChocolate;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_Blaupause.Helper.GraphQL
{
    public class GraphQLQuery
    {
        public async Task<List<User>> GetUsers([Service] IUser user) => await user.GetAllUsersAsync();
    }
}
