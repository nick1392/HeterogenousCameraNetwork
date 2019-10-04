using UnityEngine;
using System.Collections;
using Unity.Collections;
using Unity.Jobs;

public class Person : MonoBehaviour
{

    public int PersonID;

    public float stationaryForce = 2f;

    public Transform target;            //destinazione
    public bool destroyAtDestinantion = true;
    public float desiredSpeed = 1.3f;   //velocita che viene raggiunta in caso di assenza di altre forze esterne
    public float relaxationTime = 0.5f; //definisce la velocità con cui viene raggiunta la velocità desiderata

    public float d1 = 0.5f;
    public float d2 = 1.0f;
    public float d3 = 4.0f;

    public float f1 = 10.0f;
    public float f2 = 2.0f;
    public float f3 = 0.5f;

    public float attractionForce = 1.5f;
    public float angleOfVision = 90.0f;
    public float maxHeadRotation = 45.0f;
    public float socialInteractions = 2.0f;

    private Vector3 totalForce;      //somma delle forze che agiscono sulla pesrona
    private Vector3 walkingVector;   //direzione e velocità con coui si sta muovendo la persona
    private float gazingDirection;   //angolo espresso in gradi rispetto alla direzione di walkingVector

    private Animator animator;                                  //gestore delle animazioni
    private int speedHash = Animator.StringToHash("Speed");     //nome del parametro che imposta la velocità dell'animazione
    private int offsetHash = Animator.StringToHash("Offset");   //nome del parametro che imposta l'offset dell'animazione
    private int rightLeftHash = Animator.StringToHash("RightLeft");


    private string tagPerson = "Person";   //tag che identifica una persona 
    private string tagGroup = "Group";     //tag che identifica un gruppo
    private string tagStationaryGroup = "StationaryGroup";     //tag che identifica un gruppo

    private CamerasManager videoManager;

    // Elementi riaggiunti perchè eliminati, ma necessari!!!!

    private Vector3 desiredDirection;
    //private VideoManager videoManager;

    //______________________________________________
    //	        OGJ INTERACTION START
    //public ObjInteraction objInteraction;
    private float[] escapeDistance;
    private float[] escapeGradient;
    private Vector3[] eyeRayDirections;
    private Vector3 heightEye1, heightEye2, heightEye3;

    public bool Interaction = true;
    public bool DebugOption;

    public float objRange1 = 0.4f;
    public float objRange2 = 2f;
    public float objRange3 = 4f;

    public float objForce1 = 10f;
    public float objForce2 = 2.0f;
    public float objForce3 = 0.06f;

    public int numAngleDivisions = 200;
    public float height1 = 0.1f;
    public float height2 = 1f;
    public float height3 = 2f;

    public float escapeRange = 15f;
    public float escapeForce = 7.0f;
    public float escapeThreshold = 0.5f;

    public LayerMask objectsLayerMask = 9;
    //__________OGJ_INTERACTION_END_________________

    
    //__________UI_START_________________


   
    public Rect windowRect;
    public Rect closeRect;
    public bool clickMe = false;
    public bool justClicked = false;
    public Transform selection;

    //__________UI_END_________________

    // Use this for initialization
    void Start()
    {
        PersonID = IDGenerator.getNewPersonID();
        gameObject.name = gameObject.name + string.Format("_{0:D06}", PersonID);
        GameObject TargetCamera = new GameObject("TargetCamera");
        TargetCamera.transform.parent = transform;
        TargetCamera.transform.localPosition = Vector3.zero;
        TargetCamera.transform.localRotation = Quaternion.identity;
        transform.tag = tagPerson;
        walkingVector = Vector3.zero;
        gazingDirection = 0.0f;
        videoManager = GameObject.Find("CamerasManager").GetComponent<CamerasManager>();

        //______________________________________________
        //	        OGJ INTERACTION START
        float angleIncrement = 360f / numAngleDivisions;
        eyeRayDirections = new Vector3[numAngleDivisions];
        eyeRayDirections[0] = new Vector3(1f, 0f, 0f);
        //Pre-salva le direzioni di analisi per diminuire il carico di elaborazione dopo;
        for (int i = 1; i < numAngleDivisions; i++)
        {
            eyeRayDirections[i] = Quaternion.Euler(0, -angleIncrement * i, 0) * eyeRayDirections[0];
        }
        heightEye1 = new Vector3(0f, height1, 0f);
        heightEye2 = new Vector3(0f, height2, 0f);
        heightEye3 = new Vector3(0f, height3, 0f);

        //__________OGJ_INTERACTION_END_________________

        windowRect = new Rect(20, 20, 140, 100);
        closeRect = new Rect(20, 20, 140, 100);
        selection = transform.Find("New Sprite(Clone)");
        selection.gameObject.SetActive(false);
        PersonCollection.Instance.People.Add(gameObject);
    }


