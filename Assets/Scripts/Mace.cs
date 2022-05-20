using UnityEngine;

public class Mace : MonoBehaviour {
    
    void Update() {
        // ALL MACES ROTATE IN THE SAME ANGLE TO AVOID UNNECESSARY DIFFICULTY
        float angle = 30f * Mathf.Sin( Time.time * 2f);
        transform.localRotation = Quaternion.Euler( 0, 0, angle);
    }
}