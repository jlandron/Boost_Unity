using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]

public class Oscillator : MonoBehaviour{

    [SerializeField] Vector3 movementVector = new Vector3(10f, 10f, 10f);

    [Range(1,20)][SerializeField] float period = 2f;

    Vector3 startingPos;
    // Start is called before the first frame update
    void Start(){
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update() {
        Ocsillate( );
    }

    private void Ocsillate( ) {
        if( (int)period <= 0 ) {
            return;
        }
        float cycles = Time.time / period;

        const float tau = Mathf.PI * 2;
        float sin = Mathf.Abs( Mathf.Sin( cycles * tau ) );
        Vector3 offset = movementVector * sin;
        transform.position = startingPos + offset;
    }
}
