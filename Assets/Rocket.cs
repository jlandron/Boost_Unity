using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {
    [SerializeField] float thrust = 100f;  //<-[] Allows the Unity engine to activly adjust this value
    [SerializeField] float RCSthrust = 100f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip dead;
    [SerializeField] AudioClip win;

    [SerializeField] ParticleSystem mainEngineParticlePort;
    [SerializeField] ParticleSystem mainEngineParticleStbd;
    [SerializeField] ParticleSystem deadParticle;
    [SerializeField] ParticleSystem winParticle;


    Rigidbody rigidBody;
    AudioSource audioSource;

    int level = 0;
    enum LevelState {
        ALIVE, DEAD, TRANSCENDING
    }
    LevelState levelState = LevelState.ALIVE;
    // Start is called before the first frame update
    void Start( ) {
        rigidBody = GetComponent<Rigidbody>( );
        audioSource = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update( ) {
        ProcessInput( );
    }

    private void ProcessInput( ) {
        if( levelState == LevelState.ALIVE ) {
            RespondToThrustInput( );
            RespondToRotateInput( );
        }
    }
    void OnCollisionEnter( Collision collision ) {
        if( !(levelState == LevelState.ALIVE) ) {
            return;
        }
        switch (collision.gameObject.tag){
            case "Friendly":
                //do nothing
                break;
            case "Finish":
                StartSuccess( );
                break;
            default:
                KillShip( );
                break;
        }
    }

    private void StartSuccess( ) {
        audioSource.Stop( );
        audioSource.PlayOneShot( win );
        winParticle.Play( );
        levelState = LevelState.TRANSCENDING;
        Invoke( "LoadNextLevel", 1f );  //paramertarize the load time
    }

    private void KillShip( ) {
        audioSource.Stop( );
        audioSource.PlayOneShot( dead );
        deadParticle.Play( );
        levelState = LevelState.DEAD;
        Invoke( "LoadFirstLevel", 1f );
    }

    private void LoadNextLevel( ) {
        if( level < SceneManager.sceneCount ) {
            level = level + 1;
        }
        SceneManager.LoadScene( level );
    }
    private void LoadFirstLevel( ) {
        SceneManager.LoadScene( 0 );
    }

    private void RespondToThrustInput( ) {
        if( Input.GetKey( KeyCode.Space ) ) {
            rigidBody.AddRelativeForce( Vector3.up * thrust * Time.smoothDeltaTime );
            if( !audioSource.isPlaying ) {
                audioSource.PlayOneShot( mainEngine );
            }
            mainEngineParticlePort.Play( );
            mainEngineParticleStbd.Play( );
        }
        else {
            audioSource.Stop( );
            mainEngineParticlePort.Stop( );
            mainEngineParticleStbd.Stop( );
        }
    }
    private void RespondToRotateInput( ) {
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
