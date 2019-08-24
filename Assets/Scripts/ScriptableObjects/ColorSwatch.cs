using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="ColorSwatch", menuName ="ScriptableObjects/ColorSwatch")]
public class ColorSwatch : ScriptableObject
{
    [System.Serializable]
    public class Entry
    {
        public string name;
        public Color color;
    }

    public List<Entry> colors = new List<Entry>();

    public Color GetColor(string name)
    {
        Entry entry = colors.Find(c => c.name == name);
        if (entry != null)
        {
            return entry.color;
        }
        return Color.white;
    }
}
