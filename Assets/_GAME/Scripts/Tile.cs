using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [Header("References")]
    public int X;
    public int Y;
    private Item item;
    public Item Item
    {
        get => item;


        set 
        {
            if (item == value) return;

            item = value;

            Icon.sprite = item.Sprite;
        }
    }
    public Image Icon;
    public Button Button;

    private void Start()
    {
        Button.onClick.AddListener(() => Board.Instance.Select(tile:this));

    }
}
