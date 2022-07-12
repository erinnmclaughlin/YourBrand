﻿using System;

using YourBrand.Marketing.Application.Addresses;

namespace YourBrand.Marketing.Application.Contacts;

public record ContactDto(string Id, string FirstName, string LastName, string SSN, string? PhoneHome, string? PhoneMobile, string? Email);
