# 🚀 TechSalary Platform - API Gateway Configuration

## ✅ All APIs Connected to Gateway (Port 5000)

All 5 microservices are now accessible through the API Gateway on **port 5000**.

---

## 📊 Architecture Overview

```
                     ┌──────────────────┐
                     │   API Gateway    │
                     │   Port: 5000     │
                     └────────┬─────────┘
                              │
        ┌─────────────────────┼──────────────────────┬──────────────────┐
        │                     │                      │                  │
        ▼                     ▼                      ▼                  ▼
┌──────────────┐     ┌──────────────┐      ┌──────────────┐   ┌──────────────┐
│ Identity API │     │  Salary API  │      │   Vote API   │   │  Stats API   │
│ Port: 5178   │     │ Port: 5002   │      │ Port: 5215   │   │ Port: 5003   │
└──────────────┘     └──────────────┘      └──────────────┘   └──────────────┘
                              │
                              ▼
                     ┌──────────────┐
                     │  Search API  │
                     │ Port: 5254   │
                     └──────────────┘
```

---

## 🔌 Service Ports

| Service | Direct Port | Gateway Route |
|---------|------------|---------------|
| **API Gateway** | 5000 | - |
| **Identity API** | 5178 | `/api/auth/*` |
| **Salary Submission API** | 5002 | `/api/salaries/*` |
| **Vote API** | 5215 | `/api/vote/*` |
| **Stats API** | 5003 | `/api/stats` |
| **Search API** | 5254 | `/api/search/*` |

---

## 📡 API Endpoints (Through Gateway - Port 5000)

### 🔐 1. Identity API (`/api/auth/*`)

**Base URL**: `http://localhost:5000/api/auth`

- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - User login
- `GET /api/auth/profile` - Get user profile
- `PUT /api/auth/profile` - Update profile
- `DELETE /api/auth/profile` - Delete account

---

### 💰 2. Salary Submission API (`/api/salaries/*`)

**Base URL**: `http://localhost:5000/api/salaries`

#### Submit New Salary
```bash
POST http://localhost:5000/api/salaries
Content-Type: application/json

{
  "country": "Sri Lanka",
  "company": "TechCorp",
  "role": "Software Engineer",
  "experienceLevel": "Mid-Level",
  "salaryAmount": 150000,
  "currency": "LKR",
  "anonymize": true
}
```

#### Get Pending Submissions
```bash
GET http://localhost:5000/api/salaries/pending
```

#### Get Pending After Date
```bash
GET http://localhost:5000/api/salaries?createdAfter=2026-02-28T10:00:00Z
```

---

### 🗳️ 3. Vote API (`/api/vote/*`)

**Base URL**: `http://localhost:5000/api/vote`

#### Cast Vote
```bash
POST http://localhost:5000/api/vote
Content-Type: application/json

{
  "submissionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "voteType": "Upvote"
}
```

#### Get Votes for Submission
```bash
GET http://localhost:5000/api/vote/{submissionId}
```

**Response:**
```json
{
  "submissionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "upvotes": 15,
  "downvotes": 3
}
```

---

### 📊 4. Stats API (`/api/stats`)

**Base URL**: `http://localhost:5000/api/stats`

#### Get Salary Statistics
```bash
GET http://localhost:5000/api/stats?role=Engineer&country=SriLanka&company=Google&experienceLevel=Mid
```

**Query Parameters:**
- `role` (optional) - Job role filter
- `country` (optional) - Country filter
- `company` (optional) - Company filter
- `experienceLevel` (optional) - Experience level filter

**Response:**
```json
{
  "count": 120,
  "averageSalary": 145000,
  "medianSalary": 140000,
  "minSalary": 80000,
  "maxSalary": 250000,
  "currency": "LKR"
}
```

---

### 🔍 5. Search API (`/api/search/*`)

**Base URL**: `http://localhost:5000/api/search`

#### Search Salary Records
```bash
GET http://localhost:5000/api/search?keyword=engineer&country=SriLanka&minSalary=100000&pageSize=20&pageNumber=1
```

**Query Parameters:**
- `keyword` (optional) - Search keyword
- `country` (optional) - Filter by country
- `company` (optional) - Filter by company
- `role` (optional) - Filter by role
- `minSalary` (optional) - Minimum salary
- `maxSalary` (optional) - Maximum salary
- `pageSize` (optional) - Results per page (default: 10)
- `pageNumber` (optional) - Page number (default: 1)

#### Get All Companies
```bash
GET http://localhost:5000/api/search/companies
```

#### Get All Designations
```bash
GET http://localhost:5000/api/search/designations
```

---

## 🚀 How to Start All Services

