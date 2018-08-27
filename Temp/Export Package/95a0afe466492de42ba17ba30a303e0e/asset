using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CamerasManager : MonoBehaviour
{

    public bool enableVideoSave;
    public bool enableDataSave;
    public bool limitTrajectories;
	private int maxTrajectories = 400;

    public string folder = "AcquisizioneVideo";
    public int frameRate = 25;

    public List<CameraType> cameras = new List<CameraType>();

    private GameObject[] persone;
    private GameObject[] targetsCamera;

    private string tagPerson = "Person";
    private string tagGroup = "Group";
    private string tagStationaryGroup = "StationaryGroup";
    private string sceneName;

    RenderTexture rt = null;
    Rect rect;
    Texture2D screenShot;
    byte[] bytes;
    string filename;

    // inizializzazione
    void Start()
    {
        sceneName = SceneManager.GetActiveScene().name;

        //UNCOMMENT HERE
        if (enableVideoSave)
        {
            Time.captureFramerate = frameRate;

            if (cameras.Count > 0)
            {
                // crea la cartella principale nella quale verranno create una sottocartella per ogni telecamera
                folder = folder + System.DateTime.Now.ToString("_yyyy-MM-dd_HH-mm-ss");
                if (System.IO.Directory.Exists(folder))
                {
                    System.IO.Directory.Delete(folder, true);
                }
                System.IO.Directory.CreateDirectory(folder);

                // crea una sottocartella per ogni telecamera
                for (int i = 0; i < cameras.Count; ++i)
                {
                    if (cameras[i].Camera)
                    {
                        cameras[i].Folder = string.Format("{0}/{1}_{2}x{3}_{4}fps",
                                                           folder,
                                                           cameras[i].Name,
                                                           cameras[i].Camera.rect.width,
                                                           cameras[i].Camera.rect.height,
                                                           frameRate);
                        System.IO.Directory.CreateDirectory(cameras[i].Folder);
                    }
                }
            }
        }
    }

    // funzione chiamata dopo Update()
    void LateUpdate()
    {
        if (!rt)
        {
            rt = new RenderTexture(1280, 720, 24);
            rect = new Rect(0, 0, 1280, 720);
            screenShot = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
        }
        if (enableVideoSave)
        {
            // per ogni telecamera
            for (int i = 0; i < cameras.Count; ++i)
            {
                if (cameras[i].Camera)
                {

                    //UNCOMMENT HERE FOR VIDEO SAVING
                    ////RenderTexture rt = new RenderTexture((int)cameras[i].Camera.rect.width, (int)cameras[i].Camera.rect.height, 24);
                    //cameras[i].Camera.targetTexture = rt;
                    ////rt.antiAliasing = 8;
                    ////Texture2D screenShot = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
                    //cameras[i].Camera.Render();
                    //RenderTexture.active = rt;
                    ////Rect rect = new Rect(0, 0, rt.width, rt.height);
                    //screenShot.ReadPixels(rect, 0, 0);
                    //cameras[i].Camera.targetTexture = null;
                    //RenderTexture.active = null; // JC: added to avoid errors
                    //Destroy(rt);
                    //bytes = screenShot.EncodeToPNG();
                    //filename = string.Format("{0}/frame_{1:D04}.png", cameras[i].Folder, Time.frameCount);
                    //System.IO.File.WriteAllBytes(filename, bytes);

                    if (enableDataSave)
                    {
                        string fileWorld = string.Format("{0}/../../Python/" + sceneName + "_" + "World" + System.DateTime.Now.ToString("_yyyy-MM-dd_HH") + ".txt", cameras[i].Folder);
                        //UNCOMMENT HERE
                        string fileHead = string.Format("{0}/../../Python/" + sceneName + "_" + cameras[i].Name + "_head" + System.DateTime.Now.ToString("_yyyy-MM-dd_HH") + ".txt", cameras[i].Folder);
                        string fileFeet = string.Format("{0}/../../Python/" + sceneName + "_" + cameras[i].Name + "_feet" + System.DateTime.Now.ToString("_yyyy-MM-dd_HH") + ".txt", cameras[i].Folder);



                        //      if (!System.IO.File.Exists(fileNameText)) {
                        //   //System.IO.File.WriteAllText(fileNameText, string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\n",
                        //   //                                                            "frame",
                        //   //                                                            "personID",
                        //   //                                                            "globalX",
                        //   //                                                            "globalY",
                        //   //                                                            "globalZ",
                        //   //                                                            "pixelX",
                        //   //                                                            "pixelY",
                        //   //                                                            // "globalDist",
                        //   //                                                            // "tag",
                        //   //                                                            "groupID"));
                        //}

                        persone = GameObject.FindGameObjectsWithTag(tagPerson);
                        foreach (GameObject person in persone)
                        {
                            //Debug.Log(person.name);
                            //Debug.Log(maxTrajectories.ToString());
                            //If a maximum number of trajectories has been reached
                            if (limitTrajectories && person.name.Contains(maxTrajectories.ToString()))
                            {
                                EditorApplication.isPlaying = false;
                            }
                            Transform targetCam = person.transform.Find("TargetCamera");
                            Transform head = person.transform.Find("Root");
                            while (head != null && head.gameObject.name != "Head")
                            {
                                if (head.gameObject.name == "LowerBack")
                                {
                                    head = head.GetChild(1);
                                }
                                else
                                {
                                    head = head.GetChild(0);
                                }
                                //Debug.Log(head.gameObject.name);
                            }
                            //Transform root = person.transform.Find("Root");
                            //Transform global = root.GetChild(0).Find("Global");

                            //foreach (Transform child in global)
                            //{
                            //    Debug.Log(global.name + " " + child.name);
                            //}
                            //Transform position = global.Find("Position");
                            //Transform hips = position.Find("Hips");
                            //Transform lower = hips.Find("LowerBack");
                            //Transform spine = lower.Find("Spine");
                            //Transform spine1 = spine.Find("Spine1");
                            //Transform neck = spine1.Find("Neck");
                            //Transform head = neck.Find("Head");
                            //Transform head = person.transform.Find("Root/Global/Position/Hips/LowerBack/Spine/Spine1/Neck/Head");


                            // To prevent tracking to happen when person hierarchy is not complete yet
                            if (head == null)
                            {
                                return;
                            }

                            //Camera cam = GameObject.Find("Camera_001").GetComponent<Camera>();
                            //cameraObjects[i] = GameObject.Find(cameras[i].Name).GetComponent<Camera>();
                            //Debug.Log(cameraObjects[i].pixelWidth + ", " + cameraObjects[i].pixelHeight);


                            Vector3 screenPos = cameras[i].Camera.WorldToScreenPoint(targetCam.position);
                            screenPos.y = cameras[i].Camera.pixelHeight - screenPos.y;
                            Vector3 screenPosHead = cameras[i].Camera.WorldToScreenPoint(head.position);
                            screenPosHead.y = cameras[i].Camera.pixelHeight - screenPosHead.y;

                            if (i == 2)
                            {
                                //Debug.Log("Frame: " + Time.frameCount + ", " + cameras[i].Name + screenPos);
                            }
                            //screenPos.x = screenPos.x * cameras[i].Camera.rect.width;
                            //screenPos.y = cameras[i].Camera.rect.height - (screenPos.y * cameras[i].Camera.rect.height);
                            //screenPosHead.x = screenPosHead.x * cameras[i].Camera.rect.width;
                            //screenPosHead.y = cameras[i].Camera.rect.height - (screenPosHead.y * cameras[i].Camera.rect.height);


                            // Frame; PersonID; FeetX; FeetY; FeetZ; HeadX; HeadY; HeadZ; ScreenX; ScreenY; GroupID
                            if (person.transform.parent.CompareTag(tagGroup))
                            {
                                // Create just one world coordinate file
                                if (i == 0)
                                {
                                    System.IO.File.AppendAllText(fileWorld, string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\n",
                                       Time.frameCount + 15, person.GetComponent<Person>().PersonID,
                                       targetCam.position.x, targetCam.position.y, targetCam.position.z,
                                       head.position.x, head.position.y, head.position.z,
                                       person.transform.parent.GetComponent<Group>().GroupID));
                                }
                                //UNCOMMENT HERE
                                System.IO.File.AppendAllText(fileHead, string.Format("{0}\t{1}\t{2}\t{3}\t{4}\n",
                                   Time.frameCount + 15, person.GetComponent<Person>().PersonID, screenPosHead.x, screenPosHead.y, person.transform.parent.GetComponent<Group>().GroupID));
                                System.IO.File.AppendAllText(fileFeet, string.Format("{0}\t{1}\t{2}\t{3}\t{4}\n",
                                   Time.frameCount + 15, person.GetComponent<Person>().PersonID, screenPos.x, screenPos.y, person.transform.parent.GetComponent<Group>().GroupID));

                            }
                            else if (person.transform.parent.CompareTag(tagStationaryGroup))
                            {
                                // Create just one world coordinate file
                                if (i == 0)
                                {
                                    System.IO.File.AppendAllText(fileWorld, string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\n",
                                   Time.frameCount + 15, person.GetComponent<Person>().PersonID,
                                   targetCam.position.x, targetCam.position.y, targetCam.position.z,
                                   head.position.x, head.position.y, head.position.z,
                                   person.transform.parent.GetComponent<Group>().GroupID));
                                }
                                //UNCOMMENT HERE
                                System.IO.File.AppendAllText(fileHead, string.Format("{0}\t{1}\t{2}\t{3}\t{4}\n",
                                   Time.frameCount + 15, person.GetComponent<Person>().PersonID, screenPosHead.x, screenPosHead.y, person.transform.parent.GetComponent<Group>().GroupID));
                                System.IO.File.AppendAllText(fileFeet, string.Format("{0}\t{1}\t{2}\t{3}\t{4}\n",
                                   Time.frameCount + 15, person.GetComponent<Person>().PersonID, screenPos.x, screenPos.y, person.transform.parent.GetComponent<Group>().GroupID));

        }
                            else
                            {
                                // Create just one world coordinate file
                                if (i == 0)
                                {
                                    System.IO.File.AppendAllText(fileWorld, string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\n",
                                   Time.frameCount + 15, person.GetComponent<Person>().PersonID,
                                   targetCam.position.x, targetCam.position.y, targetCam.position.z,
                                   head.position.x, head.position.y, head.position.z,
                                   "0"));
                                }
                                //UNCOMMENT HERE
                                System.IO.File.AppendAllText(fileHead, string.Format("{0}\t{1}\t{2}\t{3}\t{4}\n",
                                   Time.frameCount + 15, person.GetComponent<Person>().PersonID, screenPosHead.x, screenPosHead.y, "0"));
                                System.IO.File.AppendAllText(fileFeet, string.Format("{0}\t{1}\t{2}\t{3}\t{4}\n",
                                   Time.frameCount + 15, person.GetComponent<Person>().PersonID, screenPos.x, screenPos.y, "0"));

                            }
                        }
                    }
                }
            }
        }
    }

    public static string GetGameObjectPath(GameObject obj)
    {
        string path = "/" + obj.name;
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            path = "/" + obj.name + path;
        }
        return path;
    }
}

[System.Serializable]
public class CameraType
{
    public string Name;
    public Camera Camera;
    public string Folder;

    public CameraType(string _Name)
    {
        Name = _Name;
    }
}
