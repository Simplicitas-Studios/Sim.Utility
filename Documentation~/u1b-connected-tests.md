# U1b connected optional-integration follow-up

**Verdict: Editor tests unblocked; NOT MERGE-READY for complete integration acceptance.**
Core/runtime/serialization and package/asset GUIDs are unchanged. Historical core
player evidence is not a fresh combined Faciem/Utility build.

## Narrow repair and provenance

The user restored NuGet dependencies and corrected the local Faciem consumer.
They explicitly requested editable local packages, kept CleanConsumer open and
then authorized Pipeline tooling plus restoration of the three temporarily
commented-out cases. Pipeline **0.8.0-exp.1** was explicitly installed in that
consumer only; Unity **6000.6.5f1**, CLI **1.0.0-beta.13**.

The integration test asmdef now explicitly references genuine **R3.dll** with
`overrideReferences: true`. All five original cases/assertions are restored;
the user's removal of an unused import is retained. No test selection/assertion
was weakened. No DLL shim or runtime/serialization change.

These runs used local Utility at **d578649 plus this test-reference edit** and
local Faciem at **3eb408f plus the documented U1b compatibility edits**. They
are not claimed as an immutable Git-pinned clean-consumer campaign. Faciem's
PR #1 records the source hashes, NuGet inputs, lockfile and scope.

## Fresh evidence

| Selection | Executed | Passed | Failed/skipped/inconclusive |
|---|---:|---:|---:|
| Sim.Utility.Faciem.Tests.GuidInspectorIntegrationTests | 5 | 5 | 0 |
| Sim.Utility.Tests.SerializableGuidTests | 12 | 12 | 0 |

[Machine follow-up](u1b-connected-tests.json) and
[exact normalized records](evidence/u1b-connected) retain both final serial
connected dispatches. Earlier baseline runs overlap these identities and are
not additional distinct coverage. Eight static package contracts also passed.

The five cases cover actual AssetDatabase template/style GUID lookup, UXML
instantiation/control presence, initial model/command identity, valid/invalid
input reporting and optional assembly identity. They do **not** establish full
bound Inspector interaction, regeneration/readonly enforcement, lifecycle,
style presentation or IMGUI acceptance.

The combined consumer's Windows/Mono build helper was reached only after clean
Editor compilation and the 24-case combined test campaign. It failed **before
BuildPipeline** because Unity refuses additive scene creation while the active
scene is untitled/unsaved. No player compilation/build result or executable was
produced, no retry/fallback occurred, and the existing scene was not saved,
closed or replaced. Historical standalone-core player evidence remains separate.

Both PRs remain drafts for independent review. No Midnight Wager operations,
merge, auto-merge or deployment. Resolve the player prerequisite explicitly and
complete the remaining Inspector acceptance before claiming full readiness.
