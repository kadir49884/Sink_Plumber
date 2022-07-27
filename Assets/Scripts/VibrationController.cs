using Lofelt.NiceVibrations;
using UnityEngine;

public class VibrationController : MonoBehaviour
{
    private static VibrationController instance;

    protected bool _continuousActive = false;
    protected float _amplitudeLastFrame = -1f;
    protected float _frequencyLastFrame = -1f;

    public static VibrationController Instance { get => instance; set => instance = value; }

    

    private void Awake()
    {
        //MMNViOS.iOSInitializeHaptics();

        if(instance == null)
        {
            instance = this;
        }

    }

    public void Vibrate(HapticPatterns.PresetType hapticType)
    {
        //MMVibrationManager.Haptic(hapticType,true,true,this,-1);
        //MMVibrationManager.Haptic(hapticType, false, true, this);
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Warning);
    }
    
    public void Vibrate(float instensity , float sharpness)
    {
        //MMVibrationManager.Haptic(hapticType,true,true,this,-1);
        //HapticPatterns.TransientHaptic(instensity, sharpness,true,this);
    }

    public virtual void ContinuousHaptics(float ContinuousAmplitude, float ContinuousFrequency, float ContinuousDuration )
    {
        if (!_continuousActive)
        {
            // START
            HapticController.fallbackPreset = HapticPatterns.PresetType.LightImpact;
            HapticPatterns.PlayConstant(ContinuousAmplitude, ContinuousFrequency, ContinuousDuration);
            
        }
        else
        {
            // STOP
            HapticController.Stop();
        }
    }

    public void Vibrate(float instensity , float sharpness , float duration)
    {
        //MMVibrationManager.Haptic(hapticType,true,true,this,-1);
        //MMVibrationManager.ContinuousHaptic(instensity, sharpness, duration, HapticTypes.None, this, true);
    }
    
    public void StopContinuousHaptic()
    {
        //MMVibrationManager.Haptic(hapticType,true,true,this,-1);
        //MMVibrationManager.StopContinuousHaptic(true);
        HapticController.Stop();
    }
}
