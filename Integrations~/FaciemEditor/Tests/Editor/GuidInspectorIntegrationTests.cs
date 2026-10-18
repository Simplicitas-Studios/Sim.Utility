using System;
using System.Collections.Generic;
using NUnit.Framework;
using R3;
using Sim.Faciem;
using Sim.Faciem.Commands;
using Sim.Utility.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace Sim.Utility.Faciem.Tests
{
    public sealed class GuidInspectorIntegrationTests
    {
        private const string ViewGuid = "8cc1f80178ade534b843b4f01cc72afb";
        private const string StyleGuid = "3253c6c306f35454db12869030cd77de";
        private const string FirstGuid = "12345678-1234-5678-9abc-1234567890ab";
        private const string SecondGuid = "abcdef01-2345-6789-abcd-0123456789ab";

        [Test]
        public void TemplateRetainsItsAssetGuidsAndExpectedControls()
        {
            string viewPath = AssetDatabase.GUIDToAssetPath(ViewGuid);
            string stylePath = AssetDatabase.GUIDToAssetPath(StyleGuid);
            Assert.That(viewPath, Does.StartWith("Packages/com.sim.utility.faciem.editor/"));
            Assert.That(stylePath, Does.StartWith("Packages/com.sim.utility.faciem.editor/"));
            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(viewPath);
            Assert.That(asset, Is.Not.Null);

            TemplateContainer root = asset.Instantiate();

            Assert.That(root.Q<VisualElement>("GuidDisplayContainer"), Is.Not.Null);
            Assert.That(root.Q<Label>("lbPropertyName"), Is.Not.Null);
            Assert.That(root.Q<Label>("lbReadonlyValue"), Is.Not.Null);
            Assert.That(root.Q<TextField>("tfValue"), Is.Not.Null);
            Assert.That(root.Q<BindableButton>("btRefresh"), Is.Not.Null);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void ViewModelRetainsInitialValuesAndCommandIdentity(bool isReadonly)
        {
            using (var command = new Command(Observable.Return(true), true))
            {
                var model = new SerializableGuidEditorViewModel(command, "Identifier", FirstGuid, isReadonly);

                Assert.That(model.Guid, Is.EqualTo(FirstGuid));
                Assert.That(model.PropertyName, Is.EqualTo("Identifier"));
                Assert.That(model.IsReadonly, Is.EqualTo(isReadonly));
                Assert.That(model.RegenerateId, Is.SameAs(command));
            }
        }

        [Test]
        public void GuidChangesStillReportValidAndInvalidInput()
        {
            using (var command = new Command(Observable.Return(true), true))
            {
                var model = new SerializableGuidEditorViewModel(command, "Identifier", FirstGuid, false);
                var changes = new List<Maybe<Guid>>();
                using (model.GuidChanged.Subscribe(value => changes.Add(value)))
                {
                    model.Guid = SecondGuid;
                    model.Guid = "invalid";

                    Assert.That(changes.Count, Is.EqualTo(2));
                    Assert.That(changes[0].IsSome, Is.True);
                    Assert.That(changes[0].Value, Is.EqualTo(Guid.Parse(SecondGuid)));
                    Assert.That(changes[1].IsNone, Is.True);
                }
            }
        }

        [Test]
        public void ViewModelUsesTheNewOptionalAssemblyAndOriginalNamespace()
        {
            Assert.That(typeof(SerializableGuidEditorViewModel).FullName, Is.EqualTo("Sim.Utility.Editor.SerializableGuidEditorViewModel"));
            Assert.That(typeof(SerializableGuidEditorViewModel).Assembly.GetName().Name, Is.EqualTo("Sim.Utility.Faciem.Editor"));
            Assert.That(typeof(SerializableGuidEditor<>).Assembly.GetName().Name, Is.EqualTo("Sim.Utility.Faciem.Editor"));
        }
    }
}
