namespace ZDK.ResourceManager.Abstractions;

/// <summary>
/// Exception thrown when an error occurs while accessing the content of a resource file.
/// </summary>
[Serializable]
public class ResourceFileAccessException : Exception
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ResourceFileAccessException"/> class.
	/// </summary>
	public ResourceFileAccessException() { }

	/// <summary>
	/// Initializes a new instance of the <see cref="ResourceFileAccessException"/> class with a specified error message.
	/// </summary>
	/// <param name="message">The message that describes the error.</param>
	public ResourceFileAccessException(string message)
		: base(message) { }

	/// <summary>
	/// Initializes a new instance of the <see cref="ResourceFileAccessException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
	/// </summary>
	/// <param name="message">The error message that explains the reason for the exception.</param>
	/// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
	public ResourceFileAccessException(string message, Exception innerException)
		: base(message, innerException) { }
}