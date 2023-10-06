using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public sealed class Board : MonoBehaviour
{
    public static Board Instance { get; private set;}


    public Row[] _Rows;
    public Tile[,] _Tiles { get; private set; }

    //Multi dimensional arrays may not have a fixed value on each dimensions. Therefore 0th element will determine its length on each dimension
    public int Width => _Tiles.GetLength(dimension: 0);
    public int Height => _Tiles.GetLength(dimension: 1);

    private readonly List<Tile> _selection = new List<Tile>();

    private const float TweenDuration = 0.35f;

    private void Awake() => Instance = this;


    private void Start()
    {
        _Tiles = new Tile[_Rows.Max(selector:Row => Row.Tiles.Length), _Rows.Length];  

        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                var tile = _Rows[y].Tiles[x];

                tile.X = x;
                tile.Y = y;

                tile.Item = ItemDatabase.Items[Random.Range(0,ItemDatabase.Items.Length)];

                _Tiles[x, y] = tile;

            }
        }
            
    }

    public async void Select(Tile tile)
    {
        if(!_selection.Contains(tile))
            _selection.Add(tile);

        if (_selection.Count < 2) return;

        Debug.Log($"Selected Tiles at:({_selection[0].X}, {_selection[0].Y}), ({_selection[1].X}, {_selection[1].Y})");

        //When select happens, awaits result
        await Swap(_selection[0], _selection[1]);

        _selection.Clear();

    }

    public async Task Swap(Tile tile1,  Tile tile2)
    {
        //Get icon references from tile.Icon
        var icon1 = tile1.Icon;
        var icon2 = tile2.Icon;

        //Get transforms of each references
        var icon1Transform = icon1.transform;
        var icon2Transform = icon2.transform;

        var sequence = DOTween.Sequence();


        sequence.Join(icon1Transform.DOMove(icon2Transform.position, TweenDuration))
            .Join(icon1Transform.DOMove(icon1Transform.position, TweenDuration));


        //Awaits sequence to finish
        await sequence.Play()
            .AsyncWaitForCompletion();

        //Change parents transforms
        icon1Transform.SetParent(tile2.transform);
        icon2Transform.SetParent(tile1.transform);

        //Change icons
        tile2.Icon = icon1;
        tile1.Icon = icon2;

        var tile1Item = tile1.Item;

        //Change tiles
        tile1.Item = tile2.Item;
        tile2.Item = tile1Item;

    }


}
