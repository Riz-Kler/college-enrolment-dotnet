# College Enrolment (.NET)

CollegeEnrolmentDotNet is a sample Further Education (FE) student enrolment and timetabling system built using a familiar and practical Microsoft technology stack.

The project is intentionally designed to reflect how FE colleges and similar organisations commonly operate today, using reliable .NET applications and SQL databases rather than experimental or over engineered solutions.

This repository is suitable for technical demonstration and interview discussion.

## Project Purpose

The system demonstrates how a small FE college might manage core operational workflows such as:

- Student enrolment onto courses  
- Course capacity management  
- Timetable visibility  
- Course changes including switching and withdrawal  
- Auditability for support and compliance  

The focus is on staff facing workflows, reliability, and maintainability.

## Technology Stack

- .NET 8 (LTS)  
- ASP.NET MVC for the staff web interface  
- Windows Forms for planned admin and support tooling  
- SQL Server for data storage  
- Entity Framework Core  

Entity Framework Core is used in a pragmatic way:
- Fluent API for schema and constraints  
- LINQ for standard queries  
- Raw SQL for reporting style queries where appropriate  

## Design Principles

The solution follows a number of deliberate design principles:

- Use of familiar and supportable technologies  
- Clear separation of concerns between Domain, Data, and Web layers  
- Explicit audit trails for critical actions  
- Secure by design approach with GDPR awareness  
- Designed to support classroom, blended, and distance learning delivery  

## Core Concepts

The system is designed around the following concepts:

- Students can enrol, withdraw, or switch courses  
- Courses define delivery type, capacity, and timetable  
- Staff actions are recorded in an audit log  
- Admin tooling supports operational and Service Desk teams  

## Security and GDPR Considerations

Security and data protection are considered from the outset:

- Role based access for staff and administrators  
- Minimal exposure of student data  
- Full audit trail of enrolment changes  
- SSO ready architecture suitable for Entra ID or similar providers  
- Clear separation between code and environment specific configuration  

No real student data is used. All data is synthetic and seeded for demonstration purposes.

## AI Considerations

AI is treated as an assistive capability rather than a decision making system.

Potential uses include:
- Explaining errors in plain language  
- Assisting staff with common queries  
- Reducing repetitive Service Desk tickets  

AI does not assess students or make enrolment decisions.

## Project Status

The repository currently contains:
- Solution and project structure  
- Domain model  
- Data access setup prior to database implementation  
- Clean and intentional commit history  

Database context, migrations, and seed data will be added incrementally.

## Notes

This project is designed to be:
- Easy to reason about in technical interviews  
- Familiar to organisations using Microsoft based stacks  
- Extendable without requiring replacement of existing systems  

## Author

Rizwan Kler  
Senior Software Engineer  
