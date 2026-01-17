# Project Structure

## Repository Layout
```
/backend                    # .NET backend API
/frontend                   # React frontend application
/requirements_plan.md       # Product requirements (Chinese)
```

## Backend Structure (`/backend/src/AWSomeShop.API`)

### Core Directories
- **Controllers/**: API endpoints organized by feature
  - Naming: `{Feature}Controller.cs` (e.g., `ProductsController.cs`)
  - Route pattern: `/api/{resource}` for employees, `/api/admin/{resource}` for admins
  
- **Services/**: Business logic layer
  - Interface pattern: `Interfaces/I{Service}.cs`
  - Implementation: `{Service}.cs`
  - Background services in `BackgroundServices/`

- **Repositories/**: Data access layer
  - Interface pattern: `Interfaces/I{Repository}.cs`
  - Implementation: `{Repository}.cs`
  - Direct EF Core DbContext interaction

- **Models/**: Data structures
  - `Entities/`: Database entities (EF Core models)
  - `DTOs/`: Data transfer objects for API requests/responses
  - Naming: `{Feature}Dtos.cs` for DTOs

- **Infrastructure/**: Cross-cutting concerns
  - `DbContext/`: EF Core context and database seeding
  - `Cache/`: Redis caching service
  - `FileStorage/`: File storage abstractions

- **Middleware/**: Custom middleware (e.g., `RoleAuthorizationMiddleware.cs`)

- **Extensions/**: Service registration extensions (`ServiceCollectionExtensions.cs`)

## Frontend Structure (`/frontend/src`)

### Core Directories
- **pages/**: Route components organized by user role
  - `employee/`: Employee-facing pages (ProductList, Cart, Points, etc.)
  - `admin/`: Admin pages (Dashboard, ProductManagement, PointsManagement)
  - Each feature has its own folder with index.ts barrel export

- **components/**: Reusable UI components
  - `Layout/`: Layout wrappers (MainLayout, AdminLayout)
  - Feature components in dedicated folders (e.g., `ProductCard/`, `PointsBalance/`)
  - Each component folder contains `.tsx`, `.css`, and `index.ts`

- **services/**: API client functions
  - Naming: `{feature}Service.ts` (e.g., `productService.ts`)
  - Uses centralized request utility from `utils/request.ts`

- **store/**: Zustand state management
  - Naming: `{feature}Slice.ts` (e.g., `productSlice.ts`)
  - Separate stores for auth, cart, products, points

- **utils/**: Utility functions
  - `request.ts`: Axios wrapper with auth interceptors
  - `auth.ts`: Token management
  - `validators.ts`: Form validation helpers

- **types/**: TypeScript type definitions (`index.ts`)

- **constants/**: Application constants (`index.ts`)

- **router/**: React Router configuration

## Architecture Patterns

### Backend
- **Layered Architecture**: Controllers → Services → Repositories → DbContext
- **Dependency Injection**: All services registered in `ServiceCollectionExtensions.cs`
- **Repository Pattern**: Abstraction over data access
- **DTO Pattern**: Separate API contracts from domain entities
- **Middleware Pipeline**: Custom authorization middleware for role-based access

### Frontend
- **Feature-based Organization**: Pages and components grouped by feature
- **Barrel Exports**: `index.ts` files for clean imports
- **Centralized State**: Zustand stores for global state
- **Service Layer**: API calls abstracted into service modules
- **Type Safety**: TypeScript interfaces for all data structures

## Naming Conventions

### Backend (C#)
- PascalCase for classes, methods, properties
- Async methods suffixed with `Async`
- Interfaces prefixed with `I`
- Private fields prefixed with `_`

### Frontend (TypeScript)
- PascalCase for components and types
- camelCase for functions and variables
- Component files: `ComponentName.tsx`
- Style files: `ComponentName.css`
- Barrel exports in `index.ts`

## Configuration Files
- Backend: `appsettings.json`, `appsettings.Development.json`
- Frontend: `.env.development`, `.env.example`
- Build: `AWSomeShop.API.csproj`, `package.json`, `vite.config.ts`
