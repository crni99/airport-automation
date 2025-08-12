
# AirportAutomation <br /> [![.NET](https://github.com/crni99/AirportAutomation/actions/workflows/dotnet.yml/badge.svg)](https://github.com/crni99/AirportAutomation/actions/workflows/dotnet.yml) [![CodeQL](https://github.com/crni99/AirportAutomation/actions/workflows/github-code-scanning/codeql/badge.svg)](https://github.com/crni99/AirportAutomation/actions/workflows/github-code-scanning/codeql)

In this project, I have designed and implemented a robust Web API as part of a full-stack application that includes both MVC and React frontends. Here's a brief overview of what you can expect to find in this project:

## 🔧 Backend: [AirportAutomationAPI](https://github.com/crni99/AirportAutomation/tree/main/AirportAutomation/Airport%D0%90utomationApi)

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

## 🌐 Web Frontend (MVC): [AirportAutomationWEB](https://github.com/crni99/AirportAutomation/tree/main/AirportAutomation/AirportAutomationWeb)

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

## ⚛️ React Frontend: [AirportAutomationReact](https://github.com/crni99/AirportAutomationReact)  
[![Node.js CI](https://github.com/crni99/AirportAutomationReact/actions/workflows/node.js.yml/badge.svg)](https://github.com/crni99/AirportAutomationReact/actions/workflows/node.js.yml)
[![CodeQL](https://github.com/crni99/AirportAutomationReact/actions/workflows/github-code-scanning/codeql/badge.svg)](https://github.com/crni99/AirportAutomationReact/actions/workflows/github-code-scanning/codeql)
[![Vercel](https://img.shields.io/badge/Vercel-Deploy-success?logo=vercel&logoColor=white)](https://airport-automation-react.vercel.app/)
[![Netlify Status](https://api.netlify.com/api/v1/badges/8a260e13-1391-41a7-b0f0-17fdfad59a40/deploy-status)](https://airport-automation-react.netlify.app/)

This React application provides a modern, responsive frontend that communicates directly with the [AirportAutomationAPI](https://github.com/crni99/AirportAutomation). It is designed to enhance user experience and facilitate intuitive interactions for flight booking, airport data management, and more.

### Highlights:
- Built with modern React (hooks, functional components).
- State management with Context API or Redux.
- API integration for live data fetching and updates.
- Secure authentication with JWT and role-based access.
- Dynamic routing with `React Router`.
- Form handling with React Hook Form and validation.
- Deployed on **[Vercel](https://airport-automation-react.vercel.app/)** and **[Netlify](https://airport-automation-react.netlify.app/)**.
___

This project represents my commitment to learning and mastering various facets of web development, particularly in the realm of Web APIs. I hope that the knowledge and insights shared here prove valuable to fellow developers and enthusiasts who are embarking on similar journeys.
