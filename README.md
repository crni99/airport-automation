# <img src="https://github.com/crni99/airport-automation/blob/main/docs/AirportAutomationLogoFull.png" alt="Airport_Automation_Logo" width="50%" height="50%"> <br />

This project showcases a comprehensive full-stack solution, combining a robust **ASP.NET Core Web API backend**, an **MVC web frontend**, and a modern **React single-page application** — all seamlessly integrated to provide a highly secure, scalable, and user-friendly system.
<br />
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
[![Deploy .NET API to Azure Web App (CD)](https://github.com/crni99/airport-automation/actions/workflows/azure-api-cicd.yml/badge.svg)](https://github.com/crni99/airport-automation/actions/workflows/azure-api-cicd.yml)
[![Deploy React Frontend to Azure Static Web Apps (CD)](https://github.com/crni99/airport-automation/actions/workflows/azure-react-cicd.yml/badge.svg)](https://github.com/crni99/airport-automation/actions/workflows/azure-react-cicd.yml) <br />
[![Deploy React Frontend to Vercel (CD)](https://github.com/crni99/airport-automation/actions/workflows/vercel-react-deployment.yml/badge.svg)](https://github.com/crni99/airport-automation/actions/workflows/vercel-react-deployment.yml)
[![Mirror to GitLab and Bitbucket](https://github.com/crni99/airport-automation/actions/workflows/mirror-to-gitlab-and-bitbucket.yml/badge.svg)](https://github.com/crni99/airport-automation/actions/workflows/mirror-to-gitlab-and-bitbucket.yml)
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
  
- [🔐 Demo Credentials](#-demo-credentials)
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
<img src="https://raw.githubusercontent.com/crni99/airport-automation/a2f52a17f201a9c39e605a05935bea25bc0b2c26/docs/AA_Extended_Architecture.svg" alt="Extended_Architecture" width="100%" height="100%">
</a>

___
<br />

<a name="-backend-aspnet-core-web-api"></a>
## 📡 [Backend - ASP.NET Core Web API](https://github.com/crni99/airport-automation/tree/main/backend/AirportAutomationApi) 🡥

### Database Setup and Management
- **`SQL Server`** (or a relevant relational database) is used as the primary data store.
- Manage the database schema and migrations via **`Entity Framework Core`** to ensure version control and smooth schema evolution.
- Map EF Core entities directly to database tables to facilitate efficient data access and manipulation.

### Defining RESTful API Endpoints and Implementing Logic to Serve Requested Resources via HTTP
- Establish the foundation of the Web API and define endpoints.
- Implement the logic to retrieve and return resources in response to client requests.

### Manipulating Resources and Validating Input
- Demonstrate how to manipulate data resources and ensure data integrity.
- Implement input validation techniques to enhance the reliability of the API.

### API Behavior Customization
- Customize model validation error responses to conform to **`RFC 9457`**, with detailed problem details and trace identifiers for easier debugging.

### Working with Services and Dependency Injection
- Employ services and dependency injection to promote modularity and maintainability in the codebase.

### Getting Acquainted with Entity Framework Core
- Utilize Entity Framework Core for database interactions and ORM capabilities within repository and service layers.
- Abstract EF Core queries and commands away from controllers to maintain separation of concerns and improve testability.
- Ensure controllers receive processed data from services, maintaining clean and manageable API endpoints.

### Separation of Concerns with EF Core
- Use EF Core exclusively in repositories and services, keeping controllers free from direct database operations.
- Promote modularity, easier testing, and cleaner controller logic focused on handling HTTP requests and responses.

### Searching, Filtering, and Paging Resources
- Implement advanced features such as searching, filtering, and paging to improve the API’s usability and performance.

### Exporting Data to PDF and Excel
- Implement endpoints to export data in PDF and Excel formats for reporting and offline analysis.
- Ensure exported documents preserve formatting and reflect applied filters or search criteria.
- Use **`QuestPDF`** to generate high-quality, customizable **`PDF`** documents.
- Use **`ClosedXML`** to create Excel files with structured data, formatting, and support for advanced **`Excel`** features.

### Securing API and CORS Implementation
- Secure the API using robust authentication and authorization mechanisms.
- Passwords are securely stored using the **`BCrypt`** hashing algorithm, which incorporates a robust salt and adaptive iteration count to defend against brute-force and rainbow table attacks.
- Handle authentication with **`JWT tokens`** to enable secure, stateless client-server communication.
- Configure **`CORS`** to allow authorized cross-origin requests while maintaining protection against unauthorized access.

### Role-Based Authorization
- Implement role-based access control with policies:
  - **`RequireSuperAdminRole`**
  - **`RequireAdminRole`**
  - **`RequireUserRole`**
- Enable fine-grained endpoint access control based on user roles to ensure secure handling of sensitive data.

### CORS Policy
- Enable a global CORS policy (**`_AllowAll`**) to facilitate frontend-backend communication during development.
- Plan for more restrictive CORS policies in production environments for enhanced security.

### Versioning and Documenting API with Swagger
- Manage API versions to maintain backward compatibility.
- Document API endpoints for easy use by developers.
- Customize Swagger UI with a toggleable dark/light mode to enhance usability and align with user preferences.
  - Toggleable dark/light mode to enhance usability and align with user preferences.
  - A custom favicon and custom logo to match the project’s branding.
  - Injected custom JavaScript and CSS to modify Swagger UI behavior and appearance.
- Implement custom controller ordering in Swagger UI using the [SwaggerControllerOrder] attribute and a helper class (SwaggerControllerOrder<T>), ensuring a logical and predictable display order.

### Logging and Exception Handling for Error Management
- Integrate Serilog for structured, centralized logging, with configuration sourced from app settings.
- Add custom middleware (**`RequestLogContextMiddleware`**) to enrich logs with request context, such as trace identifiers.
- Implement global exception handler middleware (**`GlobalExceptionHandler`**) to standardize error responses and capture unhandled exceptions.

### Middleware Customization
- Implement custom middleware to enhance logging context and provide consistent exception handling, improving diagnostics and debugging.

### Configuration Management
- Centralize all configurations (e.g., Serilog, rate limiting, authentication secrets, pagination settings) in **`appsettings.json`** for better maintainability and environment flexibility.

### API Rate Limiting
- Implement a rate limiter to protect API resources from abuse, mitigate DDoS attacks, and enhance overall API performance.

### Unit Testing with xUnit
- Write comprehensive unit tests using the xUnit framework to validate individual components in isolation.
- Ensure tests improve code reliability, support refactoring, and simplify debugging.

### Monitoring Application Health with HealthChecks
- Monitor the health of critical components such as databases and external services.
- Configure health check endpoints to provide visibility into system status and integrate them into Swagger documentation for easy access.
___
<br />

<a name="-mvc-web-frontend"></a>
## 🌐 [MVC Web Frontend](https://github.com/crni99/airport-automation/tree/main/mvc-frontend/AirportAutomationWeb) 🡥

### Consuming APIs with HttpClientFactory
- Implement efficient and reusable API calls using **`HttpClientFactory`** to improve performance, manage resources effectively, and avoid socket exhaustion.
- Implement a robust resilience strategy using the **`Polly`** library, configuring policies for:
  - **`Transient Fault Handling`**: Automatic retries with exponential backoff to recover from temporary network or server errors.
  - **`Circuit Breaking`**: Automatically halting requests to services experiencing prolonged failures to prevent cascading errors and allow the service time to recover.
  - **`Request Timeout`**: Enforcing a maximum duration for API calls to prevent application hanging on unresponsive services.
- Centralize **`HttpClient`** configuration to ensure consistent request headers, including JSON content type, user agent, and authorization tokens.

### Generic and Typed API Interaction
- Utilize generic methods for CRUD operations (**`CreateData<T>`**, **`ReadData<T>`**, etc.) to enable type-safe, reusable API communication across different data models.
- Dynamically construct API endpoints based on model types with custom pluralization rules for flexible routing.
- Support advanced filtering through dynamically built query strings tailored to each data model's specific filter requirements.
- Implement pagination and optional retrieval of all data items for optimized data loading and exporting.

### Integrating Web Services and APIs
- Consume external and backend web services to fetch real-time data and integrate third-party functionalities seamlessly.
- Manage robust error handling and detailed logging for HTTP responses, including success, conflicts, unauthorized, forbidden, and other error states to improve debugging and user feedback.

### Managing Data Presentation and User Input
- Handle dynamic data presentation using MVC templates and model binding to ensure consistent and user-friendly data display.
- Develop user input forms with validation to maintain data accuracy and integrity.
- Utilize the modern, responsive design and components of **`Bootstrap 5`** for consistent and high-quality UI styling.

### Client-Side Scripting and AJAX Requests
- Leverage JavaScript, jQuery, and AJAX to build responsive and interactive user interfaces, enabling asynchronous data fetching and partial page updates without full reloads.

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
- Utilize the rich set of components from `Material UI (MUI)` to implement a sleek, professional, and accessible user interface based on Material Design principles.

### State Management
- Manage application state via **`Context API`** or **`Redux`**.
- Handle asynchronous operations efficiently using the native **`fetch`** API and middleware where necessary.

### Data Fetching and Integration
- Fully integrate with the backend API to retrieve and manage data such as flights, passengers, and airport operations.
- Dynamically render components based on API responses and user interactions.

### Form Handling and Validation
- Manage form inputs (e.g., bookings, user data) with **`React Hook Form`**.
- Provide real-time validation with user-friendly error handling and feedback.

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
- Apply role-based access control to restrict features based on user permissions.

### Performance Optimization
- Improve performance through **`lazy loading`**, **`code splitting`**, and **`memoization`**.
- Optimize re-renders using React best practices to enhance responsiveness.

### Progressive Web App (PWA) Support
- Implement a complete web app manifest (manifest.json) including appropriate names, unique ID, theme colors (#009be5), and icons (including a maskable icon).
- Ensure the application meets all core PWA installability criteria (served over HTTPS, valid manifest, and service worker) to allow users to install it as a native application on their devices.
___
<br />

<a name="-deployment"></a>
## 🚀 [Deployment](https://github.com/crni99/airport-automation/tree/main/.github/workflows) 🡥
- The .NET API is deployed to **[Azure Web App](https://airport-automation-bucbb0eff0dzcuaz.switzerlandnorth-01.azurewebsites.net/swagger/index.html)** (service name `airport-automation`) via the automated GitHub Actions CI/CD pipeline.
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

<a name="-demo-credentials"></a>
## 🔐 [Demo Credentials](https://github.com/crni99/airport-automation/blob/main/backend/AirportAutomationInfrastructure/Data/create_airport_automation_db_mssql.sql#L213-L218) 🡥

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
      <td>CRUD operations + exporting data + managing other roles</td>
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
