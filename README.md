# <img src="https://github.com/crni99/airport-automation/blob/main/docs/AirportAutomationLogoFull.png" alt="Airport_Automation_Logo" width="50%" height="50%"> <br />

This project showcases a comprehensive full-stack solution, combining a robust **ASP.NET Core Web API backend**, an **MVC web frontend**, and a modern **React single-page application** — all seamlessly integrated to provide a highly secure, scalable, and user-friendly system.

<br />

## ⭐ Live Demo
> **Note:** All services are hosted on **Free Tier** plans. The database automatically enters **paused mode** after a period of inactivity. When making the initial request, please allow **1-2 minutes** for the database to wake up and for the application to become fully operational.

<table>
  <thead>
    <tr>
      <th>Application</th>
      <th>Platform</th>
      <th>Link</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td>ASP.NET Core API</td>
      <td>Azure (Swagger/Docs)</td>
      <td><a href="https://airport-automation-bucbb0eff0dzcuaz.switzerlandnorth-01.azurewebsites.net/swagger/index.html"><b>View API Documentation 🡥</b></a></td>
    </tr>
    <tr>
      <td>ASP.NET Web MVC</td>
      <td>Azure</td>
      <td><a href="https://airport-automation-mvc-d8ekhabyg5dgeedk.switzerlandnorth-01.azurewebsites.net/"><b>Launch MVC Frontend 🡥</b></a></td>
    </tr>
    <tr>
      <td>React SPA</td>
      <td>Azure</td>
      <td><a href="https://salmon-sea-0fb92c303.3.azurestaticapps.net/"><b>Launch React Frontend 🡥</b></a></td>
    </tr>
    <tr>
      <td>React SPA</td>
      <td>Vercel</td>
      <td><a href="https://airport-automation.vercel.app/"><b>Launch React Frontend 🡥</b></a></td>
    </tr>
  </tbody>
</table>

___
<br />

### 🔐 Demo Credentials
> **Note:** These demo credentials are provided for testing and demonstration purposes only.

<table>
  <thead>
    <tr>
      <th>Username</th>
      <th>Password</th>
      <th>Role</th>
      <th>Description</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td>og</td>
      <td>og</td>
      <td>SuperAdmin</td>
      <td>Full CRUD access, data export, and role management</td>
    </tr>
    <tr>
      <td>aa</td>
      <td>aa</td>
      <td>Admin</td>
      <td>CRUD operations + exporting data</td>
    </tr>
    <tr>
      <td>uu</td>
      <td>uu</td>
      <td>User</td>
      <td>Read and filter data only</td>
    </tr>
  </tbody>
</table>

___
<br />

