using UnityEngine;
using System.Collections;

public class MouseDrag : MonoBehaviour {

    //float distance = 10;
    private Vector3 offset = new Vector3(0,0,0);
    private Vector3 offset1 = new Vector3(0, 0, 0);
    private Vector3 offset0 = new Vector3(0, 0, 0);
    //public Camera cam;

    private void OnMouseDrag()
    {
        offset1 = Camera.main.transform.position - transform.position;
        if (offset1.magnitude < offset0.magnitude + 20)
            {
            offset = offset1;
            }
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, offset.magnitude);
        Vector3 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        objPosition.y = transform.position.y;
        transform.position = objPosition;

        if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            transform.Rotate(Vector3.forward * 10f, Space.Self);
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            transform.Rotate(Vector3.back * 10f, Space.Self);
        }

        if(Input.GetKey(KeyCode.Backspace))
        {
            Destroy(gameObject);
        }
    }

    private void OnMouseDown()
    {
        offset = Camera.main.transform.position - transform.position;
        offset0 = offset;
        offset1 = new Vector3(0, 0, 0);
    }

    
}
