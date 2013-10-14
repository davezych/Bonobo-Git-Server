using System.Linq;
using Bonobo.Git.Server.Data;
using Bonobo.Git.Server.Security;

namespace Bonobo.Git.Server.Email
{
    public class CommitEmail : EmailHelperBase
    {
        public CommitEmail(IRepositoryRepository repositoryRepository, IMembershipService membershipService) : base(repositoryRepository, membershipService)
        {

        }

        public void SendMailOnCommit(string project)
        {
            //Need to check if smtp is specified

            var repository = RepositoryRepository.GetRepository(project);

            MailMessageBase.Subject = "A new commit";
            MailMessageBase.Body = "body of commit email";

            var allUsersInRepo = repository.Users.Concat(repository.Administrators);

            foreach (var userName in allUsersInRepo)
            {
                var user = MembershipService.GetUser(userName);
                //Also check if user wants emails
                if (!string.IsNullOrEmpty(user.Email))
                {
                    MailMessageBase.To.Add(user.Email);
                }
            }

            SmtpClient.Send(MailMessageBase);
        }
    }
}