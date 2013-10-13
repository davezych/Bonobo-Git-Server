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

            foreach (var userName in repository.Users)
            {
                var user = MembershipService.GetUser(userName);
                MailMessageBase.To.Add(user.Email);
            }

            SmtpClient.Send(MailMessageBase);
        }
    }
}