# WindowFocusTerminal

## Overview

**WindowFocusTerminal** is a lightweight C# console application that brings **Windows Terminal** to the foreground when the **Shift** key is double-tapped within a short interval. This app utilizes global keyboard hooks to detect keypresses and focuses the terminal window automatically.

## Features

- Detects double-tap on the `Shift` key to trigger focus switch.
- Brings **Windows Terminal** to the foreground with retry mechanisms to improve reliability.
- Designed to run in the background.

## Requirements

- **.NET Framework** or **.NET Core** SDK to build and run.
- **Windows Terminal** installed.
- **Administrator privileges** for proper keyboard hook access and focus management.

## Installation

1. **Clone or download** the repository:
   ```bash
   git clone https://github.com/your-username/WindowFocusTerminal.git
	```
	
2. **Open** the project in Visual Studio or your preferred C# IDE.

3. **Build** the project in **Release** mode.

4. Optionally, **configure Task Scheduler** to run the application automatically on system startup for background use.

## Usage

1. **Run the executable**:
   - After building, navigate to the output directory (e.g., `bin\Release\netX.0`) and run the `.exe` file.

2. **Use**:
   - With the application running, double-tap the **Shift key** to bring **Windows Terminal** to the foreground.

3. **Set up Task Scheduler** (optional):
   - Open **Task Scheduler** and create a **new task**.
   - Under the **General** tab, select **Run with highest privileges**.
   - Set a **trigger** to start the application **at system startup**.
   - Under the **Actions** tab, set the path to your executable.

## Troubleshooting

- **Focus not working**:
  - Ensure the application has **administrator privileges**.
  - Make sure **Windows Terminal** is running when trying to focus.
- **Task Scheduler**:
  - Confirm that the task is configured to run with **highest privileges** for reliable operation.

## License

This project is licensed under the MIT License.
