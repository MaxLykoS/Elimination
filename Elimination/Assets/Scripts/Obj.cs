using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obj
{
    public Bound Bound;
    public bool isLoaded;

    private GameObject Go;
    public Obj(GameObject go)
    {
        this.Go = go;
        isLoaded = false;
        Bound = new Bound(new Vector2(go.transform.position.x, go.transform.position.z), new Vector2(1, 1));
    }
}
