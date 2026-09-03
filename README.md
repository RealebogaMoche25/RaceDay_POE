#RaceDay

##Project Overview

RaceDay is a web-based event management system designed for the South African road running, walking, and cycling community.
The system is designed to allow event organisers to manage sporting events, event categories, participant enrolments, and race results. Participants can create accounts, view available events, enrol in events, and view their personal race results.
The project is being developed progressively using ASP.NET Core, C#, Entity Framework Core, SQL Server, RESTful APIs, and GitHub.

The RaceDay system consists of two main user roles:

###Organiser
The Organiser is responsible for managing sporting events. Organisers can:
-Create and manage events.
-Create and manage event categories.
-View participants enrolled in their events.
-Record participant race results.
-View results for their events.

###Participant
The Participant is a user who takes part in sporting events. Participants can:
-Register and log into the system.
-Manage their profile.
-View available events.
-View event details and categories.
-Enrol in events.
-View their enrolments.
-View their personal race results.

##Part 1 – Planning and Database Design
Part 1 focuses on the planning and design of the RaceDay system.
The project documentation includes:
-Entity Relationship Diagram (ERD)
-Database design
-SQL database script
-API Endpoint Plan
-Project documentation

All required Part 1 documentation is stored in the Docs folder of this repository.

###Part 1 Documentation
The following files are included:
-Docs/erdprog.drawio.png – RaceDay Entity Relationship Diagram
-Docs/PROG6212 PART 1.pdf – Part 1 planning and project documentation
-Docs/RACEDAY_POE.sql – RaceDay SQL database script

##Database Design
The RaceDay database is designed to support users, events, event categories, participant enrolments, and race results.
The main entities include:
-USER – Stores system user information and user roles.
-EVENT – Stores information about sporting events.
-ROUTE – Stores route information associated with events.
-CATEGORY – Stores participation categories for events.
-ENROLMENT – Records participants enrolled in events.
-RESULT – Stores participant finishing times and finishing positions.

The relationships between these entities are represented in the RaceDay ERD.
The SQL database script is provided in:
Docs/RACEDAY_POE.sql

 ##API Endpoint Plan

The Part 1 API Endpoint Plan defines the RESTful API that will be developed for RaceDay.
The plan identifies:
-HTTP method
-API route
-Endpoint purpose
-Required user role
-Request body
-Expected response and HTTP status codes

The API Endpoint Plan contains endpoints covering:
-Authentication
-User profiles
-Events
-Event categories
-Participant enrolments
-Race results

The complete API Endpoint Plan is included in:
Docs/PROG6212 PART 1.pdf
The planned API includes 21 endpoints covering the main functionality required by the RaceDay system.

##Repository Structure
The repository is organised to keep the project documentation and GitHub Actions workflow clearly separated.
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
├── README.md
└── RaceDay_POE.slnx

The .github/workflows directory contains the GitHub Actions workflow used to validate the repository.
The Docs directory contains the required Part 1 documentation.

##Setup Notes
To access the RaceDay project:
1. Clone or download the repository from GitHub.
2. Open the solution in Visual Studio.
3. Review the documentation in the Docs folder.
4. Open RACEDAY_POE.sql in SQL Server Management Studio (SSMS) to view or execute the database script.
5. Review erdprog.drawio.png to view the database ERD.
6. Review PROG6212 PART 1.pdf for the complete Part 1 planning documentation and API Endpoint Plan.
The project uses SQL Server for the database component.

##GitHub and Version Control
GitHub is used to manage the RaceDay project and track the development process.
The repository contains meaningful commits documenting the development and progression of the project.
The project includes at least 20 meaningful commits created during the development process.
Version control is used to:
-Track changes to the project.
-Maintain the project history.
-Store the required documentation.
-Manage the GitHub Actions workflow.
-Provide evidence of the development process.

##CI/CD and GitHub Actions
GitHub Actions is used to automatically validate the RaceDay repository.
The workflow is located at:
.github/workflows/repository-check.yml
The workflow checks that the required project structure and documentation are present in the repository.
The validation includes checking for:
-The Docs folder.
-README.md.
-The RaceDay ERD.
-The Part 1 PDF documentation.
-The RaceDay SQL script.
The workflow runs automatically when changes are pushed to the repository or when a pull request is created.

A successful workflow run is shown below:

<img width="1920" height="1020" alt="PROG6212 PART 1 - Word 03 Sept 2026 18_51_03" src="https://github.com/user-attachments/assets/392cfd78-a14e-4cc3-895d-d12ec6cf03d3" />

<img width="1920" height="1020" alt="RaceDay_POE_ github_workflows_repository-check yml at main · RealebogaMoche25_RaceDay_POE and 13 more pages - Personal - Microsoft​ Edge 03 Sept 2026 18_53_03" src="https://github.com/user-attachments/assets/50c27093-bcfa-4221-a348-01771b858f34" />

The successful green build confirms that the repository structure and required Part 1 documentation have passed the automated validation.

##Project Demonstration Video
A demonstration video has been created to provide an overview of the RaceDay project and its Part 1 requirements.
###YouTube Video:

The video demonstrates the project documentation, database design, API Endpoint Plan, GitHub repository structure, version control, and successful GitHub Actions workflow.

##Project Status
###Part 1
Part 1 includes:
-Project planning
-System requirements
-User roles
-Database design
-Entity Relationship Diagram
-SQL database script
-API Endpoint Plan
-GitHub repository
-Version control
-GitHub Actions repository validation
-Project documentation
-Demonstration video

###Future Development
The planned later stages of RaceDay will involve implementing the RESTful API and developing the MVC front-end.
The API implementation will build on the endpoint plan created during Part 1.


