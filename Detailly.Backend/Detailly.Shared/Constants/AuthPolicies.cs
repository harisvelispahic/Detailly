namespace Detailly.Shared.Constants;

public static class AuthPolicies
{
    public const string AdminOnly = "AdminOnly";
    public const string ManagerOnly = "ManagerOnly";
    public const string EmployeeOnly = "EmployeeOnly";
    public const string FleetOnly = "FleetOnly";

    public const string Staff = "Staff";                            //  admin/manager/employee
    public const string AdminOrManager = "AdminOrManager";          //  admin/manager
    public const string Authenticated = "Authenticated";             //  any authenticated user
    public const string StandardClientOnly = "StandardClientOnly";  //  IsFleet = false
}