using SMBLibrary.Client;
using SMBLibrary;
using System.Net;
using FileAttributes = SMBLibrary.FileAttributes;
using System.Security.Cryptography.X509Certificates;
using System.Security.AccessControl;

namespace Samba
{
    public class SambaHelper
    {
        private void CreateSubDirectory(string[] directories, ISMBFileStore fileStore)
        {
            string directory = string.Empty;
            for (int i = 0; i < directories.Length; i++)
            {
                directory = directory + directories[i];
                object fileHandle;
                FileStatus fileStatus;
                NTStatus status = fileStore.CreateFile(out fileHandle, out fileStatus, directory, AccessMask.GENERIC_WRITE | AccessMask.SYNCHRONIZE, FileAttributes.Normal, ShareAccess.None, CreateDisposition.FILE_CREATE, CreateOptions.FILE_DIRECTORY_FILE | CreateOptions.FILE_SYNCHRONOUS_IO_ALERT, null);
                if (status != NTStatus.STATUS_SUCCESS && status != NTStatus.STATUS_OBJECT_NAME_COLLISION)
                {
                    throw new Exception("Failed to create directory");
                }
                if(status != NTStatus.STATUS_OBJECT_NAME_COLLISION)
                    fileStore.CloseFile(fileHandle);
                directory = directory + "\\";
            }
        }
        public async Task WriteToFileAsync(SambaConnection connection, string shareName, string remoteFilePath, byte[] data)
        {
            if (data == null)
                throw new Exception("File is empty.");
            if (data.Length == 0)
                throw new Exception("File is empty.");
            object fileHandle;
            FileStatus fileStatus;
            NTStatus status;
            var client = GetConnection(connection);
            var fileStore = GetFileStore(client, shareName);
            MemoryStream ms = new MemoryStream(data);
            List<string> directories = new List<string>();
            directories.AddRange(remoteFilePath.Split('\\'));
            directories.RemoveAt(directories.Count - 1);
            CreateSubDirectory(directories.ToArray(), fileStore);
            status = fileStore.CreateFile(out fileHandle, out fileStatus, remoteFilePath, AccessMask.GENERIC_WRITE | AccessMask.SYNCHRONIZE, FileAttributes.Normal, ShareAccess.None, CreateDisposition.FILE_OVERWRITE_IF, CreateOptions.FILE_NON_DIRECTORY_FILE | CreateOptions.FILE_SYNCHRONOUS_IO_ALERT, null);
            if (status == NTStatus.STATUS_SUCCESS)
            {
                int writeOffset = 0;
                while (ms.Position < ms.Length)
                {
                    byte[] buffer = new byte[(int)client.MaxWriteSize];
                    int bytesRead = ms.Read(buffer, 0, buffer.Length);
                    if (bytesRead < (int)client.MaxWriteSize)
                    {
                        Array.Resize<byte>(ref buffer, bytesRead);
                    }
                    int numberOfBytesWritten;
                    status = fileStore.WriteFile(out numberOfBytesWritten, fileHandle, writeOffset, buffer);
                    if (status != NTStatus.STATUS_SUCCESS)
                    {
                        client.Logoff();
                        client.Disconnect();
                        throw new Exception("Failed to write file to share.");
                    }
                    writeOffset += bytesRead;
                }
                status = fileStore.CloseFile(fileHandle);
            }
            else
            {
                client.Logoff();
                client.Disconnect();
                throw new Exception("Can't create file at share.");
            }
            ms.Close();
            ms.Dispose();
            client.Logoff();
            client.Disconnect();
        }
        private ISMBFileStore GetFileStore(SMB2Client client, string shareName)
        {
            NTStatus status;
            FileStatus fileStatus;
            ISMBFileStore fileStore = client.TreeConnect(shareName, out status);
            if (status == NTStatus.STATUS_SUCCESS)
            {
                return fileStore;
            }
            else
            {
                client.Logoff();
                client.Disconnect();
                throw new Exception($"Can't get access to share {shareName}.");
            }
        }
        private SMB2Client GetConnection(SambaConnection connection)
        {
            SMB2Client client = new SMB2Client(); // SMB2Client can be used as well
            NTStatus status;
            bool isConnected = client.Connect(connection.IP, SMBTransportType.DirectTCPTransport);
            if (isConnected)
            {
                status = client.Login(connection.Domain, connection.Name, connection.Password);
                if (status == NTStatus.STATUS_SUCCESS)
                {
                    return client;
                }
                else
                {
                    client.Disconnect();
                    throw new Exception("Login has not pass.");
                }
            }
            else
            {
                client.Disconnect();
                throw new Exception($"Store {connection.IP.ToString()} is not aviable.");
            }
        }
        public async Task<byte[]> ReadFileAsync(SambaConnection connection, string shareName, string remoteFilePath)
        {
            object fileHandle;
            FileStatus fileStatus;
            NTStatus status;
            var client = GetConnection(connection);
            var fileStore = GetFileStore(client, shareName);
            status = fileStore.CreateFile(out fileHandle, out fileStatus, remoteFilePath, AccessMask.GENERIC_READ | AccessMask.SYNCHRONIZE, FileAttributes.Normal, ShareAccess.Read, CreateDisposition.FILE_OPEN, CreateOptions.FILE_NON_DIRECTORY_FILE | CreateOptions.FILE_SYNCHRONOUS_IO_ALERT, null);
            byte[] data = null;
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            if (status == NTStatus.STATUS_SUCCESS)
            {
                long bytesRead = 0;
                while (true)
                {
                    status = fileStore.ReadFile(out data, fileHandle, bytesRead, (int)client.MaxReadSize);
                    if (status != NTStatus.STATUS_SUCCESS && status != NTStatus.STATUS_END_OF_FILE)
                    {
                        status = fileStore.CloseFile(fileHandle);
                        status = fileStore.Disconnect();
                        client.Logoff();
                        client.Disconnect();
                        throw new Exception("Failed to read from file");
                    }

                    if (status == NTStatus.STATUS_END_OF_FILE || data.Length == 0)
                    {
                        break;
                    }
                    bytesRead += data.Length;
                    stream.Write(data, 0, data.Length);
                }
            }
            else
            {
                client.Logoff();
                client.Disconnect();
                throw new Exception("Can't open file.");
            }
            status = fileStore.CloseFile(fileHandle);
            status = fileStore.Disconnect();
            client.Logoff();
            client.Disconnect();
            return stream.ToArray();
        }
    }
}