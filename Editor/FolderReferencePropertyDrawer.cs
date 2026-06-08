using System.IO;
using Sim.Dispositio.Shared;
using UnityEditor;
using UnityEngine;

namespace Sim.Utility.Editor
{
    [CustomPropertyDrawer(typeof(FolderReference))]
    public sealed class FolderReferencePropertyDrawer : PropertyDrawer
    {
        private bool _initialized;
        private SerializedProperty _guid;
        private Object _obj;

        private void Init(SerializedProperty property)
        {
            _initialized = true;
            _guid = property.FindPropertyRelative(nameof(FolderReference.FolderId));
            _obj = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(_guid.stringValue));
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!_initialized)
            {
                Init(property);
            }

            GUIContent guiContent = EditorGUIUtility.ObjectContent(_obj, typeof(DefaultAsset));

            Rect r = EditorGUI.PrefixLabel(position, label);

            Rect textFieldRect = r;
            textFieldRect.width -= 19f;

            GUIStyle textFieldStyle = new GUIStyle("TextField")
            {
                imagePosition = _obj ? ImagePosition.ImageLeft : ImagePosition.TextOnly
            };

            if (GUI.Button(textFieldRect, guiContent, textFieldStyle) && _obj)
            {
                EditorGUIUtility.PingObject(_obj);
            }

            if (textFieldRect.Contains(Event.current.mousePosition))
            {
                if (Event.current.type == EventType.DragUpdated)
                {
                    Object reference = DragAndDrop.objectReferences[0];
                    string path = AssetDatabase.GetAssetPath(reference);
                    DragAndDrop.visualMode = Directory.Exists(path) ? DragAndDropVisualMode.Copy : DragAndDropVisualMode.Rejected;
                    Event.current.Use();
                }
                else if (Event.current.type == EventType.DragPerform)
                {
                    Object reference = DragAndDrop.objectReferences[0];
                    string path = AssetDatabase.GetAssetPath(reference);
                    if (Directory.Exists(path))
                    {
                        _obj = reference;
                        _guid.stringValue = AssetDatabase.AssetPathToGUID(path);
                    }

                    Event.current.Use();
                }
            }

            Rect objectFieldRect = r;
            objectFieldRect.x = textFieldRect.xMax + 1f;
            objectFieldRect.width = 19f;

            if (GUI.Button(objectFieldRect, "", GUI.skin.GetStyle("IN ObjectField")))
            {
                string path = EditorUtility.OpenFolderPanel("Select a folder", "Assets", "");
                if (path.Contains(Application.dataPath))
                {
                    path = "Assets" + path.Substring(Application.dataPath.Length);
                    _obj = AssetDatabase.LoadAssetAtPath(path, typeof(DefaultAsset));
                    _guid.stringValue = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(_obj));
                }
                else Debug.LogError("The path must be in the Assets folder");
            }
        }
    }
}
