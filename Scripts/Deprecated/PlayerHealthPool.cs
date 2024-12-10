using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthPool : MonoBehaviour
{
    public int totalHealth = 1;
    public int currentHealth = 1;

    [SerializeField] private RectTransform _barRect;

    [SerializeField] private RectMask2D _mask;

    private float _maxRightMask;
    private float _initialRightMask;

    private void Start()
    {
        _maxRightMask = 370;
        //_initialRightMask = _mask.padding.z;
    }

    private void Update()
    {
        rf_SetValue();
    }

    //[ClientRpc]
    public void rf_SetValue()// ClientRPC()
    {
        float curHealth = currentHealth;
        float maxHealth = totalHealth;

        float targetWidth = curHealth / maxHealth; // gets health percentage
        var newRightMaskPercent = (targetWidth - 1) * -1; // inverse percent
       
        // reverses it by subtracting 1 and then turning it back into positives
        var newRightMask = newRightMaskPercent * _maxRightMask;
        var padding = _mask.padding;


        padding.z = newRightMask;
        _mask.padding = padding;
    }
}
