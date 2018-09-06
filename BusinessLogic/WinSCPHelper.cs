using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinSCP;

namespace OpenScapImpl
{
    class WinSCPHelper
    {
        public string ip;
        public string username;
        public string password;

        public WinSCPHelper(string ip, string username, string password)
        {
            this.ip = ip;
            this.username = username;
            this.password = password;
        }

        public bool TransferFile(string FilePath, string Destination)
        {
            bool retVal = false;

            try
            {
                // Setup session options
                SessionOptions sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Sftp,
                    HostName = ip,
                    UserName = username,
                    Password = password,
                    SshHostKeyFingerprint = "your machine sshkey",
                };

                using (WinSCP.Session session = new WinSCP.Session())
                {
                    //sessionOptions.GiveUpSecurityAndAcceptAnyTlsHostCertificate = true ;
                    // Connect
                    session.Open(sessionOptions);

                    // Download files
                    TransferOptions transferOptions = new TransferOptions();
                    transferOptions.TransferMode = TransferMode.Binary;

                    TransferOperationResult transferResult;
                    transferResult =
                        session.GetFiles(FilePath, Destination, false, transferOptions);

                    // Throw on any error
                    transferResult.Check();

                    // Print results
                    foreach (TransferEventArgs transfer in transferResult.Transfers)
                    {
                        Console.WriteLine("Download of {0} succeeded", transfer.FileName);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return retVal;
        }

    }
}
