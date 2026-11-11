# RaceDay

## Project Overview

RaceDay is a web-based event management system designed for the South African road running, walking, and cycling community.

The system allows event organisers to manage sporting events, event categories, participant enrolments, routes, and race results. Participants can create accounts, view available events, enrol in events, manage their profiles, and view their personal race results.

The project is developed using ASP.NET Core, C#, Entity Framework Core, SQL Server, RESTful APIs, Azure Blob Storage, Docker, and GitHub.

The RaceDay system consists of two main user roles:

### Organiser

The Organiser is responsible for managing sporting events. Organisers can:

- Create and manage events.
- Update and delete their own events.
- Create and manage categories for their own events.
- Create and manage routes for their own events.
- View participants enrolled in their events.
- Record participant race results.
- Update results for their events.
- View results for their events.

### Participant

The Participant is a user who takes part in sporting events. Participants can:

- Register and log into the system.
- Manage their own profile.
- Upload a profile picture.
- View available events.
- View event details and categories.
- View available routes.
- Enrol in events.
- View their own enrolments.
- View their personal race results.

---

## Part 1 – Planning and Database Design

Part 1 focuses on the planning and design of the RaceDay system.

The project documentation includes:

- Entity Relationship Diagram (ERD)
- Database design
- SQL database script
- API Endpoint Plan
- Project documentation

All required Part 1 documentation is stored in the `Docs` folder of this repository.

### Part 1 Documentation

The following files are included:

- `Docs/erdprog.drawio.png` – RaceDay Entity Relationship Diagram.
- `Docs/PROG6212 PART 1.pdf` – Part 1 planning and project documentation.
- `Docs/RACEDAY_POE.sql` – RaceDay SQL database script.

### Database Design

The RaceDay database is designed to support users, events, event categories, participant enrolments, routes, and race results.

The main entities include:

- **USER** – Stores system user information and user roles.
- **EVENT** – Stores information about sporting events.
- **ROUTE** – Stores route information associated with events.
- **CATEGORY** – Stores participation categories for events.
- **ENROLMENT** – Records participants enrolled in events.
- **RESULT** – Stores participant finishing times and finishing positions.

The relationships between these entities are represented in the RaceDay ERD.

The Part 1 SQL database script is provided in:

`Docs/RACEDAY_POE.sql`

### API Endpoint Plan

The Part 1 API Endpoint Plan defines the RESTful API developed for RaceDay.

The plan identifies:

- HTTP method
- API route
- Endpoint purpose
- Required user role
- Request body
- Expected response and HTTP status codes

The API includes endpoints covering authentication, user profiles, events, event categories, participant enrolments, race routes, and race results.

The complete API Endpoint Plan is included in:

`Docs/PROG6212 PART 1.pdf`

---

## Part 2 – ASP.NET Core Web API

Part 2 focuses on developing the RaceDay RESTful Web API using ASP.NET Core and C#.

The API provides the application's backend functionality and connects to the SQL Server database using Entity Framework Core.

Key functionality includes:

- User registration and login.
- Cookie-based authentication.
- Role-based authorization.
- Event management.
- Category management.
- Route management.
- Participant enrolments.
- Race result management.
- User profile management.
- API documentation using Swagger.

### Database Integration

The API uses Entity Framework Core with SQL Server.

The Part 2 database is named:

`RACEDAY_POE_PART2`

The database setup script is located at:

`PART2/RACEDAY_POE_PART2.sql`

The API uses the `DefaultConnection` connection string to connect to the database.

### Authentication and Authorization

The API uses cookie-based authentication to identify authenticated users.

Role-based authorization restricts access to functionality according to the user's role.

Organisers can manage their own events and related resources, while participants can manage their own profiles, enrolments, and results.

---

## Part 3 – ASP.NET Core MVC Application

Part 3 introduces the RaceDay front-end application using ASP.NET Core MVC and Razor views.

The MVC application provides a user-friendly interface through which organisers and participants interact with the RaceDay system.

The MVC application communicates with the Part 2 Web API through a dedicated `ApiService` class and a configured HTTP client.

### MVC Functionality

The application supports:

- Organiser and participant account access.
- Event browsing and management.
- Event category and route management.
- Participant enrolments.
- Participant profile management.
- Profile picture uploads.
- Event banner uploads.
- Participant race results.
- Organiser result management.

### MVC Structure

The MVC project is located in:

`PART3/RACEDAY_MVC`

It contains the application's controllers, models, services, view models, Razor views, static assets, and configuration.

The application uses:

- ASP.NET Core MVC
- C#
- Razor views
- Dependency injection
- HTTP client services
- Session management
- Model validation

