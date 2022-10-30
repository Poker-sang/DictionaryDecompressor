using SharpCompress.Common;

namespace DictionaryDecompressor;

public struct CompressInfo
{
    public ArchiveType ArchiveType = ArchiveType.GZip;
    public string Password = "";
    public EncryptionType EncryptionType = EncryptionType.NoEncrypt;

    public CompressInfo()
    {
    }

    public override string ToString() =>
        $@"ArchiveType: {ArchiveType}
Password: {Password}
EncryptionType: {EncryptionType}";
}
