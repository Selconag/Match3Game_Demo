using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public sealed class Board : MonoBehaviour
{
    [Header("Variables")]
    private const float TweenDuration = 0.35f;

    [Header("References")]
    public Row[] _Rows;
    public Tile[,] _Tiles { get; private set; }

    [SerializeField] private AudioClip collectSound;
    [SerializeField] private AudioSource audioSource;


    //Multi dimensional arrays may not have a fixed value on each dimensions. Therefore 0th element will determine its length on each dimension
    public int Width => _Tiles.GetLength(dimension: 0);
    public int Height => _Tiles.GetLength(dimension: 1);

    private readonly List<Tile> _selection = new List<Tile>();

    public static Board Instance { get; private set; }

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

                tile.Item = ItemDatabase.Items[UnityEngine.Random.Range(0,ItemDatabase.Items.Length)];

                _Tiles[x, y] = tile;

            }
        }

        //Check for start popping //Add an spawning algorithm later
        Pop();
    }


    public async void Select(Tile tile)
    {
        if (!_selection.Contains(tile))
        {
            if (_selection.Count > 0)
            {
                if (Array.IndexOf(_selection[0].Neighbours, tile) != -1) _selection.Add(tile);
            }
            else
                _selection.Add(tile);

        }



        if (_selection.Count < 2) return;

        Debug.Log($"Selected Tiles at:({_selection[0].X}, {_selection[0].Y}), ({_selection[1].X}, {_selection[1].Y})");

        //When select happens, awaits result
        await Swap(_selection[0], _selection[1]);

        //if we can't keep the resulst(pop) go on, else revert selection
        if (CanPop())
        {
            Pop();
        }
        else
        {
            // revert selection
            await Swap(_selection[0], _selection[1]);
        }


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
                .Join(icon2Transform.DOMove(icon1Transform.position, TweenDuration));


        //Awaits sequence to finish
        await sequence.Play()
            .AsyncWaitForCompletion();

        //Change parents transforms
        icon1Transform.SetParent(tile2.transform);
        icon2Transform.SetParent(tile1.transform);

        //Change icons
        tile1.Icon = icon2;
        tile2.Icon = icon1;

        var tile1Item = tile1.Item;

        //Change tiles
        tile1.Item = tile2.Item;
        tile2.Item = tile1Item;

    }

    private bool CanPop()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x <Width; x++)
            {
                if (_Tiles[x, y].GetConnectedTiles().Skip(1).Count() >= 2)
                {
                    return true;

                }
            }
        }
        return false;
    }

    //Pop tiles and replace them with new ones
    private async void Pop()
    {
        for(var  y = 0; y < Height; ++y)
        {
            for(var  x = 0; x < Width; ++x)
            {
                var tile = _Tiles[x, y];

                var connectedTiles = tile.GetConnectedTiles();

                //Check if there is too less tiles to pop
                if(connectedTiles.Skip(1).Count() < 2)
                {
                    continue;
                }

                var deflateSequence = DOTween.Sequence();

                foreach (var connectedTile in connectedTiles)
                {
                    deflateSequence.Join(connectedTile.Icon.transform.DOScale(Vector3.zero, TweenDuration));

                }

                //play collcetion sound
                audioSource.PlayOneShot(collectSound);

                //Add score on Text
                ScoreCounter.Instance.Score += tile.Item.Value * connectedTiles.Count;



                //Wait until upper declared sequence ( deflateSequence ) completes its run
                await deflateSequence.Play().AsyncWaitForCompletion();



                var inflateSequence = DOTween.Sequence();

                foreach (var connectedTile in connectedTiles)
                {
                    connectedTile.Item = ItemDatabase.Items[UnityEngine.Random.Range(0, ItemDatabase.Items.Length)];

                    inflateSequence.Join(connectedTile.Icon.transform.DOScale(Vector3.one, TweenDuration));

                }

                //Wait until upper declared sequence ( inflateSequence ) completes its run
                await inflateSequence.Play().AsyncWaitForCompletion();


                x = 0;
                y = 0;


            }
        }

    }

}
