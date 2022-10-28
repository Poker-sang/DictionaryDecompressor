namespace UnRar.Models.Enums;

public enum RarError
{
    Success = 0,
    /// <summary>
    /// Only returned by <see cref="UnRar.RarReadHeader"/>
    /// </summary>
    EndOfArchive = 10,
    InsufficientMemory = 11,
    //--- Returned by any methods
    BadData = 12,
    //-^-
    BadArchive = 13,
    UnknownFormat = 14,
    OpenError = 15,
    CreateError = 16,
    CloseError = 17,
    //---
    ReadError = 18,
    WriteError = 19,
    //-^-
    BufferTooSmall = 20,
    //---
    UnknownError = 21,
    //-^-
    MissingPassword = 22,
    ReferenceError = 23,
    //---
    BadPassword = 24
    //-^-
}
