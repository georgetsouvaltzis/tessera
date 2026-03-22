# M4 API Gate Evidence (Latest)

Date: 2026-03-22

## Commands + Results

### 1) Public API ergonomics/boundary/XML docs tests

Command:

```bash
dotnet test tests/TeaSharp.Tests --no-restore --nologo --filter "ApiErgonomics|PublicApiBoundary|PublicApiXmlDocs"
```

Result:

- Exit code: `0`
- `Passed!  - Failed: 0, Passed: 40, Skipped: 0, Total: 40`
- Duration: `114 ms` (`TeaSharp.Tests.dll`)

### 2) Full solution tests

Command:

```bash
dotnet test TeaSharp.slnx --no-restore --nologo --tl:off -v minimal
```

Result:

- Exit code: `0`
- `TeaSharp.Tests.dll`: `Passed 871 / 871` (Failed `0`, Skipped `0`, Duration `3 s`)
- `TeaSharp.IntegrationTests.dll`: `Passed 10 / 10` (Failed `0`, Skipped `0`, Duration `2 s`)

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
- Duration: `2.50 s`

### 4) Canonical examples smoke

Command:

```bash
scripts/smoke_examples_v1.sh 4
```

Result:

- Exit code: `0`
- `PASS HelloWorld startup alive >=4s`
- `PASS CounterForm startup alive >=4s`
- `PASS WorkspaceApp startup alive >=4s`
- `SUMMARY pass=3 fail=0`

## Gate Summary

- Overall M4 gate command set: **PASS**
- Public API ergonomics/boundary/XML docs checks: **PASS**
- Full solution tests: **PASS**
- Examples solution build: **PASS**
- Examples smoke run: **PASS**

## Warning Hotspots

- No warning hotspots were emitted by the above gate commands in this run.
