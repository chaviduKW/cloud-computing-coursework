# API Testing Guide - Authentication Endpoints

## Quick Start Testing

### Prerequisites

- API running on: `https://localhost:7000` (or your configured port)
- Postman or similar API client installed
- Base URL: `https://localhost:7000/api/auth`

---

## **1. User Registration**

**Endpoint:** `POST /api/auth/register`

**Request:**

```json
{
  "firstName": "Mahela",
  "lastName": "Sulakkhana",
  "email": "mahela@techsalary.com",
  "password": "Password123!",
  "confirmPassword": "Password123!"
}
```

**Expected Response (201):**

```json
{
  "success": true,
  "message": "Registration successful",
  "user": {
    "userId": "550e8400-e29b-41d4-a716-446655440000",
    "firstName": "Mahela",
    "lastName": "Sulakkhana",
    "email": "mahela@techsalary.com",
    "role": "User",
    "isActive": true
  }
}
```

**Test Cases:**

- ✅ Valid registration
- ❌ Email already exists (returns 400)
- ❌ Password < 6 characters (returns 400)
- ❌ Passwords don't match (returns 400)
- ❌ Invalid email format (returns 400)

---

## **2. User Login**

**Endpoint:** `POST /api/auth/login`

**Request:**

```json
{
  "email": "mahela@techsalary.com",
  "password": "Password123!"
}
```

**Expected Response (200):**

```json
{
  "success": true,
  "message": "Login successful",
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiI1NTBlODQwMC1lMjliLTQxZDQtYTcxNi00NDY2NTU0NDAwMDAiLCJlbWFpbCI6Im1haGVsYUB0ZWNoc2FsYXJ5LmNvbSIsInVuaXF1ZV9uYW1lIjoiTWFoZWxhIFN1bGFra2hhbmEiLCJyb2xlIjoiVXNlciIsIklzQWN0aXZlIjoidHJ1ZSIsImV4cCI6MTcwNDA2NzIwMCwiaXNzIjoiVGVjaFNhbGFyeUFQSSIsImF1ZCI6IlRlY2hTYWxhcnlDbGllbnQifQ.signature...",
  "refreshToken": "base64encodedtoken123...",
  "user": {
    "userId": "550e8400-e29b-41d4-a716-446655440000",
    "firstName": "Mahela",
    "lastName": "Sulakkhana",
    "email": "mahela@techsalary.com",
    "role": "User",
    "isActive": true
  },
  "expiresAt": "2026-03-02T11:20:00Z"
}
```

**Postman Setup:**

1. In the response, copy the `accessToken` value
2. Go to the Authorization tab
3. Select "Bearer Token"
4. Paste the token

**Test Cases:**

- ✅ Valid credentials
- ❌ Wrong password (returns 401)
- ❌ Non-existent email (returns 401)
- ❌ Inactive user account (returns 401)

---

## **3. Get Current User (Protected)**

**Endpoint:** `GET /api/auth/me`

**Headers Required:**

```
Authorization: Bearer {accessToken}
```

**Expected Response (200):**

```json
{
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "firstName": "Mahela",
  "lastName": "Sulakkhana",
  "email": "mahela@techsalary.com",
  "role": "User",
  "isActive": true
}
```

**Test Cases:**

- ✅ With valid token
- ❌ Without token (returns 401)
- ❌ With expired token (returns 401)
- ❌ With invalid token (returns 401)

---

## **4. Check User Role**

**Endpoint:** `GET /api/auth/check-role`

**Headers Required:**

```
Authorization: Bearer {accessToken}
```

**Expected Response (200):**

```json
{
  "role": "User"
}
```

---

## **5. Refresh Access Token**

**When to use:** When access token expires (15 minutes)

**Endpoint:** `POST /api/auth/refresh-token`

**Request:**

```json
{
  "refreshToken": "base64encodedtoken123..."
}
```

**Expected Response (200):**

```json
{
  "success": true,
  "message": "Token refreshed successfully",
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiI1NTBlODQwMC1l...",
  "refreshToken": "base64encodedtoken123...",
  "expiresAt": "2026-03-02T12:00:00Z"
}
```

**Test Cases:**

- ✅ With valid refresh token
- ❌ With expired refresh token (returns 401)
- ❌ With revoked refresh token (returns 401)
- ❌ With non-existent token (returns 401)

---

## **6. Logout (Revoke Token)**

**Endpoint:** `POST /api/auth/logout`

**Headers Required:**

```
Authorization: Bearer {accessToken}
```

**Request:**

```json
{
  "refreshToken": "base64encodedtoken123..."
}
```

**Expected Response (200):**

```json
{
  "success": true,
  "message": "Logged out successfully"
}
```

---

## **7. Admin-Only Endpoint**

