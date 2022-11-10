using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSheet : MonoBehaviour
{
    public int StartIndex;
    public int SpriteCount;
    [SerializeField] SpriteRenderer _renderer;
    public string Path;
    bool _reverse;
    public bool Reverse { get => _reverse; 
        set
        {
            _reverse = value;
            StartIndex = value ? SpriteCount : 0;
        }
    }
    bool _finished;
    Sprite[] _sheet;
    public bool Play;
    void Start()
    {
        _sheet = Resources.LoadAll<Sprite>(Path);
    }
    void Update()
    {
        if (!Play || _sheet == null) return;
        if (Reverse)
        {
            _renderer.sprite = _sheet[StartIndex--];
            if (StartIndex == 0)
            {
                Play = false;
                StartIndex = SpriteCount;
            }
        }
        else
        {
            _renderer.sprite = _sheet[StartIndex++];
            if (StartIndex == SpriteCount)
            {
                Play = false;
                StartIndex = 0;
            }
        }
    }
}