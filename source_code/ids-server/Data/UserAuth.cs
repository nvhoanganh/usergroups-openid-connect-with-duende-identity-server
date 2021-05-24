using System;
using System.Security.Principal;
using Microsoft.AspNetCore.Identity;

namespace idsserver
{
    public class UserAuth
    {
        // this is the ID field
        public string UserAuthId { get; set; } = Guid.NewGuid().ToString();
        public string UserName { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string HashedPassword { get; set; }
        public bool Has2Fa { get; set; }
        public string TwoFaToken { get; set; }
    }

    public class ApplicationRole
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
    }
}