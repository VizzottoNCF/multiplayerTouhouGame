using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class canvasOnline : MonoBehaviour
{
    [SerializeField] Button clientBtn;
    [SerializeField] Button hostBtn;
    private void Awake()
    {
        clientBtn.onClick.AddListener(() => { NetworkManager.Singleton.StartClient(); clientBtn.transform.parent.gameObject.SetActive(false); });

        hostBtn.onClick.AddListener(() => { NetworkManager.Singleton.StartHost(); clientBtn.transform.parent.gameObject.SetActive(false); });
    }
}
