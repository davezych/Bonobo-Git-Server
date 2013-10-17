using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Bonobo.Git.Server.Data.Mapping
{
    public class UserRepositoryPermissionMap : EntityTypeConfiguration<UserRepositoryPermission>
    {
        public UserRepositoryPermissionMap()
        {
            ToTable("UserRepository_Permission");
            HasKey(u => new { u.Repository_Name, u.User_Username });
        }
    }
}