# U1a final integration validation — NOT MERGE-READY

## Verdict

**Do not merge PR #1.** One fresh pinned isolated consumer resolved the three
packages, then failed compilation before test discovery/execution. The five
integration cases are **unexecuted**, not five failing/skipped/passing tests.
No integration acceptance or Utility split regression is established by this run.
Stopped at the requested unchanged-Faciem prerequisite boundary: no retry,
backend fallback, fake assembly, dependency guess, framework patch or production fix.

Reviewed Utility head: `04ea8da9accd0537860da7d66f3eb7d47d8ca83c`.
Utility base: `29354fafd28a59896f6f7f541fbe137b5a3d6a6b`.
Faciem pin: `7c7c9fc8a1a41477808b8569ee29e813e339963d` (Faciem PR #1's
audit head; its runtime source remains baseline `52853a83b5db11cd85c63abe65f49e96db893599`).
These are the **tested inputs**, not a claim that the new documentation commit
was installed in the consumer. All subsequent changes are documentation/evidence only.

## Complete PR and migration review

All 55 diff entries were reviewed, including source, moved assets/metas,
package/asmdef changes, authored tests, validation helper/project, migration notes
and prior raw evidence. Full UTF-8/LF diff SHA-256:
`83ee889c07f909fca9740388f8ba0a0418639b596d367d589a09d1026ae9c3ae`.
The machine report lists all reviewed paths and original validated source hashes
remain available in the historical U1a evidence.

- Core remains independent; its Runtime, Editor and CodeGenerator identities
  stay in place. SerializableGuid's only code change is the guarded Editor import.
- Optional Editor assembly/package split and approved consumer assembly/type-string
  migration are preserved. No runtime model duplication or serialized-data rewrite.
- UXML's two new VM assembly names and stylesheet package path agree with the
  extracted package. Drawer GUID loader and all four original asset/script metas
  agree with the reviewed source. VM, styles and drawer policy remain preserved.
- **Coverage limitation:** the five authored cases check template/control presence,
  initial VM fields/command identity, valid/invalid GUID notifications and assembly
  identity. They do not execute a complete bound Inspector edit/regenerate/readonly/
  style/lifecycle campaign. Even a future five-case pass alone would not prove all
  requested Inspector behavior.

## Isolated attempt and actual resolution

Consumer: `ReviewWorktrees/u1a-final-review/Consumer`, separate from the Utility
checkout, Faciem checkout and all Midnight Wager projects. Unity **6000.6.5f1
(3ff58d469c8a)**, CLI **1.0.0-beta.13**, UTF **1.8.0**, builtin NUnit **2.1.0**.
Exact copy was closed with no lock before launch. No GUI Editor was opened or closed.

The [consumer manifest](evidence/u1a-final-integration/consumer-manifest.json)
pins Utility core and its optional `?path=/Integrations~/FaciemEditor` package to
**the same 04ea8da commit**, and Faciem to **7c7c9fc**. Both Utility package
lock entries confirm the requested revision. Both packages plus Faciem were
registered/extracted by UPM. Core-only hidden-folder exclusion was already
validated in U1a; this run confirms the optional subpackage URL resolves too.

The [actual lockfile](evidence/u1a-final-integration/consumer-packages-lock.json)
contains **14 packages and no dependency cycle**. Optional integration -> Utility
and Faciem; Utility -> no packages; Faciem -> Addressables. Its three assembly
GUID references resolve in this actual cache to:

| GUID | Assembly |
|---|---|
| `73131d858481d53408ae35d0e5a78385` | Sim.Utility |
| `0256ac6985bf6624da798665f1d64436` | Sim.Faciem |
| `c68e2f7abac45cd409bf1e2a2e91700e` | Sim.Faciem.Shared |

All **19 extracted integration files** match the pinned Git subtree after line-ending
normalization, with one normal UPM metadata distinction: package.json gains
`_fingerprint`. Its cache folder suffix `624f575c6ee7` is that subpackage fingerprint,
**not a different Git revision**. The first read-only comparison assumed the cached
manifest was byte-identical; after inspecting the injected field, the corrected
comparison excluded only `_fingerprint` and checked every declared field unchanged.
No package/cache/source file was modified to make this comparison pass.

