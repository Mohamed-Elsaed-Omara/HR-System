# 🚀 HR Leave Management System

A production-ready Leave Management System built with **ASP.NET Core** following **Clean Architecture** principles and modern backend engineering practices.

This project demonstrates scalable architecture, authentication & authorization, background jobs, API security, logging, validation, testing, and enterprise-level backend development concepts.

---

# ✨ Features

## 🔐 Authentication & Authorization

* ASP.NET Core Identity
* JWT Authentication
* Refresh Tokens with Rotation
* Role-Based Authorization
* Email Confirmation
* Forgot Password & Reset Password
* Logout & Token Revocation
* Secure Password Policies

---

## 🏗 Clean Architecture

Project organized into multiple layers:

* API Layer
* Application Layer
* Domain Layer
* Infrastructure Layer
* Identity Layer
* Persistence Layer
* Blazor UI Layer

Using:

* CQRS Pattern
* MediatR
* Repository Pattern
* Dependency Injection

---

## 📋 Leave Management Features

### Leave Types

* Create Leave Type
* Update Leave Type
* Delete Leave Type
* Get All Leave Types
* Get Leave Type Details

### Leave Requests

* Submit Leave Request
* Approve / Reject Requests
* Cancel Requests
* Employee Leave Tracking

### Leave Allocations

* Automatic Leave Allocation
* Allocation Validation
* Employee Leave Balance Tracking

---

## 📧 Email Services

* Email Confirmation
* Password Reset Emails
* SendGrid Integration

---

## 🔄 Background Jobs

Using **Hangfire**:

* Refresh Token Cleanup
* Scheduled Jobs
* Retry Policies
* Dashboard Monitoring

---

## ⚡ Performance & Reliability

* IP Rate Limiting
* User Rate Limiting
* Global Exception Handling
* Health Checks
* Structured Logging with Serilog

---

## 🧪 Testing

* Unit Testing with xUnit
* Mocking using Moq
* Assertions using Shouldly

---

## 📜 API Documentation

* Swagger / OpenAPI
* JWT Authorization Support
* API Testing Ready

---

# 🛠 Technologies Used

## Backend

* ASP.NET Core Web API
* .NET
* Entity Framework Core
* SQL Server
* MediatR
* AutoMapper
* FluentValidation

## Security

* ASP.NET Core Identity
* JWT Tokens
* Refresh Tokens

## Background Processing

* Hangfire

## Logging

* Serilog

## Testing

* xUnit
* Moq
* Shouldly

## Frontend

* Blazor WebAssembly

---

# 📂 Project Structure

```text
HRLeaveManagementClean
│
├── HRLeaveManagement.API
├── HRLeaveManagement.Application
├── HRLeaveManagement.Domain
├── HRLeaveManagement.Identity
├── HRLeaveManagement.Infrastructure
├── HRLeaveManagement.Persistence
├── HRLeaveManagement.BlazorUI
└── HRLeaveManagement.Application.UnitTests
```

---

# 🔑 Authentication Flow

## Register

1. User registers
2. Confirmation email is sent
3. User confirms email

## Login

1. Validate credentials
2. Generate JWT Access Token
3. Generate Refresh Token

## Refresh Token

1. Validate expired access token
2. Validate refresh token
3. Rotate tokens
4. Return new token pair

---

# 🧪 Running the Project

## 1️⃣ Clone Repository

```bash
git clone https://github.com/YOUR_USERNAME/HRLeaveManagementSystem.git
```

---

## 2️⃣ Update Connection String

Inside:

```text
appsettings.json
```

Update:

```json
"ConnectionStrings": {
  "HrDatabaseConnectionString": "YOUR_CONNECTION_STRING"
}
```

---

## 3️⃣ Apply Migrations

```bash
Update-Database
```

or

```bash
dotnet ef database update
```

---

## 4️⃣ Run the Project

```bash
dotnet run
```

Swagger:

```text
https://localhost:7142/swagger
```

Hangfire Dashboard:

```text
https://localhost:7142/hangfire
```

---

# 📸 Screenshots

## Swagger

*Add screenshot here*

## Hangfire Dashboard

*Add screenshot here*

## Health Checks

*Add screenshot here*

---


# 👨‍💻 Author

## Mohamed Elsaed Omara

Backend Developer specializing in:

* ASP.NET Core
* Clean Architecture
* REST APIs
* Authentication & Security
* Scalable Backend Systems



