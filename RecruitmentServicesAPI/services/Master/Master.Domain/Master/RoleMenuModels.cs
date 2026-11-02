using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Master.Domain.Master;

public class DtoRoleMasterItem
{
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string RoutePrefix { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class DtoSaveRoleRequest
{
    public int RoleId { get; set; }

    [Required]
    [MaxLength(100)]
    public string RoleName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string RoutePrefix { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}

public class DtoMenuFilterRequest
{
    public string? RoutePrefix { get; set; }
}

public class DtoMenuListItem
{
    public int MenuId { get; set; }
    public string MenuName { get; set; } = string.Empty;
    public string? MenuNameMarathi { get; set; }
    public string? RouteUrl { get; set; }
    public int? ParentMenuId { get; set; }
    public string? ParentMenuName { get; set; }
    public string? Icon { get; set; }
    public int SortOrder { get; set; }
    public bool IsVisibleInMenu { get; set; }
    public bool IsActive { get; set; }
    public string AssignedRoles { get; set; } = string.Empty;
}

public class DtoParentMenuDropdownItem
{
    public int MenuId { get; set; }
    public string MenuName { get; set; } = string.Empty;
    public string? MenuNameMarathi { get; set; }
    public int SortOrder { get; set; }
}

public class DtoSaveMenuRequest
{
    public int MenuId { get; set; }

    [Required]
    [MaxLength(200)]
    public string MenuName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? MenuNameMarathi { get; set; }

    [MaxLength(500)]
    public string? RouteUrl { get; set; }

    public int? ParentMenuId { get; set; }

    [MaxLength(100)]
    public string? Icon { get; set; }

    public int SortOrder { get; set; }

    public bool IsVisibleInMenu { get; set; } = true;

    public bool IsActive { get; set; } = true;

    [MaxLength(50)]
    public string? AssignToRoutePrefix { get; set; }
}

public class DtoRoleMenuMappingItem
{
    public int MenuId { get; set; }
    public string MenuName { get; set; } = string.Empty;
    public string? MenuNameMarathi { get; set; }
    public string? RouteUrl { get; set; }
    public int? ParentMenuId { get; set; }
    public string? ParentMenuName { get; set; }
    public string? Icon { get; set; }
    public int SortOrder { get; set; }
    public bool IsVisibleInMenu { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsSelected { get; set; }
}

public class DtoGetRoleMenuMappingRequest
{
    [Required]
    public string RoutePrefix { get; set; } = string.Empty;
}

public class DtoSaveRoleMenuMappingRequest
{
    [Required]
    public string RoutePrefix { get; set; } = string.Empty;

    public List<int> MenuIds { get; set; } = [];
}
