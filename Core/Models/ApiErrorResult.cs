namespace Core.Models;

/// <summary>
/// The ApiErrorResult class represents a well handled error that bubbles to the front-end of the API
/// so the proper status code and message can be returned.
/// </summary>
public class ApiErrorResult
{
    /// <summary>
    /// The FriendlyErrorMessage is a message that should be sent back to the user of the API.
    /// FriendlyErrorMessage should always have a value if the ApiError instance itself is not null.
    /// </summary>
    public string FriendlyErrorMessage { get; set; } = "Sorry, there has been an internal API error.";

    /// <summary>
    /// The ReturnStatusCode represents the way to set a specific status code as a reason
    /// for an error. It should always have a valid status code.
    /// Ex: If a resource already exists on a POST request, ReturnStatusCode should
    /// be a StatusCodes.Status409Conflict, a 409.
    /// </summary>
    public int ReturnStatusCode { get; set; } = 500; // 500 = StatusCodes.Status500InternalServerError
}
