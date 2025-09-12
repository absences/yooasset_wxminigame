using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingPanel : ShowHidable {

    public float Speed
    {
        get { return anim.speed; }
        set { anim.speed = value; }
    }
}
