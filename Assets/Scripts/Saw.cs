using System;
using UnityEngine;

public class Saw : MonoBehaviour {
    private float[] nextY  = { 3.6f, -3.6f };
    private int     i = 0;

    void Update() {

        var pos = transform.position;
        
        if (Math.Abs(pos.y - nextY[i]) < 0.05)
            i = (i + 1) % 2;

        transform.position = Vector3.MoveTowards(pos, new Vector2(pos.x,nextY[i]),
            2 * Time.deltaTime);
        transform.Rotate(0, 0, 1.1f);
    }
}