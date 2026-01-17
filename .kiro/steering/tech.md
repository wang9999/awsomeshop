# Technology Stack

## Backend
- **Framework**: ASP.NET Core 8.0 (C#)
- **Database**: MySQL with Entity Framework Core 8.0.2
- **ORM**: Pomelo.EntityFrameworkCore.MySql 8.0.2
- **Cache**: Redis (StackExchange.Redis 2.10.1)
- **Authentication**: JWT Bearer tokens (Microsoft.AspNetCore.Authentication.JwtBearer 8.0.2)
- **Password Hashing**: BCrypt.Net-Next 4.0.3
- **API Documentation**: Swagger/OpenAPI

## Frontend
- **Framework**: React 19.2.0 with TypeScript 5.9.3
- **Build Tool**: Vite 7.2.4
- **UI Library**: Ant Design 6.2.0
- **State Management**: Zustand 5.0.10
- **Routing**: React Router DOM 7.12.0
- **HTTP Client**: Axios 1.13.2
- **Linting**: ESLint 9.39.1

## Common Commands

### Backend
```bash
# Navigate to backend
cd backend/src/AWSomeShop.API

# Restore dependencies
dotnet restore

# Build
dotnet build

# Run development server
dotnet run

# Run with watch (hot reload)
dotnet watch run
```

### Frontend
```bash
# Navigate to frontend
cd frontend

# Install dependencies
npm install

# Run development server (port 5173)
npm run dev

# Build for production
npm run build

# Preview production build
npm run preview

# Lint code
npm run lint
```

## Development Environment
- Backend runs on HTTPS with CORS enabled for `http://localhost:3000` and `http://localhost:5173`
- Database seeding occurs automatically in development mode
- JWT configuration requires `Jwt:SecretKey`, `Jwt:Issuer`, and `Jwt:Audience` in appsettings
