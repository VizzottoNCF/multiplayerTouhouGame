using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;
using Unity.Netcode.Transports.UTP;

public class canvasOnline : MonoBehaviour
{
    [SerializeField] GameObject NetworkManagera;
    [SerializeField] Button clientBtn;
    [SerializeField] Button hostBtn;
    [SerializeField] TMP_Text IP_ADRESS;
    private void Awake()
    {
        clientBtn.onClick.AddListener(() => { NetworkManager.Singleton.StartClient(); clientBtn.transform.parent.gameObject.SetActive(false); });

        hostBtn.onClick.AddListener(() => { NetworkManager.Singleton.StartHost(); clientBtn.transform.parent.gameObject.SetActive(false); });
    }
}
