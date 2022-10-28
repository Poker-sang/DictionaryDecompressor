using System.Runtime.InteropServices;

namespace UnRar.Models.Delegates;

public delegate int ProcessDataProc([MarshalAs(UnmanagedType.LPStr)] string addr, int size);

