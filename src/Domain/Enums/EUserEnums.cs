using System.ComponentModel;

namespace Domain.Enums
{
    public enum EUserStatus
    {
        [Description("Active")]
        Active = 1,
        [Description("Pending")]
        Pending = 2,
        [Description("Disabled")]
        Disabled = 3,
    }

    public enum EUserRole
    {
        [Description("Admin")]
        Admin = 1,
        [Description("User")]
        User = 2,
    }
}
