<h1 align="center">
  <br>
  <img src="https://raw.githubusercontent.com/ITU-ISE2025-GROUP8-PT/Annotations/d0f5d65d039f3a011a141c922194e763b18fd9ec/Logo.png" alt="PerfusionTech">
</h1>

<h4 align="center">Project to build a medical image annotation app especially for Angiography.</h4>

<p align="center">
    <a href="https://github.com/ITU-ISE2025-GROUP8-PT/Annotations/commits/master">
    <img src="https://img.shields.io/github/last-commit/ITU-ISE2025-GROUP8-PT/Annotations.svg?style=flat-square&logo=github&logoColor=white"
         alt="GitHub last commit">
	</a>
    <a href="https://team8-itu-2025.atlassian.net/jira/software/projects/PT/summary">
    <img src="https://img.shields.io/badge/Jira-Gruppe 8-blue?style=flat-square&logo=jira&logoColor=white"
         alt="Jira">
	</a>
    <a href="https://github.com/ITU-ISE2025-GROUP8-PT/Annotations/pulls">
    <img src="https://img.shields.io/github/issues-pr-raw/ITU-ISE2025-GROUP8-PT/Annotations.svg?style=flat-square&logo=github&logoColor=white"
         alt="GitHub pull requests">
	</a>
	<a href="https://github.com/ITU-ISE2025-GROUP8-PT/Annotations/issues?q=is%3Aclosed">
    <img src="https://img.shields.io/github/issues-pr-closed/ITU-ISE2025-GROUP8-PT/Annotations.svg?style=flat-square&logo=github&logoColor=white"
         alt="GitHub total closed pull requests">
	</a>
</p>
      


---


# Installation

## Downloading and installing steps
1. **[Download](https://github.com/ITU-ISE2025-GROUP8-PT/Annotations)** the latest version available.
2. **Navigate** to the Annotations root directory<br>
	Choose here to navigate to either Annotations.Blazor directory, which is the application, or the Annotations.API directory.<br>
	Certain functions will require both to run individually to be fully function.
3. **Launch** the API or GUI by **typing** in the _console_ the following command: `dotnet run`
   * If needed then do the exact same for the other directory.
      
       
> [!NOTE]  
> You can run the projects with `dotnet watch` to have "hot reload" available for development. When working with the API, this automatically opens the Swagger UI debug tool to help in the development process.

## Solution Structure

The runnable projects within the solution are:

- `Annotations.API` : Backend of Annotations with endpoints for the various functions. 
- `Annotations.Blazor` : Frontend for Annotations as a Blazor web application. 

# Contribution guidelines

## Branching Strategy
Annotations follows a structure with 3 kinds of branches
- Main
- Integration
- Feature

There is only _one_ **Main** branch and _one_ **Integration** branch.

If you wish to develop on the project, you need to create a **Feature** branch - following below Branch Naming Conventions.

If your **Feature** branch needs integration with other **Feature** branches in order to be deployable, you need to merge these branches using the **Integration** branch.

When the **Integration** branch is ready (up to date with **Main**, and with working additions from the **Feature** branches), the **Integration** branch can be merged into **Main**.

If a **Feature** branch _does not_ need integration with other **Feature** branches and is deployable, it can be merged directly into the **Main** branch.

### Code Review Rules
- When merging directly into **Main** from a **Feature** branch, at least 2 detailed peer code reviews are required on the Pull Request.

- When merging into the **Integration** branch, at least 2 detailed peer code reviews are needed for each merge.

- When merging into **Main** _from the **Integration** branch_, 2 peer code reviews are still needed, but do not need to be detailed, as they have already been reviewed in detail when merging to the **Integration** branch.

## Branch Naming Conventions

All branches follow the convention of

```bash
feature/[function_of_the_feature_and_ticket_id]/[possible_version_number]

bugfix/[what_to_fix]

refactor/[what_to_refactor]  

test/[what_to_test]
```

Examples of naming branches: 

- `feature/upload-image-pt-1`
- `bugfix/misplaced-css-5`
- `refactor/file-system`
- `test/user-registration`

Branch names should be lower-case only, as case-sensitivity for branch names is inconsistent across systems. To be _extra nice_, please use `kebab-skewer-case` rather than underscores. 

## Commit Message Convention

The title is no longer than 50 characters. A description is only needed when the commit is complicated. There must be a blank line between title and description and any co-authors added.

A co-author is added as follows:

```bash
Co-authored-by: firstname lastname <my@githubmail.com>
```

Multiple authors can be added with additional lines of co-authored-by statements.

Example of commit message:

```bash
Added image upload functionality.

A user can now upload their Images through the specified user interface. 

Co-authored-by: Hans Christiansen <hc@gmail.com>
```

## Database Migration
If you wish to add new migrations to the database, you need to run the following commands in the Annotations.API directory:

```bash
dotnet ef Migrations add <name>
dotnet ef database update
```
When running the program all pending migrations will be added to the database.

## Testing
As per our DoD, testing is required for each addition to the program. As of now we have support for unit testing the API, using xUnit, and the Blazor project, using bUnit. E2E testing our website is done with Playwright, using a test user with a locally stored authentication state. Testing should follow the "Arrange, Act, Assert" pattern and cover all relevant functionality of a new feature. Always test that safety measures - such as authorization - work correctly on new features. 


