using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public enum MotorcyleState
{
    idle, driving, drifting
}
[System.Serializable]
public struct MotorcycleSounds
{
    public AudioClip idleClip;
    public AudioClip startDrivingClip;
    public AudioClip breakClip;
    public AudioClip drivingClip;
    public AudioClip startDriftingClip;
    public AudioClip driftingClip;
    public AudioClip stopDriftingClip;
}
public class MotorcycleSoundManager : MonoBehaviour
{
    [SerializeField] MotorcyleState currentState;

    [Header("Sound Clips")]
    [SerializeField] private MotorcycleSounds grassSounds;
    [SerializeField] private MotorcycleSounds curbSounds;
    [SerializeField] private MotorcycleSounds streetSounds;
    [SerializeField] private AudioClip onCurbSound;
    private MotorcycleSounds currentSounds;
    [Header("Sound Values")]
    [Header("Driving Sound Values")]
    [SerializeField] private AnimationCurve velocityToVolume;
    [SerializeField] private AnimationCurve velocityToPitch;
    [SerializeField] private float velocityXScale;
    [Header("Drifting Sound Values")]
    [SerializeField] private AnimationCurve sideVelocityToVolume;
    [SerializeField] private float sideVelocityXScale;
    [Header("Transition Values")]
    [SerializeField] private float minTransitionDelay=0.5f;
    private float timeOfLastTransition=-99f;
    [Header("Idle Transition Values")]
    [SerializeField, Range(0, 1f)] private float minStartDrivingMagnitude=0.2f;
    [SerializeField] private float minStartDrivingVelocity = 1f;
    [Header("Driving Transition Values")]
    [SerializeField, Range(0, 1f)] private float maxStartIdleMagnitude = 0.1f;
    [SerializeField] private float minStartDriftingVelocity = 0.6f;
    [Header("Drifting Transition Values")]
    [SerializeField] private float maxStopDriftingVelocity = 0.4f;
    public void Awake()
    {
        currentSounds = streetSounds;
    }
    public void Start()
    {
        PlayerScript.instance.changeSurfaceEvent.AddListener(ChangeSurface);
    }
    public void ChangeSurface()
    {
        Debug.Log("CHANGED SURFACE");
        switch (PlayerScript.instance.currentSurface)
        {
            case SurfaceType.street:
                currentSounds = streetSounds;
                break;
            case SurfaceType.curb:
                GlobalSoundManager.instance.playSFX(onCurbSound, 1f);
                currentSounds = curbSounds;
                break;
            default:
                currentSounds = grassSounds;
                break;
        }
    }
    // Update is called once per frame
    void Update()
    {
        //Debug.Log($"Velocity: {PlayerScript.Instance.rb2D.velocity.magnitude}");
        switch (currentState)
        {
            case MotorcyleState.idle:
                IdleBehaviour();
                IdleTransitions();
                break;
            case MotorcyleState.driving:
                DrivingBehaviour();
                DrivingTransitions();
                break;
            case MotorcyleState.drifting:
                DriftingBehaviour();
                DriftingTransitions();
                break;
            default:
                Debug.LogError("UNRECOGNIZED MOTORCYCLE STATE");
                break;
        }    
    }
    private void IdleBehaviour()
    {
        GlobalSoundManager.instance.playLoopSFX(currentSounds.idleClip);
        GlobalSoundManager.instance.SetLoopSFXVolume(0.5f);

    }
    private void IdleTransitions()
    {
        if (Time.time-timeOfLastTransition>minTransitionDelay&&JoystickScript.leftJoystick.sqrMagnitude > minStartDrivingMagnitude * minStartDrivingMagnitude)
        {
            GlobalSoundManager.instance.playSFX(currentSounds.startDrivingClip);
            timeOfLastTransition = Time.time;
            currentState = MotorcyleState.driving;
        }
    }
    private void DrivingBehaviour()
    {
        GlobalSoundManager.instance.playLoopSFX(currentSounds.drivingClip);
        GlobalSoundManager.instance.SetLoopSFXVolume(velocityToVolume.Evaluate(PlayerScript.instance.rb2D.velocity.magnitude/velocityXScale));
        GlobalSoundManager.instance.SetLoopSFXPitch(velocityToPitch.Evaluate(PlayerScript.instance.rb2D.velocity.magnitude / velocityXScale));
    }
    private void DrivingTransitions()
    {
        if (Time.time - timeOfLastTransition > minTransitionDelay && JoystickScript.leftJoystick.sqrMagnitude < maxStartIdleMagnitude * maxStartIdleMagnitude)
        {
            timeOfLastTransition = Time.time;

            GlobalSoundManager.instance.playSFX(currentSounds.breakClip,0.2f);

            currentState = MotorcyleState.idle;
        }
        else if (Time.time - timeOfLastTransition > minTransitionDelay && PlayerScript.instance.isDrifting&& PlayerScript.instance.rb2D.velocity.sqrMagnitude > minStartDriftingVelocity * minStartDriftingVelocity)
        {
            timeOfLastTransition = Time.time;

            GlobalSoundManager.instance.playSFX(currentSounds.startDriftingClip, sideVelocityToVolume.Evaluate(Vector2.Dot(PlayerScript.instance.rb2D.velocity, transform.right) / sideVelocityXScale));
            currentState = MotorcyleState.drifting;
        }
    }
    private void DriftingBehaviour()
    {
        GlobalSoundManager.instance.playLoopSFX(currentSounds.driftingClip);
        GlobalSoundManager.instance.SetLoopSFXVolume(sideVelocityToVolume.Evaluate(Vector2.Dot(PlayerScript.instance.rb2D.velocity, transform.right)/sideVelocityXScale));
    }
    private void DriftingTransitions()
    {
        if (Time.time - timeOfLastTransition > minTransitionDelay && !PlayerScript.instance.isDrifting||PlayerScript.instance.rb2D.velocity.sqrMagnitude < maxStopDriftingVelocity * maxStopDriftingVelocity)
        {
            timeOfLastTransition = Time.time;

            currentState = MotorcyleState.driving;
            GlobalSoundManager.instance.playSFX(currentSounds.stopDriftingClip, sideVelocityToVolume.Evaluate(Vector2.Dot(PlayerScript.instance.rb2D.velocity, transform.right) / sideVelocityXScale));

        }
    }
}
