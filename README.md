[![](https://img.shields.io/github/actions/workflow/status/soenneker/Soenneker.ZipCodes.Runners.GeoNames/build-and-test.yml?style=for-the-badge)](https://github.com/soenneker/Soenneker.ZipCodes.Runners.GeoNames/actions/workflows/build-and-test.yml)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/Soenneker.ZipCodes.Runners.GeoNames/daily-automatic-update.yml?style=for-the-badge&label=Daily%20Update)](https://github.com/soenneker/Soenneker.ZipCodes.Runners.GeoNames/actions/workflows/daily-automatic-update.yml)

# Soenneker.ZipCodes.Runners.GeoNames

Defines the file operations util contract.

> This is an automation runner, not a package intended for application consumption.

## What the runner does

- `IFileOperationsUtil.BuildZipCodeGeometryFile(zipFilePath, cancellationToken)` — Builds zip code geometry file.
- `Constants.DownloadUri` — The download uri.
- `Constants.SourceFileName` — The source file name.
- `Constants.FileName` — The file name.
- `Constants.Library` — The library.

## What you get

- `IFileOperationsUtil` — Defines the file operations util contract.
- `Constants` — Represents the constants.
- `ConsoleHostedService` — Represents the console hosted service.

## API at a glance

| API | What it does | Result / important behavior |
| --- | --- | --- |
| `ConsoleHostedService.StartAsync(cancellationToken)` | Starts the Console Hosted Service and begins its background work. | A task that completes after the Console Hosted Service has started. |
| `ConsoleHostedService.StopAsync(cancellationToken)` | Stops the Console Hosted Service and waits for its background work to finish. | A task that completes after the Console Hosted Service has stopped. |

## Practical notes

- Cancellation stops pending work; it does not undo work that has already completed.
