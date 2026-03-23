# TechSalary API - Complete Authentication & Authorization System

## 📖 Overview

This is a **production-ready authentication and authorization system** built into the TechSalary API using .NET 8, Entity Framework Core, PostgreSQL, and JWT-based token authentication.

**Key Features:**

- ✅ User registration and secure login
- ✅ JWT access tokens (15-minute expiration)
- ✅ Refresh token mechanism (7-day expiration)
- ✅ Role-based access control (RBAC)
- ✅ Password hashing with BCrypt
- ✅ Token revocation support
- ✅ Protected API endpoints
- ✅ Claim-based authorization

---

## 🚀 Quick Start

### 1. Prerequisites

- .NET 8.0 SDK or higher
- PostgreSQL 12+ running on localhost:25432
- Visual Studio Code or Visual Studio 2022

### 2. Configure Database

Update `TechSalary.API/appsettings.json`:

```json
{
  "Jwt": {
    "SecretKey": "generate-a-secure-32-character-key-here!",
    "Issuer": "TechSalaryAPI",
    "Audience": "TechSalaryClient"
  }
}
```

### 3. Run the API

```bash
cd IdentityApi
dotnet build
dotnet run --project TechSalary.API/IdentityApi.csproj
```

### 4. Access Swagger UI

Navigate to: `https://localhost:7000/swagger/index.html`

---

## 📚 Documentation

| Document                      | Purpose                               |
| ----------------------------- | ------------------------------------- |
| **AUTHENTICATION_GUIDE.md**   | Complete technical documentation      |
| **API_TESTING_GUIDE.md**      | Testing procedures & Postman examples |
| **QUICK_START.md**            | Setup and deployment guide            |
| **ARCHITECTURE.md**           | System architecture & flow diagrams   |
| **IMPLEMENTATION_SUMMARY.md** | What was implemented & features       |

---

## 🔐 API Endpoints

### Public Endpoints

- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - User login
- `POST /api/auth/refresh-token` - Refresh access token

### Protected Endpoints (Requires valid JWT)

- `GET /api/auth/me` - Get current user profile
- `GET /api/auth/check-role` - Get user role
- `POST /api/auth/logout` - Logout (revoke token)
- `GET /api/auth/users` - List all users (Admin only)

---

## 🔄 Authentication Flow

1. **Register** → Validate & hash password → Create user
2. **Login** → Verify credentials → Generate JWT & refresh token
3. **Access Protected** → Validate JWT → Return resource
4. **Refresh Token** → Validate refresh token → Issue new JWT
5. **Logout** → Revoke refresh token → Clear tokens

---

## 💾 New Database Tables

### Users Table

- user_id (UUID, PK)
- email (VARCHAR, UNIQUE)
- password_hash (VARCHAR)
- first_name, last_name (VARCHAR)
- role (VARCHAR - Admin/Manager/User)
- is_active, is_email_verified (BOOLEAN)
- created_at, updated_at, last_login_at (TIMESTAMP)

### Auth Refresh Tokens Table

- refresh_token_id (UUID, PK)
- user_id (UUID, FK)
- token (TEXT)
- expires_at (TIMESTAMP)
- is_revoked (BOOLEAN)
- created_at (TIMESTAMP)

---

## 🔑 Example Usage

### Register

```bash
curl -X POST https://localhost:7000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "John",
    "lastName": "Doe",
    "email": "john@example.com",
    "password": "SecurePass123!",
    "confirmPassword": "SecurePass123!"
  }'
```

### Login

```bash
curl -X POST https://localhost:7000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email": "john@example.com", "password": "SecurePass123!"}'
```

### Access Protected Endpoint

```bash
curl -X GET https://localhost:7000/api/auth/me \
  -H "Authorization: Bearer {accessToken}"
```

---

## 🛡️ Security Features

✅ Password Security

- BCrypt hashing with automatic salt
- Minimum 6-character validation
- Constant-time comparison

✅ Token Security

- HMAC-SHA256 signatures
- JWT validation (signature, expiration, issuer, audience)
- Configurable expiration times
- Token revocation on logout

✅ Access Control

- Role-based authorization
- Claim-based permissions
- Database-backed token revocation

✅ CORS Protection

- Configurable allowed origins
- HTTP method restrictions

---

## 🛠️ Available Roles

- **User** - Default role, basic API access
- **Manager** - Can manage non-admin operations
- **Admin** - Full system access, user management

