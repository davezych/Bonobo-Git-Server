using System.Configuration;
using System.IO;
using System.Web;
using Bonobo.Git.Server.Configuration;

namespace Bonobo.Git.Server.Helpers
{
    public class GitRunner
    {
        private readonly GitCommand _command;
        private readonly string _unknownCommand;
        private readonly string _workingDirectory;
        private readonly bool _advertiseRefs;

        public GitRunner(GitCommand command, string workingDirectory, bool advertiseRefs)
        {
            _command = command;
            _workingDirectory = workingDirectory;
            _advertiseRefs = advertiseRefs;
        }

        public GitRunner(string command, string workingDirectory, bool advertiseRefs)
        {
            _unknownCommand = command;
            _command = GitCommand.Unknown;
            _workingDirectory = workingDirectory;
            _advertiseRefs = advertiseRefs;
        }

        public enum GitCommand
        {
            Receive,
            UploadPack,
            Unknown,
        }

        public void RunGitCmd(Stream inStream, Stream outStream)
        {
            string commandToRun;
            if (_command == GitCommand.Unknown)
            {
                commandToRun = _unknownCommand;
            }
            else
            {
                commandToRun = _command.ToCommandString();
            }

            var args = string.Format("{0} --stateless-rpc", commandToRun);
            if (_advertiseRefs)
                args += " --advertise-refs";
            args += string.Format(@" ""{0}""", _workingDirectory);

            var gitPath = Path.IsPathRooted(ConfigurationManager.AppSettings["GitPath"])
                ? ConfigurationManager.AppSettings["GitPath"]
                : HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["GitPath"]);
            var info = new System.Diagnostics.ProcessStartInfo(gitPath, args)
            {
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WorkingDirectory = Path.GetDirectoryName(UserConfiguration.Current.Repositories),
            };

            using (var process = System.Diagnostics.Process.Start(info))
            {
                inStream.CopyTo(process.StandardInput.BaseStream);
                process.StandardInput.Write('\0');
                process.StandardOutput.BaseStream.CopyTo(outStream);

                process.WaitForExit();
            }
        }
    }

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