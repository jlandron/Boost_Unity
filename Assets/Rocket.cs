﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {
    [SerializeField] float thrust = 100f;  //<-[] Allows the Unity engine to activly adjust this value
    [SerializeField] float RCSthrust = 100f;
    [SerializeField] float levelLoadDelay = 1.5f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip dead;
    [SerializeField] AudioClip win;

    [SerializeField] ParticleSystem mainEngineParticlePort;
    [SerializeField] ParticleSystem mainEngineParticleStbd;
    [SerializeField] ParticleSystem deadParticle;
    [SerializeField] ParticleSystem winParticle;


    Rigidbody rigidBody;
    AudioSource audioSource;

    bool devMode;
    bool CollisionOff = false;

    int currentLevel;
    enum LevelState {
        ALIVE, DEAD, TRANSCENDING
    }
    LevelState levelState = LevelState.ALIVE;
    // Start is called before the first frame update
    void Start( ) {
        rigidBody = GetComponent<Rigidbody>( );
        audioSource = GetComponent<AudioSource>();
        devMode = Debug.isDebugBuild;
        
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
        if( devMode ) {
            CheckDevKeys( );
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
        audioSource.volume = 0.5f;
        audioSource.PlayOneShot( win );
        winParticle.Play( );
        levelState = LevelState.TRANSCENDING;
        Invoke( "LoadNextLevel", levelLoadDelay);  //paramertarize the load time
    }

    private void KillShip( ) {
        if( CollisionOff ) {
            return;
        }
        audioSource.Stop( );
        audioSource.volume = 0.5f;
        audioSource.PlayOneShot( dead );
        deadParticle.Play( );
        levelState = LevelState.DEAD;
        Invoke( "LoadPreviousLevel", levelLoadDelay );
    }

    private void LoadNextLevel( ) {
        currentLevel = SceneManager.GetActiveScene( ).buildIndex;
        int nextScene = (currentLevel + 1 < SceneManager.sceneCountInBuildSettings) ? currentLevel + 1 : 1;
        SceneManager.LoadScene( nextScene );
    }
    private void LoadPreviousLevel( ) {
        currentLevel = SceneManager.GetActiveScene( ).buildIndex;
        int previousScene = (currentLevel - 1 > 1) ? currentLevel - 1 : 1;
        SceneManager.LoadScene( previousScene );
    }

    private void RespondToThrustInput( ) {
        if( Input.GetKey( KeyCode.Space ) ) {
            rigidBody.AddRelativeForce( Vector3.up * thrust * Time.deltaTime );
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
        
        if( Input.GetKey( KeyCode.A ) ) {
            Rotate( 1 );
        }
        else if( Input.GetKey( KeyCode.D ) ) {
            Rotate( -1 );
        }
        
    }

    private void Rotate( int direction ) {
        rigidBody.freezeRotation = true; //take control of rotation
        transform.Rotate( direction * Vector3.forward * RCSthrust * Time.deltaTime );
        rigidBody.freezeRotation = false; //release control of rotation
    }

    private void CheckDevKeys( ) {
        if( Input.GetKeyDown( KeyCode.L ) ) {
            LoadNextLevel( );
        }
        if( Input.GetKeyDown( KeyCode.C ) ) {
            CollisionOff = true;
        }
    }
}

