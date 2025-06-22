using Microsoft.Extensions.DependencyInjection;
using QatarPaymentTest.Repositories.Interface;
using QatarPaymentTest.Repositories.Repos;
using QatarPaymentTest.Services.Implementation;
using QatarPaymentTest.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QatarPaymentTest.Data.DbContextApp;
using QatarPaymentTest.Models.Entities;
using QatarPaymentTest.Models.Enum;
using System.Diagnostics;

namespace QatarPaymentTest.Extensions
{

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
        {
            // Register AutoMapper with assembly scanning
            services.AddAutoMapper(typeof(MappingProfile));

            // Register Repositories
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IContactRepository, ContactRepository>();

            // Register Services
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IContactService, ContactService>();
            services.AddScoped<ICustomFieldService, CustomFieldService>();

            return services;
        }

        public static async Task SeedDataAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Ensure database is created and migrated
            await context.Database.MigrateAsync();

            if (!await context.Companies.AnyAsync())
            {
                // Add Companies
                var companies = new List<Company>
                {
                    new Company { Name = "Qatar Airways", CreatedAt = DateTime.UtcNow },
                    new Company { Name = "Qatar National Bank", CreatedAt = DateTime.UtcNow },
                    new Company { Name = "Ooredoo Qatar", CreatedAt = DateTime.UtcNow }
                };
                await context.Companies.AddRangeAsync(companies);
                await context.SaveChangesAsync();
            }

            if (!await context.Contacts.AnyAsync())
            {
                // Add Contacts
                var contacts = new List<Contact>
                {
                    new Contact { FirstName = "Mohammed Al-Thani", CreatedAt = DateTime.UtcNow },
                    new Contact { FirstName = "Fatima Al-Sayed", CreatedAt = DateTime.UtcNow },
                    new Contact { FirstName = "Ahmed Al-Kuwari", CreatedAt = DateTime.UtcNow }
                };
                await context.Contacts.AddRangeAsync(contacts);
                await context.SaveChangesAsync();
            }

            if (!await context.CustomFields.AnyAsync())
            {
                // Add Custom Fields
                var customFields = new List<CustomField>
                {
                    new CustomField
                    {
                        Name = "Email",
                        EntityType = "Company",
                        FieldType = CustomFieldType.Text,
                        IsRequired = true,
                        Description = "Company email address",
                        CreatedAt = DateTime.UtcNow
                    },
                    new CustomField
                    {
                        Name = "Phone",
                        EntityType = "Company",
                        FieldType = CustomFieldType.Text,
                        IsRequired = true,
                        Description = "Company phone number",
                        CreatedAt = DateTime.UtcNow
                    },
                    new CustomField
                    {
                        Name = "Email",
                        EntityType = "Contact",
                        FieldType = CustomFieldType.Text,
                        IsRequired = true,
                        Description = "Contact email address",
                        CreatedAt = DateTime.UtcNow
                    },
                    new CustomField
                    {
                        Name = "Mobile",
                        EntityType = "Contact",
                        FieldType = CustomFieldType.Text,
                        IsRequired = true,
                        Description = "Contact mobile number",
                        CreatedAt = DateTime.UtcNow
                    }
                };
                await context.CustomFields.AddRangeAsync(customFields);
                await context.SaveChangesAsync();
            }



            if (!await context.Set<CompanyCustomFieldValue>().AnyAsync())
            {
                var companies = await context.Companies.ToListAsync();
                var companyFields = await context.CustomFields.Where(cf => cf.EntityType == "Company").ToListAsync();

                // Add sample custom field values for companies
                var companyValues = new List<CompanyCustomFieldValue>();
                foreach (var company in companies)
                {
                    companyValues.Add(new CompanyCustomFieldValue
                    {
                        CompanyId = company.Id,
                        CustomFieldId = companyFields[0].Id, // Email field
                        Value = $"info@{company.Name.ToLower().Replace(" ", "")}.com",
                        CreatedAt = DateTime.UtcNow
                    });
                    companyValues.Add(new CompanyCustomFieldValue
                    {
                        CompanyId = company.Id,
                        CustomFieldId = companyFields[1].Id, // Phone field
                        Value = "+974 4000 0000",
                        CreatedAt = DateTime.UtcNow
                    });
                }
                await context.Set<CompanyCustomFieldValue>().AddRangeAsync(companyValues);
                await context.SaveChangesAsync();
            }

