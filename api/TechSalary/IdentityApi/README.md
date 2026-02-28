# TechSalary Identity API - Backend

A complete .NET 8 ASP.NET Core backend for a TechSalary Identity API with multi-layered architecture.

## Project Structure

```
TechSalaryIdentity/
├── TechSalary.API/               # ASP.NET Core Web API
│   ├── Controllers/              # REST API endpoints
│   ├── DTOs/                     # Data Transfer Objects
│   ├── Services/                 # Business logic implementations
│   └── Program.cs                # Application startup configuration
├── TechSalary.Core/              # Domain Models & Interfaces
│   ├── Entities/                 # Database entity models
│   └── Interfaces/               # Service interfaces
├── TechSalary.Infrastructure/    # Data Access Layer
│   ├── Data/                     # AppDbContext
│   └── Migrations/               # EF Core migrations
├── docker-compose.yml            # Docker Compose configuration
└── TechSalaryIdentity.sln        # Solution file
```

## Features

### Authentication & Authorization

- **JWT Bearer Token** authentication
- User registration and login
- Role-based access control
- Secure password hashing with BCrypt

### Core Entities

- **Users**: System user management with roles
- **Customers**: Master customer data (hard-coded initial data)
- **Products**: Product catalog with pricing
- **Discounts**: Discount management
- **Orders**: Order management with auto-generated order IDs
- **OrderDetails**: Line items within orders with automatic calculations

### Key Calculations

- **Automatic order total calculations**: SubTotal, Discount Total, Tax applied
- **Per-line item calculations**: Quantity × Unit Price - Discount = Total

### Database

- **SQL Server** (LocalDB for development, Docker for production)
- **Windows Authentication** support for local development
- Entity Framework Core with migrations
- Properly configured decimal precision for financial data

### Tools & Documentation

- **Swagger/OpenAPI** integration for API documentation
- **Docker & Docker Compose** support for containerized deployment
- **CORS** configured for frontend integration

## Getting Started

### Prerequisites

- .NET 8 SDK or later
- SQL Server or SQL Server LocalDB installed
- Docker & Docker Compose (optional, for containerized deployment)

### Development Setup (Local Database)

1. **Clone/Open the project**

   ```bash
   cd c:\Users\MahelaSulakkhana\Desktop\OrderManagement.API
   ```

2. **Restore dependencies**

   ```bash
   dotnet restore
   ```

3. **Apply migrations to create database**

   ```bash
   dotnet ef database update --project OrderManagement.Infrastructure --startup-project OrderManagement.API
   ```

4. **Run the application**

   ```bash
   cd OrderManagement.API
   dotnet run
   ```

   The API will be available at: `http://localhost:5000` (HTTP) or `https://localhost:5001` (HTTPS)

5. **Access Swagger UI**
   Open your browser and navigate to: `http://localhost:5000/swagger`

### Database Configuration

The application uses **(localdb)\MSSQLLocalDB** with Windows Authentication by default.

**Connection String** (in `appsettings.json`):

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=OrderManagementDB;Integrated Security=true;TrustServerCertificate=True;"
}
```

To use a different SQL Server instance, update the connection string in:

- `OrderManagement.API/appsettings.json`
- `OrderManagement.API/appsettings.Development.json`

### Docker Deployment

1. **Build and run with Docker Compose**

   ```bash
   docker-compose up --build
   ```

   The API will be available at: `http://localhost:8080`

2. **Note**: Docker Compose uses SQL Server in a container with **SA authentication**
   - Username: `sa`
   - Password: `YourStrong@Password123`

3. **Stopping the containers**
   ```bash
   docker-compose down
   ```

## API Endpoints

### Authentication

- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login and get JWT token

### Products

- `GET /api/product` - Get all products
- `GET /api/product/{id}` - Get product by ID
- `POST /api/product` - Create new product (requires auth)
- `PUT /api/product/{id}` - Update product (requires auth)
- `DELETE /api/product/{id}` - Delete product (requires auth)

