# U1a — Standalone Utility and optional Faciem inspectors

**Latest verdict: NOT MERGE-READY.** The [final pinned integration attempt](u1a-final-integration-validation.md)
resolved the optional package but failed compilation before test discovery/execution.
The historical passing core evidence below is unchanged; it is not integration acceptance.

## Status and approved boundary

Implementation for review, not a release or deployment. Related audit:
[Sim.Faciem PR #1](https://github.com/Simplicitas-Studios/Sim.Faciem/pull/1),
reviewed at `7c7c9fc8a1a41477808b8569ee29e813e339963d`.
Utility base is `29354fafd28a59896f6f7f541fbe137b5a3d6a6b`.

Before edits, the user explicitly approved **“Same-repo package (Recommended)”**
after being told that moving the two public Editor types changes their
assembly-qualified identities and requires consumer asmdef/UXML migration.
That is the controlled Editor-only compatibility break in this PR. No separate
repository, runtime model movement, DI framework or root-independent Faciem API.

## Resulting dependency graph

```text
com.sim.utility                         no package dependencies
  Sim.Utility                           runtime; original assembly/GUID
  Sim.Utility.Editor                    independent Editor helpers; original assembly/GUID
  Sim.Utility.CodeGenerator             Editor-only; unchanged assembly/GUID

com.sim.utility.faciem.editor            explicitly installed, same repository subpackage
  -> com.sim.utility 1.0.0-alpha.1
  -> com.sim.faciem  1.0.0-alpha.1
  Sim.Utility.Faciem.Editor              Editor-only
    -> Sim.Utility
    -> Sim.Faciem
    -> Sim.Faciem.Shared

Future Faciem -> Utility is now possible without a package cycle.
This PR does not add that forward dependency to Faciem.
```

The original Utility Editor references to Faciem
(`0256ac6985bf6624da798665f1d64436`) and Shared
(`c68e2f7abac45cd409bf1e2a2e91700e`) are removed from the core Editor asmdef,
not hidden behind conditional compilation or unresolved optional GUIDs.
The optional assembly carries them instead. Root package dependency on Faciem
is removed. General `AssetUtil`, `FolderReferencePropertyDrawer`,
`PackageManagerUtils`, `PackagePathInfo` and `StaticPackageInfo` stay in core.
No Utility runtime type is copied into the optional package.

### Extracted files

The two C# files and UXML/USS assets move, with their original .meta files, from
`Editor/` to `Integrations~/FaciemEditor/Editor/`:

- `SerializableGuidEditorOfT.cs`: abstract generic drawer, regeneration command,
  GUID editing/rejection, IdUsage permissions, serialization writes, IMGUI fallback.
- `SerializableGuidEditorViewModel.cs`: Faciem Bindable, R3 notifications and Command.
- `SerializableGuidEditorView.uxml`: labels, readonly/text editing, two-way GUID
  binding, visibility converter and Faciem BindableButton command binding.
- `SerializableGuidEditorStyles.uss`.

The VM and stylesheet are byte-identical to base. Drawer logic from its IdUsage
selection onward is byte-identical: only the import/asset lookup changes.
The drawer now resolves the preserved UXML GUID through AssetDatabase rather
than assuming the core package path. UXML changes only its stylesheet package
path and the two assembly-qualified VM type strings. No command, validation,
binding mode, regenerate/readonly policy or subscription-lifetime redesign.

## Runtime/serialization compatibility

`Runtime/SerializableGuid.cs` changes only by putting `using UnityEditor;`
inside `#if UNITY_EDITOR`. Its existing Editor constructors/helpers retain
their existing guards and remain available in Editor builds. Removing the
import entirely would break those unqualified SerializedProperty usages.

Namespace `Sim.Dispositio.Shared`, runtime assembly identity, all models,
constructors, field names/types/attributes, caches, equality/hash behavior,
string representation and serialization methods remain unchanged.
`_guidString` is still the serialized field; parsed GUID/hash caches are not
newly serialized. FolderReference's existing guarded EditorPath is unchanged.

Core assembly GUIDs and all moved script/view/style GUIDs are preserved.
The new integration and test assemblies receive new GUIDs, not reused core GUIDs.
No blanket exception suppression, warning suppression or serialization migration.

## Installation

### Core only

In a consumer project's `Packages/manifest.json`, pin the reviewed Utility commit:

```json
{
  "dependencies": {
    "com.sim.utility": "https://github.com/Simplicitas-Studios/Sim.Utility.git#<UTILITY_REVIEWED_COMMIT>"
  }
}
```

Replace placeholders; examples are not literal installable pins. No Faciem
package, R3, UniTask, Addressables, Bebop or DI container is required by core.
Package version and declared minimum stay `1.0.0-alpha.1` / Unity `6000.0`;
this is an unreleased source PR, **not permission to republish an existing version**.

### Optional Faciem GUID inspectors

Install both Utility packages and a separately validated Faciem revision in the
**project** manifest. Package manifests use semantic version dependencies,
not Git URL dependencies:

```json
{
  "dependencies": {
    "com.sim.utility": "https://github.com/Simplicitas-Studios/Sim.Utility.git#<UTILITY_REVIEWED_COMMIT>",
    "com.sim.utility.faciem.editor": "https://github.com/Simplicitas-Studios/Sim.Utility.git?path=/Integrations~/FaciemEditor#<SAME_UTILITY_COMMIT>",
    "com.sim.faciem": "https://github.com/Simplicitas-Studios/Sim.Faciem.git#<FACIEM_COMPATIBLE_COMMIT>"
  }
}
```

The optional package is a subdirectory package in this same repository, not a
second repository. Git's `?path=` query precedes `#revision`.
The enclosing `Integrations~` folder excludes it from a core-only Unity import.
The optional package root itself has ordinary `Editor/` and `Tests/Editor/`
folders when separately registered.

**Do not interpret the placeholders as a claim that current Faciem main is
compatible.** Faciem PR #1 still records incomplete dependency closure, stale
Bebop DLL metadata, runtime Editor references and unresolved external GUIDs.
This PR does not guess missing dependencies, install substitutes or patch that
repository. In particular, the package graph is fixed here, but this is not a
claim that the unchanged Faciem package can now build. Paired PR-head validation
can follow once the separate Faciem fixes are implemented; no merge is needed
to test pinned PR heads together.

Official Unity 6000.6 references:
[local paths](https://docs.unity3d.com/6000.6/Documentation/Manual/upm-localpath.html),
[Git subdirectory/revision syntax](https://docs.unity3d.com/6000.6/Documentation/Manual/upm-git.html).
Core's native consumer import passed with no integration/Faciem assemblies present.

## Consumer migration

1. Core-only users remove any obsolete direct Faciem pin if nothing else needs
   it; keep `com.sim.utility`. General Editor helpers remain available.
2. Users of the Faciem-backed inspector explicitly install the optional package.
3. Named consumer assemblies defining drawers derived from
   `Sim.Utility.Editor.SerializableGuidEditor<T>`, or using
   `SerializableGuidEditorViewModel`, must add a reference to
   `Sim.Utility.Faciem.Editor`. Keep `Sim.Utility.Editor` too if its core
   helpers are used. Predefined Editor assemblies benefit from autoReferenced.
4. The C# namespaces and type names are unchanged, but reflection/serialized
   assembly-qualified type strings for **these two Editor types only** change
   from `Sim.Utility.Editor` to `Sim.Utility.Faciem.Editor`.
   The packaged UXML's two strings are already updated. Update external UXML,
   custom reflection registrations or assembly-qualified Editor references.
   No core forwarding dependency or duplicate type is provided.
5. Existing references to moved UXML/USS/scripts by GUID retain their original
   GUIDs. Hard-coded paths under `Packages/com.sim.utility/Editor/` for these
   four files must use the new optional package path or preserved asset GUID.
6. Review/restart consumer Editor import normally after changing its manifest.
   No runtime GUID data rewrite is required.

Before accepting the integration, test actual custom GUID inspectors in a
Faciem-compatible consumer: initial value/label, Reference editing, invalid
input restoration, Creation regenerate, Manipulation readonly behavior,
two-way binding/converters/command visibility, normal Inspector attach/detach
and IMGUI fallback. No interactive/visible acceptance is claimed here.

## Validation

See [machine-readable evidence](u1a-validation.json) for source hashes, exact
artifacts, test counts, versions and limitations; raw evidence is under
[evidence/u1a](evidence/u1a/).

- Unity **6000.6.5f1 (3ff58d469c8a)**, Windows; isolated core-only consumer.
  Unity CLI **1.0.0-beta.13**; Test Framework **1.8.0**; builtin NUnit **2.1.0**.
  These are test-environment pins, not changed production package dependencies.
- **12/12 EditMode cases passed**, no failures/skips/inconclusive. Covers default/
  empty/malformed GUIDs, constructors/copy/cache invalidation/equality, exact
  JSON schema/string format, Editor SerializedProperty reads and generic writes.
- **8/8 static package/asset-contract checks passed**. They are not mounted/
  binding tests or substitutes for the five integration tests.
- A real **StandaloneWindows64 / Mono2x Development player build succeeded**,
  zero errors / one pre-existing unused `_cashedHashCode` warning. The smoke
  scene contains a component with a serialized SerializableGuid.
  The built `Sim.Utility.dll` metadata references only `netstandard` and
  `UnityEngine.CoreModule`; no UnityEditor or Sim.Faciem type references.
  Player executable was built, **not launched**. No IL2CPP/other-platform/
  minimum-6000.0/visual acceptance.
- Six affected C# files passed `dotnet format whitespace --verify-no-changes`
  using host SDK **10.0.202**; host tools are not player dependencies.
- First isolated attempt produced **no NUnit XML and no executed tests** because
  the new build-evidence helper used uint instead of Unity's int error/warning
  counters (CS0266 at its two assignments). Exact log was inspected; after
  fixing those two field types, the fresh run passed. This was a harness compile
  error, not two failed GUID cases. Retained artifact/excerpt includes the failure.
- Optional integration **five authored cases unrun; Unity import/compile and
  Inspector acceptance blocked**, not skipped/passed. Faciem's separate
  prerequisites are not bypassed with stub frameworks or private reflection.
- No changes or Unity operations against Midnight Wager, no Faciem source
  changes, no D02/native-root investigation, no PlayMode tests, merge or deployment.
- Utility has no pinned LICENSE/COPYING notice in the reviewed tree. No license,
  copyright or transitive terms are invented or changed here; confirm them before
  redistribution/publication, including the optional Faciem integration.

## Reproduction

For core Unity validation, use the committed `Validation~/CoreConsumer`
project, whose manifest pins local core (`file:../../..`, relative to Packages)
and UTF 1.8.0 only. Use declared Unity 6000.6.5f1, a closed exact project copy,
isolated EditMode selection `Sim.Utility.Tests.SerializableGuidTests`, graphics
disabled and explicit current-run result/log paths. Then run
`-executeMethod UtilityPlayerSmokeBuild.Build -buildTarget Win64` as an isolated
headless build; it writes `Logs/u1a-player-build.json`. Never test against an
unrelated open project or automatically close its Editor.

Run `python Tools~/test_package_contract.py` for the eight static checks.
To enable the optional tests later, add `com.sim.utility.faciem.editor` to the
compatible consumer's `testables`, install UTF 1.8.0, and select
`Sim.Utility.Faciem.Tests.GuidInspectorIntegrationTests`. They check asset/
control presence, initial readonly/command data, GUID change parsing and the
new assembly identity; they do not by themselves prove visible Inspector behavior.

**Stop for independent review.** Core evidence is green; optional integration
acceptance remains explicitly open. No auto-merge or deployment.
