using UnRar.Models.Enums;

namespace DictionaryDecompressor;

public class RarFileInfo
{
    public string FileName;
    public bool ContinuedFromPrevious;
    public bool ContinuedOnNext;
    public bool IsDirectory;
    public long PackedSize;
    public long UnpackedSize;
    public HostOS HostOS;
    public long FileCRC;
    public DateTime FileTime;
    public int VersionToUnpack;
    public int Method;
    public int FileAttributes;
    public long BytesExtracted;

    public RarFileInfo(string fileName) => FileName = fileName;

    public double PercentComplete => UnpackedSize != 0 ? BytesExtracted / (double)UnpackedSize * 100.0 : 0;
}
