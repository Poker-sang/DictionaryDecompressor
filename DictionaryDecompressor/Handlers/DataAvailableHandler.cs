using DictionaryDecompressor.EventArgs;

namespace DictionaryDecompressor.Handlers;

/// <summary>
/// Represents the method that will handle data available events
/// </summary>
public delegate void DataAvailableHandler(object sender, DataAvailableEventArgs e);
