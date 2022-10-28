using DictionaryDecompressor.EventArgs;

namespace DictionaryDecompressor.Handlers;

/// <summary>
/// Represents the method that will handle extraction progress events
/// </summary>
public delegate void ExtractionProgressHandler(object sender, ExtractionProgressEventArgs e);