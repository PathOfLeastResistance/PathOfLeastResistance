using System;
using UnityEngine;

[Serializable]
public class Phrase
{
    [Multiline] public string Text;
    public BubbleAlignment Speaker;
    public BubbleTail BubbleTail;
    public BubbleForm BubbleForm;
}
