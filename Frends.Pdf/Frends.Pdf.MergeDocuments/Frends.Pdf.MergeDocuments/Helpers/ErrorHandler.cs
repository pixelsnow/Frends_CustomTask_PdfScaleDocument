using System;
using Frends.Pdf.MergeDocuments.Definitions;

namespace Frends.Pdf.MergeDocuments.Helpers;

/// <summary>
/// Handles error with usage of a standard ThrowOnFailure Frends flag
/// </summary>
public static class ErrorHandler
{
    /// <summary>
    /// Handler for exceptions
    /// </summary>
    /// <param name="exception">Caught exception</param>
    /// <param name="throwOnFailure">Frends flag</param>
    /// <param name="errorMessage">Message to throw in error event</param>
    /// <returns>Throw exception if a flag is true, else return Result with Error info</returns>
    public static Result Handle(Exception exception, bool throwOnFailure, string errorMessage)
    {
        if (throwOnFailure)
        {
            if (string.IsNullOrEmpty(errorMessage))
                throw new Exception(exception.Message, exception);

            throw new Exception(errorMessage, exception);
        }

        var message = !string.IsNullOrEmpty(errorMessage)
            ? $"{errorMessage}: {exception.Message}"
            : exception.Message;

        return new Result
        {
            Success = false,
            Error = new Error
            {
                Message = message,
                AdditionalInfo = exception,
            },
        };
    }
}