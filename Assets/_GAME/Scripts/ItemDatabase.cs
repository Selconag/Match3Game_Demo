using UnityEngine;

public static class ItemDatabase
{
    public static Item[] Items { get; private set; }

    //Helps us to load everything before scene loads, helps game run smooth!!!
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] private static void Initialize() => Items = Resources.LoadAll<Item>(path:"Items/");
}
