# BookGenerator modernization proposal

Date: 26 September 2026  
Reviewed revision: `267c020`  
Status: proposed architecture and implementation plan

## 1. Executive recommendation

Modernize this application as a small modular monolith:

- Target .NET 10 across backend, hosts, tools, and tests; align EF Core and Microsoft framework dependencies.
- Keep React and Vite; upgrade their toolchain and remove the obsolete ASP.NET SPA development integration.
- Retain and upgrade the official `OpenAI` NuGet package. Use `Microsoft.Extensions.AI.IChatClient` and the maintained `Microsoft.Extensions.AI.OpenAI` adapter to isolate provider-specific code.
- Replace MediatR with explicitly injected application services and explicit validation.
- Remove Quartz, the outbox, background workers, and generation event dispatch. Invoke generation directly and await it within the HTTP request.
- Read and write the database directly through EF Core's `BookDbContext`. Remove generic repositories, `IDbContext`, and `IUnitOfWork` wrappers.
- Keep SQLite for local development. Gate continued production SQLite use on the actual Azure storage filesystem and deployment topology.

This document proposes changes; it does not deploy them or modify application behavior.

## 2. Findings from the repository

Paths below are relative to the repository root.

| Area | Evidence | Architectural consequence |
| --- | --- | --- |
| Runtime | All 10 `.csproj` files target `net8.0`; CI uses .NET 8 and Node 22. No `global.json` or local EF tool manifest. | Developer, build, and production runtimes are not pinned together. |
| Mixed dependencies | EF Core/SQLite/Tools 6.0.15; SPA services 7.0.5; hosting abstractions 8.x; QuestPDF 2023.12.1. | Changing only the target framework is insufficient. |
| Frontend | React 18.2, Vite 4.2, TypeScript 4.9, Router 5.3, Tailwind 3.3; Jest/Babel/ts-jest configuration. | Several breaking upgrades require code/configuration changes. These are manifest ranges, not a claim about every lockfile resolution. |
| LLM | `Infrastructure/Books/ChatCompletionService.cs` consumes `OpenAI.Chat.ChatClient`; infrastructure registers it and hardcodes `gpt-4o-mini`. | Provider, model, and options are coupled to infrastructure registration. Singleton `LastErrorMessage` can mix errors across concurrent calls. |
| Generation | `BookController.Create` saves through MediatR, then directly publishes `BookCreationStartedDomainEvent`; its handler starts `new Task(async () => ...)`. | The asynchronous delegate is effectively `async void`; completion and failures are not tracked by the host. Replace the detached task with a directly awaited generation call. |
| Outbox | `Book.Create` also raises an event; `ConvertDomainEventsToOutboxMessagesInterceptor` stores it. Quartz registration in `WebApi/Program.cs` is commented out. | The Quartz processor is inactive. Delete it and the unused outbox mechanism without adding a scheduler or worker. |
| Outbox failure handling | `ProcessOutboxMessagesJob.cs` sets `ProcessedOnUtc` even after publication throws. | Its existing failure semantics do not provide reliable delivery. Remove it rather than build new behavior around it. |
| Persistence | Generic repository → `IDbContext` → EF context, plus three repositories and `IUnitOfWork`; SQLite migration already exists. `Application.csproj` retains `Microsoft.Data.SqlClient`. | Remove redundant wrapping and unused SQL Server coupling; this is already a SQLite project. |
| Validation | `ValidationPipelineBehaviour` calls `next()` without returning when no validators exist, then calls it again. | Query handlers without validators can execute twice. Explicit validation removes this defect along with the pipeline. |
| Local fake | `BookCreaterInMemory.CreateAsync` ignores the supplied ID and creates another book, then starts another untracked task. | The originally returned book can remain pending. Replace this with a fake LLM in the real generation workflow. |
| Local configuration | `Model=Test` still registers a real `ChatClient`; startup validation always requires an API key and organization. | Documented offline development is not reliably credential-free. Organization is required but unused by the current provider registration. |
| Hosting | API and SPA are separate App Services. `ClientSpa` hosts the deployed React app; the Razor `Client` is still built/published but is not the frontend deployment artifact. | Retire the Razor client after preserving any unique behavior/tests. Remove duplicate warm-up clients. |
| API contracts | POST body is a JSON string; success is HTTP 200 with PascalCase `BookId`; enum statuses are strings; frontend uses `download2`. | Preserve these contracts during infrastructure upgrades; coordinate intentional API changes separately. |

