using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fall : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform cam_pos;
    public Vector3 offset;
    public int picked = 0;
    public bool hold;
    public int counts;





    void Start()
    {
        cam_pos = GameObject.FindGameObjectWithTag("Respawn").transform;
    }
    IEnumerator waitfor2()
    {

        transform.position = Vector3.Lerp(transform.position, cam_pos.position + offset, Time.deltaTime * 1.5f);
        transform.rotation = Quaternion.Lerp(transform.rotation, cam_pos.rotation, Time.deltaTime);

        yield return new WaitForSeconds(3);
        
        picked = 0;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().isKinematic = false;


    }
    public void drop()
    {
        picked = 0;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().isKinematic = false;


    }

    void OnCollisionEnter(Collision other)
    {
        FindObjectOfType<AudioManager>().Play("tak");

    }
    public void pickup()
    {
        picked = 1;
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().isKinematic = true;
    }
    public void pickdrop()
    {

        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().isKinematic = false;
    }


    // Update is called once per frame
    void Update()
    {
        if (picked == 1)
        {
            StartCoroutine(waitfor2());
            // picked=false;

        }

    }
}
