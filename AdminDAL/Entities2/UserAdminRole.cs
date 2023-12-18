using System;
using System.Collections.Generic;

namespace AdminDAL.Entities2;

public partial class UserAdminRole
{
    public int Id { get; set; }

    public string UserName { get; set; } = null!;

    public string UserEmail { get; set; } = null!;

    public string AdminUserId { get; set; } = null!;

    public string AdminUserName { get; set; } = null!;
}
