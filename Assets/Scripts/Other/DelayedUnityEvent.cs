using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace UnityEngine.Events
{
    [Serializable]
    public class DelayActionPair
    {
        [Min(0f)]
        public float delay;
        public UnityEvent action;
    }
    [Serializable]
    public class DelayedUnityEvent
    {
        [SerializeField, HideInInspector]
        private List<DelayActionPair> items = new List<DelayActionPair>();

        public List<DelayActionPair> Items => items;

        private static readonly Dictionary<float, WaitForSeconds> _waitCache = new Dictionary<float, WaitForSeconds>() { [0f] = new WaitForSeconds(0f) };
        public void Invoke()
        {
            foreach (var item in items)
            {
                MainMechanics.instance.StartCoroutine(InvokeDelay(item));
            }
        }
        private IEnumerator InvokeDelay(DelayActionPair delayActionPair)
        {
            if (!_waitCache.TryGetValue(delayActionPair.delay, out var value))
            {
                _waitCache.Add(delayActionPair.delay, new WaitForSeconds(delayActionPair.delay));
            }
            yield return _waitCache[delayActionPair.delay];
            delayActionPair.action.Invoke();
        }

#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(DelayActionPair))]
        public class DelayActionPairPropertyDrawer : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                EditorGUI.BeginProperty(position, label, property);

                SerializedProperty delayProp = property.FindPropertyRelative("delay");
                Rect delayRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(delayRect, delayProp, new GUIContent("Delay"), false);

                SerializedProperty actionProp = property.FindPropertyRelative("action");
                Rect actionRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUI.GetPropertyHeight(actionProp, true));
                EditorGUI.PropertyField(actionRect, actionProp, new GUIContent("Action"), true);

                EditorGUI.EndProperty();
            }

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                SerializedProperty actionProp = property.FindPropertyRelative("action");
                return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing + EditorGUI.GetPropertyHeight(actionProp, true);
            }
        }

        [CustomPropertyDrawer(typeof(DelayedUnityEvent))]
        public class DelayedUnityEventPropertyDrawer : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                EditorGUI.BeginProperty(position, label, property);
                SerializedProperty itemsProp = property.FindPropertyRelative("items");
                EditorGUI.PropertyField(position, itemsProp, label, true);
                EditorGUI.EndProperty();
            }

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                SerializedProperty itemsProp = property.FindPropertyRelative("items");
                return EditorGUI.GetPropertyHeight(itemsProp, label, true);
            }
        }
#endif
    }
}