# TheGameVoice Setup Guide

## Prerequisites

Install:

* Visual Studio 2026
* .NET SDK
* PostgreSQL
* Node.js LTS
* Git

## Clone Repository

```bash
git clone <repository-url>
```

```bash
cd TheGameVoice
```

## Restore Packages

```bash
dotnet restore
```

## Install Frontend Dependencies

```bash
npm install
```

## Build Tailwind CSS

Development:

```bash
npm run css
```

Production:

```bash
npm run css:build
```

## Database

Update connection string:

```text
appsettings.Development.json
```

Run migrations:

```bash
Update-Database
```

or

```bash
dotnet ef database update
```

## Run Application

```bash
dotnet run
```

Open:

```text
https://localhost:xxxx
```

## Daily Workflow

Before starting work:

```bash
git pull
```

After finishing work:

```bash
git add .
git commit -m "Description"
git push
```
