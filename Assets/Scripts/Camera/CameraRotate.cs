using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    public Vector3 center = new Vector3(9.5f, 0, 9.5f);
    public float speed = 1f;
    private Vector3 cameraOffset;
    [Range(0.01f, 1.0f)]
    public float SmoothFactor = 0.5f;

    private GameObject lastHitObject;

    private bool lastRotateLeft = true;
    private bool stopRotate = false;

    Ray ray;
    RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        transform.LookAt(center);
        cameraOffset = transform.position - center;
    }

    // Update is called once per frame
    void Update()
    {
        if (lastRotateLeft && !stopRotate)
        {
            transform.LookAt(center);
            transform.Translate(Vector3.right * Time.deltaTime * speed * 2f);
        }
        else if (!lastRotateLeft && !stopRotate)
        {
            transform.LookAt(center);
            transform.Translate(Vector3.left * Time.deltaTime * speed * 2f);
        }
        if (Input.GetMouseButton(1))
        {
            Quaternion camTurnAngle = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * speed, Vector3.up);

            cameraOffset = camTurnAngle * cameraOffset;

            Vector3 newPos = center + cameraOffset;

            transform.position = Vector3.Slerp(transform.position, newPos, SmoothFactor);
            transform.LookAt(center);
            if (Input.GetAxis("Mouse X") > 0)
            {
                lastRotateLeft = false;
            }
            else if (Input.GetAxis("Mouse X") < 0)
            {
                lastRotateLeft = true;
            }
        }

        if (Input.GetKeyDown("space"))
        {
            stopRotate = !stopRotate;
        }

        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            //print(hit.collider.name);
            // Change colour and move up if it's colour hasn't been changed already
            if (lastHitObject && lastHitObject != hit.transform.gameObject)
            {
                lastHitObject.GetComponent<Renderer>().material.SetColor("OutlineColor", Color.black);
                lastHitObject.transform.position = lastHitObject.transform.position - new Vector3(0, 0.25f, 0); ;

            }
            if (lastHitObject != hit.transform.gameObject)
            {
                hit.transform.gameObject.GetComponent<Renderer>().material.SetColor("OutlineColor", new Color(255f/255f, 255f/255f, 235f/255f, 1));
                hit.transform.gameObject.transform.position = hit.transform.gameObject.transform.position + new Vector3(0, 0.25f, 0);
            }

            lastHitObject = hit.transform.gameObject;
        }
    }
}
