using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [Header("Variables")]
    public int X;
    public int Y;

    [Header("References")]

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

    //The leftmost tile is 0, if this tile is 0 or less, there won't be a adjacent lefter tile here
    public Tile Left => X > 0 ? Board.Instance._Tiles[X - 1, Y] : null;
    public Tile Top => Y > 0 ? Board.Instance._Tiles[X, Y - 1] : null;
    public Tile Right => X < Board.Instance.Width - 1 ? Board.Instance._Tiles[X + 1, Y] : null;
    public Tile Bottom => Y < Board.Instance.Height - 1 ? Board.Instance._Tiles[X, Y + 1] : null;

    //In order not to add neighbours again and again (to prevent stackoverflow with infinite loop) we tell the neighbour references to active element of foreach adding
    public Tile[] Neighbours => new[]
    {
        Left,
        Top,
        Right,
        Bottom
    };


    private void Start()
    {
        Button.onClick.AddListener(() => Board.Instance.Select(tile:this));

    }

    public List<Tile> GetConnectedTiles(List<Tile> exclude = null)
    {
        var result = new List<Tile> { this, };
        if (exclude == null)
        {
            exclude = new List<Tile> { this, };

        }
        else
        {
            exclude.Add(this);
        }

        foreach (var neighbour in Neighbours)
        {
            //Check here later
            if (neighbour == null || exclude.Contains(neighbour) || neighbour.item != Item) continue;

            result.AddRange(neighbour.GetConnectedTiles(exclude));
    
        }

        return result;
    }


}
