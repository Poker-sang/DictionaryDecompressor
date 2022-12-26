using SharpCompress.Archives;
using SharpCompress.Common;
using UnRar.Models.Enums;

namespace DictionaryDecompressor;

public static class Program
{
    private static readonly List<string> _pwdDict = new() { "boluoyyds", "Geass", "hmoe.top", "izaya", "paradox", "alterego", "yomega", "⑨" };

    private class PasswordUsedUpException : Exception
    {
        public PasswordUsedUpException(string message) : base(message)
        {

        }
    }

    private static int _i;
    private static string NextPassword
    {
        get
        {
            if (_i < _pwdDict.Count)
            {
                ++_i;
                return _pwdDict[_i - 1];
            }

            throw new PasswordUsedUpException("No correct password.");
        }
    }

    public static void Main(string[] args)
    {
        while (true)
        {
            var info = new CompressInfo();
            _i = 0;
            var p = Console.ReadLine()!.Trim('"');
            if (p is "")
                return;
            var filePath = new FileInfo(p);
            if (!filePath.Exists)
                return;
            // 除rar格式外加密文件名
            IArchive archive;
            try
            {
                archive = ArchiveFactory.Open(filePath);
            }
            catch (Exception)
            {
                info.EncryptionType = EncryptionType.Header;
                while (true)
                    try
                    {
                        info.Password = NextPassword;
                        archive = ArchiveFactory.Open(filePath, new() { Password = info.Password });
                        break;
                    }
                    catch (PasswordUsedUpException)
                    {
                        throw;
                    }
                    catch (Exception)
                    {
                        continue;
                    }
            }

            info.ArchiveType = archive.Type;
            if (archive.Type is not ArchiveType.Rar)
            {
                // 除rar格式外加密文件
                var entries = archive.Entries.ToList();
                for (var index = 0; index < entries.Count; index++)
                    if (!entries[index].IsDirectory)
                    {
                        var fileInfo = new FileInfo(Path.Combine(filePath.DirectoryName!, entries[index].Key));
                        if (fileInfo.Directory is { Exists: true } directory)
                            directory.Create();
                        // 除rar格式外加密文件
                        while (true)
                            try
                            {
                                entries[index].WriteToFile(fileInfo.FullName);
                                break;
                            }
                            catch (Exception)
                            {
                                info.EncryptionType = EncryptionType.Content;
                                info.Password = NextPassword;
                                archive.Dispose();
                                archive = ArchiveFactory.Open(filePath, new() { Password = info.Password });
                                entries = archive.Entries.ToList();
                            }
                    }
            }
            else
            {
                var pwd = "";
                // rar格式加密文件名
                while (true)
                {
                    using var unRarTest = UnRar.Open(filePath, OpenMode.List);
                    unRarTest.Password = pwd;
                    var r = unRarTest.ReadHeader();
                    if (r is RarError.Success)
                        break;
                    switch (r)
                    {
                        case RarError.MissingPassword:
                        case RarError.BadPassword:
                            info.EncryptionType = EncryptionType.Header;
                            info.Password = NextPassword;
                            pwd = info.Password;
                            break;
                        case RarError.BadData:
                            throw new IOException("Archive data is corrupted.");
                    }
                }

                using var unRar = UnRar.Open(filePath, OpenMode.Extract);
                unRar.Password = pwd;

                var end = false;
                while (!end)
                {
                    var result = unRar.ReadHeader();
                    switch (result)
                    {
                        case RarError.Success:
                            // rar格式加密文件
                            if (unRar.ExtractToDirectory(filePath.DirectoryName!) is RarError.BadPassword or RarError.MissingPassword)
                            {
                                info.EncryptionType = EncryptionType.Content;
                                info.Password = NextPassword;
                                unRar.Password = info.Password;
                            }

                            break;
                        case RarError.EndOfArchive:
                            end = true;
                            break;
                        default:
                            throw new IOException(result.ToString());
                    }
                }
            }

            archive.Dispose();
            Console.WriteLine("完成\n" + info);
        }
    }
}
