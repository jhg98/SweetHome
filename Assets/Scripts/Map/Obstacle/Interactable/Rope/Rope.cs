using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rope : MonoBehaviour
{
    public List<RopePiece> pieces;

    public void InitRope()
    {
        pieces = GetComponentsInChildren<RopePiece>().ToList();
        for (int i = 0; i < pieces.Count; i++)
        {
            pieces[i].index = i;
        }
    }
}
