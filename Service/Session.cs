using System;
using System.Collections.Generic;
using Service.Interfaces;

namespace Service
{
    /// <inheritdoc />
    public class Session : ISession
    {
        public Guid? UserId { get; private set; }
        public IEnumerable<string> UserRoles { get; private set; }
        public bool IsAuthorized => UserId != null;

        public void Initialize(Guid userId, IEnumerable<string> userRoles)
        {
            UserId = userId;
            UserRoles = userRoles;
        }
    }
}