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

    public void Init()
    {
        this.Go = gameObject;
        this.Bound = new Bound(Go.transform.position.x,Go.transform.position.z,Go.transform.localScale.x,Go.transform.localScale.z);
        isLoaded = false;
        BelongedNode = null;
        isHighLight = false;
    }
}
