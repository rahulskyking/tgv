# TheGameVoice

TheGameVoice is a modern gaming media platform inspired by GamingBolt, built with ASP.NET Core and Clean Architecture principles.

## Features

### Content Management

* Articles
* Categories
* Tags
* Games
* Media Library
* SEO Metadata
* Rich Text Editor (TinyMCE)

### Editorial Workflow

* Draft Articles
* Review Workflow (Planned)
* Publishing System (Planned)
* Author Management (Planned)

### Media Management

* Upload Media
* Featured Images
* Media Metadata
* Media Picker Modal
* Gallery System (Planned)

## Architecture

```text
Domain
Application
Infrastructure
Web
```

### Domain

Business rules and entities.

### Application

Use cases, commands, queries, interfaces.

### Infrastructure

EF Core, repositories, persistence.

### Web

MVC UI, Admin Panel, Public Website.

## Technology Stack

* ASP.NET Core
* Entity Framework Core
* PostgreSQL
* TailwindCSS
* TinyMCE
* TomSelect
* GitHub Actions

## Running Locally

```bash
dotnet restore
dotnet build
```

```bash
npm install
npm run css
```

Run application:

```bash
dotnet run
```

## Project Status

Active Development