**Transitive-version limitation:** Faciem declares Addressables **2.8.0**, but UPM
selected **2.11.2**; profiling core resolved to **1.0.3** and scriptable build pipeline
to **3.0.3**. No manifest upgrade was requested or edited into a repository. This
resolver-selected environment is recorded, not silently treated as exact-2.8.0
validation or approved compatibility. No second attempt pinned a different version.

## Exact failure and stop

One `unity_run_tests` call: EditMode, isolated, filter
`Sim.Utility.Faciem.Tests.GuidInspectorIntegrationTests`, JSON/NUnit reports,
600-second bound, graphics disabled. Current-run paths:

- Normalized: `Logs/pi-unity-tests-editmode-20261017T225812059Z-5a07dab341f747e28d4989fd7a6e2a13.json`.
- Expected XML: `Logs/unity-tests-editmode-20261017T225750123Z-04960d50b4cc4e99.xml` — **absent**.
- Log: `Logs/unity-tests-editmode-20261017T225750123Z-04960d50b4cc4e99.log`.

The exact current-run JSON/XML/log paths were inspected once with latest selection
disabled: inspection failed, test outcome uncertain, zero retained case records.
The log ends with script compilation errors and Unity exit code 1, not a timeout.
Representative unchanged-Faciem diagnostic:

```text
Library\PackageCache\com.sim.faciem@7c7c9fc8a1a4\Shared\DisposableBagHolder.cs(2,7): error CS0246: The type or namespace name 'R3' could not be found (are you missing a using directive or an assembly reference?)
```

Additional Shared diagnostics identify missing `DisposableBag`, `Observable<>`
and `Subject<>`. The resolved manifest closure supplies no R3 dependency. This is
an observed compile prerequisite failure, not a fabricated missing DLL test.

**Consumer limitation, separately attributed:** CS1069 also reports forwarded
UIElements types with `UnityEngine.UIElementsModule` unavailable. This minimal
consumer did **not** enable `com.unity.modules.uielements`. Those errors cannot
all be blamed on Faciem or the Utility split. The missing-R3 failures already
meet the requested stop boundary; no rerun added UIElements, guessed R3/NuGet
pins or changed Addressables. Other audited Faciem player/asmdef concerns were
not separately reproduced here and are not claimed as new observed failures.

Full distinct compiler diagnostics are retained in the
[excerpt](evidence/u1a-final-integration/compiler-diagnostics.txt), alongside the
[unaltered run-error artifact](evidence/u1a-final-integration/run-error.json).
The original log's SHA-256 and exact package pins are recorded in the
[machine report](u1a-final-integration-validation.json); no credential/licensing
session log dump is committed.

## What is and is not validated

**Confirmed:** reviewed package graph is acyclic; pinned Git/subpackage resolution;
Editor-only optional asmdef and its three GUID targets; extracted file layout,
source/meta contents and preserved GUIDs. **Not confirmed:** AssetDatabase native
GUID-to-path results, successful integration compilation, UXML instantiation,
stylesheet application, actual data binding/commands/readonly display, serialized
GUID editing through the Inspector, attachment/disposal behavior or Faciem player
compatibility. Registration/source presence is not UI import/runtime acceptance.

No production, package, asmdef or test changes were made. The original source/blob
and artifact-byte checks passed again; **8/8 static package-contract checks** passed.
The historical **12/12 core GUID tests** and **Windows x64 Mono build** remain valid
for their unchanged source (0 errors/1 pre-existing unused-field warning). They were
**not rerun/rebuilt** and must not be counted as fresh integration observations.
No relevant code change triggered task 5's conditional core rerun.

## Merge-readiness gate

1. Resolve Faciem's dependency/compatibility prerequisites in that repository's
   separately approved scope; do not patch them inside Utility or supply fake assemblies.
2. Explicitly enable/document UIElements in the next isolated consumer and choose
   intentionally reviewed transitive versions; current resolver drift is not acceptance.
3. Validate compatible pinned PR heads together, run all five authored cases, then
   exercise real bound Inspector labels/styles/editing/invalid-input restoration,
   Creation regeneration, Manipulation readonly policy and normal lifecycle/IMGUI fallback.
4. Recheck core GUID/player evidence if a justified code change affects it, and obtain
   independent review. Preserve the approved split and serialized runtime schema.

**NOT MERGE-READY.** Draft PR remains open. No merge/deployment, Faciem source
patch, Midnight Wager operation/change, package upgrade workaround, unrelated
refactor or root-independent UI API. Stop for independent review.
