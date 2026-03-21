# M4 API Gate Evidence (Latest)

Date: 2026-03-21

## Commands + Results

### 1) Public API ergonomics/boundary/XML docs tests

Command:

```bash
dotnet test tests/TeaSharp.Tests --no-restore --nologo --filter "ApiErgonomics|PublicApiBoundary|PublicApiXmlDocs"
```

Result:

- Exit code: `0`
- `Passed!  - Failed: 0, Passed: 33, Skipped: 0, Total: 33`
- Duration: `133 ms` (`TeaSharp.Tests.dll`)

### 2) Full solution tests

Command:

```bash
dotnet test TeaSharp.slnx --no-restore --nologo -v minimal
```

Result:

- Exit code: `0`
- `TeaSharp.Tests.dll`: `Passed 785 / 785` (Failed `0`, Skipped `0`, Duration `2 s`)
- `TeaSharp.IntegrationTests.dll`: `Passed 10 / 10` (Failed `0`, Skipped `0`, Duration `1 s`)

### 3) Examples build

Command:

```bash
dotnet build TeaSharp.Examples.slnx --no-restore --nologo -v minimal
```

Result:

- Exit code: `0`
- Build status: `Build succeeded.`
- Warnings: `0`
- Errors: `0`
- Duration: `1.13 s`

## Gate Summary

- Overall M4 gate command set: **PASS**
- Public API ergonomics/boundary/XML docs checks: **PASS**
- Full solution tests: **PASS**
- Examples solution build: **PASS**

## Warning Hotspots

- No warning hotspots were emitted by the above gate commands in this run.
