using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

public static class EventLogHelper
{
    // Set your application's name here.
    private static readonly string ApplicationName = "WebAppShipping";

    /// <summary>
    /// Logs an exception to the Windows Event Log, including caller details.
    /// </summary>
    /// <param name="ex">The exception to log.</param>
    /// <param name="memberName">Automatically supplied calling method name.</param>
    /// <param name="sourceFilePath">Automatically supplied source file path.</param>
    /// <param name="sourceLineNumber">Automatically supplied line number.</param>
    public static void WriteErrorLog(
        Exception ex,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        try
        {
            // Ensure the event source exists; creating it may require administrative privileges.
            if (!EventLog.SourceExists(ApplicationName))
            {
                // This creates the event source in the "Application" log by default.
                // Change the second parameter if you want a different log name.
                EventLog.CreateEventSource(ApplicationName, "WebAppShipping");
            }

            // Extract the class name from the source file path.
            string className = Path.GetFileNameWithoutExtension(sourceFilePath);

            // Build a detailed log message.
            string logMessage = $"Class: {className}\n" +
                                $"Method: {memberName}\n" +
                                $"Line: {sourceLineNumber}\n" +
                                $"Message: {ex.Message}\n" +
                                $"Stack Trace: {ex.StackTrace}";

            // Write to the event log as an Error.
            EventLog.WriteEntry(ApplicationName, logMessage, EventLogEntryType.Error);
        }
        catch
        {
            // Avoid throwing exceptions from a logging method.
        }
    }

    /// <summary>
    /// Writes a custom event (Information, Warning, etc.) to the Windows Event Log.
    /// </summary>
    /// <param name="message">The event message to log.</param>
    /// <param name="type">The type of event (default = Information).</param>
    /// <param name="eventId">An optional event ID (default = 0).</param>
    public static void WriteCustomEvent(
        string message,
        EventLogEntryType type = EventLogEntryType.Information,
        int eventId = 0)
    {
        try
        {
            if (!EventLog.SourceExists(ApplicationName))
            {
                EventLog.CreateEventSource(ApplicationName, "WebAppShipping");
            }

            // Write a custom event entry using the same event source.
            EventLog.WriteEntry(ApplicationName, message, type, eventId);
        }
        catch
        {
            // Avoid throwing exceptions from a logging method.
        }
    }
}
