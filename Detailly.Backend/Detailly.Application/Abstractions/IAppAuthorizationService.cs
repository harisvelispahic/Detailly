namespace Detailly.Application.Abstractions;

public interface IAppAuthorizationService
{
    /// <summary>Throws DetaillyUnauthorizedException if the user is not authenticated.</summary>
    void EnsureAuthenticated();

    /// <summary>Throws if not authenticated, and throws DetaillyForbiddenException if the user is neither the resource owner nor admin or manager.</summary>
    void EnsureOwnerOrStaff(int resourceOwnerId, string resourceName = "resource");

    /// <summary>Throws if not authenticated, and throws DetaillyForbiddenException if the user is neither the resource owner nor admin.</summary>
    void EnsureOwnerOrAdmin(int resourceOwnerId, string resourceName = "resource");

    /// <summary>Throws if not authenticated, and throws DetaillyForbiddenException if the user is not admin or manager.</summary>
    void EnsureAdminOrManager();

    /// <summary>Throws if not authenticated, and throws DetaillyForbiddenException if the user is not admin, manager, or employee.</summary>
    void EnsureAnyStaff();

    /// <summary>Throws if not authenticated, and throws DetaillyForbiddenException if the user is neither the resource owner nor any staff role (admin, manager, or employee).</summary>
    void EnsureOwnerOrAnyStaff(int resourceOwnerId, string resourceName = "resource");

    /// <summary>Throws if not authenticated, and throws DetaillyForbiddenException if the user is not an employee.</summary>
    void EnsureEmployee();

    /// <summary>Returns the current user's ID. Throws DetaillyUnauthorizedException if not authenticated.</summary>
    int RequireUserId();
}
