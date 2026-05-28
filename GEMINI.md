# Workflow Guidelines

- **Working Directory**: Always ensure the current working directory is the solution root (containing `global.json` and the `.slnx` solution file) before executing `dotnet` commands. This is critical for correct SDK version resolution and dependency management.

### Collecting Code Coverage

To run tests and collect code coverage:

```bash
dotnet test --coverage --coverage-output-format cobertura
```

To generate an HTML coverage report using ReportGenerator:

```bash
dotnet reportgenerator "-reports:**/*.cobertura.xml" "-targetdir:coveragereport" -reporttypes:Html
```
The report will be generated in the `coveragereport` directory.
