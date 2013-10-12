using Bonobo.Git.Server.Helpers;

namespace Bonobo.Git.Server.Extensions
{
    public static class GitCommandExtensions
    {
        public static string ToCommandString(this GitRunner.GitCommand command)
        {
            switch (command)
            {
                case GitRunner.GitCommand.Receive:
                    return "receive-pack";
                case GitRunner.GitCommand.UploadPack:
                    return "upload-pack";
                default:
                    return string.Empty;
            }
        }

        public static string ToContentTypeString(this GitRunner.GitCommand command)
        {
            switch (command)
            {
                case GitRunner.GitCommand.Receive:
                    return "application/x-git-receive-pack-result";
                case GitRunner.GitCommand.UploadPack:
                    return "application/x-git-upload-pack-result";
                default:
                    return string.Empty;
            }
        }
    }
}