    // Update is called once per frame
    void Update()
    {
        if (!animator)
        {
            animator = GetComponent<Animator>();
            if (animator)
                animator.SetFloat(offsetHash, Random.value);
        }

        totalForce = Vector3.zero;

        /*******************************
         * FORZA DI GRUPPO STAZIONARIO *
         *******************************/
        if (transform.parent.CompareTag(tagStationaryGroup))
        {
            totalForce -= (walkingVector * stationaryForce);
        }

        /******************************************
         * FORZA DI ATTRAZIONE VERSO DESTINAZIONE *
         ******************************************/
        else
        {
            desiredDirection = target.position - transform.position;
            desiredDirection.y = 0;
            desiredDirection = desiredDirection.normalized * desiredSpeed;

            totalForce += ((desiredDirection - walkingVector) / relaxationTime);
            //Debug.DrawLine(transform.position, target.position, Color.green);  // destinazione
        }

        /***********************************
         * FORZE DI REPULSIONE TRA PERSONE *
         ***********************************/
        GameObject[] people = PersonCollection.Instance.People.ToArray();

        foreach (GameObject person in people)
        {
            if (!person.Equals(gameObject))
            {
                Vector3 directionOtherPerson = person.transform.position - transform.position;
                directionOtherPerson.y = 0;

                float distance = directionOtherPerson.magnitude;
                float strength = 0;

                /**********************************************
                 * SE LA PERSONA FA PARTE DELLO STESSO GRUPPO *
                 **********************************************/
                if ((transform.parent.tag.Equals(tagGroup) || transform.parent.tag.Equals(tagStationaryGroup)) && (isInTheSameGroup(person.transform)))
                {
                    if (distance <= d2 && distance > d1)
                    {
                        float m = (f2 - f3) / (d1 - d2);
                        float t = f3 - m * d2;
                        strength = m * distance + t;
                    }
                    else if (distance <= d1)
                    {
                        float m = (f2 - f1) / (d1);
                        float t = f1;
                        strength = m * distance + t;
                    }
                }
                /*****************************************
                 * SE LA PERSONA NON FA PARTE DEL GRUPPO *
                 *****************************************/
                else
                {
                    if (distance <= d3 && distance > d2)
                    {
                        float m = (f3 - 0) / (d2 - d3);
                        float t = 0 - m * d3;
                        strength = m * distance + t;
                    }
                    else if (distance <= d2 && distance > d1)
                    {
                        float m = (f2 - f3) / (d1 - d2);
                        float t = f3 - m * d2;
                        strength = m * distance + t;
                    }
                    else if (distance <= d1)
                    {
                        float m = (f2 - f1) / (d1);
                        float t = f1;
                        strength = m * distance + t;
                    }
                }

                totalForce -= (directionOtherPerson.normalized * strength);
            }
        }


        /********************
         * FORZE DEL GRUPPO *
         ********************/
        if (transform.parent.tag.Equals(tagGroup))
        {
            Vector3 groupBarycenter = getBarycenterOtherPeople(transform);
            Vector3 barycenterGroupDirection = groupBarycenter - transform.position;

            /**********************
            * FORZA DI ATTRAZIONE *
            ***********************/
            barycenterGroupDirection.y = 0.0f;
            if (barycenterGroupDirection.magnitude > ((transform.parent.childCount - 1) / 2f))
            {
                totalForce += (barycenterGroupDirection.normalized * attractionForce);
                //Debug.DrawLine(transform.position, groupBarycenter, Color.cyan);  // mostra una linea verso il baricentro del gruppo quando viene applicata la forza di attrazione
            }

            /*******************
            * FORZA DI VISIONE *
            ********************/
            float angleGroupBarycenter = Vector3.Angle(transform.forward, barycenterGroupDirection);

            if (angleGroupBarycenter > angleOfVision)
            {
                gazingDirection = (angleGroupBarycenter - angleOfVision);
            }
            else
            {
                gazingDirection = 0.0f;
            }

            if (gazingDirection > maxHeadRotation)
            {
                totalForce -= (socialInteractions * (gazingDirection / 180.0f) * walkingVector);
            }

            gazingDirection *= Mathf.Sign(Vector3.Dot(transform.up, Vector3.Cross(transform.forward, barycenterGroupDirection)));
            //Debug.DrawLine(transform.position, groupBarycenter, Color.blue);  // direzione baricentro del gruppo

        }

        //______________________________________________
        //	        OGJ INTERACTION START


        if (Interaction)
        {
            RaycastHit hit;
            float[] hitDistance = new float[3];
            float minHitDistance;
            bool validHit = false;

            foreach (Vector3 eyeDir in eyeRayDirections)
            {
                validHit = false;
                hitDistance[0] = 100.0f;
                hitDistance[1] = 100.0f;
                hitDistance[2] = 100.0f;

                if (Physics.Raycast(gameObject.transform.position + heightEye1, eyeDir, out hit, objRange3, objectsLayerMask))
                {
                    hitDistance[0] = hit.distance;
                    if (hit.collider.CompareTag("Oggetti"))
                    {
                        validHit = true;
                    }
                    //Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + eyeDir*hit.distance, Color.blue);
                }
                if (Physics.Raycast(gameObject.transform.position + heightEye2, eyeDir, out hit, objRange3, objectsLayerMask))
                {
                    //converto in coordinate 2D

                    hitDistance[1] = hit.distance;
                    if (hit.collider.CompareTag("Oggetti"))
                    {
                        validHit = true;
                    }
                    //Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + eyeDir*hit.distance, Color.blue);
                }
                if (Physics.Raycast(gameObject.transform.position + heightEye3, eyeDir, out hit, objRange3, objectsLayerMask))
                {
                    hitDistance[2] = hit.distance;
                    if (hit.collider.CompareTag("Oggetti"))
                    {
                        validHit = true;
                    }
                    //Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + eyeDir*hit.distance, Color.blue);
                }
                minHitDistance = Mathf.Min(hitDistance);
                if (validHit == true)
                {   // Se colpisce un oggetto
                    float strength = 0;
                    float m, t = 0;

                    if (minHitDistance < objRange1)
                    { // Se nel range 1
                        m = (objForce2 - objForce1) / (objRange1);
                        t = objForce1;
                        strength = m * minHitDistance + t;
                        if (DebugOption || clickMe)
                        {
                            DrawLine(gameObject.transform.position, gameObject.transform.position + eyeDir * minHitDistance, Color.red);
                        }
                    }
                    else if (minHitDistance < objRange2)
                    { // Se nel range 2
                        m = (objForce2 - objForce3) / (objRange1 - objRange2);
                        t = objForce3 - m * objRange2;
                        strength = m * minHitDistance + t;
                        if (DebugOption || clickMe)
                        {
                            DrawLine(gameObject.transform.position, gameObject.transform.position + eyeDir * minHitDistance, Color.yellow);
                        }
                    }
                    else
                    { // Se nel range 3
                        m = (objForce3 - 0) / (objRange2 - objRange3);
                        t = 0 - m * objRange3;
                        strength = m * minHitDistance + t;
                        if (DebugOption || clickMe)
                        {
                            DrawLine(gameObject.transform.position, gameObject.transform.position + eyeDir * minHitDistance, Color.green);
                        }
                    }
                    totalForce -= (eyeDir * strength);

                }
            }




            //__________OGJ_INTERACTION_END_________________

            //__________ELABORAZIONE DELLE "OPZIONI DI FUGA"  laterale_________

            //if (totalForce.magnitude <= escapeThreshold) {
            //	totalForce += ((Quaternion.Euler(0, 90, 0)*desiredDirection).normalized * 5);
            //}


            //_________FINE OPZIONI DI FUGA laterale__________________________

            //__________ELABORAZIONE DELLE "OPZIONI DI FUGA" ricerca via libera_________

            RaycastHit hitRay;
            Vector3 escapeDirectionOK = Vector3.zero;
            if (target != null)
            {
                desiredDirection = target.position - transform.position;
                desiredDirection.y = 0;
            }
            else
            {
                desiredDirection = transform.position;
            }
            Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + desiredDirection.normalized * 10, Color.yellow);
            if (Physics.Raycast(gameObject.transform.position + heightEye2, desiredDirection, out hitRay, escapeRange, objectsLayerMask))
            {
                if (hitRay.collider.CompareTag("Oggetti"))
                {
                    // Trova una direzione il cui gradiente rispetto all'angolo di visuale sia abbastanza elevato da essere considerato come una via per schivare l'oggetto.

                    float deltaAngle = 360f / numAngleDivisions;
                    Vector3[] escapeDirections = new Vector3[numAngleDivisions];
                    escapeDistance = new float[numAngleDivisions];
                    escapeGradient = new float[numAngleDivisions];
                    Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + desiredDirection.normalized * 10, Color.red);
                    escapeDirections[0] = Quaternion.Euler(0, -180, 0) * desiredDirection;
                    for (int i = 1; i < (numAngleDivisions); i++)
                    {
                        escapeDirections[i] = Quaternion.Euler(0, +deltaAngle * i, 0) * escapeDirections[0];
                    }
                    // Il vettore escapeDirections contiene la rosa di direzioni da analizzare.
                    
                    var results = new NativeArray<RaycastHit>(numAngleDivisions, Allocator.Temp);
                    var commands = new NativeArray<RaycastCommand>(numAngleDivisions, Allocator.Temp);

                    // Set the data of the first command
                    Vector3 origin = Vector3.forward * -10;
                    Vector3 direction = Vector3.forward;

                    commands[0] = new RaycastCommand(origin, direction);

                    // Schedule the batch of raycasts

                    // Wait for the batch processing job to complete

                    // Copy the result. If batchedHit.collider is null there was no hit
                    RaycastHit batchedHit = results[0];

                    // Dispose the buffers

                    for (int i = 0; i < escapeDirections.Length; i++)
                    {
                        commands[i] = new RaycastCommand(gameObject.transform.position + heightEye2, escapeDirections[i], escapeRange);
                    }
                    var handle = RaycastCommand.ScheduleBatch(commands, results, 1);
                    handle.Complete();
                    for (int i = 0; i < escapeDirections.Length; i++)
                    {
                        if (results[i].collider)
                        {
                            if (results[i].collider.CompareTag("Oggetti"))
                                escapeDistance[i] = results[i].distance;
                            else
                                escapeDistance[i] = escapeRange;
                        }
                        else
                        {
                            escapeDistance[i] = escapeRange;
                        }
                    }
                    
//                    int pointer = 0;
//                    foreach (Vector3 dirTest in escapeDirections)
//                    {
//                        //DrawLine(gameObject.transform.position, gameObject.transform.position + dirTest.normalized * 3, Color.blue);
//
//                        if (Physics.Raycast(gameObject.transform.position + heightEye2, dirTest, out hitRay, escapeRange))
//                        {
//                            if (hitRay.collider.CompareTag("Oggetti"))
//                            {
//                                escapeDistance[pointer] = hitRay.distance;
//                            }
//                            else
//                            {
//                                escapeDistance[pointer] = escapeRange;
//                            }
//                        }
//                        else
//                        {
//                            escapeDistance[pointer] = escapeRange;
//                        }
//                        pointer++;
//                    }
                    
                    results.Dispose();
                    commands.Dispose();
                    // Calcola il gradiente:
                    for (int i = 0; i < (numAngleDivisions - 1); ++i)
                    {
                        escapeGradient[i] = Mathf.Abs(escapeDistance[i + 1] - escapeDistance[i]);
                    }
                    bool check = false;
                    bool work = true;
                    int startAngle = (int)(numAngleDivisions / 2);
                    int minAngle = startAngle;
                    int maxAngle = startAngle;
                    int outputAngle = 0;
                    while (work)
                    {
                        if (Mathf.Abs(escapeGradient[minAngle]) >= escapeThreshold)
                        {
                            work = false;
                            check = true;
                            outputAngle = minAngle - 5;
                        }
                        else if (Mathf.Abs(escapeGradient[maxAngle]) >= escapeThreshold)
                        {
                            work = false;
                            check = true;
                            outputAngle = maxAngle + 5;
                        }
                        minAngle--;
                        maxAngle++;
                        if ((minAngle <= 1) || (maxAngle >= (numAngleDivisions - 1)))
                        {
                            //Debug.Log ("Non ho trovato nessuna via di fuga");
                            work = false;
                        }
                    }
                    if (check)
                    {
                        // identifica la direzione di fuga
                        escapeDirectionOK = escapeDirections[outputAngle];
                        // Applica la forza di deviazione
                        
                        if (clickMe) DrawLine(gameObject.transform.position, gameObject.transform.position + escapeDirectionOK.normalized * hitRay.distance, Color.blue);
                        totalForce += escapeDirectionOK.normalized * escapeForce;
                    }
                }
            }
        }
        /*

		foreach (Vector3 eyeDir in eyeRayDirections) {
			if (Physics.Raycast (gameObject.transform.position + heightEye2, eyeDir, out hit,  escapeRange)) {
				hitDistance [0] = Mathf.Sqrt (hit.distance * hit.distance +  height1 *  height1);
				if (hit.collider.tag == "Oggetti") {
					validHit = true;
				}
				//Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + eyeDir*hit.distance, Color.blue);
			}


		}
*/
        //_________FINE OPZIONI DI FUGA ricerca via libera__________________________



        /**************************************
         * CONVERSIONE DA FORZA A SPOSTAMENTO *
         **************************************/
        float sqrSpeedTreshold = 0.4f;
        if (totalForce.magnitude > 20)
        {
            //Debug.Log(totalForce.magnitude);
            totalForce = totalForce.normalized;
        }

        if (transform.parent.CompareTag(tagStationaryGroup))
        {
            float deltaTime;
            if (videoManager.enableVideoSave)
            {
                deltaTime = 1.0f / videoManager.frameRate;
            }
            else
            {
                deltaTime = Time.deltaTime;
            }

            walkingVector = walkingVector + deltaTime * totalForce;
            transform.Translate(walkingVector * deltaTime, Space.World);

            if (walkingVector.sqrMagnitude > sqrSpeedTreshold)
            {
                transform.LookAt(transform.position + walkingVector);
            }
        }
        else
        {
            if ((target.position - transform.position).magnitude > 1f)
            {
                float deltaTime;
                if (videoManager.enableVideoSave)
                {
                    deltaTime = 1.0f / videoManager.frameRate;
                }
                else
                {
                    deltaTime = Time.deltaTime;
                }

                walkingVector = walkingVector + deltaTime * totalForce;
                transform.Translate(walkingVector * deltaTime, Space.World);
                transform.LookAt(transform.position + walkingVector);

                if (walkingVector.sqrMagnitude > sqrSpeedTreshold)
                {
                    transform.LookAt(transform.position + walkingVector);
                }
            }
            else
            {
                if (destroyAtDestinantion)
                {
                    if (transform.parent.childCount == 1 && transform.parent.CompareTag(tagGroup))
                    {
                        PersonCollection.Instance.People.Remove(transform.parent.gameObject);
                        Destroy(transform.parent.gameObject);
                    }
                    else
                    {
                        PersonCollection.Instance.People.Remove(gameObject);
                        Destroy(gameObject);
                    }
                }
                else
                {
                    walkingVector = Vector3.zero;
                }
            }
        }
        //Debug.DrawLine(transform.position, transform.position + walkingVector, Color.red);   // direzione di spostamento, la lunghezza rappresenta la velocità

        if(walkingVector.magnitude > 3)
        {
            walkingVector.Normalize();
            walkingVector = 0.5f * walkingVector;
            //walkingVector = new Vector3 (walkingVector)* 0.1;
        }

        //Debug.Log(walkingVector.magnitude);


        /***********************
         * GESTIONE ANIMAZIONE *
         ***********************/
        if (animator != null)
        {
            if (walkingVector.sqrMagnitude > sqrSpeedTreshold && !transform.parent.CompareTag(tagStationaryGroup))
            {
                animator.SetFloat(speedHash, walkingVector.magnitude);
                animator.SetFloat(rightLeftHash, 0.0f);
            }
            else
            {
                //animator.SetFloat(speedHash, walkingVector.x);
                //animator.SetFloat(rightLeftHash, walkingVector.z);
                animator.SetFloat(speedHash, 0.0f);
                animator.SetFloat(rightLeftHash, 0.0f);
            }
        }
    }

    private void LateUpdate()
    {
        justClicked = false;
    }


    public bool isInTheSameGroup(Transform person)
    {
        foreach (Transform groupMember in transform.parent)
        {
            if (person.Equals(groupMember))
            {
                return true;
            }
        }
        return false;
    }

    public Vector3 getBarycenterOtherPeople(Transform person)
    {
        Vector3 barycenter = Vector3.zero;
        foreach (Transform groupMember in transform.parent)
        {
            if (groupMember != person)
            {
                barycenter += groupMember.position;
            }
        }
        barycenter = (barycenter / (transform.parent.childCount - 1));
        barycenter.y = 0;
        return barycenter;
    }


    void OnDrawGizmos()
    {
        if (transform.parent.CompareTag(tagStationaryGroup))
        {
            Vector3 v1 = (transform.forward - transform.right) / 2 + transform.position;
            Vector3 v2 = (transform.forward + transform.right) / 2 + transform.position;
            Vector3 v3 = (-transform.forward + transform.right) / 2 + transform.position;
            Vector3 v4 = (-transform.forward - transform.right) / 2 + transform.position;

            Gizmos.color = Color.green;
            Gizmos.DrawLine(v1, v2);
            Gizmos.DrawLine(v2, v3);
            Gizmos.DrawLine(v3, v4);
            Gizmos.DrawLine(v4, v1);
        }
    }




    private void OnMouseDown()
    {
        clickMe = true;
        justClicked = true;
    }

    void OnGUI()
    {
        if(clickMe)
        {
            // Vector2 pos = Camera.main.WorldToScreenPoint(transform.position);
            // windowRect.position = new Vector2(pos.x, Screen.height - pos.y);
            windowRect.position = new Vector2( Screen.width / 2f, Screen.height - 110);
            windowRect = GUI.Window(0, windowRect, DoMyWindow, new GUIContent("Person ID " + PersonID.ToString() + "\n Velocity " + walkingVector.magnitude.ToString("F3") + "m/s"));
            selection.gameObject.SetActive(true);
        }
        
    }

    void DoMyWindow(int windowID)
    {
        if (GUI.Button(new Rect(20, 60, 100, 40), "Stationary \n Behavior"))
        {
            transform.SetParent(GameObject.Find("StationaryGroup_000001").transform);
            animator.SetFloat(speedHash, 0.0f);
            animator.SetFloat(rightLeftHash, 0.0f);
            this.enabled = false;
        }

    }

    void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        LineRenderer lr = myLine.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.SetColors(color, color);
        lr.SetWidth(0.2f, 0.2f);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        GameObject.Destroy(myLine, duration);
    }


}
