# Employee Data Importer CLI

This project is a C# command-line application designed to parse employee data from CSV files of varying structures, validate the data, and transform it into a common canonical format. The architecture emphasizes flexibility, maintainability, and adherence to modern .NET best practices, directly addressing the requirements of the Coding Case.

## Core Architectural Concepts

The solution is built around several key design patterns and principles to ensure it is robust and extensible.

### 1. Dynamic Parsing with the Strategy Pattern

The core requirement to "Dynamically Parse" based on a `ParserType` string is implemented using the **Strategy Pattern**.

-   **`IConvertingPipeline`:** This interface acts as the *Strategy*. It defines a common `ProcessRecords()` method that all specific parsers must implement.
-   **`TypeAConvertingPipeline` / `TypeBConvertingPipeline`:** These are the *Concrete Strategies*. Each class encapsulates the specific logic for reading, validating, and converting its designated CSV format.
-   **`ConvertingPipelineFactory`:** This factory selects and creates the appropriate pipeline strategy at runtime based on the `--parser` command-line argument. This decouples the main application logic from the specific parsing implementations.

### 2. Dependency Injection and .NET Generic Host

The application is bootstrapped using the `.NET Generic Host`, which provides a robust foundation for:
-   **Dependency Injection:** Services like `IFileSystem`, pipeline factories, and converters (`ITypeBRecordDtoConverter`) are registered and injected, promoting loose coupling and testability.
-   **Configuration:** Manages application settings.
-   **Logging:** Integrated structured logging with Serilog.

### 3. Type-Safe Ingestion and Decoupled Transformation

To ensure strong typing and separation of concerns, each pipeline follows a clear process:

1.  **Ingestion:** `CsvHelper` maps rows to a format-specific DTO (`TypeARecordDto`, `TypeBRecordDto`). This provides type-safety at the earliest stage.
2.  **Validation:** `FluentValidation` is used to apply a unique set of rules to each DTO. This addresses the "Handle Different Parser Types" and "Handle Data Variations" requirements by allowing complex, per-field validation (e.g., `TypeA`'s `FullName` vs. `TypeB`'s separate `Name`/`Surname`).
3.  **Transformation:** A dedicated converter (`TypeARecordDtoConverter`, `ITypeBRecordDtoConverter`) maps the validated, specific DTO to the `CommonModelDto`. This isolates mapping logic and allows for complex transformations, such as choosing a primary email or handling currency conversions (demonstrated via the `ICurrencyConverter` dependency).

### 4. Asynchronous Streaming and Robust Error Handling

The application is designed for efficiency and detailed reporting.

-   **`IAsyncEnumerable<RecordConversionResult>`:** The pipeline processes the file as a stream, yielding results one by one. This ensures low memory usage, allowing the application to handle very large files.
-   **`RecordConversionResult` Struct:** This result-monad struct cleanly encapsulates the outcome of each row's processing. It holds either the successful `CommonModelDto` or detailed error information (`ValidationErrors` or a `ProcessingError`), fulfilling the "Error Handling" requirement.
-   **Dual-File Output:** The `ResultsWriter` separates successes from failures, creating two files:
    -   `*.common.csv`: A clean CSV file containing only the successfully converted records.
    -   `*.errors.jsonl`: A JSON Lines file containing the original raw data and structured error details for every failed record, perfect for debugging.

## Project Structure

-   **/Common**: Contains the core abstractions (`IConvertingPipeline`), shared models (`CommonModelDto`), and factories that orchestrate the application.
-   **/TypeAPipeline**: Contains all logic specific to parsing "Type A" files: its DTO, validator, converter, and pipeline implementation.
-   **/TypeBPipeline**: Contains all logic specific to parsing "Type B" files.
-   **/Output**: Contains the logic for writing the processed results to the output files.
-   **/Sandbox**: Contains integration tests to verify the end-to-end functionality of each pipeline.

## How to Run

The application is a command-line tool. You can run it from the root of the `EmployeeImporter.Cli` project.

**Required Arguments:**
-   `-i`, `--input-file`: The full path to the source CSV file.
-   `-p`, `--parser`: The parser type to use (`TypeA` or `TypeB`).

**Example:**

```bash
dotnet run -- --input-file ./TypeAPipeline/sample.csv --parser TypeA
```

This will process `sample.csv` using the "Type A" logic and generate two new files in the same directory: `sample.common.csv` and `sample.errors.jsonl`.