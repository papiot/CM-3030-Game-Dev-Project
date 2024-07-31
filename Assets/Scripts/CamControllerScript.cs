using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class CamControllerScript : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] float screenX;
    [SerializeField] float screenY;
    private CinemachineFramingTransposer framingTransposer;
    // Start is called before the first frame update

    private void Awake()
    {
        framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    public void SetNewCameraXProperties(float screenX)
    {
        framingTransposer.m_ScreenX = screenX;
    }

    public void SetNewCameraYProperties(float screenY)
    {
        framingTransposer.m_ScreenY = screenY;
    }
}