### MVC-to-API Communication

The MVC application communicates with the API using the named HTTP client `RaceDayApi`.

The `ApiService` class handles API requests, including retrieving events, updating profiles, creating enrolments, and uploading images.

The API authentication cookie is retrieved from the MVC session and forwarded with outgoing API requests so that the API can identify the authenticated user.

---

## Azure Blob Storage Integration

RaceDay uses Azure Blob Storage to store uploaded images rather than storing image files directly in the SQL Server database.

The integration is implemented through the API's `AzureBlobStorageService`.

### Image Storage

The application uses two separate Azure Blob Storage containers:

| Container | Purpose |
|---|---|
| `profile-pictures` | Stores participant profile pictures. |
| `event-banners` | Stores event banner images. |

### Image Upload Process

The image upload process works as follows:

1. A user selects an image through the MVC application.
2. The MVC controller validates the selected file.
3. The MVC `ApiService` sends the image to the relevant API endpoint using multipart form data.
4. The API validates the request and checks authentication and authorization.
5. `AzureBlobStorageService` uploads the image to the appropriate Azure Blob Storage container.
6. The API saves the returned image URL in the relevant SQL Server database record.
7. When an image needs to be displayed, the API generates a secure, time-limited Shared Access Signature (SAS) URL.

### Image Validation and Security

The image upload functionality includes the following restrictions:

- Maximum file size: 5 MB.
- Accepted image formats: JPEG, PNG, and WebP.
- Unique image filenames generated using GUIDs.
- Authentication required for protected upload endpoints.
- Event ownership verification before an organiser can upload an event banner.
- Private blob container access.
- Read-only SAS URLs that expire after 30 minutes.

The Azure Blob Storage connection string is configured using:

`AzureBlobStorage__ConnectionString`

The actual connection string must be supplied through the application's environment configuration and must not be committed to GitHub.

---

## Repository Structure

The repository is organised to keep the project documentation, Part 2 API, automated tests, Part 3 MVC application, Docker configuration, and GitHub Actions workflow clearly separated.

```text
RaceDay_POE/
│
├── .github/
│   └── workflows/
│       └── repository-check.yml
│
├── Docs/
│   ├── erdprog.drawio.png
│   ├── PROG6212 PART 1.pdf
│   └── RACEDAY_POE.sql
│
├── PART2/
│   ├── RACEDAY_API/
│   │   ├── Controllers/
│   │   ├── Data/
│   │   ├── Models/
│   │   ├── Services/
│   │   ├── Program.cs
│   │   ├── RACEDAY_API.csproj
│   │   └── Dockerfile
│   │
│   ├── RACEDAY_API.API.Test/
│   │
│   └── RACEDAY_POE_PART2.sql
│
├── PART3/
│   └── RACEDAY_MVC/
│       ├── Controllers/
│       ├── Filters/
│       ├── Models/
│       ├── Services/
│       ├── ViewModels/
│       ├── Views/
│       ├── wwwroot/
│       ├── Program.cs
│       ├── RaceDay.Web.csproj
│       └── Dockerfile
│
├── .env.example
├── .gitignore
├── docker-compose.yml
├── README.md
└── RaceDay_POE.slnx
```

The `.github/workflows` directory contains the GitHub Actions workflow used to validate, build, and test the RaceDay project.

The `Docs` directory contains the required Part 1 documentation.

The `PART2` directory contains the RaceDay ASP.NET Core Web API, automated testing project, and Part 2 SQL database script.

The `PART3` directory contains the ASP.NET Core MVC application.

The root directory contains the Docker Compose configuration and environment-variable example file.

---

## Setup and Run Instructions – Visual Studio

### Prerequisites

The following software is required to run the applications locally:

- Visual Studio
- .NET 8 SDK
- SQL Server
- SQL Server Management Studio (SSMS)
- Git, if cloning the repository

### 1. Clone or Download the Repository

Clone the RaceDay repository from GitHub or download the repository as a ZIP file.

### 2. Open the Solution

Open the following solution in Visual Studio:

`RaceDay_POE.slnx`

The solution contains:

- `RACEDAY_API` – ASP.NET Core Web API project.
- `RACEDAY_API.API.Test` – Automated testing project.
- `RaceDay.Web` – ASP.NET Core MVC project.

### 3. Set Up the Database

Open SQL Server Management Studio (SSMS).

Use the SQL script located at:

`PART2/RACEDAY_POE_PART2.sql`

The script contains the database setup for the RaceDay Part 2 database.

The database is named:

`RACEDAY_POE_PART2`

