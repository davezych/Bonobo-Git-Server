namespace Bonobo.Git.Server.Data
{
    public class UserRepositoryPermission
    {
        public string User_Username { get; set; }
        public string Repository_Name { get; set; }
        public bool EmailOnCommit { get; set; }
        public bool EmailOnCommitWithTag { get; set; }
    }
}