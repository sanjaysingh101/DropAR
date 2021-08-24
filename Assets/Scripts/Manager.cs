using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class Manager : MonoBehaviour
{
    // Start is called before the first frame update
    public RenderTexture[] Render_Textures;
    public Material[] Materialzz;
    public GameObject[] ARPlanez;
    public GameObject[] pics;
    private ARRaycastManager ARM;
    private ARPlaneManager APM;
    private ARSessionOrigin ASO;
    private GameObject spawnedObjects;
    public GameObject dropbutton;
    public GameObject gameobjecttoinstantiate;
    public GameObject Planeobjecttoinstantiate;
    public GameObject UIz;
    public GameObject scaningground;
    public GameObject watermark;
    public bool save;
    public Animation saveON;
    public Animation saveOFf;
    public Animation WatermarkOFf;
    public Animation WatermarkON;
    public int count = 0;
    public static int fall = 0;
    private bool Placed_Done;
    //public GameObject test;

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private void Awake()
    {
        ARM = GetComponent<ARRaycastManager>();
        APM = GetComponent<ARPlaneManager>();

    }
    bool TryGetTouchPosition(out Vector2 touchposition)
    {
        if (Input.touchCount > 0)
        {
            touchposition = Input.GetTouch(0).position;
            return true;
        }
        else
        {
            touchposition = default;
            return false;

        }

    }
    public void dropall()
    {
        dropbutton.SetActive(false);

        for (int i = 0; i <= pics.Length; i++)
        {
            pics[i].GetComponent<Fall>().drop();
        }

        FindObjectOfType<AudioManager>().Play("drop");
        // fall = 1;

    }
    public void NOdrop()
    {
        fall = 0;
    }
    public void saveisON()
    {
        save = true;
        saveON.Play();
        FindObjectOfType<AudioManager>().Play("whoop");

    }
    public void saveisOFF()
    {
        save = false;
        saveOFf.Play();
        FindObjectOfType<AudioManager>().Play("whoop2");

    }
    public void WatermarkOFF()
    {

        WatermarkOFf.Play();
        FindObjectOfType<AudioManager>().Play("whoop2");

    }
    public void Watermark_ON()
    {

        WatermarkON.Play();
        FindObjectOfType<AudioManager>().Play("whoop");

    }


    // Update is called once per frame
    void Update()
    {
        pics = GameObject.FindGameObjectsWithTag("Player");

        if (ARM.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;

            if (spawnedObjects == null)
            {

                spawnedObjects = Instantiate(Planeobjecttoinstantiate, hitPose.position, hitPose.rotation);
                UIz.SetActive(true);
                scaningground.SetActive(false);

                Placed_Done = true;
            }
            else
            {
                foreach (ARPlane plane in APM.trackables)
                {
                    plane.gameObject.SetActive(false);
                }
                APM.enabled = false;
                //spawnedObjects.transform.position = hitPose.position;
            }
        }
        if (Placed_Done == true)
        {
            ASO.enabled = false;
            APM.enabled = false;
            ARM.enabled = false;


            foreach (ARPlane plane in APM.trackables)
            {
                plane.gameObject.SetActive(false);
            }
        }
        // if (Input.touchCount == 1)
        // {
        //     test.SetActive(true);
        // }


        
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


    protected const string MEDIA_STORE_IMAGE_MEDIA = "android.provider.MediaStore$Images$Media";
    protected static AndroidJavaObject m_Activity;

    protected static string SaveImageToGallery(Texture2D a_Texture, string a_Title, string a_Description)
    {
        using (AndroidJavaClass mediaClass = new AndroidJavaClass(MEDIA_STORE_IMAGE_MEDIA))
        {
            using (AndroidJavaObject contentResolver = Activity.Call<AndroidJavaObject>("getContentResolver"))
            {
                AndroidJavaObject image = Texture2DToAndroidBitmap(a_Texture);
                return mediaClass.CallStatic<string>("insertImage", contentResolver, image, a_Title, a_Description);
            }
        }
    }

    protected static AndroidJavaObject Texture2DToAndroidBitmap(Texture2D a_Texture)
    {
        byte[] encodedTexture = a_Texture.EncodeToPNG();
        using (AndroidJavaClass bitmapFactory = new AndroidJavaClass("android.graphics.BitmapFactory"))
        {
            return bitmapFactory.CallStatic<AndroidJavaObject>("decodeByteArray", encodedTexture, 0, encodedTexture.Length);
        }
    }

    protected static AndroidJavaObject Activity
    {
        get
        {
            if (m_Activity == null)
            {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                m_Activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            }
            return m_Activity;
        }
    }
    public void Click()
    {

        //UIz.SetActive(false);
        FindObjectOfType<AudioManager>().Play("click");

        StartCoroutine(CaptureScreenshotCoroutine(Screen.width, Screen.height));


        if (count == 19)
        {
            count = 0;
        }




    }
    private IEnumerator CaptureScreenshotCoroutine(int width, int height)
    {
        UIz.SetActive(false);
        watermark.SetActive(true);

        yield return new WaitForEndOfFrame();
        Texture2D tex = new Texture2D(width, height);
        //RenderTexture rtex = new RenderTexture();
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();
        if (count == 0)
        {
            Graphics.Blit(tex, Render_Textures[0]);

        }
        if (count == 1)
        {
            Graphics.Blit(tex, Render_Textures[1]);
        }

        if (count == 2)
        {
            Graphics.Blit(tex, Render_Textures[2]);
        }

        if (count == 3)
        {
            Graphics.Blit(tex, Render_Textures[3]);
        }

        if (count == 4)
        {
            Graphics.Blit(tex, Render_Textures[4]);
        }

        if (count == 5)
        {
            Graphics.Blit(tex, Render_Textures[5]);
        }

        if (count == 6)
        {
            Graphics.Blit(tex, Render_Textures[6]);
        }

        if (count == 7)
        {
            Graphics.Blit(tex, Render_Textures[7]);
        }

        if (count == 8)
        {
            Graphics.Blit(tex, Render_Textures[8]);
        }

        if (count == 9)
        {
            Graphics.Blit(tex, Render_Textures[9]);
        }

        if (count == 10)
        {
            Graphics.Blit(tex, Render_Textures[10]);
        }

        if (count == 11)
        {
            Graphics.Blit(tex, Render_Textures[11]);
        }
        if (count == 12)
        {
            Graphics.Blit(tex, Render_Textures[12]);
        }
        if (count == 13)
        {
            Graphics.Blit(tex, Render_Textures[13]);
        }
        if (count == 14)
        {
            Graphics.Blit(tex, Render_Textures[14]);
        }
        if (count == 15)
        {
            Graphics.Blit(tex, Render_Textures[15]);
        }
        if (count == 16)
        {
            Graphics.Blit(tex, Render_Textures[16]);
        }
        if (count == 17)
        {
            Graphics.Blit(tex, Render_Textures[17]);
        }
        if (count == 18)
        {
            Graphics.Blit(tex, Render_Textures[18]);
        }
        if (count == 19)
        {
            Graphics.Blit(tex, Render_Textures[19]);
        }
        if (count == 0)
        {
            GameObject go = Instantiate(gameobjecttoinstantiate, new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z), Quaternion.Euler(Camera.main.transform.localEulerAngles.x, Camera.main.transform.localEulerAngles.y, Camera.main.transform.localEulerAngles.z));
            go.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = Materialzz[0];

        }
        if (count == 1)
        {
            GameObject go = Instantiate(gameobjecttoinstantiate, new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z), Quaternion.Euler(Camera.main.transform.localEulerAngles.x, Camera.main.transform.localEulerAngles.y, Camera.main.transform.localEulerAngles.z));
            go.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = Materialzz[1];
        }
        if (count == 2)
        {
            GameObject go = Instantiate(gameobjecttoinstantiate, Camera.main.transform.position, Quaternion.Euler(Camera.main.transform.localEulerAngles.x, Camera.main.transform.localEulerAngles.y, Camera.main.transform.localEulerAngles.z));
            go.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = Materialzz[2];
        }
        if (count == 3)
        {
            GameObject go = Instantiate(gameobjecttoinstantiate, Camera.main.transform.position, Quaternion.Euler(Camera.main.transform.localEulerAngles.x, Camera.main.transform.localEulerAngles.y, Camera.main.transform.localEulerAngles.z));
            go.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = Materialzz[3];
        }
        if (count == 4)
        {
            GameObject go = Instantiate(gameobjecttoinstantiate, Camera.main.transform.position, Quaternion.Euler(Camera.main.transform.localEulerAngles.x, Camera.main.transform.localEulerAngles.y, Camera.main.transform.localEulerAngles.z));
            go.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = Materialzz[4];
        }
        if (count == 5)
        {
            GameObject go = Instantiate(gameobjecttoinstantiate, Camera.main.transform.position, Quaternion.Euler(Camera.main.transform.localEulerAngles.x, Camera.main.transform.localEulerAngles.y, Camera.main.transform.localEulerAngles.z));
            go.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = Materialzz[5];
        }
        if (count == 6)
        {
            GameObject go = Instantiate(gameobjecttoinstantiate, Camera.main.transform.position, Quaternion.Euler(Camera.main.transform.localEulerAngles.x, Camera.main.transform.localEulerAngles.y, Camera.main.transform.localEulerAngles.z));
            go.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = Materialzz[6];
        }
        if (count == 7)
        {
            GameObject go = Instantiate(gameobjecttoinstantiate, Camera.main.transform.position, Quaternion.Euler(Camera.main.transform.localEulerAngles.x, Camera.main.transform.localEulerAngles.y, Camera.main.transform.localEulerAngles.z));
            go.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = Materialzz[7];
        }
        if (count == 8)
        {
            GameObject go = Instantiate(gameobjecttoinstantiate, Camera.main.transform.position, Quaternion.Euler(Camera.main.transform.localEulerAngles.x, Camera.main.transform.localEulerAngles.y, Camera.main.transform.localEulerAngles.z));
            go.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = Materialzz[8];
        }
        if (count == 9)
        {
            GameObject go = Instantiate(gameobjecttoinstantiate, Camera.main.transform.position, Quaternion.Euler(Camera.main.transform.localEulerAngles.x, Camera.main.transform.localEulerAngles.y, Camera.main.transform.localEulerAngles.z));
            go.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = Materialzz[9];
        }
        if (count == 10)
        {
            GameObject go = Instantiate(gameobjecttoinstantiate, Camera.main.transform.position, Quaternion.Euler(Camera.main.transform.localEulerAngles.x, Camera.main.transform.localEulerAngles.y, Camera.main.transform.localEulerAngles.z));
            go.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = Materialzz[10];
        }
        if (count == 11)
        {
            GameObject go = Instantiate(gameobjecttoinstantiate, Camera.main.transform.position, Quaternion.Euler(Camera.main.transform.localEulerAngles.x, Camera.main.transform.localEulerAngles.y, Camera.main.transform.localEulerAngles.z));
            go.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = Materialzz[11];
        }
        if (count == 12)
        {
            GameObject go = Instantiate(gameobjecttoinstantiate, Camera.main.transform.position, Quaternion.Euler(Camera.main.transform.localEulerAngles.x, Camera.main.transform.localEulerAngles.y, Camera.main.transform.localEulerAngles.z));
            go.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = Materialzz[12];
        }
        if (count == 13)
        {
            GameObject go = Instantiate(gameobjecttoinstantiate, Camera.main.transform.position, Quaternion.Euler(Camera.main.transform.localEulerAngles.x, Camera.main.transform.localEulerAngles.y, Camera.main.transform.localEulerAngles.z));
            go.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = Materialzz[13];
        }
        if (count == 14)
        {
            GameObject go = Instantiate(gameobjecttoinstantiate, Camera.main.transform.position, Quaternion.Euler(Camera.main.transform.localEulerAngles.x, Camera.main.transform.localEulerAngles.y, Camera.main.transform.localEulerAngles.z));
            go.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = Materialzz[14];
        }
        if (count == 15)
        {
            GameObject go = Instantiate(gameobjecttoinstantiate, Camera.main.transform.position, Quaternion.Euler(Camera.main.transform.localEulerAngles.x, Camera.main.transform.localEulerAngles.y, Camera.main.transform.localEulerAngles.z));
            go.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = Materialzz[15];
        }
        if (count == 16)
        {
            GameObject go = Instantiate(gameobjecttoinstantiate, Camera.main.transform.position, Quaternion.Euler(Camera.main.transform.localEulerAngles.x, Camera.main.transform.localEulerAngles.y, Camera.main.transform.localEulerAngles.z));
            go.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = Materialzz[16];
        }
        if (count == 17)
        {
            GameObject go = Instantiate(gameobjecttoinstantiate, Camera.main.transform.position, Quaternion.Euler(Camera.main.transform.localEulerAngles.x, Camera.main.transform.localEulerAngles.y, Camera.main.transform.localEulerAngles.z));
            go.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = Materialzz[17];
        }
        if (count == 18)
        {
            GameObject go = Instantiate(gameobjecttoinstantiate, Camera.main.transform.position, Quaternion.Euler(Camera.main.transform.localEulerAngles.x, Camera.main.transform.localEulerAngles.y, Camera.main.transform.localEulerAngles.z));
            go.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = Materialzz[18];
        }
        if (count == 19)
        {
            GameObject go = Instantiate(gameobjecttoinstantiate, Camera.main.transform.position, Quaternion.Euler(Camera.main.transform.localEulerAngles.x, Camera.main.transform.localEulerAngles.y, Camera.main.transform.localEulerAngles.z));
            go.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = Materialzz[19];
        }
        count += 1;






        yield return tex;
        if (save == true)
        {
            string path = SaveImageToGallery(tex, "Name", "Description");

        }

        //
        watermark.SetActive(false);



        yield return new WaitForSeconds(0.5f);
        UIz.SetActive(true);




        //Debug.Log("Picture has been saved at:\n" + path);

    }

}
