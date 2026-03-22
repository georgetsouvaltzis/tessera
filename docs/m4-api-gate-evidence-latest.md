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
- `Passed!  - Failed: 0, Passed: 42, Skipped: 0, Total: 42`
- Duration: `277 ms` (`TeaSharp.Tests.dll`)

### 2) Full solution tests

Command:

```bash
dotnet test TeaSharp.slnx --no-restore --nologo --tl:off -v minimal
```

Result:

- Exit code: `0`
- `TeaSharp.Tests.dll`: `Passed 873 / 873` (Failed `0`, Skipped `0`, Duration `2 s`)
- `TeaSharp.IntegrationTests.dll`: `Passed 10 / 10` (Failed `0`, Skipped `0`, Duration `2 s`)

### 3) Examples build

Command:

```bash
dotnet build TeaSharp.Examples.slnx --no-restore --nologo -v minimal
```

Result:

- Exit code: `1`
- Build status: `Build FAILED`
- Error snippet:
  - `NETSDK1004`: missing assets file for `examples/ControlPlaneOpsDashboard/ControlPlaneOpsDashboard.csproj` under `--no-restore`
  - `CS0246`: `ScreenContext` and `Screen` unresolved in `ControlPlaneOpsDashboard.Analytics.cs`

### 4) Canonical examples targeted builds

Command:

```bash
dotnet build examples/HelloWorld/HelloWorld.csproj --no-restore --nologo -v minimal
dotnet build examples/CounterForm/CounterForm.csproj --no-restore --nologo -v minimal
dotnet build examples/WorkspaceApp/WorkspaceApp.csproj --no-restore --nologo -v minimal
```

Result:

- Exit code: `0` for all three commands
- Build status: `Build succeeded.` for all three canonical onboarding examples
- Warnings: `0`
- Errors: `0`

### 5) Canonical examples smoke

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

- Overall M4 gate command set: **PASS for API/test gates; examples-solution no-restore build currently blocked by in-flight example project**
- Public API ergonomics/boundary/XML docs checks: **PASS**
- Full solution tests: **PASS**
- Examples solution build (`TeaSharp.Examples.slnx --no-restore`): **BLOCKED** (local workspace contains incomplete `ControlPlaneOpsDashboard`)
- Canonical onboarding examples targeted builds: **PASS**
- Examples smoke run: **PASS**

## Warning Hotspots

- `NETSDK1004` (`project.assets.json` missing for `examples/ControlPlaneOpsDashboard` under `--no-restore`)
- `CS0246` unresolved types in `examples/ControlPlaneOpsDashboard/ControlPlaneOpsDashboard.Analytics.cs`
