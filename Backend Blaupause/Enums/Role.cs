using System.Runtime.Serialization;

namespace Backend_Blaupause.Enums
{
    public enum Role
    {
        [EnumMember(Value = "USER")]
        USER,

        [EnumMember(Value = "ADMINISTRATOR")]
        ADMINISTRATOR
    }
}
