using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenScapImpl
{
    class SSHHelper
    {
        public string ip;
        public string username;
        public string password;

        public SSHHelper(string ip, string username, string password)
        {
            this.ip = ip;
            this.username = username;
            this.password = password;
        }

        public string executeSSHCommand(string SSHCommand)
        {
            using (var sshClient = new SshClient(ip, username, password))
            {
                try
                {
                    sshClient.Connect();
                    //container compliance check
                    var cmd = sshClient.RunCommand(SSHCommand);
                    //var cmd = sshClient.RunCommand("oscap-docker image registry.access.redhat.com/rhel7 oval eval com.redhat.rhsa-al.xml ");
                    string output = cmd.Result;
                    Console.WriteLine("done");
                    return output;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    throw ex;
                }
            }
        }

        public bool TransferFileToHost(string remotefilename)
        {
            using (var sftp = new SftpClient(ip, username, password))
            {
                sftp.Connect();
                using (Stream fileStream = File.Create(@"D:\download\" + remotefilename))
                {
                    sftp.DownloadFile("/root/" + remotefilename, fileStream);
                }
            }
            return true;
        }
    }
}
