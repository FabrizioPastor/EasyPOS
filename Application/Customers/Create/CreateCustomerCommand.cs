using Domain.Customers;
using Domain.Primitives;
using Domain.ValueObject;
using ErrorOr;
using MediatR;

namespace Application.Customers.Create;

public record CreateCustomerCommand(
    string Name,
    string LastName,
    string Email,
    string PhoneNumber,
    string Country,
    string Line1,
    string Line2,
    string City,
    string State,
    string ZipCode
    ): IRequest<ErrorOr<Unit>>;

internal sealed class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, ErrorOr<Unit>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerCommandHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));;
    }

    public async Task<ErrorOr<Unit>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        if (PhoneNumber.Create(request.PhoneNumber) is not { } phoneNumber)
            return Error.Validation("Customer.PhoneNumber", "Phone number has not valid format.");

        if (Address.Create(request.Country, request.Line1, request.Line2, request.City, request.State, request.ZipCode)
            is not { } address)
            return Error.Validation("Customer.Address", "Address is not valid.");

        var customer = new Customer(
            new CustomerId(Guid.NewGuid()),
            request.Name,
            request.LastName,
            request.Email,
            phoneNumber,
            address,
            true
        );
        try
        {
            await _customerRepository.Add(customer);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
        catch (Exception e)
        {
            return Error.Failure("CreateCustomer.Failure", e.Message);
        }
    }
}