# Sim.Utility Faciem Editor Integration

Optional Editor-only GUID inspectors extracted from Sim.Utility.
Explicitly install `com.sim.utility.faciem.editor` in addition to compatible
`com.sim.utility` and `com.sim.faciem` packages.

This package retains the original inspector source/asset GUIDs and namespaces.
Its two public types now live in `Sim.Utility.Faciem.Editor`; named consumer
assemblies and external assembly-qualified type strings may need updating.

The full [migration and validation report](../../Documentation~/u1a-standalone-utility.md)
is in the source repository (it is outside this subpackage when installed
through Git `?path=`). See the repository's root README at the same reviewed
commit if this relative link is unavailable in the installed subpackage.

**Acceptance remains pending:** the separate Faciem U1 dependency/player
prerequisites must be resolved before real integration compilation, its five
tests and Inspector behavior can be validated. Do not infer integration
acceptance from the core Utility's passing tests/player build.
