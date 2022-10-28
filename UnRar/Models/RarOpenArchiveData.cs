using System.Runtime.InteropServices;
using UnRar.Models.Enums;

namespace UnRar.Models;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct RarOpenArchiveData
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
    public string ArcName;

    public OpenMode OpenMode;
    public RarError OpenResult;

    [MarshalAs(UnmanagedType.LPStr)]
    public string CmtBuf;

    public uint CmtBufSize;
    public uint CmtSize;
    public uint CmtState;

    public void Initialize()
    {
        CmtBuf = new((char)0, 65536);
        CmtBufSize = 65536;
    }
}