Configure the `DefaultConnection` connection string in the API's configuration.

The project uses SQL Server for persistent application data.

The Part 1 SQL database script is also available in the `Docs` folder.

### 4. Configure Azure Blob Storage

Provide a valid Azure Storage connection string through the API's environment configuration.

The configuration key is:

`AzureBlobStorage__ConnectionString`

Do not commit actual Azure Storage credentials to the repository.

### 5. Restore NuGet Packages

When the solution is opened in Visual Studio, restore the required NuGet packages.

The API uses packages including:

- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Tools
- Microsoft.EntityFrameworkCore.Design
- BCrypt.Net-Next
- Swashbuckle.AspNetCore

### 6. Build the Solution

Build the solution in Visual Studio using:

`Build → Build Solution`

The solution should build successfully.

### 7. Run the Applications

Set the appropriate project as the startup project in Visual Studio.

Run the API to access its Swagger documentation and run the MVC application to access the RaceDay user interface.

Swagger can be used to view and test the available API endpoints.

---

## Docker Setup and Deployment

RaceDay includes Docker support for running the API and MVC applications in separate containers.

Docker Compose is used to build, configure, and run both applications together.

### Prerequisites

Before running RaceDay using Docker, install:

- Docker Desktop
- Docker Compose
- SQL Server accessible from the Docker environment
- A valid Azure Blob Storage connection string

### 1. Create the Environment File

The repository includes an example environment file:

`.env.example`

Create a copy in the repository root and name it:

`.env`

The `.env` file contains the configuration required by the API.

Example:

```dotenv
# SQL Server connection
ConnectionStrings__DefaultConnection=Server=host.docker.internal,1433;Database=RACEDAY_POE_PART2;User Id=YOUR_SQL_USERNAME;Password=YOUR_SQL_PASSWORD;MultipleActiveResultSets=true;TrustServerCertificate=True

# Azure Blob Storage connection
AzureBlobStorage__ConnectionString=YOUR_AZURE_STORAGE_CONNECTION_STRING
```

Replace the placeholder values with the appropriate SQL Server and Azure Blob Storage credentials.

**Important:** The `.env` file contains sensitive configuration and must not be committed to GitHub. The repository includes `.env.example` as a safe template.

### 2. Configure SQL Server

Ensure that SQL Server is running and accessible from Docker.

The supplied example uses:

`host.docker.internal,1433`

This allows the API container to connect to SQL Server running on the Docker host.

The SQL Server instance must:

- Allow TCP/IP connections.
- Be accessible on the configured port.
- Have the `RACEDAY_POE_PART2` database available.
- Have valid SQL Server authentication credentials configured.

### 3. Build and Start the Containers

Open a terminal in the root directory of the repository, where `docker-compose.yml` is located.

Build and start both applications using:

```bash
docker compose up -d --build
```

Docker Compose builds the API and MVC images and starts both containers.

### 4. Access the Applications

Once both containers are running, access the applications using the following addresses:

| Application | URL |
|---|---|
| RaceDay MVC | http://localhost:5098 |
| RaceDay API Swagger | http://localhost:5099/swagger |

The MVC application communicates with the API internally through:

`http://api:8080/`

The `api` hostname refers to the API service within the Docker Compose network.

### 5. Check Container Status

To view the running containers, use:

```bash
docker compose ps
```

To view application logs, use:

```bash
docker compose logs
```

To view the API logs only:

```bash
docker compose logs api
```

To view the MVC logs only:

```bash
docker compose logs mvc
```

### 6. Stop the Applications

To stop and remove the running containers, use:

```bash
docker compose down
```

This command stops and removes the containers. It does not delete the Docker images or the database hosted on the external SQL Server.

### Docker Configuration

The repository includes:

- `PART2/RACEDAY_API/Dockerfile` – Builds and runs the RaceDay Web API.
- `PART3/RACEDAY_MVC/Dockerfile` – Builds and runs the RaceDay MVC application.
- `docker-compose.yml` – Configures and runs both services.
- `.env.example` – Provides an example of the required environment variables.

Both Dockerfiles use multi-stage builds with the .NET 8 SDK image for building and the .NET 8 ASP.NET runtime image for running the applications.

Docker Compose maps the API to port `5099` and the MVC application to port `5098` on the host machine. Both applications listen on port `8080` inside their respective containers.

---

## Project Roles and Authorization

The RaceDay API uses role-based authorization to control access to protected functionality.

### Organiser

Organisers can:

- Create events.
- Update and delete their own events.
- Create, update, and delete categories belonging to their events.
- Create, update, and delete routes belonging to their events.
- View participants enrolled in their events.
- Create and update race results for their events.
- View race results for their events.
- Upload event banners for their own events.

