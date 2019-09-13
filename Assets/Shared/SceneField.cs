using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Shared {
    [System.Serializable]
    public class SceneField {
        [SerializeField] private Object sceneAsset;
        [SerializeField] private string sceneName = "";
        public string SceneName => sceneName;

        // makes it work with the existing Unity methods (LoadLevel/LoadScene)
        public static implicit operator string(SceneField sceneField) => sceneField.SceneName;

        public static bool operator true(SceneField sceneField) => !string.IsNullOrEmpty(sceneField.SceneName);
        public static bool operator false(SceneField sceneField) => string.IsNullOrEmpty(sceneField.SceneName);
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SceneField))]
    public class SceneFieldPropertyDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, GUIContent.none, property);
            var sceneAsset = property.FindPropertyRelative("sceneAsset");
            var sceneName = property.FindPropertyRelative("sceneName");
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            if (sceneAsset != null) {
                sceneAsset.objectReferenceValue =
                    EditorGUI.ObjectField(position, sceneAsset.objectReferenceValue, typeof(SceneAsset), false);
                if (sceneAsset.objectReferenceValue != null) sceneName.stringValue = ((SceneAsset) sceneAsset.objectReferenceValue).name;
            }

            EditorGUI.EndProperty();
        }
    }
#endif
}