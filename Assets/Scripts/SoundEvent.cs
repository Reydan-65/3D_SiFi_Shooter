using UnityEngine;
using UnityEngine.Events;

public class SoundEvent : MonoBehaviour
{
    public static UnityEvent<Vector3, float> OnSoundEmmited = new UnityEvent<Vector3, float>();

    public static void EmitSound(Vector3 position, float range)
    {
        OnSoundEmmited.Invoke(position, range);
    }
}
