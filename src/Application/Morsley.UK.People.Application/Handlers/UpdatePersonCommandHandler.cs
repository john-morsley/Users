﻿namespace Morsley.UK.People.Application.Handlers;

public sealed class UpdatePersonCommandHandler : IRequestHandler<UpdatePersonCommand, Person>
{
    private readonly IPersonRepository _personRepository;
    private readonly IMapper _mapper;
    private readonly IEventBus _bus;

    public UpdatePersonCommandHandler(
        IPersonRepository personRepository,
        IMapper mapper,
        IEventBus bus)
    {
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _bus = bus ?? throw new ArgumentNullException(nameof(bus));
    }

    public async Task<Person> Handle(UpdatePersonCommand command, CancellationToken ct)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));

        var person = await _personRepository.GetByIdAsync(command.Id);
        if (person == null) throw new ArgumentException("Person not found!", nameof(command));
        return await UpdatePerson(person, command, ct);
    }

    private async Task<Person> UpdatePerson(Person person, UpdatePersonCommand command, CancellationToken ct)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));

        if (person.FirstName != command.FirstName && command.FirstName is not null) person.FirstName = command.FirstName;
        if (person.LastName != command.LastName && command.LastName is not null) person.LastName = command.LastName;
        if (person.Sex != command.Sex) person.Sex = command.Sex;
        if (person.Gender != command.Gender) person.Gender = command.Gender;
        if (command.DateOfBirth is null)
        {
            person.DateOfBirth = null;
        }
        else if (command.DateOfBirth is not null && command.DateOfBirth.Length == Constants.International_Date_Format.Length)
        {
            if (DateTime.TryParseExact(
                    command.DateOfBirth,
                    Constants.International_Date_Format,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var dateOfBirth))
            {
                person.DateOfBirth = dateOfBirth;
            }
        }

        await _personRepository.UpdateAsync(person);

        RaisePersonUpdatedEvent(person);

        return person;
    }

    private void RaisePersonUpdatedEvent(Person person)
    {
        var personUpdatedEvent = _mapper.Map<PersonUpdatedEvent>(person!);
        _bus.Publish(personUpdatedEvent);
    }
}