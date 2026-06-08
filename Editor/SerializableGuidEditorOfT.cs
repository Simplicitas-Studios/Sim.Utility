using System;
using System.Reflection;
using R3;
using Sim.Dispositio.Shared;
using Sim.Faciem.Commands;
using Sim.Faciem.Shared;
using Sim.Utility.Editor.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sim.Utility.Editor
{
    public abstract class SerializableGuidEditor<T> : PropertyDrawer where T : SerializableGuid, new()
    {
        protected bool DrawLabel { get; set; } = true;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = AssetUtil.Load<VisualTreeAsset>("Editor/SerializableGuidEditorView.uxml", StaticPackageInfo.SimUtility)
                .Instantiate();

            var usage = fieldInfo.GetCustomAttribute<IdUsageAttribute>()?.Usage ?? IdUsage.Reference;

            var disposables = root.RegisterDisposableBag();
            var regenerateCommand = new Command(
                Observable.Return(usage == IdUsage.Creation), Observable.Return(usage == IdUsage.Creation), false);


            var label = preferredLabel; //DrawLabel ? property.displayName : string.Empty;

            var typedGuidInstance = SerializableGuid.GetPropertyGuid<T>(property);

            var vm = new SerializableGuidEditorViewModel(
                regenerateCommand,
                label,
                typedGuidInstance.Instance.ToString(),
                usage == IdUsage.Manipulation);

            disposables.Add(vm.GuidChanged
                .Subscribe(maybeNewGuid =>
                {
                    if (maybeNewGuid.IsNone)
                    {
                        vm.Guid = typedGuidInstance.Instance.ToString();
                    }
                    else
                    {
                        var newInstance = new T();
                        newInstance.SetGuid(maybeNewGuid.Value);
                        SerializableGuid.SetPropertyGuid<T>(property, newInstance);
                        property.serializedObject.ApplyModifiedProperties();
                    }
                }));

            disposables.Add(regenerateCommand
                .Subscribe(_ =>
                {
                    var newInstance = new T();
                    newInstance.SetGuid(Guid.NewGuid());
                    SerializableGuid.SetPropertyGuid<T>(property, newInstance);
                    property.serializedObject.ApplyModifiedProperties();
                    vm.Guid = SerializableGuid.GetPropertyGuid<T>(property).Instance.ToString();
                }));

            root.dataSource = vm;

            return root;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var instance = SerializableGuid.GetPropertyGuid<T>(property).Instance;
            var pos = EditorGUI.PrefixLabel(position, label);
            EditorGUI.LabelField(pos, instance.ToString());
        }
    }
}