Organisers cannot modify resources belonging to another organiser.

### Participant

Participants can:

- Register and log into the system.
- View available events.
- View event details and categories.
- View available routes.
- Manage their own profile.
- Upload a profile picture.
- Enrol in events.
- View their own enrolments.
- View their own race results.

Participants cannot create or manage events, categories, routes, or race results.

---

## Automated Testing

The RaceDay API includes automated unit tests using xUnit and Entity Framework Core InMemory.

The tests use isolated in-memory databases so that individual tests do not depend on the SQL Server database.

The automated test suite currently contains eight tests covering:

- Registration success.
- Duplicate email registration.
- Successful login.
- Incorrect password login failure.
- Organiser event creation.
- Event authorization.
- Participant enrolment.
- Enrolment authorization.

All eight automated tests pass successfully.

---

## GitHub and Version Control

GitHub is used to manage the RaceDay project and track the development process.

The repository contains meaningful commits documenting the development and progression of the project.

Version control is used to:

- Track changes to the project.
- Maintain the project history.
- Store the required documentation.
- Manage the GitHub Actions workflow.
- Provide evidence of the development process.

---

## CI/CD and GitHub Actions

GitHub Actions is used to automatically validate, build, and test the RaceDay project.

The workflow is located at:

`.github/workflows/repository-check.yml`

The workflow runs automatically when changes are pushed to the repository or when a pull request is created.

The workflow:

- Checks out the repository.
- Sets up .NET 8.
- Validates the required repository structure and documentation.
- Builds the RaceDay solution in Release configuration.
- Runs the automated unit tests.

### Successful Green Build

A successful green build was completed using GitHub Actions.

The screenshots below provide evidence of the successful build and automated test execution.

<img width="1766" height="913" alt="Screenshot 2026-11-11 180810" src="https://github.com/user-attachments/assets/c47b2965-9c1e-436d-adfb-c19421573abd" />
<img width="1733" height="945" alt="Screenshot 2026-11-11 180643" src="https://github.com/user-attachments/assets/f15b8dbf-0ede-4d19-8508-c8e335fb501c" />


The successful green build confirms that the RaceDay solution builds successfully and that the automated tests pass in GitHub Actions.

---

## Project Demonstration Videos

Demonstration videos are provided for the different stages of the RaceDay project.

### Part 1 YouTube Video

https://youtu.be/PDVLrLdDqpQ?feature=shared

The Part 1 video demonstrates the project documentation, database design, API Endpoint Plan, GitHub repository structure, version control, and GitHub Actions workflow.

### Part 2 YouTube Video

https://youtu.be/saAEh8LWw7w?feature=shared

The Part 2 video demonstrates the RaceDay ASP.NET Core Web API, authentication and authorization, organiser and participant functionality, API testing, automated unit tests, and the successful GitHub Actions build.

### Part 3 YouTube Video

**Video link to be added after recording.**

The Part 3 video will demonstrate:

- The running RaceDay MVC application.
- The MVC project structure and how controllers, views, and services work together.
- Communication between the MVC application and the Web API.
- Azure Blob Storage integration, including profile picture and event banner uploads.
- Docker configuration and the process of building and running both applications using Docker Compose.

---

## Project Status

### Part 1 – Completed

Part 1 includes:

- Project planning.
- System requirements.
- User roles.
- Database design.
- Entity Relationship Diagram.
- SQL database script.
- API Endpoint Plan.
- GitHub repository.
- Version control.
- GitHub Actions repository validation.
- Project documentation.
- Demonstration video.

### Part 2 – Completed

Part 2 includes:

- ASP.NET Core Web API.
- Entity Framework Core.
- SQL Server database integration.
- User registration and login.
- Cookie authentication.
- Role-based authorization.
- Organiser functionality.
- Participant functionality.
- Event management.
- Category management.
- Route management.
- Participant enrolments.
- Race results.
- Swagger API documentation.
- Automated unit tests.
- GitHub Actions continuous integration.
- Part 2 SQL database script.

### Part 3 – Completed

Part 3 includes:

- ASP.NET Core MVC application.
- Razor views and MVC controllers.
- Participant and organiser interfaces.
- MVC-to-API communication.
- Session-based access management.
- Profile management.
- Profile picture uploads.
- Event banner uploads.
- Azure Blob Storage integration.
- Secure image access using SAS URLs.
- Dockerfiles for the API and MVC applications.
- Docker Compose configuration.
- Docker deployment and application testing.
- Part 3 demonstration video.

