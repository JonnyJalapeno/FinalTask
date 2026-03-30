# Sauce Demo Test Automation

## Description

Automated test suite for [saucedemo.com](https://www.saucedemo.com) using Selenium WebDriver with NUnit.
The suite covers three use cases testing the login form, runs in parallel across Chrome and Edge,
and includes both data-driven NUnit tests and BDD scenarios with Reqnroll.

---

## Configuration

`testconfig.json` controls browsers, URL and timeout:
```json
{
  "BaseUrl": "https://www.saucedemo.com",
  "Browsers": [ "chrome", "edge" ],
  "BrowserOptions": [ "--start-maximized" ],
  "DefaultTimeoutSeconds": 10
}
```

Browsers - supports only chromium browsers: Chrome and Edge. 
BrowserOptions can be any combination CLI/flag supported by Selenium WebDriver, supports multiple flags.

---

## Test Cases

| UC | Description | Expected Result |
|----|-------------|-----------------|
| UC-1 | Enter username, enter password, clear all inputs, click login | Error message: "Username is required" |
| UC-2 | Enter username, enter password, clear password only, click login | Error message: "Password is required" |
| UC-3 | Enter valid username, enter valid password, click login | Redirected to inventory page with all elements visible |

### UC-3 verified elements

- Burger menu button
- "Swag Labs" label
- Shopping cart icon
- Sorting filter dropdown
- Inventory item list

---

## Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download) or higher
- Google Chrome installed
- Microsoft Edge installed

---

## Project Structure

```
FinalTask/
├── Business/                           # Page Object Model
│   ├── BasePage.cs                     # Base page for all other pages, has WebDriverWait, WaitForElement
│   ├── LoginPage.cs                    # Login page locators and interactions
│   └── InventoryPage.cs                # Inventory page locators and validation
│
├── Core/                               # WebDriver infrastructure
│   ├── Config/
│   │   └── TestConfig.cs               # Class that reads testconfig.json values and expose them for other classes
│   ├── Extensions/
│   │   └── WebElementExtensions.cs     # ClearElement helper for WebDriver elements for which simple .Clear() doesn't work
│   ├── ChromeDriverFactory.cs          # Creates ChromeDriver with options
│   ├── DriverFactory.cs                # Abstract factory — returns correct factory by the type of browser
│   ├── EdgeDriverFactory.cs            # Creates EdgeDriver with options
│   └── IAbstractDriverFactory.cs       # Factory interface
│
└── Tests/                              # Test layer
    ├── Features/
    │   ├── Login.Chrome.feature        # BDD scenarios for Chrome
    │   └── Login.Edge.feature          # BDD scenarios for Edge
    ├── Helpers/
    │   └── BrowserResolver.cs          # Helper for BDD tests, to check if feature browser tags match the browsers set in testconfig.json
    ├── Sources/
    │   ├── BrowserFixtureSource.cs     # Static helper for NUnit TestFixtureSource which needs reflection based access
    │   └── CredentialsSource.cs        # Static class with credentials for data driven approach
    ├── Steps/
    │   └── LoginSteps.cs               # Reqnroll step definitions
    ├─- AssemblyInfo.cs                 # Assembly directive to use parallelization per fixture
    ├── log4net.config                  # Log4Net configuration
    ├── LoginPageTests.cs               # Data-driven NUnit tests (UC-1, UC-2, UC-3)
    ├── reqnroll.json                   # Reqnroll configuration
    └── testconfig.json                 # Configuration for test run - starting url, browser types, browser option, timeout. 
    
    
```

---

## Running Tests

### All tests
```bash
dotnet test
```

### With detailed logging
```bash
dotnet test --logger "console;verbosity=detailed"
```

### NUnit tests only
```bash
dotnet test --filter "FullyQualifiedName~LoginPageTests"
```

### BDD Chrome only
```bash
dotnet test --filter "FullyQualifiedName~LoginChromeFeature"
```

### BDD Edge only
```bash
dotnet test --filter "FullyQualifiedName~LoginEdgeFeature"
```

---

## Design Patterns

### Singleton — `TestConfig.cs`
Ensures one TestConfig instance per runtime. Lazily allocated to `Instance` property using `LoadConfig` method with private constructor that the `[JsonConstructor]` uses.  
First use is slower due to JSON file read and deserialization, but subsequent accesses are fast because the instance is cached in memory.

### Concrete factory classes — `ChromeDriverFactory` / `EdgeDriverFactory`
Each factory encapsulates the creation of its specific driver. The caller never calls `new ChromeDriver()` or `new EdgeDriver()` directly.

### Abstract Factory — `DriverFactory`
Returns the correct concrete factory based on `browser` string. The consumer only depends on `IAbstractDriverFactory`.

### Page Object Model — `BasePage`, `LoginPage`, `InventoryPage`
All page interactions are encapsulated in page objects. Tests never call `FindElement` directly.
`BasePage` provides shared `WebDriverWait` and `WaitForElement` helper.

---

## Parallel Execution

NUnit runs Chrome and Edge fixtures simultaneously using:

```csharp
[TestFixtureSource(typeof(BrowserFixtureSource), nameof(BrowserFixtureSource.Browsers))]
[Parallelizable(ParallelScope.Fixtures)]
```

Reqnroll BDD tests, on the other hand run in parallel thanks to necessary assembly reference
`[assembly: Parallelizable(ParallelScope.Fixtures)]` and duplication of feature files with appropriate browser tags.


---

## Data-Driven Testing

NUnit tests use `[TestCaseSource]` to run the same test with multiple credential sets:

```csharp
private static readonly object[] ValidCredentials =
{
    new object[] { "standard_user", "secret_sauce" }
};

[Test, TestCaseSource(nameof(ValidCredentials))]
```

"BDD scenarios are written in feature files. `Scenario Outline` allows parameterized scenarios, 
with `Examples` tables supplying the input values."

---

## Logging

Log4Net is configured via `log4net.config` with two appenders:
- **ConsoleAppender** — outputs to console during test run
- **RollingFileAppender** — writes to `logs/test-run.log` with daily rotation

Log output format:
```
[INFO] 14:32:01 [Thread-1] LoginPageTests — Initializing Chrome driver
[INFO] 14:32:03 [Thread-1] LoginPageTests — Chrome driver initialized successfully
[INFO] 14:32:03 [Thread-1] LoginPageTests — Navigating to saucedemo.com on Chrome
[INFO] 14:32:04 [Thread-1] LoginPageTests — LoginPage initialized
[INFO] 14:32:04 [Thread-1] LoginPageTests — UC1 — LoginWithEmptyCredentials username: ddd, password: eee
[INFO] 14:32:05 [Thread-1] LoginPageTests — UC1 — Error message received: Epic sadface: Username is required
```

---

## BDD

Reqnroll scenarios in `Login.Chrome.feature` and `Login.Edge.feature` describe the three UCs
in plain English using Gherkin syntax. Step definitions in `LoginSteps.cs` map each Gherkin
line to the existing page object methods. Each feature file is tagged with `@Chrome` or `@Edge`
to control which browser is used.

```
gherkin

@chrome
Feature: Login Chrome

  Scenario Outline: UC1 Login with empty credentials
    Given I am on the login page
    When I enter "<username>" and "<password>"
    And I clear all credentials
    And I click login
    Then I should see error containing "Username is required"

    Examples:
      | username | password |
      | ddd      | eee      |
```

### Notes

One feature file per browser is required due to Reqnroll's parallel execution model 
— a single feature file cannot be run twice against different browsers natively[?]

---

## Test Count

| Suite | Chrome | Edge | Total |
|-------|--------|------|-------|
| NUnit | 3 | 3 | 6 |
| BDD (Reqnroll) | 3 | 3 | 6 |
| **Total** | **6** | **6** | **12** |

---

## Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| Selenium.WebDriver | 4.41.0 | Browser automation |
| NUnit | 4.3.2 | Test framework |
| NUnit3TestAdapter | 5.0.0 | Visual Studio Test Explorer integration |
| Microsoft.NET.Test.Sdk | 17.14.0 | Test runner |
| FluentAssertions | 8.9.0 | Readable assertions |
| log4net | 2.0.17 | Logging framework |
| Reqnroll | 3.3.3 | BDD framework (SpecFlow successor) |
| Reqnroll.NUnit | 3.3.3 | Reqnroll NUnit integration |
| Reqnroll.Tools.MsBuild.Generation | 3.3.3 | Feature file code generation |
| coverlet.collector | 6.0.4 | Code coverage |
| NUnit.Analyzers | 4.7.0 | NUnit code analysis |
