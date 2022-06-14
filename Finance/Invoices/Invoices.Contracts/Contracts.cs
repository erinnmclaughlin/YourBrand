﻿namespace YourBrand.Invoices.Contracts;

public record InvoicesBatch(IEnumerable<Invoice> Invoices);

public record Invoice(int Id);

public record InvoicePaid(int Id);