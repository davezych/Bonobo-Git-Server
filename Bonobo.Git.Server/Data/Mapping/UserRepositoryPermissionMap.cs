using System.Data.Entity.ModelConfiguration;

namespace Bonobo.Git.Server.Data.Mapping
{
    public class UserRepositoryPermissionMap : EntityTypeConfiguration<UserRepositoryPermission>
    {
        public UserRepositoryPermissionMap()
        {
            ToTable("UserRepository_Permission");
            HasKey(u => new { u.Repository_Name, u.User_Username });
            Property(u => u.EmailOnCommit).HasColumnName("EmailOnCommit");
            Property(u => u.EmailOnCommitWithTag).HasColumnName("EmailOnCommitWithTag");
        }
    }
}