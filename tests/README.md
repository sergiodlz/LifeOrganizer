# LifeOrganizer Test Projects

This folder contains test projects for the LifeOrganizer solution:

- **LifeOrganizer.Business.Tests**: Unit tests for business logic and services. Uses Moq for mocking dependencies.
- **LifeOrganizer.Api.Tests**: Integration tests for API controllers. Uses Moq for mocking dependencies.
- **LifeOrganizer.Data.Tests**: Unit tests for data/repository layer. Uses Moq for mocking dependencies.

## Running Tests

You can run all tests from this folder with:

```
dotnet test
```

## Structure
- Each test project references its corresponding main project.
- All projects use xUnit as the test framework.
- Place new test files in the appropriate test project and namespace.

## Best Practices
- Mock external dependencies (repositories, services) using Moq.
- Use Arrange-Act-Assert pattern in all tests.
- Keep tests isolated and repeatable.
