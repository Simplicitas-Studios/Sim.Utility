# Sim.Utility
Utility Framework for Simplicitas Unity Packages

The core `com.sim.utility` package is independent of Sim.Faciem. Runtime models,
GUID serialization, general Editor helpers and code generation remain in core.

Faciem-backed GUID inspectors are an **explicitly installed optional package**:
`com.sim.utility.faciem.editor`, in `Integrations~/FaciemEditor` in this repository.
The `~` parent keeps its sources out of the core package import.

See [U1a migration, installation and validation](Documentation~/u1a-standalone-utility.md)
before updating: the two inspector types retain their namespaces but move to the
`Sim.Utility.Faciem.Editor` assembly. Existing consumer asmdef and type-string
references may need updating. Runtime/core assembly and moved asset GUIDs are preserved.

**Validation scope:** Unity 6000.6.5f1 core Editor tests and Windows x64 Mono build
passed. The optional integration's Unity compilation, five tests and interactive
inspector acceptance remain blocked by the separate Faciem U1 prerequisites;
they are not claimed as passing. No Faciem compatibility fix is included here.
