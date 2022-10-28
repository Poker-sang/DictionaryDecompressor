using DictionaryDecompressor.EventArgs;

namespace DictionaryDecompressor.Handlers;

/// <summary>
/// Represents the method that will handle new file notifications
/// </summary>
public delegate void NewFileHandler(object sender, NewFileEventArgs e);