**Endpoint:** `GET /api/auth/users`

**Headers Required:**

```
Authorization: Bearer {adminAccessToken}
```

**Note:** Only works if user has "Admin" role

**Expected Response (200):**

```json
{
  "message": "This endpoint requires Admin role"
}
```

**Test Cases:**

- ✅ With admin user token
- ❌ With regular user token (returns 403)
- ❌ Without token (returns 401)

---

## **Postman Collection Template**

Save this as `auth-collection.json`:

```json
{
  "info": {
    "name": "TechSalary Auth API",
    "schema": "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
  },
  "item": [
    {
      "name": "Register",
      "request": {
        "method": "POST",
        "header": [
          {
            "key": "Content-Type",
            "value": "application/json"
          }
        ],
        "body": {
          "mode": "raw",
          "raw": "{\n  \"firstName\": \"Test\",\n  \"lastName\": \"User\",\n  \"email\": \"test@example.com\",\n  \"password\": \"Password123!\",\n  \"confirmPassword\": \"Password123!\"\n}"
        },
        "url": {
          "raw": "https://localhost:7000/api/auth/register",
          "protocol": "https",
          "host": ["localhost"],
          "port": "7000",
          "path": ["api", "auth", "register"]
        }
      }
    },
    {
      "name": "Login",
      "request": {
        "method": "POST",
        "header": [
          {
            "key": "Content-Type",
            "value": "application/json"
          }
        ],
        "body": {
          "mode": "raw",
          "raw": "{\n  \"email\": \"test@example.com\",\n  \"password\": \"Password123!\"\n}"
        },
        "url": {
          "raw": "https://localhost:7000/api/auth/login",
          "protocol": "https",
          "host": ["localhost"],
          "port": "7000",
          "path": ["api", "auth", "login"]
        }
      }
    },
    {
      "name": "Get Current User",
      "request": {
        "method": "GET",
        "header": [
          {
            "key": "Authorization",
            "value": "Bearer {{accessToken}}"
          }
        ],
        "url": {
          "raw": "https://localhost:7000/api/auth/me",
          "protocol": "https",
          "host": ["localhost"],
          "port": "7000",
          "path": ["api", "auth", "me"]
        }
      }
    },
    {
      "name": "Refresh Token",
      "request": {
        "method": "POST",
        "header": [
          {
            "key": "Content-Type",
            "value": "application/json"
          }
        ],
        "body": {
          "mode": "raw",
          "raw": "{\n  \"refreshToken\": \"{{refreshToken}}\"\n}"
        },
        "url": {
          "raw": "https://localhost:7000/api/auth/refresh-token",
          "protocol": "https",
          "host": ["localhost"],
          "port": "7000",
          "path": ["api", "auth", "refresh-token"]
        }
      }
    },
    {
      "name": "Logout",
      "request": {
        "method": "POST",
        "header": [
          {
            "key": "Authorization",
            "value": "Bearer {{accessToken}}"
          },
          {
            "key": "Content-Type",
            "value": "application/json"
          }
        ],
        "body": {
          "mode": "raw",
          "raw": "{\n  \"refreshToken\": \"{{refreshToken}}\"\n}"
        },
        "url": {
          "raw": "https://localhost:7000/api/auth/logout",
          "protocol": "https",
          "host": ["localhost"],
          "port": "7000",
          "path": ["api", "auth", "logout"]
        }
      }
    }
  ],
  "variable": [
    {
      "key": "accessToken",
      "value": ""
    },
    {
      "key": "refreshToken",
      "value": ""
    }
  ]
}
```

---

## **Common Error Responses**

### 400 Bad Request

```json
{
  "success": false,
  "message": "Invalid email format"
}
```

### 401 Unauthorized

```json
{
  "success": false,
  "message": "Invalid email or password"
}
```

### 403 Forbidden

```json
{
  "code": 403,
  "message": "Forbidden"
}
```

### 500 Internal Server Error

```json
{
  "success": false,
  "message": "Registration failed"
}
```

---

## **Testing Checklist**

- [ ] Register new user
- [ ] Attempt to register with existing email
- [ ] Login with valid credentials
- [ ] Login with invalid password
- [ ] Get current user profile (protected)
- [ ] Try to access protected endpoint without token
- [ ] Check user role
- [ ] Refresh token with valid refresh token
- [ ] Refresh token with expired refresh token
- [ ] Logout and verify token revocation
- [ ] Try accessing with revoked token
- [ ] Verify JWT content (decode and check claims)

---

## **JWT Token Decoding**

Visit https://jwt.io to decode the access token and verify:

- Issued at (iat)
- Expiration (exp)
- User ID and Email
- Role claim
- IsActive claim
- Issuer and Audience

---

**API Documentation Current as of**: March 2, 2026
