using Domain.Common;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class User : IdentityUser<Guid>, IAuditableEntity
    {
        public Guid Id { get; set; }
        private string _firstName { get; set; }
        public string FirstName { get { return _firstName; } set { _firstName = value; } }
        private string _lastName { get; set; }
        public string LastName { get { return _lastName; } set { _lastName = value; } }
        private string? Gender { get; set; }
        public string Status { get; set; } = EUserStatus.Pending.ToString();
        private string _password { get; set; }
        public string Password { get { return _password; } set { _password = value; } }
        public DateTime LastLogin { get; set; }
        public string Role { get; set; } //= EUserRole.User.ToString();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastLoginAt { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

        //Navigational Properties
        public ReadList? ReadList { get; set; }
        public ICollection<Review>? Reviews { get; set; }

    }
    
}