### Option 1: Using PowerShell Script (Recommended)
```powershell
.\start-all-apis.ps1
```

### Option 2: Manual Start (Individual Terminals)

**Terminal 1 - Identity API:**
```powershell
cd IdentityApi\TechSalary.API
dotnet run
```

**Terminal 2 - Salary Submission API:**
```powershell
cd SalarySubmissionApi
dotnet run
```

**Terminal 3 - Vote API:**
```powershell
cd VoteApi
dotnet run
```

**Terminal 4 - Stats API:**
```powershell
cd StatsApi
dotnet run
```

**Terminal 5 - Search API:**
```powershell
cd SearchApi
dotnet run
```

**Terminal 6 - API Gateway:**
```powershell
cd ApiGateway
dotnet run
```

---

## 🧪 Testing the Gateway

### 1. Access Swagger UI
Open: `http://localhost:5000/swagger`

You should see **5 APIs** in the dropdown:
1. Identity API
2. Salary Submission API
3. Vote API
4. Stats API
5. Search API

### 2. Test API Call Through Gateway

**Example - Submit Salary:**
```bash
curl -X POST http://localhost:5000/api/salaries \
  -H "Content-Type: application/json" \
  -d '{
    "country": "Sri Lanka",
    "company": "ABC Tech",
    "role": "Developer",
    "experienceLevel": "Junior",
    "salaryAmount": 100000,
    "currency": "LKR",
    "anonymize": true
  }'
```

**Example - Get Stats:**
```bash
curl "http://localhost:5000/api/stats?role=Engineer&country=SriLanka"
```

**Example - Search Salaries:**
```bash
curl "http://localhost:5000/api/search?keyword=engineer&pageSize=10"
```

---

## 🔧 Configuration Files Changed

### 1. `ApiGateway/ocelot.json`
✅ Added routes for Vote, Stats, and Search APIs  
✅ Added Swagger endpoints for all 5 APIs  

### 2. `StatsApi/Properties/launchSettings.json`
✅ Changed port from 5178 to 5003 (resolved conflict with Identity API)

### 3. `VoteApi/Program.cs`
✅ Added Swagger configuration with versioning

### 4. `StatsApi/Program.cs`
✅ Added Swagger configuration with versioning

### 5. `SearchApi/Program.cs`
✅ Added Swagger configuration with versioning

---

## 🎯 Benefits of Using Gateway (Port 5000)

✅ **Single Entry Point** - All APIs accessible through one port  
✅ **Unified Swagger UI** - Test all APIs from one interface  
✅ **Centralized Routing** - Easier to manage and monitor  
✅ **CORS Management** - Simplified cross-origin configuration  
✅ **Load Balancing** - Can add multiple instances of backend services  
✅ **Authentication** - Add JWT validation at gateway level  

---

## ⚠️ Important Notes

1. **All services must be running** for Gateway to work properly
2. If a service is down, Gateway will return **503 Service Unavailable**
3. **Port conflicts** - Make sure no other applications are using these ports:
   - 5000 (Gateway)
   - 5178 (Identity)
   - 5002 (Salary)
   - 5215 (Vote)
   - 5003 (Stats)
   - 5254 (Search)

4. **Database connections** - Ensure PostgreSQL is running for services that need it:
   - Salary Submission API
   - Vote API
   - Stats API
   - Search API

---

## 🐛 Troubleshooting

### Gateway shows "Service Unavailable"
✅ Check if backend service is running  
✅ Check logs for connection errors  
✅ Verify port numbers in `ocelot.json`

### Swagger not showing all APIs
✅ Ensure all services have Swagger configured  
✅ Check `SwaggerEndPoints` in `ocelot.json`  
✅ Verify each service's swagger.json is accessible

### Port Already in Use
```powershell
# Find process using port
netstat -ano | findstr :5000

# Kill process
taskkill /PID <process_id> /F
```

---

## 📞 API Summary

| API | Route Pattern | Methods | Description |
|-----|--------------|---------|-------------|
| Identity | `/api/auth/*` | GET, POST, PUT, DELETE | User authentication |
| Salary | `/api/salaries/*` | GET, POST, PUT, DELETE | Salary submissions |
| Vote | `/api/vote/*` | GET, POST | Vote on submissions |
| Stats | `/api/stats` | GET | Salary statistics |
| Search | `/api/search/*` | GET | Search salary data |

---

## ✅ Success Checklist

- [x] All 5 APIs connected to Gateway
- [x] ocelot.json updated with routes
- [x] Port conflicts resolved
- [x] Swagger configured for all APIs
- [x] Start script created
- [x] Documentation complete

---

**🎉 Your TechSalary platform is now fully connected through API Gateway on port 5000!**
