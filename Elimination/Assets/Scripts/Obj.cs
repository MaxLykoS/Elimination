using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obj: MonoBehaviour
{
    public bool isHighLight;
    public Bound Bound;
    public bool isLoaded;
    public QNode BelongedNode;

    private GameObject Go;

    public void Init(Bound bound)
    {
        this.Bound = bound;
        this.Go = gameObject;
        isLoaded = false;
        BelongedNode = null;
        isHighLight = false;
    }
}