Primary code anchors: `src/BookGenerator.WebApi/Program.cs`, `src/BookGenerator.WebApi/Controllers/BookController.cs`, `src/BookGenerator.Application/Books/Events/BookCreationStartedDomainEventHandler.cs`, `src/BookGenerator.Infrastructure/DependencyInjection.cs`, `src/BookGenerator.Persistence/DependencyInjection.cs`, and `.github/workflows/publish.yml`.

### Review limitations

Source, configuration, migrations, tests, package manifests, and CI were inspected. Local tool discovery found macOS ARM64, .NET SDK 10.0.302 with runtime 10.0.10, Node 24.20.0, and npm 11.11.0. No .NET 8 runtime is installed on this machine. A newer SDK alone does not supply the runtime required to execute the current .NET 8 applications/tests.

No restore, build, test suite, database migration, paid model request, or Azure configuration change was performed. Azure control-plane settings, logs, service-plan limits, and database contents were not inspected. Compatibility recommendations below require the acceptance checks in section 9.

## 3. Supported version baseline

Use current stable releases, with LTS runtimes where available. The following is a dated baseline, checked against publisher documentation on 26 September 2026. Pin exact resolved versions in the implementation PR and recheck servicing updates before deployment.

| Component | Proposed target | Migration notes/source |
| --- | --- | --- |
| .NET SDK / runtime | SDK **10.0.401**, `net10.0`, runtime **10.0.12**, C# 14 | Published .NET 10 baseline; install ARM64 on Apple Silicon. .NET 11 is still a prerelease on the download page. [Downloads](https://dotnet.microsoft.com/en-us/download/dotnet/10.0), [support policy](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core). |
| EF Core | **10.0.12** for EF, SQLite provider, Design, and `dotnet-ef` | Align versions and examine breaking changes across EF 7–10. [SQLite package](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Sqlite), [EF 10 changes](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-10.0/breaking-changes). |
| LLM integration | Official `OpenAI` SDK plus stable **10.x** `Microsoft.Extensions.AI.Abstractions` and `Microsoft.Extensions.AI.OpenAI`; `Microsoft.Extensions.AI` as needed | Resolve and pin compatible versions together. The checked adapter is 10.10.1 and requires OpenAI 2.14.0 or later. [Adapter dependencies](https://www.nuget.org/packages/Microsoft.Extensions.AI.OpenAI), [core package](https://www.nuget.org/packages/Microsoft.Extensions.AI). |
| Node | **24 LTS**, latest serviced patch; observed upstream **24.21.0** | Use LTS for reproducible frontend builds; pin npm as well. [Node releases](https://github.com/nodejs/node/releases). |
| React / React DOM | **19.3**, same patch version | Upgrade types together; check effects, test utilities, and Strict Mode behavior. [Release](https://react.dev/blog/2026/09/09/react-19-3). |
| Vite | **8.x**, compatible stable React plugin | Migrate through the intervening major-version guides. [Vite 8](https://vite.dev/blog/announcing-vite8). |
| TypeScript | **7.x** | Resolve 5.x/6.x configuration deprecations before moving to 7; check editor and ESLint parser compatibility. [Release](https://devblogs.microsoft.com/typescript/announcing-typescript-7-0/). |
| React Router | **8.x**; current docs list **8.4.0** | Replace v5 `Switch`, `component`, and `useHistory`; use declarative routing, without adding a server rendering framework. [Changelog](https://reactrouter.com/changelog). |
| Tailwind | **4.x**, compatible Vite integration | Migrate CSS/configuration deliberately and compare screenshots. Confirm supported browser versions before adopting v4. [Upgrade guide](https://tailwindcss.com/docs/upgrade-guide). |
| Frontend tests | **Vitest 5.x** + compatible Testing Library/jsdom | Consolidate around Vite; remove Jest/Babel/ts-jest dependencies used only for testing. [Guide](https://vitest.dev/guide/). |
| PDF | QuestPDF **2026.9.1** baseline | Upgrade API/native dependencies together and verify rendered output on macOS and Linux. [Package](https://www.nuget.org/packages/QuestPDF). |
| Remaining libraries | Current stable FluentValidation, test SDK/MSTest, coverlet, ESLint and typescript-eslint releases with compatible peer ranges | Record exact versions after restore/build verification. Prefer upgrading the existing .NET test framework over rewriting tests. |

Add `global.json`, `Directory.Build.props`, `Directory.Packages.props`, a local `dotnet-ef` manifest, and a Node version file. Centralize nullable/implicit-using settings and package versions. Commit NuGet lockfiles and use locked restore in CI; retain `package-lock.json` and use `npm ci`. Do not use floating `latest` installations in the pipeline.

Remove `Microsoft.Data.SqlClient` and the Visual Studio-only EF Tools workflow. Use `Microsoft.EntityFrameworkCore.Design` with private assets and the local CLI tool. Replace Swashbuckle 6 with .NET 10's first-party OpenAPI generation, or upgrade Swashbuckle if its UI is retained; preserve a usable development API explorer. Keep production exception responses sanitized.

## 4. Target architecture and MediatR removal

Use direct service calls and direct EF Core access:

```text
React SPA
   │ HTTP: create / status / download
   ▼
WebApi controllers ──► BookApplicationService ──► BookDbContext ──► SQLite
                                │
                                │ awaited generation
                                ▼
                       BookGenerationService ──► BookDbContext
                                │
                                ▼
                           IChatClient
                                │
                 Microsoft.Extensions.AI.OpenAI
                                │
                       official OpenAI SDK
                                │
                            OpenAI API
```

Proposed project boundaries:

- **Domain:** `Book`, `Chapter`, progress/state rules, and plain value/result types. No MediatR, EF, or provider SDK dependencies.
- **Persistence:** `BookDbContext`, entity mapping, and migrations. Reference Domain and EF Core; remove its Application reference after deleting the persistence interfaces.
- **Application:** application/generation services, validators, public contracts, and PDF conversion ports. Reference Persistence for direct `BookDbContext` access and `Microsoft.Extensions.AI.Abstractions` for model access.
- **Infrastructure:** official SDK registration/adapter and PDF conversion. Remove scheduler, event-processing, and outbox code.
- **WebApi:** HTTP mapping, dependency composition, configuration, and health checks.
- **React client:** keep its current directory during upgrades. Keep `PdfPreviewer` as an optional development tool.

The project dependency direction becomes `Application → Persistence → Domain`; Persistence must no longer reference Application. This deliberately accepts EF Core coupling in application services to reduce indirection. Infrastructure implements application ports and WebApi composes the services.

Inject application services directly into controllers. Replace `sender.Send(...)` with explicit calls such as `CreateAsync`, `GetStatusAsync`, and `GetBookAsync`. Keep small request records if useful, but remove their MediatR interfaces.

The create service must call `IValidator<CreateBookRequest>.ValidateAsync` exactly once before persistence and map failures to the existing validation response. Keep cancellation tokens and explicit error/result mapping. Remove `ValidationPipelineBehaviour`, `ICommand*`, `IQuery*`, `ISender`/`IPublisher` injection, registration scans, and both MediatR package references.

Remove `BookCreationStartedDomainEvent`, its handler, `IDomainEvent`, and `AggregateRoot` event buffering when no consumers remain; `Book` can derive from `Entity`. The create service calls and awaits generation explicitly.

## 5. LLM access with the official OpenAI SDK

Retain the official `OpenAI` NuGet package and upgrade it together with `Microsoft.Extensions.AI.OpenAI`. Register the SDK's `OpenAI.Chat.ChatClient` in Infrastructure and adapt it using `AsIChatClient()`. Application services consume `Microsoft.Extensions.AI.IChatClient`. [Official SDK documentation](https://developers.openai.com/api/docs/libraries), [Microsoft adapter](https://www.nuget.org/packages/Microsoft.Extensions.AI.OpenAI).

Replace `IChatCompletionService` and its custom wrapper with the standard abstraction. The maintained adapter handles the SDK-to-abstraction mapping; the application owns book prompts, generation sequencing, output validation, and persistence. [IChatClient guidance](https://learn.microsoft.com/en-us/dotnet/ai/ichatclient).

Implementation requirements:

1. Configure `Llm:Provider`, `Endpoint`, `Model`, `ApiKey`, timeout, and output limits. Keep the existing model as the migration baseline until account availability and output quality are tested.
2. Keep SDK-specific construction and options in Infrastructure. Consume messages, output text, finish reason, model ID, and usage through Microsoft AI types in application code.
3. Remove shared mutable `LastErrorMessage`. Use per-call exceptions/results and propagate cancellation distinctly from a provider failure. Persist safe user-facing failure messages when possible.
4. Set one bounded retry policy, accounting for SDK retries so policies do not multiply attempts. Respect provider retry guidance, avoid retrying authentication/validation failures, and keep all attempts inside the HTTP request's total generation budget. A timed-out paid request may already have run remotely.
5. Log correlation IDs, duration, model, and token counts without API keys or full manuscript/prompt text.
6. Validate a bounded, ordered outline and reject empty/truncated chapters. Prefer a structured outline where the configured model supports it; otherwise parse and validate text explicitly.

Register a deterministic fake `IChatClient` for `Llm:Provider=Fake`, then run the same directly awaited generation service and EF persistence code as production. Skip real SDK construction and secret validation in fake mode. Remove `BookCreaterInMemory`; refactor `BookCreaterChatGpt` into `BookGenerationService`.

Acceptance: the official SDK is retained, provider-specific types stay in Infrastructure, and real/fake clients use the same application workflow.

## 6. Direct persistence and removal of background processing

### Read and write through EF Core

Remove `GenericRepository`, the three repository interfaces/implementations, `IDbContext`, and `IUnitOfWork`. Inject the scoped `BookDbContext` into application services and use EF queries plus `SaveChangesAsync` directly. Add no replacement repository/store abstraction.

Keep `Books`, `Chapters`, and `BookProgresses` initially to minimize schema changes. Progress describes the book's stored state; it does not schedule work. Retain existing numeric enum values (`None=0`, `Pending=1`, `Completed=2`, `Failed=3`).

- Create book/progress rows in one `SaveChangesAsync` operation.
- Save each generated chapter and its progress update together. Add a unique `(BookId, Order)` constraint after checking existing data for duplicates.
- Set `Completed` only after all expected chapters have been saved.
- Use projections or `AsNoTracking` for read-only status/download queries. Include chapters in explicit order when generating a PDF.
- Pass cancellation tokens through EF operations. Keep database transactions short and outside model calls. Do not access a shared `DbContext` concurrently.
- Return `409 Conflict` for download attempts before completion and update the UI accordingly. Keep PDF generation on download initially.

Keep the duplicated progress title during the first migration; removing it can be a later schema cleanup.

### Remove Quartz and workers without replacement

Delete:

- `ProcessOutboxMessagesJob.cs`, commented scheduler registration, and the `Quartz` / `Quartz.Extensions.Hosting` packages.
- `ConvertDomainEventsToOutboxMessagesInterceptor`, `OutboxMessage`, and outbox context mappings/registration.
- Generation event publication and its detached-task handler.
- Detached tasks in the fake generator, replacing that generator with the fake `IChatClient` described above.
- The clients' `StartupWorker` implementations and hosted-service registrations, along with tests that cover only those removed warm-up workers.

No scheduler, hosted worker, timer, channel, job table, polling processor, or lease mechanism is introduced.

### Directly awaited generation

The inactive Quartz job is separate from the generation task currently started by the controller's event handler. Remove both asynchronous dispatch paths while preserving book generation through an explicit awaited service call:

1. The create request validates the title and saves the book/progress rows directly.
2. The application service awaits `BookGenerationService`, which calls `IChatClient` and writes chapters/progress directly through EF.
3. On success, return the existing HTTP 200 / PascalCase `BookId` response after generation completes.
4. On failure or cancellation, stop generation and record `Failed` when possible. A cancellation cleanup write may use a separate, bounded token; it must still be awaited within the request's lifetime. Return a documented problem response with the created book ID when available.

**Observable behavior changes:** POST stays open until generation finishes. The frontend shows a submitting/generating state during that request, then navigates to the completed book. Existing status/download endpoints remain useful for books whose IDs are already known. The current create-then-poll interaction must change with this backend cutover.

**Execution limits:** apply a total generation deadline and bound chapter count/output size against measured browser, server, and Azure gateway timeouts. Increasing the SDK timeout cannot extend an upstream gateway limit. Verify realistic book sizes in the deployed topology before rollout; if they exceed that budget, reduce the supported generation size for this request-based design. Do not silently fall back to detached tasks.

A process termination can leave partially written books marked `Pending`. There is no automatic recovery or scheduled retry. Interrupted records require an explicit maintenance action; do not treat them as queued work or report that they will resume. Do not automatically retry an entire create POST after an ambiguous network timeout, because it may create another book and repeat paid calls.

### Remove outbox data safely

Preserve EF migration history. Add a new migration to drop `OutboxMessages` and update the current model snapshot; historical migrations may still describe the old table.

Before cutover:

1. Pause new generation requests, allow active requests/tasks to finish where practical, and take a consistent database backup with a verified restore procedure.
2. Inspect stored progress and chapters. Keep completed books intact; identify incomplete records for explicit cleanup. Do not replay unprocessed outbox rows, since direct event publication may already have generated those books.
3. Deploy the direct-call implementation and apply the reviewed schema migration during the controlled cutover. Verify existing book reads/downloads and a new fake generation flow.
4. Keep the matching old artifact and database backup for rollback. After dropping the table, reverting only the application binary is insufficient; restore a compatible schema/data snapshot or apply a forward repair.

### SQLite on macOS and Azure

SQLite removes the need for Windows LocalDB or a local database server. Anchor relative database paths to an explicit content/data root instead of the shell's working directory. Remove the current heuristic that silently converts SQL Server-like strings to SQLite defaults; fail with a clear configuration error.

The configured production path is `/home/data/bookgenerator/bookgenerator.db`. Inspect its actual App Service mount and locking behavior. SQLite cautions about network filesystems; WAL requires participating processes to be on the same host. A single App Service instance alone does not establish filesystem safety. [Network filesystem caveats](https://www.sqlite.org/useovernet.html), [WAL constraints](https://www.sqlite.org/wal.html).

Production decision gate:

- Retain SQLite only on verified suitable persistent storage with one active app instance, bounded writes, backup/restore tests, and restart tests. Do not blindly enable WAL on a network share.
- If the current Azure filesystem cannot satisfy those conditions, use a managed relational database (for example PostgreSQL) in production. Continue direct EF Core access with provider-specific migrations and CI coverage. Treat that move as a separate operational workstream.
- Keep the database outside deployment output and ephemeral storage.

## 7. macOS development and frontend modernization

### macOS baseline

Support CLI development on Apple Silicon and Intel Macs, plus Linux CI. No IIS Express, Visual Studio, SQL Server LocalDB, or Windows-only shell commands should be required.

- Install the pinned .NET 10 SDK for the machine architecture and Node 24 LTS. Use `dotnet dev-certs https --trust` when running the HTTPS profile.
- Add an explicit local HTTP profile at `http://localhost:5080` for the simplest Vite proxy setup; restrict it to local development. Keep the existing HTTPS API port 7445 as an alternative.
- Make `Fake` provider mode work without any API key or organization. Put real-provider secrets in .NET user-secrets or environment configuration. Never expose them through `VITE_*` variables.
- Replace the `ClientSpa.csproj` shell expression `echo ... & npm ...`: `&` has different semantics under Unix shells. Run npm explicitly in CI or use separate portable MSBuild `Exec` tasks with reliable exit-code propagation.
- Remove manual TypeScript compile item lists and `UseReactDevelopmentServer`; run Vite directly and use ordinary static-file hosting for its build output.
- Upgrade QuestPDF and remove manually pinned old Linux Skia/HarfBuzz assets only after resolving the new package's native dependency graph. Verify both `osx-arm64` and `osx-x64` execution, and Linux publishing.
- Keep font loading based on `AppContext.BaseDirectory`; ensure fonts are copied into publish output. Compare Latin, Cyrillic, and CJK output, line breaks, pagination, and missing glyphs. Replace the old previewer API with the supported optional Companion workflow, or simply generate and open a PDF. [Companion downloads](https://www.questpdf.com/companion/download.html).

Proposed onboarding commands **after these changes are implemented**:

```bash
# From repository root, after installing the pinned SDK/Node versions:
dotnet tool restore
dotnet restore BookGenerator.sln --locked-mode
dotnet build BookGenerator.sln --no-restore
dotnet test BookGenerator.sln --no-build

# Terminal 1: proposed local profile and configuration keys
Llm__Provider=Fake dotnet run \
  --project src/BookGenerator.WebApi/BookGenerator.WebApi.csproj \
  --launch-profile http

# Terminal 2: retain this frontend directory during migration
cd src/BookGenerator.ClientSpa/ClientApp
npm ci
npm run dev
```

Startup migration may remain enabled for local development. Production schema changes should be a controlled deployment step with a backup and a single migration owner.

### Frontend changes

1. Upgrade Node, React/types, Vite/plugin, TypeScript, Router, and test tooling in small verified steps. Replace `Switch` with `Routes`, `component` props with elements, and `useHistory` with `useNavigate`; validate route parameters before API calls.
2. Replace Axios with a small typed `fetch` wrapper if no Axios-specific feature is needed. For the existing contract, explicitly send `JSON.stringify(title)` with POST `Content-Type: application/json`. Do not add that content type to GET requests unnecessarily.
3. Change `BookForm.onGenerate` to return `Promise<void>`; await it, restore form controls on failure, and display the error. Correct its label/input ID mismatch.
4. Keep the create request awaited and display a generating state until it finishes. Navigate to download on success; display failures and distinguish an ambiguous timeout from a confirmed failure. Remove the automatic post-create polling loop. Retain status views for known book IDs and describe incomplete records without implying that background work is running.
5. Preserve PascalCase responses initially. Correct `ErrorMessage` to `string | null` in TypeScript and test all enum states. Preserve `download2` until consumers move to a coordinated replacement.
6. Support `filename*` as well as a safe fallback filename when parsing download metadata. Revoke created object URLs after use.
7. Move tests to Vitest/Testing Library, use an ESLint flat configuration, and add explicit `typecheck`, `lint`, `test:ci`, and `build` scripts. Keep browser regression checks for routing and Tailwind migration.
8. Build frontend assets once with `npm ci` and `npm run build`; copy the resulting `dist` into the chosen host's publish output. Add a history fallback for SPA routes, with `/api/*` excluded so API errors never become `index.html` responses.

For local development, use a Vite `/api` proxy to `http://localhost:5080` and a relative API base URL. Production `.env` currently bakes the Azure API URL into the bundle; changing ASP.NET appsettings cannot change that compiled value.

## 8. Delivery sequence and rollback

| Stage | Work | Exit condition |
| --- | --- | --- |
| 1. Reproducible baseline | Pin SDK/Node; upgrade all retained .NET projects to 10 and EF to 10; align packages; fix portable publishing and fake-mode startup. | Build/test/publish on macOS and Linux. |
| 2. Provider boundary | Upgrade the official OpenAI SDK; introduce `IChatClient` through the maintained adapter, fake client, options, and per-call error handling. | SDK/fake integration tests pass; provider types remain in Infrastructure. |
| 3. Direct execution and persistence | Replace MediatR with services; access `BookDbContext` directly; await generation; delete Quartz, outbox processing, events, and all background workers. Update the frontend create flow in the same release. | Validation runs once; generation completes inside the request; direct reads/writes and cancellation tests pass. |
| 4. Frontend toolchain | Upgrade React/Vite/TS/Router/Tailwind; modernize tests and error/download behavior. | Browser create/status/download and deep-link tests pass against the new backend. |
| 5. Cleanup and rollout | Remove obsolete interfaces/settings/docs and the outbox table through a reviewed migration; retire Razor client after preserving any unique functionality/tests. | Backup restore tested; dependency graph and publish artifact verified. |

Keep stages independently reviewable. Frontend toolchain work can proceed separately, but the awaited POST behavior and frontend interaction must ship together. Pause intake and finish or explicitly interrupt old generation tasks before cutover. If staging slots are unavailable, use an isolated test deployment before a maintenance-window release.

Production upgrade includes the Azure runtime stack: setting `net10.0` in source does not update App Service. Verify the available Linux .NET 10 stack and configure it, or explicitly choose a supported .NET 10 container deployment. Keep previous deployable artifacts and database backups. Run migrations once before enabling traffic.

CI should run on pull requests, with deployment confined to the protected release path. Add Linux and macOS jobs, explicit frontend checks, SQLite integration tests, and a publish-artifact smoke test. Update obsolete GitHub action majors, pin reviewed action revisions, and install a pinned Bruno CLI locally instead of global `latest`. Build assets once. Update Bruno to assert that successful POST completion is followed by `Completed` status and a valid PDF; remove its polling retry loop. Run deterministic generation tests in an isolated environment.

## 9. Acceptance criteria

### Dependency and contract checks

- All retained .NET projects target `net10.0`; runtime/EF tooling are pinned and aligned.
- The official `OpenAI` SDK and compatible Microsoft AI adapter are retained. Application code consumes `IChatClient`.
- `dotnet list BookGenerator.sln package --include-transitive` contains no `MediatR`, `Quartz`, or `Quartz.Extensions.Hosting`.
- Application services query and save through `BookDbContext` directly; repository/store/unit-of-work wrappers are removed.
- POST keeps its JSON-string input and HTTP 200/PascalCase success payload, now returned after completed generation. Frontend and Bruno behavior match that timing change.
- Missing/invalid input is validated once; unknown books return 404; unfinished downloads return the documented 409 response.

### Execution and data checks

- No hosted background worker, outbox processor, generation event handler, or detached generation task remains.
- Successful create writes one book with correctly ordered chapters and `Completed` progress, then returns the same book ID.
- Provider failure, malformed/empty output, deadline expiry, and request cancellation terminate the awaited operation and produce the documented state/error behavior.
- A process termination preserves committed data. Interrupted books are identified for explicit maintenance and are not automatically resumed.
- Measure realistic generation durations against the deployed request timeout budget and verify the chosen chapter/output limits.
- Migrate both an empty database and a copy of the existing SQLite schema/data. Verify outbox removal, enum values, chapter uniqueness/order, relationships, and backup restoration.
- Use real SQLite for transaction/migration tests; EF's in-memory provider does not validate relational behavior.

### macOS, UI, and deployment checks

- Fresh macOS ARM64 checkout builds and runs the documented fake flow without credentials or Windows dependencies; run PDF smoke tests on Intel macOS and Linux as well.
- Generate/download a readable PDF with the returned book ID; fonts are included in the publish output.
- Browser tests cover the awaited generation state, failed creation/re-enabled form, ambiguous timeouts, known-book status, deep-link refresh, download filename, and object URL cleanup.
- Database survives a tested redeploy/restart; storage and App Service runtime conform to the chosen production topology.

The modernization is complete when these behaviors are demonstrated. Package-version changes alone do not establish reliability or macOS compatibility.
