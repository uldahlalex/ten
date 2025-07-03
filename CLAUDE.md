# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Development Commands

### Client (React + TypeScript + Vite)
- `cd client && npm run dev` - Start development server
- `cd client && npm run build` - Build for production (runs TypeScript check + Vite build)
- `cd client && npm run lint` - Run ESLint with TypeScript extensions
- `cd client && npm run preview` - Preview production build

### Server (ASP.NET Core + .NET 9)
- `cd server/api && dotnet run` - Start development server
- `cd server/api && dotnet build` - Build the API project
- `cd server/tests && dotnet run -c Release` - Run all TUnit tests
- `cd server/tests && dotnet run -c Release --output Detailed` - Run tests with detailed output
- `cd server/tests && dotnet run -c Release --treenode-filter /*/*/GetMyTags/*` - Run filtered tests
### E2E Testing
- `./run-e2e-with-docker.sh` - Run E2E tests using Playwright server in Docker (automated)
- See `E2E-TESTING-SETUP.md` for IDE integration with Rider
- For manual setup: Start Playwright server, set `PW_TEST_CONNECT_WS_ENDPOINT=ws://127.0.0.1:3000/`, run tests from IDE

### Database
- `cd server/efscaffold && ./scaffold.sh` - Regenerate Entity Framework models from database schema
- Database schema is automatically generated at `server/api/schema_according_to_dbcontext.sql` in non-production environments

## Architecture Overview

This is a full-stack task management application (inspired by TickTick) with the following structure:

### Frontend (client/)
- **Tech Stack**: React 19, TypeScript, Vite, TailwindCSS 4, DaisyUI, Jotai (state management)
- **Routing**: React Router 7 with protected routes for authenticated areas
- **State Management**: Jotai atoms for JWT tokens, tasks, lists, tags, and query parameters
- **Authentication**: JWT-based with support for both password and TOTP (2FA) authentication
- **API Client**: Auto-generated TypeScript client from OpenAPI spec (`generated-client.ts`)

### Backend (server/)
- **Tech Stack**: ASP.NET Core (.NET 9), Entity Framework Core, PostgreSQL
- **Authentication**: JWT tokens with TOTP support using Otp.NET
- **API Documentation**: OpenAPI/Swagger with Scalar API reference
- **Database**: PostgreSQL with schema `ticktick`, using Code-First EF approach
- **Testing**: TUnit framework with AwesomeAssertions and Moq

### Key Architectural Patterns
- **API Client Generation**: TypeScript client is auto-generated from OpenAPI spec during server startup
- **Entity Framework Scaffolding**: Database entities are scaffolded into separate `efscaffold` project
- **Service Layer**: Business logic separated into services (`ITaskService`, `ISecurityService`)
- **Global Exception Handling**: Centralized error handling with `GlobalExceptionHandler`
- **CORS**: Configured to allow any origin for development

### Database Schema
Core entities: `User`, `Tasklist`, `Tickticktask`, `Tag`, `TaskTag` (many-to-many)
- Users can have multiple task lists and tags
- Tasks belong to lists and can have multiple tags
- TOTP secrets stored per user for 2FA
- All timestamps use UTC

### Authentication Flow
1. **Password Auth**: Register/Login with email/password
2. **TOTP Auth**: Optional 2FA setup with QR code generation
3. **JWT Tokens**: Used for API authorization via Authorization header


### Development Notes
- Server auto-generates TypeScript client on startup
- Database seeding happens automatically in non-production environments
- Both client and server have Fly.io deployment configurations
- Tests include integration tests that spin up the full API
- The seeder makes all users with password "password" (uses hashing and salting)
- The testing environment uses a prebuilt JWT which corresponds with user-1's JWT like: {
  "id": "2663757f-0249-4983-bf0d-874c64de093a"
  }