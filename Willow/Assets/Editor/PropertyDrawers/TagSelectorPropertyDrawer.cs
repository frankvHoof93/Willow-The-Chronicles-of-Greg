﻿using nl.SWEG.Willow.Utils.Attributes;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace nl.SWEG.Willow.Editor.PropertyDrawers
{
    /// <summary>
    /// Custom Inspector-Drawer for TagSelector-Attribute.
    /// <para>
    /// Used to select one or more Tags in Serialized Field
    /// </para>
    /// </summary>
    [CustomPropertyDrawer(typeof(TagSelectorAttribute))]
    public class TagSelectorPropertyDrawer : PropertyDrawer
    {
        /// <summary>
        /// Draws Tag-Property in Inspector
        /// </summary>
        /// <param name="position">Rect in Inspector-Window</param>
        /// <param name="property">Property to draw for</param>
        /// <param name="label">Label for Property</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                EditorGUI.BeginProperty(position, label, property);
                TagSelectorAttribute attrib = (TagSelectorAttribute)attribute;

                if (attrib.UseDefaultTagFieldDrawer) // Use Default Drawer
                    property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
                else
                {
                    //generate the taglist + custom tags
                    List<string> tagList = new List<string>
                    {
                        "<NoTag>" // String-Value for Empty
                    };
                    tagList.AddRange(UnityEditorInternal.InternalEditorUtility.tags);
                    string propertyString = property.stringValue;
                    int index = -1;
                    if (propertyString == "")
                    {
                        //The tag is empty
                        index = 0; //first index is the special <notag> entry
                    }
                    else
                    {
                        //check if there is an entry that matches the entry and get the index
                        //we skip index 0 as that is a special custom case
                        for (int i = 1; i < tagList.Count; i++)
                        {
                            if (tagList[i] == propertyString)
                            {
                                index = i;
                                break;
                            }
                        }
                    }
                    //Draw the popup box with the current selected index
                    index = EditorGUI.Popup(position, label.text, index, tagList.ToArray());
                    //Adjust the actual string value of the property based on the selection
                    property.stringValue = index < 1 ? string.Empty : tagList[index];
                }
                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }
    }
}