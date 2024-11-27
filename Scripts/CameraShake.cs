using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public void rf_CameraShake(float _duration, float _magnitude)
    {
        StartCoroutine(rIE_CameraShake(_duration, _magnitude));
    }

    private IEnumerator rIE_CameraShake(float _duration, float _magnitude)
    {
        // grabs cam starting pos
        Vector3 originalPos = transform.localPosition;

        float timer = 0f;
        
        while (timer < _duration)
        {
            // makes coords for camera shake
            float shakeX = originalPos.x + Random.Range(-1f, 1f) * _magnitude;
            float shakeY = originalPos.y + Random.Range(-1f, 1f) * _magnitude;

            // sets new coords
            transform.localPosition = new Vector3 (shakeX, shakeY, originalPos.z);

            timer += Time.deltaTime;

            yield return null;
        }

        // resets position
        transform.localPosition = originalPos;
    }
}
