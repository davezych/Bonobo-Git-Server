using System;
using System.Net;
using System.Net.Mail;
using Bonobo.Git.Server.Configuration;
using Bonobo.Git.Server.Data;
using Bonobo.Git.Server.Security;

namespace Bonobo.Git.Server.Email
{
    public abstract class EmailHelperBase : IDisposable
    {
        protected readonly IRepositoryRepository RepositoryRepository;
        protected readonly IMembershipService MembershipService;
        protected SmtpClient SmtpClient;
        protected MailMessage MailMessageBase;

        public static bool CanSendMail { get { return !string.IsNullOrWhiteSpace(UserConfiguration.Current.SmtpHost); } }

        private string _fromEmail
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["FromEmail"]; }
        }

        protected EmailHelperBase(IRepositoryRepository repositoryRepository, IMembershipService membershipService)
        {
            RepositoryRepository = repositoryRepository;
            MembershipService = membershipService;

            SmtpClient = new SmtpClient(UserConfiguration.Current.SmtpHost, UserConfiguration.Current.SmtpPort)
                {
                    Credentials =
                        new NetworkCredential(UserConfiguration.Current.SmtpUsername,
                                              UserConfiguration.Current.SmtpPassword)
                };

            MailMessageBase = new MailMessage
                {
                    From = new MailAddress(_fromEmail)
                };
        }

        protected void Send()
        {
            SmtpClient.Send(MailMessageBase);
        }

        public void Dispose()
        {
            SmtpClient.Dispose();
        }
    }
}