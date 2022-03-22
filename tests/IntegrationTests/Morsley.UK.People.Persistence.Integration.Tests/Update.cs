﻿namespace Morsley.UK.People.Persistence.Integration.Tests;

internal class Update : PersonRepositoryTests
{
    [Test]
    public async Task Updating_A_Person_Should_Result_In_That_Person_Being_Updated()
    {
        // Arrange...
        NumberOfPeopleInDatabase().Should().Be(0);

        var existing = GenerateTestPerson();
        AddPersonToDatabase(existing);

        NumberOfPeopleInDatabase().Should().Be(1);

        var sut = new PersonRepository(MongoContext!);

        // Act...
        existing.FirstName = AutoFixture.Create<string>();
        existing.LastName = AutoFixture.Create<string>();
        await sut.UpdateAsync(existing);

        // Assert...
        NumberOfPeopleInDatabase().Should().Be(1);

        var updated = GetPersonFromDatabase(existing.Id);
        updated.Id.Should().Be(existing.Id);
        updated.Should().BeEquivalentTo(existing);
    }
}