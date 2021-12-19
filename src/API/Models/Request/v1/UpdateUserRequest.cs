﻿namespace Users.API.Models.Request.v1;

public class UpdateUserRequest
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public Sex? Sex { get; set; }

    public Gender? Gender { get; set; }
}