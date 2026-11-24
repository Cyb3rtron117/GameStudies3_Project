using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;

public class StartMenu : MonoBehaviour
{
    public Animator MenuAnim;
    public CinemachineCamera cineCam;
    public Transform StartLookat;
    public Transform MenuLookat;
    public GameObject[] inputs;
    private void Start()
    {
        cineCam.Target.TrackingTarget = StartLookat;
        cineCam.GetComponent<CinemachineOrbitalFollow>().VerticalAxis.Value = 17.2f;
    }
    public void ShowMenu(string controlScheme)
    {
        switch (controlScheme)
        {
            case "Mouse & Keyboard":
                MenuAnim.SetTrigger("ShowKey");
                break;
            case "Gamepad":
                MenuAnim.SetTrigger("ShowCont");
                break;
        }
        
        cineCam.Target.TrackingTarget = MenuLookat;
        cineCam.GetComponent<CinemachineOrbitalFollow>().VerticalAxis.Value = 33.5f;
    }
}
