<!-- Use this file to provide workspace-specific custom instructions to Copilot. For more details, visit https://code.visualstudio.com/docs/copilot/copilot-customization#_use-a-githubcopilotinstructionsmd-file -->

This workspace contains test projects for each main layer of the LifeOrganizer solution:
- LifeOrganizer.Business.Tests: Unit tests for business/services (uses Moq for mocking)
- LifeOrganizer.Api.Tests: Integration tests for controllers (uses Moq for mocking)
- LifeOrganizer.Data.Tests: Unit tests for data/repositories (uses Moq for mocking)

All test projects use xUnit as the test framework. Ensure tests are isolated, mock dependencies where appropriate, and follow best practices for .NET testing.
