namespace DictionaryDecompressor.EventArgs;

public class DataAvailableEventArgs
{
    public readonly byte[] Data;
    public bool ContinueOperation = true;

    public DataAvailableEventArgs(byte[] data) => Data = data;
}
