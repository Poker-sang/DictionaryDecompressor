using SharpCompress.Archives;
using SharpCompress.Common;
using UnRar.Models.Enums;

public static class Program
{
    private static readonly List<string> _pwdDict = new() { "buoluoyyds", "Geass", "hmoe.top", "izaya", "paradox", "123", "alterego", "yomega", "⑨" };

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

            throw new("No correct password.");
        }
    }
    public static void Main(string[] args)
    {
        while (true)
        {
            _i = 0;
            var p = Console.ReadLine()!.Trim('"');
            if (p is "")
                return;
            var filePath = new FileInfo(p);
            if (!filePath.Exists)
                return;
            var archive = ArchiveFactory.Open(filePath);
            if (archive.Type is not ArchiveType.Rar)
            {
                var entries = archive.Entries.ToList();
                for (var index = 0; index < entries.Count; index++)
                    if (!entries[index].IsDirectory)
                    {
                        var fileInfo = new FileInfo(Path.Combine(filePath.DirectoryName!, entries[index].Key));
                        if (fileInfo.Directory is { Exists: true } directory)
                            directory.Create();
                        while (true)
                            try
                            {
                                entries[index].WriteToFile(fileInfo.FullName);
                                break;
                            }
                            catch (Exception)
                            {
                                archive.Dispose();
                                archive = ArchiveFactory.Open(filePath, new() { Password = NextPassword });
                                entries = archive.Entries.ToList();
                            }
                    }
            }
            else
            {
                var pwd = "";
                while (true)
                {
                    using var unRarTest = DictionaryDecompressor.UnRar.Open(filePath, OpenMode.List);
                    unRarTest.Password = pwd;
                    var r = unRarTest.ReadHeader();
                    if (r is RarError.Success)
                        break;
                    switch (r)
                    {
                        // 加密文件名
                        case RarError.MissingPassword:
                        case RarError.BadPassword:
                            pwd = NextPassword;
                            break;
                        case RarError.BadData:
                            throw new IOException("Archive data is corrupted.");
                    }
                }
                using var unRar = DictionaryDecompressor.UnRar.Open(filePath, OpenMode.Extract);
                unRar.Password = pwd;

                var end = false;
                while (!end)
                {
                    var result = unRar.ReadHeader();
                    switch (result)
                    {
                        case RarError.Success:
                            // 加密文件
                            if (unRar.ExtractToDirectory(filePath.DirectoryName!) is RarError.BadPassword or RarError.MissingPassword)
                                unRar.Password = NextPassword;
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
            Console.WriteLine("完成");
        }
    }
}
