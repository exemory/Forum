using System;
using System.Collections.Generic;

namespace Service.Interfaces
{
    /// <summary>
    /// Represents request session
    /// </summary>
    public interface ISession
    {
        /// <summary>
        /// Checks whether the user is authorized or not
        /// </summary>
        public bool IsAuthorized { get; }
        
        /// <summary>
        /// Guid of the authorized user, if authorized, null otherwise
        /// </summary>
        public Guid? UserId { get; }
        
        /// <summary>
        /// Roles of the authorized user, if authorized, null otherwise
        /// </summary>
        public IEnumerable<string> UserRoles { get; }

        /// <summary>
        /// Initializes request session
        /// </summary>
        /// <param name="userId">Guid of the authorized user</param>
        /// <param name="userRoles">Roles of the authorized user</param>
        public void Initialize(Guid userId, IEnumerable<string> userRoles);
        
        /// <summary>
        /// Checks whether the user contains the specified role or not
        /// </summary>
        /// <param name="role">Role to check</param>
        /// <returns>
        /// <c>true</c> if the user authorized and has <paramref name="role"/>, <c>false</c> otherwise
        /// </returns>
        public bool HasRole(string role);
    }
}