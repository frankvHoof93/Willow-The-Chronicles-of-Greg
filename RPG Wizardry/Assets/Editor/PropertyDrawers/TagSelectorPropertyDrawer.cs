﻿using nl.SWEG.RPGWizardry.Utils.Attributes;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace nl.SWEG.RPGWizardry.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(TagSelectorAttribute))]
    public class TagSelectorPropertyDrawer : PropertyDrawer
    {
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
                    if (index < 1)
                        property.stringValue = "";
                    else
                        property.stringValue = tagList[index];
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