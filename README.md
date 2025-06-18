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
  - `DownloadFileTask` – downloads a file from a URL
  - `SemanticKernelTask` – executes a prompt using Microsoft Semantic Kernel
- Plugin loader for discovering additional tasks or engines from a `plugins` directory
- Workflow definitions can be specified in `workflow.json`

## Usage

1. Create a `workflow.json` describing the steps to run (see the provided example).
2. Build and run the `Automation.Runner` project with `dotnet run`.
3. Inspect the console output and generated files to verify behaviour.

### Configuring Chrome

The CDP engine uses Google Chrome for browser automation. It attempts to locate
the browser automatically on Windows, macOS, and Linux and will fall back to
Microsoft Edge if Chrome is not found. You can override the discovery logic by
setting one of the following environment variables to the full path of your
browser executable:

- `CHROME_EXECUTABLE`
- `CHROME_PATH`
- `CHROME_BIN`
- `CHROME`
- `GOOGLE_CHROME_SHIM`
- `EDGE_EXECUTABLE`

For example, you can specify the executable path explicitly:

```powershell
$env:CHROME_EXECUTABLE="C:\Program Files\Google\Chrome\Application\chrome.exe"
```

If none of these variables are set and no installed browser can be located, the
engine automatically downloads a compatible copy using PuppeteerSharp. The
browser is stored under the application's output folder. The lookup is handled
by the `ChromeFinder` helper used by the CDP engine.

You can also configure the Chrome user profile directory and debug port using
`CHROME_PROFILE` and `CHROME_DEBUG_PORT` respectively.
If the variable is not set and Chrome cannot be found, PuppeteerSharp will fall
back to downloading its own copy under the application's output folder.

> **Note:** the repository does not include the .NET SDK. Install the SDK to build or run the project locally.

### Running on Linux or macOS

The core libraries and the CDP automation engine target `net8.0` and work on any platform supported by .NET. A minimal cross-platform runner is provided in the **ConsoleRunner** project:

```bash
dotnet run --project ConsoleRunner
```

This runner omits Windows specific features such as UI automation but allows executing workflows that rely on the CDP engine and general tasks like `RunProcessTask` or `WriteFileTask`.