### ⚙️ Compatibility / Continuous Integration (CI) Checks
[![Build & Test .NET Backend](https://github.com/crni99/airport-automation/actions/workflows/dotnet.yml/badge.svg)](https://github.com/crni99/airport-automation/actions/workflows/dotnet.yml)
[![Build & Test React Frontend](https://github.com/crni99/airport-automation/actions/workflows/node.js.yml/badge.svg)](https://github.com/crni99/airport-automation/actions/workflows/node.js.yml)
---
### 🔍 CodeQL, ESLint, and Security Analysis 🛡️
[![CodeQL Analysis (.NET/C#)](https://github.com/crni99/airport-automation/actions/workflows/csharp-codeql.yml/badge.svg)](https://github.com/crni99/airport-automation/actions/workflows/csharp-codeql.yml)
[![CodeQL Analysis (React/JS)](https://github.com/crni99/airport-automation/actions/workflows/react-codeql.yml/badge.svg)](https://github.com/crni99/airport-automation/actions/workflows/react-codeql.yml)
[![ESLint](https://github.com/crni99/airport-automation/actions/workflows/eslint.yml/badge.svg)](https://github.com/crni99/airport-automation/actions/workflows/eslint.yml)
---
### ☁️ Deployment (CD) and Mirroring
<br />
<div>
  <p>
    <a href="https://github.com/crni99/airport-automation/actions/workflows/azure-api-cicd.yml">
      <img src="https://github.com/crni99/airport-automation/actions/workflows/azure-api-cicd.yml/badge.svg">
    </a>
  </p>
  <p>
    <a href="https://github.com/crni99/airport-automation/actions/workflows/azure-mvc-cicd.yml">
      <img src="https://github.com/crni99/airport-automation/actions/workflows/azure-mvc-cicd.yml/badge.svg">
    </a>
  </p>
  <p>
    <a href="https://github.com/crni99/airport-automation/actions/workflows/azure-react-cicd.yml">
      <img src="https://github.com/crni99/airport-automation/actions/workflows/azure-react-cicd.yml/badge.svg">
    </a>
  </p>
  <p>
    <a href="https://github.com/crni99/airport-automation/actions/workflows/vercel-react-deployment.yml">
      <img src="https://github.com/crni99/airport-automation/actions/workflows/vercel-react-deployment.yml/badge.svg">
    </a>
  </p>
  <p>
    <a href="https://github.com/crni99/airport-automation/actions/workflows/mirror-to-gitlab-and-bitbucket.yml">
      <img src="https://github.com/crni99/airport-automation/actions/workflows/mirror-to-gitlab-and-bitbucket.yml/badge.svg">
    </a>
  </p>
</div>

___
<br />

## 📖 Table of Contents
- [🛫 Getting Started](#-getting-started)

- [🏗️ Architecture](#-architecture)

- [📡 Backend - ASP.NET Core Web API](#-backend-aspnet-core-web-api)
  
- [🌐 MVC Web Frontend](#-mvc-web-frontend)
  
- [⚛️ React Frontend](#-react-frontend)
  
- [🚀 Deployment](#-deployment)

- [🔄 Mirroring](#-mirroring)
___
<br />

<a name="-getting-started"></a>
## 🛫 Getting Started
### To run the project locally:
1. **Clone the repository**
```bash
git clone https://github.com/crni99/airport-automation.git
```
2. **Navigate to the backend API project**
```bash
cd airport-automation/backend/AirportAutomationApi
```
3. **Configure Database Provider and Connection String**
- Open `appsettings.json` and choose your preferred database provider.
- Update the `DatabaseProvider` value to match your choice (`SqlServer`, `Postgres`, or `MySql`).
- Update the corresponding connection string with your local server details, credentials, and port.
```json
"DatabaseProvider": "SqlServer", // <- CHANGE this to your selected provider!
"ConnectionStrings": {
  "SqlServerConnection": "Data Source=YOUR_SQL_SERVER_INSTANCE;Initial Catalog=AirportAutomation;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=true;",
  "PostgresConnection": "Host=localhost;Port=5432;Username=YOUR_USER;Password=YOUR_PASSWORD;Database=AirportAutomation",
  "MySqlConnection": "Server=localhost;Port=3306;Database=AirportAutomation;Uid=YOUR_USER;Pwd=YOUR_PASSWORD;"
}
```
4. **Initialize the database**
- You will need to run the appropriate SQL script for your chosen provider to create and seed the database. 
- These scripts are located in **[Data](https://github.com/crni99/airport-automation/tree/main/backend/AirportAutomationInfrastructure/Data)** folder.
<table>
  <thead>
    <tr>
      <th>Database Provider</th>
      <th>SQL File to Run</th>
      <th>Action</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td><code>SqlServer</code></td>
      <td><a href="https://github.com/crni99/airport-automation/blob/main/backend/AirportAutomationInfrastructure/Data/create_airport_automation_db_mssql.sql"><code>mssql.sql 🡥</code></a></td>
      <td>Execute script against SQL Server instance.</td>
    </tr>
    <tr>
      <td><code>Postgres</code></td>
      <td><a href="https://github.com/crni99/airport-automation/blob/main/backend/AirportAutomationInfrastructure/Data/create_airport_automation_db_npgsql.sql"><code>npgsql.sql 🡥</code></a></td>
      <td>Execute script against PostgreSQL server (e.g., via pgAdmin).</td>
    </tr>
    <tr>
      <td><code>MySql</code></td>
      <td><a href="https://github.com/crni99/airport-automation/blob/main/backend/AirportAutomationInfrastructure/Data/create_airport_automation_db_mysql.sql"><code>mysql.sql 🡥</code></a></td>
      <td>Execute script against MySQL server (e.g., via MySQL Workbench).</td>
    </tr>
  </tbody>
</table>

5. **In your IDE (e.g., Visual Studio / JetBrains Rider):**
- Set `AirportAutomationApi` (Web API) and `AirportAutomationWeb` (MVC Web) as the startup projects.
- This ensures the API and MVC frontend run together.
- Alternatively (using CLI/VS Code): Open two separate terminal windows in the `airport-automation/backend` directory and run the following commands concurrently:
    - For the **Web API**: `dotnet run --project AirportAutomationApi/AirportAutomationApi.csproj`
    - For the **MVC Frontend**: `dotnet run --project AirportAutomationWeb/AirportAutomationWeb.csproj`
6. Start the application
```bash
dotnet run
```
### **Optional: To use the React instead of the MVC frontend:**
1. **Navigate to the React frontend**
```bash
cd ../../react-frontend
```
2. **Install dependencies**
```bash
npm install
```
3. **Start the React frontend**
```bash
npm run start
```
___
<br />

<a name="-architecture"></a>
## 🏗️ Architecture

### Core Architecture
<a href="https://github.com/crni99/airport-automation/blob/main/docs/AA_Core_Architecture.svg" target="_blank">
<img src="https://raw.githubusercontent.com/crni99/airport-automation/3522c30f0cb3f048c684829ecbd822c90c62f121/docs/AA_Core_Architecture.svg" alt="Core_Architecture" width="66%" height="66%">
</a>

### Extended Architecture (CI/CD + Deployments)
<a href="https://github.com/crni99/airport-automation/blob/main/docs/AA_Extended_Architecture.svg" target="_blank">
<img src="https://raw.githubusercontent.com/crni99/airport-automation/b109f0eccdc424cfda36aee0501211854a474670/docs/AA_Extended_Architecture.svg" alt="Extended_Architecture" width="100%" height="100%">
</a>

___
<br />

<a name="-backend-aspnet-core-web-api"></a>
## 📡 [Backend - ASP.NET Core Web API](https://github.com/crni99/airport-automation/tree/main/backend/AirportAutomationApi) 🡥

### Modern .NET 8 Architecture
- Built on **`.NET 8 (LTS)`** - Long-term support until November 2026
- **`C# 12`** features - Primary constructors, collection expressions
- **Clean Architecture Principles** - Separation of concerns through dedicated layers and shared package versioning.

### Database and Entity Framework Core
- Support multiple database providers: **SQL Server**, **PostgreSQL**, and **MySQL**.
- Manage database schema and migrations via **Entity Framework Core** for version control.
- Abstract database operations through repository pattern in dedicated layers.
- Configure entities, relationships, and migrations for schema management.

### RESTful API Design
- Design and implement RESTful endpoints following HTTP best practices.
- Support standard HTTP methods (GET, POST, PUT, PATCH, DELETE) with appropriate status codes.
- Structure endpoints with consistent naming conventions and resource hierarchies.

### Input Validation and Data Integrity
- Implement comprehensive input validation using data annotations and custom validators.
- Validate requests at multiple layers (DTOs, model binding, business logic).
- Return detailed validation error messages following RFC 9457 problem details standard.

### RFC 9457 Problem Details for Validation Errors
- Implement RFC 9457 standard for structured error responses.
- Return detailed problem details including:
  - Error type and title
  - HTTP status code
  - Trace identifier for debugging
  - Specific validation errors per field
- Provide consistent error format across all API endpoints.

### Dependency Injection and Service Layer
- Leverage ASP.NET Core's built-in dependency injection container.
- Register services with appropriate lifetimes (Scoped, Transient, Singleton).
- Implement service interfaces for loose coupling and testability.
- Organize business logic in dedicated service layer separate from controllers.

### Searching, Filtering, and Paging Resources
- Implement advanced features such as searching, filtering, and paging to improve the API’s usability and performance.

### Advanced Filtering with Filter Pattern
- Implement **dedicated filter classes** for each entity type (PassengerSearchFilter, PilotSearchFilter, etc.) to enable multi-field search capabilities.
- Create **filter extension methods** to encapsulate query building logic and promote reusability.
- Support case-insensitive search across multiple fields simultaneously (e.g., search passengers by first name, last name, UPRN, passport, address, and phone).
- Apply filter pattern to maintain clean separation between controller logic and query construction.

### Exporting Data to PDF and Excel
- Implement endpoints to export data in PDF and Excel formats for reporting and offline analysis.
- Ensure exported documents preserve formatting and reflect applied filters or search criteria.
- Use **`QuestPDF`** to generate high-quality, customizable **`PDF`** documents.
- Use **`ClosedXML`** to create Excel files with structured data, formatting, and support for advanced **`Excel`** features.

### Securing API and CORS Implementation
- Enable a global CORS policy (`_AllowAll`) for development to facilitate frontend-backend communication.
- **Important:** Configure more restrictive CORS policies for production environments:
  - Specify allowed origins (e.g., your frontend domain)
  - Define allowed HTTP methods and headers
  - Enable credentials if needed
- Plan migration from permissive to strict CORS policies before production deployment.

### Role-Based Authorization
- Implement role-based access control with policies:
  - **`RequireSuperAdminRole`**
  - **`RequireAdminRole`**
  - **`RequireUserRole`**
- Enable fine-grained endpoint access control based on user roles to ensure secure handling of sensitive data.

### Versioning and Documenting API with Swagger
- Manage API versions to maintain backward compatibility.
- Document API endpoints comprehensively for developer-friendly integration.
- Customize Swagger UI with advanced features:
  - **Toggleable dark/light theme** for enhanced usability
  - **Custom branding** with favicon and logo
  - **Custom styling and behavior** via injected CSS and JavaScript
  - **Custom controller ordering** using SwaggerControllerOrder attribute

### Logging and Middleware
- Integrate **Serilog** for structured, centralized logging with configuration from appsettings.json.
- Implement custom middleware for enhanced diagnostics:
  - **RequestLogContextMiddleware** - Enriches logs with request context (trace IDs, user info)
  - **GlobalExceptionHandler** - Standardizes error responses and captures unhandled exceptions
- Configure logging levels and outputs (console, file, external services) for different environments.

### Configuration Management
- Centralize all configurations (e.g., Serilog, rate limiting, authentication secrets, pagination settings) in **`appsettings.json`** for better maintainability and environment flexibility.

### API Rate Limiting
- Implement a rate limiter to protect API resources from abuse, mitigate DDoS attacks, and enhance overall API performance.

### Central Package Management
- Maintain all NuGet package versions in **`Directory.Packages.props`** at solution root.
- Enable `ManagePackageVersionsCentrally` for unified version control across all projects.
- Benefits:
  - **Version consistency** - All projects use identical package versions
  - **Easy updates** - Change version once, updates everywhere
  - **Cleaner .csproj files** - Package references without version attributes
  - **No conflicts** - Eliminates version mismatch issues

### Unit Testing with xUnit
- Write comprehensive unit tests using the xUnit framework to validate individual components in isolation.
- Ensure tests improve code reliability, support refactoring, and simplify debugging.

### Monitoring Application Health with HealthChecks
- Monitor the health of critical components with **custom health check implementations**:
  - **`DatabaseHealthCheck`** - Verifies database connectivity and accessibility
  - **`ApiHealthCheck`** - Monitors API availability and responsiveness
- Configure health check endpoints to provide visibility into system status and integrate them into Swagger documentation for easy access.
- Return detailed health status with Healthy/Degraded/Unhealthy states and custom messages.
___
<br />

<a name="-mvc-web-frontend"></a>
## 🌐 [MVC Web Frontend](https://github.com/crni99/airport-automation/tree/main/mvc-frontend/AirportAutomationWeb) 🡥

### HTTP Communication Layer
- Implement HttpClientFactory with Polly resilience policies:
  - Transient fault handling with exponential backoff
  - Circuit breaker for prolonged failures
  - Request timeout enforcement
- Centralize HttpClient configuration for consistent headers.

### Generic API Service Layer
- Create generic CRUD methods (CreateData<T>, ReadData<T>, etc.) for type-safe communication.
- Dynamically construct endpoints with custom pluralization rules.
- Support advanced filtering, pagination, and data export.

### View Layer
- Handle data presentation with MVC templates and model binding.
- Develop validated input forms with Bootstrap 5 styling.
- Implement AJAX requests for asynchronous updates.

### Client-Side Scripting and AJAX Requests
- Leverage JavaScript, jQuery, and AJAX to build responsive and interactive user interfaces, enabling asynchronous data fetching and partial page updates without full reloads.

### Centralized Message Management
- Implement **Resource Files (.resx)** for centralized alert and error message management.
- Create **`AlertService`** to retrieve localized messages from resource files.
- Use ResourceManager to access messages dynamically based on message keys.
- Prepare foundation for future internationalization (i18n) support.

### Exporting Data to PDF and Excel
- Integrate the export functionality from the API into the MVC frontend, allowing users to generate and download PDF and Excel reports directly from the web interface.
- Provide options to reflect applied filters or search terms in the exported documents, ensuring consistency between the UI and downloaded data.
- Ensure user-friendly interactions with appropriate UI components (e.g., export buttons, spinners, error handling) for a seamless reporting experience.

### Ensuring Web Application Security
- Enforce HTTPS to secure data transmission between client and server.
- Implement protections against common vulnerabilities such as **`Cross-Site Scripting (XSS)`**, **`Cross-Site Request Forgery (CSRF)`**, and control **`Cross-Origin Resource Sharing (CORS)`**.
- Secure API calls with bearer token authorization headers automatically added to all HTTP requests.
___
<br />

<a name="-react-frontend"></a>
## ⚛️ [React Frontend](https://github.com/crni99/airport-automation/tree/main/react-frontend/src) 🡥

### User Interface Design
- Build the frontend using functional components and **`React Hooks`**.
- Design a responsive, mobile-friendly layout with modern styling techniques.
- Utilize the rich set of components from **`Material UI (MUI)`** to implement a sleek, professional, and accessible user interface based on Material Design principles.
- Implement **toggleable dark/light theme** with user preference persistence in localStorage.
- Customize Material-UI theme configuration for consistent styling across the application.

### State Management
- Manage application state via **`Context API`** with multiple dedicated contexts:
  - **`DataContext`** - Centralized API URL configuration
  - **`ThemeContext`** - User theme preference (dark/light mode)
  - **`SidebarContext`** - Sidebar collapse/expand state
- Handle asynchronous operations efficiently using the native **`fetch`** API and middleware where necessary.

### Custom Hooks for Reusability
- Implement custom hooks to abstract common data operations and promote code reuse:
  - **`useFetch`** - Generic data fetching with pagination, filtering, and error handling
  - **`useCreate`** - Unified logic for creating new entities with validation
  - **`useUpdate`** - Consistent update operations across all entities
  - **`useDelete`** - Reusable delete functionality with confirmation
  - **`useExport`** - Centralized export logic for PDF and Excel generation

### Data Fetching and Integration
- Fully integrate with the backend API to retrieve and manage data such as flights, passengers, and airport operations.
- Dynamically render components based on API responses and user interactions.

### Form Handling and Validation
- Manage form inputs using custom hooks (useCreate, useUpdate) with built-in validation.
- Provide real-time validation with user-friendly error handling and feedback via Snackbar notifications.

### Comprehensive Client-Side Validation
- Implement **entity-specific validation rules** with 245+ lines of validation logic.
- Support multiple validation types:
  - Required fields, string lengths, numeric ranges, date/time formats, foreign keys
- Validate specific business rules (e.g., UPRN 13 chars, Passport 9 chars, FlyingHours 0-40000).
- Provide real-time error messages with centralized validation utilities.

### Error Handling
   - Implement centralized error handling with **`CustomError`** class for consistent error messages.
   - Use **`errorUtils`** to differentiate between network errors, server errors, and validation errors.
   - Provide user-friendly error messages and graceful degradation for edge cases.

### Routing and Navigation
- Handle navigation using **`React Router`**, including dynamic and nested routes for scalability.
- Enable seamless page transitions without full reloads.

### Exporting Data to PDF and Excel
- Integrate the frontend with API endpoints to allow users to export data to PDF and Excel formats.
- Allow users to apply filters or search terms before export, with generated documents reflecting those criteria.
- Accompany export actions with UI feedback such as loading spinners and error messages for an improved user experience.
- Provide download links directly in the UI for quick access to exported files.

### Security and Authentication
- Implement secure user login using **`JWT-based authentication`**.
- Apply role-based access control with **`ProtectedRoute`** component to restrict features based on user permissions.
- Automatically redirect unauthenticated users to login page.
- Store authentication tokens securely and manage token expiration.

### Google Maps Integration
- Integrate **Google Maps embedding** to display destination locations visually.
- Implement `MapEmbed` component for dynamic map rendering based on address input.
- Use iframe embedding with lazy loading for optimized performance.
- Provide visual context for airport destinations with interactive maps.

### Performance Optimization
- Improve performance through **`lazy loading`**, **`code splitting`**, and **`memoization`**.
- Optimize re-renders using React best practices to enhance responsiveness.

### Progressive Web App (PWA) Support
- Implement a complete web app manifest (manifest.json) including appropriate names, unique ID, theme colors (#009be5), and icons (including a maskable icon).
- Configure **`service-worker.js`** for offline functionality and caching strategies.
- Ensure the application meets all core PWA installability criteria (served over HTTPS, valid manifest, and service worker) to allow users to install it as a native application on their devices.
___
<br />

<a name="-deployment"></a>
## 🚀 [Deployment](https://github.com/crni99/airport-automation/tree/main/.github/workflows) 🡥
- The .NET API is deployed to **[Azure Web App](https://airport-automation-bucbb0eff0dzcuaz.switzerlandnorth-01.azurewebsites.net/swagger/index.html)** (service name `airport-automation`) via the automated GitHub Actions CI/CD pipeline.
- The .NET Web MCC is deployed to **[Azure Web App](https://airport-automation-mvc-d8ekhabyg5dgeedk.switzerlandnorth-01.azurewebsites.net/)**.
- The React Frontend is deployed to two platforms to ensure high availability and redundancy:
  - **[Azure Static Web Apps](https://salmon-sea-0fb92c303.3.azurestaticapps.net/)**
  - **[Vercel](https://airport-automation.vercel.app/)**
- Integrate basic logging and monitoring solutions to track application health and capture errors in production environments.
___
<br />

<a name="-mirroring"></a>
## 🔄 [Mirroring](https://github.com/crni99/airport-automation/blob/main/.github/workflows/mirror-to-gitlab-and-bitbucket.yml) 🡥
- The project is mirrored from **`GitHub`** to **`GitLab`** and **`Bitbucket`**, where custom **`GitHub Actions`** are configured to automatically trigger **`CI/CD pipelines`** on code changes.
- This mirroring setup ensures continuous integration and deployment on both GitLab and Bitbucket by synchronizing code changes pushed to GitHub.
- Both GitLab and Bitbucket automatically pick up the changes pushed from GitHub, triggering their respective CI/CD pipelines for seamless integration and deployment.
___
<br />

