using Unity.Mathematics.Geometry;
using UnityEngine;

public class CameraSmoothDesk : MonoBehaviour
{
    private Vector3 awakeRot;

    private Vector2 mousePos;
    private float mag;

    private float _y;
    private float _y2;
    private float _x;
    private float _x2;

    [SerializeField] float smooth = 10f;
    [SerializeField] float smooth2 = 3f;

    private void Awake()
    {
        awakeRot = transform.localRotation.eulerAngles;
    }

    void Update()
    {
        mousePos = Input.mousePosition;

        if(_y < mousePos.y)
        {
            _y = mousePos.y;
            _y2 = Mathf.Clamp(_y2 + smooth * Time.deltaTime, -1,1);
            // Add Y
            Debug.Log("add y");
        }

        if(_y > mousePos.y)
        {
            _y = mousePos.y;
            _y2 = Mathf.Clamp(_y2 - smooth * Time.deltaTime, -1, 1);
            // Remove Y
            Debug.Log("remove y");
        }

        if(_x < mousePos.x)
        {
            _x = mousePos.x;
            _x2 = Mathf.Clamp(_x2 + smooth * Time.deltaTime, -1, 1);
            // Add X
        }

        if(_x > mousePos.x)
        {
            _x = mousePos.x;
            _x2 = Mathf.Clamp(_x2 - smooth * Time.deltaTime, -1, 1);
            // Remove X
        }

        Vector3 targetRotOffset = new Vector3(-(_y2), _x2, 0);

        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(awakeRot + targetRotOffset), smooth2 * Time.deltaTime);
    }
}
