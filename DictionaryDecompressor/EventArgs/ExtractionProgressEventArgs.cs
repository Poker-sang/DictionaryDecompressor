namespace DictionaryDecompressor.EventArgs;

public class ExtractionProgressEventArgs
{
    public string FileName;
    public long FileSize;
    public long BytesExtracted;
    public double PercentComplete;
    public bool ContinueOperation = true;

    public ExtractionProgressEventArgs(string fileName, long fileSize, long bytesExtracted, double percentComplete)
    {
        FileName = fileName;
        FileSize = fileSize;
        BytesExtracted = bytesExtracted;
        PercentComplete = percentComplete;
    }

    public ExtractionProgressEventArgs(RarFileInfo fileInfo)
        : this(fileInfo.FileName, fileInfo.UnpackedSize, fileInfo.BytesExtracted, fileInfo.PercentComplete)
    {
    }
}
