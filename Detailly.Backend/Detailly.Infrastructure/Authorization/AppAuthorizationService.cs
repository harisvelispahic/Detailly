using Detailly.Application.Abstractions;
using Detailly.Application.Common.Exceptions;

namespace Detailly.Infrastructure.Authorization;

public sealed class AppAuthorizationService(IAppCurrentUser currentUser) : IAppAuthorizationService
{
    public void EnsureAuthenticated()
    {
        if (!currentUser.IsAuthenticated || currentUser.ApplicationUserId is null)
            throw new DetaillyUnauthorizedException("User is not authenticated.");
    }

    public void EnsureOwnerOrStaff(int resourceOwnerId, string resourceName = "resource")
    {
        EnsureAuthenticated();
        var isStaff = currentUser.IsAdmin || currentUser.IsManager;
        if (!isStaff && currentUser.ApplicationUserId!.Value != resourceOwnerId)
            throw new DetaillyForbiddenException($"You are not allowed to access this {resourceName}.");
    }

    public void EnsureAdmin()
    {
        EnsureAuthenticated();
        if (!currentUser.IsAdmin)
            throw new DetaillyForbiddenException("This action requires admin access.");
    }

    public void EnsureOwnerOrAdmin(int resourceOwnerId, string resourceName = "resource")
    {
        EnsureAuthenticated();
        if (!currentUser.IsAdmin && currentUser.ApplicationUserId!.Value != resourceOwnerId)
            throw new DetaillyForbiddenException($"You are not allowed to access this {resourceName}.");
    }

    public void EnsureAdminOrManager()
    {
        EnsureAuthenticated();
        if (!currentUser.IsAdmin && !currentUser.IsManager)
            throw new DetaillyForbiddenException("This action requires manager or admin access.");
    }

    public void EnsureAnyStaff()
    {
        EnsureAuthenticated();
        if (!currentUser.IsAdmin && !currentUser.IsManager && !currentUser.IsEmployee)
            throw new DetaillyForbiddenException("This action requires staff access.");
    }

    public void EnsureOwnerOrAnyStaff(int resourceOwnerId, string resourceName = "resource")
    {
        EnsureAuthenticated();
        var isStaff = currentUser.IsAdmin || currentUser.IsManager || currentUser.IsEmployee;
        if (!isStaff && currentUser.ApplicationUserId!.Value != resourceOwnerId)
            throw new DetaillyForbiddenException($"You are not allowed to access this {resourceName}.");
    }

    public void EnsureEmployee()
    {
        EnsureAuthenticated();
        if (!currentUser.IsEmployee)
            throw new DetaillyForbiddenException("This action requires employee access.");
    }

    public int RequireUserId()
    {
        EnsureAuthenticated();
        return currentUser.ApplicationUserId!.Value;
    }
}
