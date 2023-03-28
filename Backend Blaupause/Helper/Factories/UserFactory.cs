using Backend_Blaupause.Models;
using Bogus;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Backend_Blaupause.Helper.Factories
{
    public class UserFactory
    {
        public static List<User> GenerateUsers(int numberOfUsers)
        {
            if(numberOfUsers <= 0)
            {
                return new List<User>();
            }


            var faker = new Faker<User>()
                        .RuleFor(u => u.FirstName, f => f.Person.FirstName)
                        .RuleFor(u => u.LastName, f => f.Person.LastName)
                        .RuleFor(u => u.LastLogin, f => DateTime.Parse(f.Date.RecentDateOnly().ToString("dd.MM.yyyy")).ToUniversalTime())
                        .RuleFor(u => u.CreatedAt, f => DateTime.Parse(f.Date.PastDateOnly().ToString("dd.MM.yyyy")).ToUniversalTime())
                        .RuleFor(u => u.ExpirationDate, f => DateTime.Parse(f.Date.FutureDateOnly().ToString("dd.MM.yyyy")).ToUniversalTime())
                        .RuleFor(u => u.ModifiedAt, f => DateTime.Parse(f.Date.PastDateOnly().ToString("dd.MM.yyyy")).ToUniversalTime())
                        .RuleFor(u => u.UserName, f => f.Person.UserName)
                        .RuleFor(u => u.NormalizedUserName, f => f.Person.UserName.ToUpper())
                        .RuleFor(u => u.Email, f => $"{f.Person.FirstName}.{f.Person.LastName}@test.com")
                        .RuleFor(u => u.NormalizedEmail, f => ($"{f.Person.FirstName}.{f.Person.LastName}@test.com").ToUpper())
                        .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber());

            return faker.Generate(numberOfUsers);
        }
    }
}