### Discounts

- `GET /api/discount` - Get all discounts
- `GET /api/discount/{id}` - Get discount by ID
- `POST /api/discount` - Create new discount (requires auth)
- `PUT /api/discount/{id}` - Update discount (requires auth)
- `DELETE /api/discount/{id}` - Delete discount (requires auth)

### Customers

- `GET /api/customer` - Get all customers
- `GET /api/customer/{customerCode}` - Get customer by code
- `POST /api/customer` - Create new customer (requires auth)

### Orders

- `GET /api/order` - Get all orders
- `GET /api/order/{id}` - Get order by ID
- `POST /api/order` - Create new order (requires auth) - **Auto-calculates totals**
- `PUT /api/order/{id}` - Update order (requires auth)
- `DELETE /api/order/{id}` - Delete order (requires auth)

## JWT Token Configuration

Edit `appsettings.json` to customize JWT token settings:

```json
"Jwt": {
  "Key": "YourSuperSecretKeyHere1234567890-ChangeThisInProduction",
  "Issuer": "OrderManagementAPI",
  "Audience": "OrderManagementClient",
  "ExpiryMinutes": 60
}
```

**IMPORTANT**: Change the secret key in production!

## Database Seed Data

The application seeds 3 default customers on first run:

- **C001** - John Doe Enterprises
- **C002** - Lanka Traders Pvt Ltd
- **C003** - ABC Solutions

These are referenced in orders by their `CustomerCode`.

## Project Dependencies

### OrderManagement.Core

- No external dependencies (pure domain models & interfaces)

### OrderManagement.Infrastructure

- Microsoft.EntityFrameworkCore.SqlServer 8.0.\*
- Microsoft.EntityFrameworkCore.Tools 8.0.\*
- Microsoft.EntityFrameworkCore.Design 8.0.\*

### OrderManagement.API

- Microsoft.AspNetCore.Authentication.JwtBearer 8.0.\*
- Swashbuckle.AspNetCore 6.4.\*
- BCrypt.Net-Next 4.1.0

## Architecture

This project follows **Clean Architecture** principles:

1. **Core Layer** (OrderManagement.Core)
   - Domain entities
   - Service interfaces
   - Business logic contracts

2. **Infrastructure Layer** (OrderManagement.Infrastructure)
   - Entity Framework Core DbContext
   - Database migrations
   - Data access implementations

3. **API Layer** (OrderManagement.API)
   - REST controllers
   - Data Transfer Objects (DTOs)
   - Service implementations
   - Dependency injection configuration

## Troubleshooting

### Database Connection Issues

- Ensure SQL Server is running
- Verify connection string in appsettings.json
- Check Windows Authentication is properly configured

### Migration Errors

```bash
# Remove the last migration if needed
dotnet ef migrations remove --project OrderManagement.Infrastructure

# Re-apply migrations
dotnet ef migrations add InitialCreate --project OrderManagement.Infrastructure --startup-project OrderManagement.API
dotnet ef database update --project OrderManagement.Infrastructure --startup-project OrderManagement.API
```

### JWT Token Issues

- Ensure token is sent in Authorization header: `Authorization: Bearer <token>`
- Verify token hasn't expired (default expiry: 60 minutes)
- Check secret key matches between token generation and validation

## Development Notes

- All monetary decimal fields are configured with `HasPrecision(18, 2)` for proper financial data handling
- Orders automatically calculate: GrandTotal = ((SubTotal - DiscountTotal) \* (1 + TaxRate/100))
- ProductCode in OrderDetail items are text references (not foreign keys) for flexibility
- Database uses cascade delete for Order → OrderDetail relationship

## Future Enhancements

- [ ] Azure AD/Entra ID integration
- [ ] Payment integration
- [ ] Inventory management
- [ ] Order status tracking
- [ ] Audit logging
- [ ] Unit tests
- [ ] Integration tests
- [ ] Rate limiting
- [ ] API versioning

## License

This project is provided as-is for educational purposes.
