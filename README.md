# Sim.Utility
Utility Framework for Simplicitas Unity Packages

**Not merge-ready:** the [connected follow-up](Documentation~/u1b-connected-tests.md) now passes all five optional integration cases and twelve core cases in the locally edited Faciem consumer. Full bound-Inspector acceptance remains incomplete, and the combined player attempt was blocked before building by an unsaved untitled scene. The [earlier pinned failure](Documentation~/u1a-final-integration-validation.md) remains historical evidence.

The core `com.sim.utility` package is independent of Sim.Faciem. Runtime models,
GUID serialization, general Editor helpers and code generation remain in core.

Faciem-backed GUID inspectors are an **explicitly installed optional package**:
`com.sim.utility.faciem.editor`, in `Integrations~/FaciemEditor` in this repository.
The `~` parent keeps its sources out of the core package import.

See [U1a migration, installation and validation](Documentation~/u1a-standalone-utility.md)
before updating: the two inspector types retain their namespaces but move to the
`Sim.Utility.Faciem.Editor` assembly. Existing consumer asmdef and type-string
references may need updating. Runtime/core assembly and moved asset GUIDs are preserved.

**Validation scope:** historical standalone-core Windows x64 Mono evidence is unchanged/not rebuilt. Fresh Unity 6000.6.5f1 connected runs pass 12 core and 5 optional integration cases. This follow-up adds a test assembly reference, not core/runtime/schema changes or Faciem production fixes. Interactive Inspector and current combined player acceptance are not established.
