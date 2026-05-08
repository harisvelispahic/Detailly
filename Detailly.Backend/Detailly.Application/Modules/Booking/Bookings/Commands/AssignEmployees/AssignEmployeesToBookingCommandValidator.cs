namespace Detailly.Application.Modules.Booking.Bookings.Commands.AssignEmployees;

public sealed class AssignEmployeesToBookingCommandValidator : AbstractValidator<AssignEmployeesToBookingCommand>
{
    public AssignEmployeesToBookingCommandValidator()
    {
        RuleFor(x => x.BookingId)
            .GreaterThan(0)
            .WithMessage("Booking ID must be greater than zero.");

        RuleFor(x => x.EmployeeIds)
            .NotNull()
            .WithMessage("Employee list must not be null.");

        RuleForEach(x => x.EmployeeIds)
            .GreaterThan(0)
            .WithMessage("Each employee ID must be greater than zero.");
    }
}