### Using Roles

```csharp
[Authorize]                              // Any authenticated user
public IActionResult GetAll() { ... }

[Authorize(Roles = "Admin")]             // Admin only
public IActionResult Delete() { ... }

[Authorize(Roles = "Admin,Manager")]     // Multiple roles
public IActionResult Create() { ... }
```

---

## 📦 Project Structure

```
IdentityApi/
├── TechSalary.Core/
│   ├── Entities/
│   │   ├── User.cs                    ✨ NEW
│   │   └── AuthRefreshToken.cs        ✨ NEW
│   ├── DTOs/
│   │   ├── RegisterRequestDto.cs      ✨ NEW
│   │   ├── LoginRequestDto.cs         ✨ NEW
│   │   ├── AuthResponseDto.cs         ✨ NEW
│   │   └── RefreshTokenRequestDto.cs  ✨ NEW
│   ├── Interfaces/
│   │   ├── ITokenService.cs           ✨ NEW
│   │   └── IAuthenticationService.cs  ✨ NEW
│   └── ... (existing entities)
│
├── TechSalary.API/
│   ├── Controllers/
│   │   ├── AuthController.cs          ✨ NEW
│   │   └── ... (existing controllers)
│   ├── Services/
│   │   ├── TokenService.cs            ✨ NEW
│   │   ├── AuthenticationService.cs   ✨ NEW
│   │   └── ... (existing services)
│   ├── Program.cs                     📝 UPDATED
│   ├── appsettings.json               📝 UPDATED
│   └── ... (existing files)
│
├── TechSalary.Infrastructure/
│   ├── Data/
│   │   └── AppDbContext.cs            📝 UPDATED
│   ├── Migrations/
│   │   └── 20260302052129_AddAuthenticationEntities.cs ✨ NEW
│   └── ... (existing files)
│
└── Documentation/
    ├── README.md                      📄 This file (UPDATED)
    ├── AUTHENTICATION_GUIDE.md        📄 NEW - Technical docs
    ├── API_TESTING_GUIDE.md           📄 NEW - Testing guide
    ├── QUICK_START.md                 📄 NEW - Setup guide
    ├── ARCHITECTURE.md                📄 NEW - Architecture diagrams
    └── IMPLEMENTATION_SUMMARY.md      📄 NEW - Implementation details
```

---

## 🧪 Testing

### Using Postman

1. Open Swagger UI at https://localhost:7000/swagger
2. Test `/api/auth/register` endpoint
3. Test `/api/auth/login` endpoint
4. Copy the accessToken
5. Click "Authorize" button and paste token
6. Test protected endpoints

### Testing Checklist

- [ ] Register new user
- [ ] Login with valid credentials
- [ ] Access protected endpoint with token
- [ ] Refresh expired token
- [ ] Logout and verify revocation
- [ ] Test role-based access
- [ ] Test expired token rejection

---

## 🚀 Deployment

### Production Checklist

- [ ] Update JWT secret key (32+ characters)
- [ ] Enable HTTPS/SSL certificates
- [ ] Configure production CORS origins
- [ ] Set up logging and monitoring
- [ ] Configure database backups
- [ ] Implement rate limiting
- [ ] Add email verification
- [ ] Set up password reset flow
- [ ] Enable audit logging

---

## 📞 Support

1. **Quick Start**: See `QUICK_START.md`
2. **Testing**: See `API_TESTING_GUIDE.md`
3. **Technical Details**: See `AUTHENTICATION_GUIDE.md`
4. **Architecture**: See `ARCHITECTURE.md`

---

## ✅ Completed Implementation

- ✅ User registration with validation
- ✅ Secure login with BCrypt
- ✅ JWT token generation (15-minute expiration)
- ✅ Refresh token mechanism (7-day expiration)
- ✅ Role-based access control
- ✅ Protected API endpoints
- ✅ Token revocation on logout
- ✅ Complete documentation
- ✅ Database migrations

---

**Version**: 1.0  
**Status**: ✅ Production Ready  
**Last Updated**: March 2, 2026
"ConnectionStrings": {
"DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=OrderManagementDB;Integrated Security=true;TrustServerCertificate=True;"
}

````

To use a different SQL Server instance, update the connection string in:

- `OrderManagement.API/appsettings.json`
- `OrderManagement.API/appsettings.Development.json`

### Docker Deployment

1. **Build and run with Docker Compose**

   ```bash
   docker-compose up --build
````

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
