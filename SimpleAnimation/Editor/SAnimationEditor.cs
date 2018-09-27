
using NUnit.Framework;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SAnimationSystem))]
public class SAnimationEditor : Editor
{
    static class Styles
    {
        public static GUIContent animations = new GUIContent("Animations", "Animation list");
        public static GUIContent cullingMode = new GUIContent("Culling Mode", "Controls what is updated when the object has been culled");
        public static GUIContent animatorUpdateMode = new GUIContent("Update Mode", "The update mode of the Animator");
        public static GUIContent startAnimationIndex = new GUIContent("Start Animation", "Start animation index among list");
    }

    SerializedProperty states;
    SerializedProperty cullingMode;
    SerializedProperty animatorUpdateMode;
    SerializedProperty startAnimationIndex;

    void OnEnable()
    {
        states = serializedObject.FindProperty("m_States");
        cullingMode = serializedObject.FindProperty("m_CullingMode");
        animatorUpdateMode = serializedObject.FindProperty("m_AnimatorUpdateMode");
        startAnimationIndex = serializedObject.FindProperty("m_StartAnimationIndex");
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(states, Styles.animations, true);
        EditorGUILayout.PropertyField(startAnimationIndex, Styles.startAnimationIndex);
        EditorGUILayout.PropertyField(cullingMode, Styles.cullingMode);
        EditorGUILayout.PropertyField(animatorUpdateMode, Styles.animatorUpdateMode);
        serializedObject.ApplyModifiedProperties();
    }
}



[CustomPropertyDrawer(typeof(SAnimationSystem.EditorState))]
class SClipStateDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        EditorGUILayout.BeginHorizontal();
        // Calculate rects
        Rect clipRect = new Rect(position.x, position.y, position.width / 2 - 5, position.height);
        Rect nameRect = new Rect(position.x + position.width / 2 + 5, position.y, position.width / 2 - 5, position.height);

        EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("clip"), GUIContent.none);
        EditorGUI.PropertyField(clipRect, property.FindPropertyRelative("state"), GUIContent.none);
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.EndHorizontal();
        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
