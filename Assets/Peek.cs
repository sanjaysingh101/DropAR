using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peek : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    ///test.SetActive(true);
                    Debug.Log("This is player");
                    if (hit.transform.gameObject.GetComponent<Fall>().picked == 0)
                    {
                        
                        hit.transform.gameObject.GetComponent<Fall>().pickup();

                    }

                }
                
            }
        }

        if (Input.touchCount == 1)
        {


            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;

            // Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow, 100f);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Player"))
                {            //test.SetActive(true);



                    Debug.Log("This is player");

                    if (hit.transform.gameObject.GetComponent<Fall>().picked == 0)
                    {
                        hit.transform.gameObject.GetComponent<Fall>().pickup();

                    }
                }
            }
        }

    }
}
