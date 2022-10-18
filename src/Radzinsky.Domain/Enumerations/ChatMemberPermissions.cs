namespace Radzinsky.Domain.Enumerations;

public enum ChatMemberPermissions
{
    SetRolesForLowerPriorities,
    
    BanDefaultPriority,
    BanLowerPriorities,
    
    KickDefaultPriority,
    KickLowerPriorities,
    
    RestrictDefaultPriority,
    RestrictLowerPriorities
}