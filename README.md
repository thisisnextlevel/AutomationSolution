# AutomationSolution

A proof-of-concept framework for automating tasks on a Windows machine. Tasks are defined as small classes implementing `IAutomationTask` and executed through a simple workflow engine.

## Features

- `AutomationContext` for sharing data between tasks
- Task graph execution with dependency and priority support
- Built-in tasks:
  - `SendChatGPTMessageTask` – posts a message to ChatGPT using the CDP engine
  - `WriteFileTask` – writes text to a file
  - `RunProcessTask` – runs a shell command
  - `SleepTask` – pauses for a configurable time
- Plugin loader for discovering additional tasks or engines from a `plugins` directory
- Workflow definitions can be specified in `workflow.json`

## Usage

1. Create a `workflow.json` describing the steps to run (see the provided example).
2. Build and run the `Automation.Runner` project with `dotnet run`.
3. Inspect the console output and generated files to verify behaviour.

### Configuring Chrome

The CDP engine uses Google Chrome for browser automation. It attempts to locate
the browser automatically on Windows, macOS, and Linux. You can
override this by setting the `CHROME_EXECUTABLE` environment variable to the
full path of your Chrome installation, e.g.

```powershell
$env:CHROME_EXECUTABLE="C:\Program Files\Google\Chrome\Application\chrome.exe"
```

If the variable is not set and Chrome cannot be found, PuppeteerSharp will
download its own copy under the application's output folder. The lookup is
handled by the `ChromeFinder` helper used by the CDP engine.

> **Note:** the repository does not include the .NET SDK. Install the SDK to build or run the project locally.
