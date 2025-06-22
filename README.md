# Customer Management API

A comprehensive RESTful API built with ASP.NET Core 8 and PostgreSQL for managing companies, contacts, and custom fields with many-to-many relationships.

## 🏗️ Architecture

This application follows Clean Architecture principles with a layered structure:

- **Controllers**: API endpoints and request handling
- **Services**: Business logic and orchestration
- **Repositories**: Data access abstraction
- **Models**: Entities, DTOs, and enums
- **Data**: Entity Framework configuration and database context

## 🚀 Features

- **Company Management**: CRUD operations for companies
- **Contact Management**: CRUD operations for contacts
- **Custom Fields**: Dynamic custom fields for both companies and contacts
- **Many-to-Many Relationships**: Contacts can belong to multiple companies
- **Custom Field Values**: Store dynamic data with type validation
- **Exception Handling**: Global exception middleware
- **Repository Pattern**: Generic repository with specific implementations

## 🛠️ Tech Stack

- **Framework**: ASP.NET Core 8
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core
- **Mapping**: AutoMapper
- **Architecture**: Clean Architecture / Onion Architecture

## 📋 Prerequisites

- .NET 8 SDK
- PostgreSQL 12+
- Visual Studio 2022 or VS Code

## ⚙️ Setup & Installation

### 1. Clone the Repository
```bash
git clone <repository-url>
cd CustomerManagement.API
```

### 2. Database Configuration
Update the connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=CustomerManagementDB;Username=your_username;Password=your_password"
  }
}
```

### 3. Install Dependencies
```bash
dotnet restore
```

### 4. Database Migration
```bash
# Add migration (if not already created)
dotnet ef migrations add InitialCreate

# Update database
dotnet ef database update
```

### 5. Run the Application
```bash
dotnet run
```

The API will be available at `https://localhost:7000` and `http://localhost:5000`

## 📊 Database Schema

### Core Entities

**Companies**
- Id (Primary Key)
- Name (Required, Max 200 chars)
- CreatedAt
- UpdatedAt

**Contacts**
- Id (Primary Key)
- Name (Required, Max 200 chars)
- CreatedAt
- UpdatedAt

**CustomFields**
- Id (Primary Key)
- Name (Required, Max 100 chars)
- EntityType (Company/Contact)
- FieldType (Text/Number/Date/Boolean)
- IsRequired
- Description
- CreatedAt

### Custom Field Values

**CompanyCustomFieldValue**
- Id (Primary Key)
- CompanyId (Foreign Key)
- CustomFieldId (Foreign Key)
- Value (JSON)

**ContactCustomFieldValue**
- Id (Primary Key)
- ContactId (Foreign Key)
- CustomFieldId (Foreign Key)
- Value (JSON)

### Relationships

- **Company ↔ Contact**: Many-to-Many
- **Company → CompanyCustomFieldValue**: One-to-Many
- **Contact → ContactCustomFieldValue**: One-to-Many
- **CustomField → CustomFieldValue**: One-to-Many

## 🔌 API Endpoints

### Companies
```
GET    /api/companies              # Get all companies
GET    /api/companies/{id}         # Get company by ID
POST   /api/companies              # Create company
PUT    /api/companies/{id}         # Update company
DELETE /api/companies/{id}         # Delete company
```

### Contacts
```
GET    /api/contacts               # Get all contacts
GET    /api/contacts/{id}          # Get contact by ID
POST   /api/contacts               # Create contact
PUT    /api/contacts/{id}          # Update contact
DELETE /api/contacts/{id}          # Delete contact
```

### Custom Fields
```
GET    /api/customfields           # Get all custom fields
GET    /api/customfields/{id}      # Get custom field by ID
POST   /api/customfields           # Create custom field
PUT    /api/customfields/{id}      # Update custom field
DELETE /api/customfields/{id}      # Delete custom field
```

## 📝 Sample Requests

### Create Company
```json
POST /api/companies
{
  "name": "Qatar Tech Solutions",
  "contactIds": [1, 2],
  "customFields": {
    "industry": "Technology",
    "revenue": "5000000",
    "employeeCount": "150"
  }
}
```

### Create Contact
```json
POST /api/contacts
{
  "name": "Ahmed Al-Mansouri",
  "companyIds": [1, 2],
  "customFields": {
    "phoneNumber": "+974-1234-5678",
    "department": "Engineering",
    "yearsExperience": "8"
  }
}
```

### Create Custom Field
```json
POST /api/customfields
{
  "name": "Annual Revenue",
  "entityType": "Company",
  "fieldType": "Number",
  "isRequired": false,
  "description": "Company's annual revenue in USD"
}
```

## 🏗️ Project Structure

```
CustomerManagement.API/
├── Controllers/              # API Controllers
├── Models/
│   ├── Entities/            # Database entities
│   ├── DTOs/                # Data transfer objects
│   └── Enums/               # Enumerations
├── Data/
│   ├── ApplicationDbContext.cs
│   └── Configurations/      # EF Core configurations
├── Services/
│   ├── Interfaces/          # Service contracts
│   └── Implementation/      # Service implementations
├── Repositories/
│   ├── Interfaces/          # Repository contracts
│   └── Implementation/      # Repository implementations
├── Middleware/              # Custom middleware
├── Extensions/              # Service extensions
└── Program.cs               # Application entry point
```

## 🔧 Configuration

### AutoMapper Profiles
The application uses AutoMapper for entity-DTO mapping with circular reference handling:

```csharp
services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
```

### Dependency Injection
Services are registered in `ServiceCollectionExtensions.cs`:

- Repository pattern registration
- Service layer registration
- AutoMapper configuration
- Database context configuration



