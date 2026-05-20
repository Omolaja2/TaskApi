# TaskFlow API

A RESTful Task Management API built with ASP.NET Core and MySQL.

## Features

- CRUD Operations
- JWT Authentication
- Pagination
- Filtering
- Swagger Documentation
- MySQL Database
- Entity Framework Core

## Tech Stack

- ASP.NET Core Web API
- MySQL
- Entity Framework Core
- JWT Authentication
- Swagger

## API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| POST | /api/auth/register | Register |
| POST | /api/auth/login | Login |
| GET | /api/tasks | Get Tasks |
| POST | /api/tasks | Create Task |
| PUT | /api/tasks/{id} | Update Task |
| DELETE | /api/tasks/{id} | Delete Task |

## Setup

```bash
git clone YOUR_REPO_LINK
cd TaskFlowAPI
dotnet restore
dotnet run
```

## Environment Variables

Update appsettings.json:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "YOUR_CONNECTION"
  }
}
```

## Deployment

Live API:
https://your-api-url.com