            if (!await context.Set<ContactCustomFieldValue>().AnyAsync())
            {
                var contacts = await context.Contacts.ToListAsync();
                var contactFields = await context.CustomFields.Where(cf => cf.EntityType == "Contact").ToListAsync();

                // Add sample custom field values for contacts
                var contactValues = new List<ContactCustomFieldValue>();
                foreach (var contact in contacts)
                {
                    contactValues.Add(new ContactCustomFieldValue
                    {
                        ContactId = contact.Id,
                        CustomFieldId = contactFields[0].Id, // Email field
                        Value = $"{contact.Name.ToLower().Replace(" ", ".")}@gmail.com",
                        CreatedAt = DateTime.UtcNow
                    });
                    contactValues.Add(new ContactCustomFieldValue
                    {
                        ContactId = contact.Id,
                        CustomFieldId = contactFields[1].Id, // Mobile field
                        Value = "+974 3000 0000",
                        CreatedAt = DateTime.UtcNow
                    });
                }
                await context.Set<ContactCustomFieldValue>().AddRangeAsync(contactValues);
                await context.SaveChangesAsync();
            }
        }

        public static async Task GenerateMillionRecordsAsync(this IServiceProvider serviceProvider, int numberOfRecords = 1000000)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                // Get existing custom fields
                var companyFields = await context.CustomFields
                    .Where(cf => cf.EntityType == "Company")
                    .ToListAsync();

                var contactFields = await context.CustomFields
                    .Where(cf => cf.EntityType == "Contact")
                    .ToListAsync();

                if (!companyFields.Any() || !contactFields.Any())
                {
                    throw new InvalidOperationException("Custom fields must be seeded before generating test data");
                }

                // Generate Companies in batches
                const int batchSize = 10000;
                var totalBatches = (numberOfRecords + batchSize - 1) / batchSize;

                Console.WriteLine($"Starting to generate {numberOfRecords:N0} records...");

                for (int batchIndex = 0; batchIndex < totalBatches; batchIndex++)
                {
                    var currentBatchSize = Math.Min(batchSize, numberOfRecords - batchIndex * batchSize);
                    var companies = new List<Company>(currentBatchSize);
                    var contacts = new List<Contact>(currentBatchSize);

                    // First, create and save companies and contacts
                    for (int i = 0; i < currentBatchSize; i++)
                    {
                        var company = new Company
                        {
                            Name = $"Test Company {batchIndex * batchSize + i + 1}",
                            CreatedAt = DateTime.UtcNow
                        };
                        companies.Add(company);

                        var contact = new Contact
                        {
                            FirstName = $"Test Contact {batchIndex * batchSize + i + 1}",
                            LastName = "Test",
                            Email = $"test{batchIndex * batchSize + i + 1}@example.com",
                            CreatedAt = DateTime.UtcNow
                        };
                        contacts.Add(contact);
                    }

                    // Save companies and contacts first to get their IDs
                    await context.Companies.AddRangeAsync(companies);
                    await context.Contacts.AddRangeAsync(contacts);
                    await context.SaveChangesAsync();

                    // Now create custom field values with the correct IDs
                    var companyCustomFields = new List<CompanyCustomFieldValue>();
                    var contactCustomFields = new List<ContactCustomFieldValue>();

                    foreach (var company in companies)
                    {
                        foreach (var field in companyFields)
                        {
                            companyCustomFields.Add(new CompanyCustomFieldValue
                            {
                                CompanyId = company.Id,
                                CustomFieldId = field.Id,
                                Value = field.Name switch
                                {
                                    "Email" => $"info@testcompany{company.Id}.com",
                                    "Phone" => $"+974 {Random.Shared.Next(1000, 9999)} {Random.Shared.Next(1000, 9999)}",
                                    _ => $"Value for {field.Name}"
                                },
                                CreatedAt = DateTime.UtcNow
                            });
                        }
                    }

                    foreach (var contact in contacts)
                    {
                        foreach (var field in contactFields)
                        {
                            contactCustomFields.Add(new ContactCustomFieldValue
                            {
                                ContactId = contact.Id,
                                CustomFieldId = field.Id,
                                Value = field.Name switch
                                {
                                    "Email" => $"contact{contact.Id}@example.com",
                                    "Mobile" => $"+974 {Random.Shared.Next(1000, 9999)} {Random.Shared.Next(1000, 9999)}",
                                    _ => $"Value for {field.Name}"
                                },
                                CreatedAt = DateTime.UtcNow
                            });
                        }
                    }

                    // Save custom field values
                    await context.CompanyCustomFieldValues.AddRangeAsync(companyCustomFields);
                    await context.ContactCustomFieldValues.AddRangeAsync(contactCustomFields);
                    await context.SaveChangesAsync();

                    // Report progress
                    var progress = (batchIndex + 1.0) / totalBatches * 100;
                    var elapsed = stopwatch.Elapsed;
                    var estimatedTotal = TimeSpan.FromTicks((long)(elapsed.Ticks / progress * 100));
                    var remaining = estimatedTotal - elapsed;

                    Console.WriteLine($"Progress: {progress:F2}% | " +
                                    $"Elapsed: {elapsed.ToString(@"hh\:mm\:ss")} | " +
                                    $"Estimated Remaining: {remaining.ToString(@"hh\:mm\:ss")} | " +
                                    $"Records: {(batchIndex + 1) * batchSize:N0} of {numberOfRecords:N0}");
                }

                stopwatch.Stop();
                Console.WriteLine($"Completed generating {numberOfRecords:N0} records in {stopwatch.Elapsed.ToString(@"hh\:mm\:ss")}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating test data: {ex.Message}");
                throw;
            }
        }
    }

}
