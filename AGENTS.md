# Repository Guidelines

## Project Structure & Module Organization

SR-ATS is a Godot 4.6 .NET project. Core metadata lives in `project.godot`, with the main scene in `main.tscn`. Runtime scripts are C# files. Shared scripts are in `scripts/`; ATS-specific modules are under `ATS-P/`, `ATS-Ps/`, and `ATS-S/` with their own scenes, scripts, and assets. UI/config scenes are under `config/`. Global media and localization files are in `assets/`, `fonts/`, `screenshot/`, `news/`, and `translation.csv`. The Windows installer project is under `SR-ATS-Setup/`.

## Build, Test, and Development Commands

- `godot --editor project.godot`: open the project in the Godot editor.
- `godot --path .`: run the default scene locally.
- `dotnet build SR-ATS.sln`: compile the Godot C# assembly and catch C# errors.
- `dotnet build SR-ATS-Setup/SR-ATS-Setup.sln`: build the installer solution when Visual Studio setup project tooling is available.

Use Godot 4.6.x with .NET support. The desktop target is `net9.0`, with `vJoyInterface.dll` and `vJoyInterfaceWrap.dll` referenced from the repository root.

## Coding Style & Naming Conventions

Use C# for runtime scripts. C# scripts use four-space indentation, braces on the declaration line, PascalCase for classes and signal names, and camelCase for private fields unless a file has a local pattern. Keep scene paths stable and update `.tscn` references through the editor when possible.

Retain the Apache license header used in existing scripts for new source files.

## Testing Guidelines

There is no automated test suite yet. Verify changes with `dotnet build SR-ATS.sln` and `godot --path .`. For gameplay or UI changes, manually exercise the affected ATS module scene and capture screenshots when visual behavior changes. If adding tests later, place them in `tests/` and document the runner here.

All remaining legacy runtime scripts were migrated to C# .NET. If adding new script logic, prefer C# and avoid introducing alternate script languages unless there is a documented compatibility blocker.

## Commit & Pull Request Guidelines

Recent history uses short, imperative commit messages such as `fix typo`, `finish ATS-P`, and `remove unused file, add windows installer project`. Keep commits focused and describe the changed subsystem when useful.

Pull requests should include a brief summary, affected modules or scenes, manual test steps, and screenshots for UI or indicator changes. Link related issues or milestones, and call out changes to bundled DLLs, installer files, or Godot import metadata.

## Agent-Specific Instructions

Avoid broad rewrites of generated Godot `.import` files or scene UIDs unless required by an asset or scene change. Do not remove bundled DLLs without confirming replacement behavior for vJoy integration.
