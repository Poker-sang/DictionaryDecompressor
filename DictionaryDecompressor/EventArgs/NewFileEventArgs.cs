namespace DictionaryDecompressor.EventArgs;

public class NewFileEventArgs
{
    public RarFileInfo FileInfo;
    public NewFileEventArgs(RarFileInfo fileInfo) => FileInfo = fileInfo;
}
