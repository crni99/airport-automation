# Airport Automation <br /> [![Build and Test .NET](https://github.com/crni99/airport-automation/actions/workflows/dotnet.yml/badge.svg)](https://github.com/crni99/airport-automation/actions/workflows/dotnet.yml) [![React Frontend CI](https://github.com/crni99/airport-automation/actions/workflows/node.js.yml/badge.svg)](https://github.com/crni99/airport-automation/actions/workflows/node.js.yml) [![CodeQL](https://github.com/crni99/airport-automation/actions/workflows/github-code-scanning/codeql/badge.svg)](https://github.com/crni99/airport-automation/actions/workflows/github-code-scanning/codeql) [![Vercel - deploy](https://img.shields.io/badge/Vercel-deployed-30c352?logo=vercel&labelColor=2f353b)](https://airport-automation.vercel.app/) [![Netlify Status](https://api.netlify.com/api/v1/badges/f68f50c9-da24-4df3-a645-973662999506/deploy-status)](https://app.netlify.com/projects/airport-automation/deploys)


This project demonstrates a complete full-stack solution featuring a robust ASP.NET Core Web API backend, an MVC web frontend, and a modern React single-page application — all working together to deliver a secure, scalable, and responsive airport automation system.

## 📡 [Backend - ASP.NET Core Web API](https://github.com/crni99/airport-automation/tree/main/backend/Airport%D0%90utomationApi) 🡥

### Creating the API and Returning Resources:
- Establishing the foundation of the Web API and defining endpoints.
- Implementing the logic to retrieve and return resources in response to client requests.

### Manipulating Resources and Validating Input:
- Demonstrating how to manipulate data resources and ensure data integrity.
- Implementing input validation techniques to enhance the reliability of the API.

### Working with Services and Dependency Injection:
- Employing services and dependency injection to promote modularity and maintainability in the codebase.

### Getting Acquainted with Entity Framework Core:
- Introducing Entity Framework Core, a powerful ORM tool for data access.
- Learning how to configure and interact with the database using Entity Framework Core.

### Using Entity Framework Core in Controllers:
- Integrating Entity Framework Core into API controllers to perform CRUD (Create, Read, Update, Delete) operations on data resources.

### Searching, Filtering, and Paging Resources:
- Implementing advanced features like searching, filtering, and paging to enhance the API's usability and performance.

### Securing API and CORS Implementation:
- Ensuring API security involves robust authentication and authorization mechanisms.
- CORS is carefully configured to permit legitimate cross-origin requests while maintaining security against unauthorized access.

### Role-Based Authorization:
- Implemented role-based access control with policies:
  - `RequireSuperAdminRole`
  - `RequireAdminRole`
  - `RequireUserRole`
- Fine-grained endpoint access based on user roles ensures secure handling of sensitive data.

### CORS Policy:
- Enabled a global CORS policy (`_AllowAll`) to facilitate frontend-backend communication during development.
- Consider configuring more restrictive CORS policies in production for enhanced security.

### Versioning and Documenting API with Swagger:
- Managing API versions to maintain backward compatibility.
- Documenting the API endpoints for easy consumption by developers.

### Logging and Exception Handling for Error Management:
- Integrated Serilog for structured, centralized logging with configuration read from app settings.
- Added custom middleware (`RequestLogContextMiddleware`) to enrich logs with request context such as trace identifiers.
- Implemented a global exception handler middleware (`GlobalExceptionHandler`) to ensure consistent error responses and to capture unhandled exceptions in logs.

### API Rate Limiting:
- Implementing a rate limiter is crucial for Protecting API Resources by preventing abuse, fortifying the system against DDoS Attacks, and Enhancing API Performance.

### Unit Testing with xUnit: 
- Writing comprehensive unit tests using the xUnit framework to validate individual components of the application, ensuring that each unit functions correctly in isolation. 
- These tests help maintain code integrity, improve software reliability, and facilitate easier debugging and refactoring.

### Monitoring Application Health with HealthChecks:
- Monitoring the health of critical components, such as databases and external dependencies, to proactively identify and address potential issues.
- Configuring health check endpoints to offer insights into the overall well-being of the application, and integrating these checks into the Swagger documentation for visibility and ease of access.
___

## 🌐 [MVC Web Frontend](https://github.com/crni99/airport-automation/tree/main/mvc-frontend/AirportAutomationWeb) 🡥

### Consuming APIs with HttpClientFactory:
- Implementing efficient API calls using HttpClientFactory for improved performance and resource management, ensuring seamless integration with external APIs while maintaining optimal usage of resources.

### Integrating Web Services and APIs:
- Consuming external web services and APIs to fetch real-time data or integrate third-party functionalities, allowing seamless data exchange and integration with backend services.

### Managing Data Presentation and User Input:
- Handling dynamic data presentation on web pages using templates and binding techniques while implementing user input forms and validating submitted data for accuracy.

### Client-Side Scripting and AJAX Requests:
- Utilizing JavaScript and AJAX requests to create dynamic and responsive user interfaces, enabling asynchronous data fetching and updating without full page reloads.

### Ensuring Web Application Security:
- Implementing security measures to protect against common web vulnerabilities, enforcing HTTPS, securing data transmission, and safeguarding against threats like Cross-Site Scripting (XSS), Cross-Site Request Forgery (CSRF) and Cross-Origin Resource Sharing (CORS).
___

## ⚛️ [React Frontend](https://github.com/crni99/airport-automation/tree/main/react-frontend/src) 🡥

### User Interface Design
- Built with functional components and React Hooks.
- Responsive, mobile-friendly layout using modern styling techniques.

### State Management
- Application state is managed via Context API or Redux.
- Efficient handling of asynchronous operations using the native `fetch` API and middleware when necessary.

### Data Fetching and Integration
- Full integration with the backend API to retrieve and manage data such as flights, passengers, and airport operations.
- Dynamic rendering of components based on API responses and user interaction.

### Form Handling and Validation
- Form inputs (e.g., bookings, user data) managed with **React Hook Form**.
- Real-time validation with user-friendly error handling and feedback.

### Routing and Navigation
- Navigation handled via **React Router**, including dynamic and nested routes for scalability.
- Seamless page transitions without full reloads.

### Security and Authentication
- Secure user login with **JWT-based authentication**.
- Role-based access control for restricting features based on user permissions.

### Performance Optimization
- Performance improvements through **lazy loading**, **code splitting**, and **memoization**.
- Optimized re-renders using React best practices to enhance responsiveness.

### Deployment and Monitoring
- Deployed to both <strong><a href="https://airport-automation.vercel.app/" target="_blank">Vercel</a></strong> and <strong><a href="https://airport-automation.netlify.app/" target="_blank">Netlify</a></strong> for high availability.
- Integrated basic logging and monitoring to track app health and errors in production environments.
