using UnityEngine;

public class CameraFacing : MonoBehaviour
{

    private Camera _camera;

    void Start()
    {
        _camera = Camera.main;
    }

    void Update()
    {
        transform.forward = _camera.transform.forward;
    }
}
