﻿namespace Morsley.UK.People.API.Contracts.Shared;

public class PersonResourceConverter : JsonConverter<PersonResource>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(PersonResource).IsAssignableFrom(typeToConvert);
    }

    public override PersonResource Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)  throw new JsonException("Expected StartObject token");

        var personResource = new PersonResource();
        var id = Guid.Empty;
        string? firstName = null;
        string? lastName = null;
        string? sex = null;
        string? gender = null;
        string? dateOfBirth = null;
        var links = new List<Link>();
        var embedded = new List<PersonResource>();

        try
        {
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject) break;

                switch (reader.TokenType)
                {
                    case JsonTokenType.PropertyName:

                        var propertyName = reader.GetString();
                        reader.Read();
                        if (propertyName == null) break;
                        switch (propertyName.ToLower())
                        {
                            case "id":
                                id = reader.GetGuid();
                                break;
                            case "firstname":
                                firstName = reader.GetString();
                                //if (!string.IsNullOrWhiteSpace(firstName)) data.FirstName = firstName;
                                break;
                            case "lastname":
                                lastName = reader.GetString();
                                //if (!string.IsNullOrWhiteSpace(lastName)) data.LastName = lastName;
                                break;
                            case "sex":
                                var potentialSex = reader.GetString();
                                if (string.IsNullOrEmpty(potentialSex)) break;
                                if (!Enum.TryParse<Sex>(potentialSex, ignoreCase: true, out var resultSex))
                                {
                                    throw new InvalidOperationException($"Sex is not valid: Actual value: '{potentialSex}'");
                                }
                                sex = resultSex.ToString();
                                break;
                            case "gender":
                                var potentialGender = reader.GetString();
                                if (string.IsNullOrEmpty(potentialGender)) break;
                                if (!Enum.TryParse<Gender>(potentialGender, ignoreCase: true, out var resultGender))
                                {
                                    throw new InvalidOperationException($"Gender is not valid: Actual value: '{potentialGender}'");
                                }
                                gender = resultGender.ToString();
                                break;
                            case "dateofbirth":
                                var potentialDate = reader.GetString();
                                if (string.IsNullOrEmpty(potentialDate)) break;
                                if (potentialDate.Length != 10 ||
                                    !DateTime.TryParseExact(potentialDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var dt))
                                {
                                    throw new InvalidOperationException($"DateOfBirth is not valid: Expected format is 'YYYY-MM-DD', actual value: '{potentialDate}'");
                                }
                                dateOfBirth = dt.ToString("yyyy-MM-dd");
                                break;
                            case "_links":
                                if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException("Expected StartArray token");

                                while (reader.Read())
                                {
                                    if (reader.TokenType == JsonTokenType.EndArray) break;
                                    var link = JsonSerializer.Deserialize<Link>(ref reader, options);
                                    if (link != null) links.Add(link);
                                }

                                break;
                            case "_embedded":
                                if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException("Expected StartArray token");

                                while (reader.Read())
                                {
                                    if (reader.TokenType == JsonTokenType.EndArray) break;
                                    var embeddedPersonResource = JsonSerializer.Deserialize<PersonResource>(ref reader, options);
                                    if (embeddedPersonResource != null) embedded.Add(embeddedPersonResource);
                                }

                                break;
                            default: break;
                        }

                        break;

                    default: break;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        if (id != Guid.Empty)
        {
            var person = new PersonResponse(id)
            {
                FirstName = firstName, 
                LastName = lastName,
                Sex = sex,
                Gender = gender,
                DateOfBirth = dateOfBirth
            };
            if (person.Id != Guid.Empty) personResource.AddData(person);
        }

        if (links.Count > 0) personResource.AddLinks(links);
        if (embedded.Count > 0) personResource.AddEmbedded(embedded);

        return personResource;
    }

    public override void Write(Utf8JsonWriter writer, PersonResource value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();

        //writer.WriteStartObject();

        //writer.WriteNumber("CurrentPage", value.CurrentPage);
        //writer.WriteNumber("PageSize", value.PageSize);
        //writer.WriteNumber("TotalPages", value.TotalPages);
        //writer.WriteNumber("TotalCount", value.TotalCount);
        //writer.WriteBoolean("HasPrevious", value.HasPrevious);
        //writer.WriteBoolean("HasNext", value.HasNext);

        //writer.WriteStartArray("Items");
        //foreach(var item in value)
        //{
        //    var json = JsonSerializer.Serialize(item, options).Replace("\\", "");
        //    writer.WriteRawValue(json);
        //}
        //writer.WriteEndArray();

        //writer.WriteEndObject();
    }
}