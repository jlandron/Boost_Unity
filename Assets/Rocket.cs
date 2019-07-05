using UnityEngine;

public class Rocket : MonoBehaviour {
    [SerializeField] float thrust = 100f;  //<-[] Allows the Unity engine to activly adjust this value
    [SerializeField] float RCSthrust = 100f;

    Rigidbody rigidBody;
    AudioSource audioSource;
    // Start is called before the first frame update
    void Start( ) {
        rigidBody = GetComponent<Rigidbody>( );
        audioSource = GetComponent<AudioSource>( );
    }

    // Update is called once per frame
    void Update( ) {
        ProcessInput( );
    }

    private void ProcessInput( ) {
        Thrust( );
        Rotate( );
    }
    void OnCollisionEnter( Collision collision ) {
        switch (collision.gameObject.tag){
            case "Friendly":
                break;
            default:
                print( "ded" );
                break;
        }
    }
    private void Thrust( ) {
        if( Input.GetKey( KeyCode.Space ) ) {
            rigidBody.AddRelativeForce( Vector3.up * thrust * Time.smoothDeltaTime );
            if( !audioSource.isPlaying ) {
                audioSource.Play( );
            }
        }
        else {
            audioSource.Stop( );
        }
    }
    private void Rotate( ) {
        rigidBody.freezeRotation = true; //take control of rotation
        if( Input.GetKey( KeyCode.A ) ) {
            transform.Rotate( Vector3.forward * RCSthrust * Time.smoothDeltaTime );
        }
        else if( Input.GetKey( KeyCode.D ) ) {
            transform.Rotate( -Vector3.forward * RCSthrust * Time.smoothDeltaTime );
        }
        rigidBody.freezeRotation = false; //release control of rotation
    }
}
