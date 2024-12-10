using UnityEngine;

public class SpaceMaterialShift : MonoBehaviour
{
    [SerializeField] float speedX = 0.1f;
    [SerializeField] float speedY = 0.1f;

    public Material mat;

    private void Update()
    {
        Vector2 offset = new Vector2(speedX * Time.deltaTime, speedY * Time.deltaTime);
        mat.mainTextureOffset += offset;
        //Debug.Log($"New Offset: {mat.mainTextureOffset}");
    }

}
