"""Static package/asset contracts, not Editor binding or player behavior tests."""
from pathlib import Path
import json
import re
import unittest
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[1]
INTEGRATION = ROOT / "Integrations~/FaciemEditor"
UTILITY_GUID = "73131d858481d53408ae35d0e5a78385"
FACIEM_GUIDS = {"0256ac6985bf6624da798665f1d64436", "c68e2f7abac45cd409bf1e2a2e91700e"}


def read_json(path):
    return json.loads(path.read_text(encoding="utf-8-sig"))


def read_guid(path):
    return re.search(r"^guid: ([0-9a-f]{32})$", path.read_text(), re.MULTILINE).group(1)


class PackageContractTests(unittest.TestCase):
    def test_core_manifest_has_no_dependencies(self):
        self.assertEqual(read_json(ROOT / "package.json")["dependencies"], {})

    def test_core_assemblies_have_no_external_faciem_edges(self):
        for folder in ["Runtime", "Editor", "CodeGenerator"]:
            for path in (ROOT / folder).rglob("*.asmdef"):
                references = read_json(path)["references"]
                self.assertTrue(all(reference == "GUID:" + UTILITY_GUID for reference in references), path)
            for path in (ROOT / folder).rglob("*.cs"):
                self.assertNotIn("Sim.Faciem", path.read_text(encoding="utf-8-sig"), path)

    def test_original_core_assembly_guids_are_stable(self):
        expected = {
            "Runtime/Sim.Utility.asmdef.meta": UTILITY_GUID,
            "Editor/Sim.Utility.Editor.asmdef.meta": "b9e2fbc2b540c7949b5e5784d30c6176",
        }
        for path, guid in expected.items():
            self.assertEqual(read_guid(ROOT / path), guid)

    def test_optional_package_depends_on_both_packages_only(self):
        package = read_json(INTEGRATION / "package.json")
        core = read_json(ROOT / "package.json")
        self.assertEqual(package["name"], "com.sim.utility.faciem.editor")
        self.assertEqual(package["dependencies"], {
            "com.sim.utility": core["version"], "com.sim.faciem": "1.0.0-alpha.1"})
        self.assertTrue(INTEGRATION.relative_to(ROOT).parts[0].endswith("~"))

    def test_optional_assembly_is_editor_only(self):
        assembly = read_json(INTEGRATION / "Editor/Sim.Utility.Faciem.Editor.asmdef")
        self.assertEqual(assembly["name"], "Sim.Utility.Faciem.Editor")
        self.assertEqual(assembly["includePlatforms"], ["Editor"])
        self.assertEqual(set(assembly["references"]), {"GUID:" + guid for guid in FACIEM_GUIDS | {UTILITY_GUID}})

    def test_view_and_style_guids_are_preserved(self):
        self.assertEqual(read_guid(INTEGRATION / "Editor/SerializableGuidEditorView.uxml.meta"),
                         "8cc1f80178ade534b843b4f01cc72afb")
        self.assertEqual(read_guid(INTEGRATION / "Editor/SerializableGuidEditorStyles.uss.meta"),
                         "3253c6c306f35454db12869030cd77de")

    def test_template_references_optional_assembly_and_preserved_assets(self):
        template = ET.parse(INTEGRATION / "Editor/SerializableGuidEditorView.uxml")
        types = [node.attrib["data-source-type"] for node in template.iter()
                 if "data-source-type" in node.attrib]
        self.assertEqual(types, ["Sim.Utility.Editor.SerializableGuidEditorViewModel, Sim.Utility.Faciem.Editor"] * 2)
        style = template.find("Style").attrib["src"]
        self.assertIn("Packages/com.sim.utility.faciem.editor/Editor/", style)
        self.assertIn("guid=3253c6c306f35454db12869030cd77de", style)
        drawer = (INTEGRATION / "Editor/SerializableGuidEditorOfT.cs").read_text(encoding="utf-8-sig")
        self.assertIn('ViewAssetGuid = "8cc1f80178ade534b843b4f01cc72afb"', drawer)
        self.assertIn("AssetDatabase.GUIDToAssetPath(ViewAssetGuid)", drawer)

    def test_core_consumer_contains_no_faciem_package(self):
        manifest = ROOT / "Validation~/CoreConsumer/Packages/manifest.json"
        dependencies = read_json(manifest)["dependencies"]
        self.assertEqual(set(dependencies), {"com.sim.utility", "com.unity.test-framework"})
        self.assertEqual((manifest.parent / dependencies["com.sim.utility"].removeprefix("file:")).resolve(), ROOT)


if __name__ == "__main__":
    unittest.main(verbosity=2)
