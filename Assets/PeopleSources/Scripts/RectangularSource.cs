using UnityEngine;
using System.Collections.Generic;
using UMA;

public class RectangularSource : MonoBehaviour
{

    public UMAGeneratorBase generator;
    public SlotLibrary slotLibrarry;
    public OverlayLibrary overlayLibrary;
    public RaceLibrary raceLibrary;
    public RuntimeAnimatorController animController;

    public GameObject selection;

    private int numberOfSlots = 15;
    private UMADynamicAvatar umaDynamicAvatar;
    private UMAData umaData;
    private UMADnaHumanoid umaDna;
    private UMADnaTutorial umaTutorialDNA;

    public float var = 0.83f;
    public float[] groupSizeProbability;
    public float minTime = 0.5f, maxTime = 2.0f;

    public List<TargetType> targetList = new List<TargetType>();
    private int frameRate;

    private Transform peopleTransform;

    // Use this for initialization
    void Start()
    {
        // Poisson distribution
        float coef = Mathf.Pow(2.718281828f, -var);
        groupSizeProbability = new float[6];
        groupSizeProbability[0] = coef * Mathf.Pow(var, 1) / (1 * (1 - coef));
        groupSizeProbability[1] = coef * Mathf.Pow(var, 2) / (2 * (1 - coef));
        groupSizeProbability[2] = coef * Mathf.Pow(var, 3) / (6 * (1 - coef));
        groupSizeProbability[3] = coef * Mathf.Pow(var, 4) / (24 * (1 - coef));
        groupSizeProbability[4] = coef * Mathf.Pow(var, 5) / (120 * (1 - coef));
        groupSizeProbability[5] = coef * Mathf.Pow(var, 6) / (720 * (1 - coef));
        frameRate = GameObject.Find("CamerasManager").GetComponent<CamerasManager>().frameRate;
        peopleTransform = GameObject.Find("People").transform;
    }

    // Update is called once per frame
    private int nextGeneration = 0;
    void Update()
    {
        nextGeneration--;
        if (nextGeneration < 0 && targetList.Count > 0)
        {
            nextGeneration = (int)Random.Range(minTime * frameRate, maxTime * frameRate);

            float randomTarget = Random.value;
            float probability = 0.0f;
            Transform target = targetList[targetList.Count - 1].target;
            bool destroy = targetList[targetList.Count - 1].destroyAtDestination;
            for (int i = 0; i < (targetList.Count - 1); ++i)
            {
                probability += targetList[i].probability;
                if (randomTarget < probability)
                {
                    target = targetList[i].target;
                    destroy = targetList[i].destroyAtDestination;
                    break;
                }
            }
            //Debug.Log("target: " + target);


            float groupSize = Random.value;
            probability = 0.0f;
            Vector3 GenerationPoint = (Random.Range(-0.5f, 0.5f) * transform.localScale.x * transform.right + Random.Range(-0.5f, 0.5f) * transform.localScale.z * transform.forward) + transform.position;


			//uncomment to generate groups(also uncomment the break below)

            //for (int i = 0; i < 6; ++i)
            {
				int i = 0;
                probability += groupSizeProbability[i];
                //if (groupSize < probability)
                {
                    if (i == 0)
                    {
                        GameObject person = new GameObject("Person");
                        person.transform.SetParent(peopleTransform);
                        person.transform.position = GenerationPoint;
                        Person script = person.AddComponent<Person>();
                        script.target = target;
                        script.destroyAtDestinantion = destroy;
                        CapsuleCollider personCollider = person.AddComponent<CapsuleCollider>();
                        personCollider.center = new Vector3(0, 1, 0);
                        personCollider.direction = 1;
                        personCollider.height = 2.184f;
                        personCollider.radius = 0.5f;
                        GameObject sel = Instantiate<GameObject>(selection, person.transform, true);
                        sel.transform.position = new Vector3(GenerationPoint.x, 0.01f, GenerationPoint.z);
                        GenerateUMA(person);
                    }
                    else
                    {
                        GameObject Group = new GameObject(string.Format("Group_{0}p", i + 1));
                        Group.transform.parent = peopleTransform;
                        Group.tag = "Group";
                        Group.AddComponent<Group>();

                        for (int n = 0; n < (i + 1); ++n)
                        {
                            GameObject Person = new GameObject("Person");
                            Person.transform.parent = Group.transform;
                            Person.transform.position = GenerationPoint;
                            Person script = Person.AddComponent<Person>();
                            script.target = target;
                            script.destroyAtDestinantion = destroy;
                            CapsuleCollider personCollider = Person.AddComponent<CapsuleCollider>();
                            personCollider.center = new Vector3(0, 1, 0);
                            personCollider.direction = 1;
                            personCollider.height = 2.184f;
                            personCollider.radius = 0.5f;
                            GameObject sel = Instantiate<GameObject>(selection, Person.transform, true);
                            sel.transform.position = new Vector3(GenerationPoint.x, 0.01f, GenerationPoint.z);
                            GenerateUMA(Person);
                        }
                    }

                    //Debug.Log("pp" + (i+1));
                    //break;
                }
            }
        }
    }

    void GenerateUMA(GameObject Person)
    {
        // add UMA components to the game object
        umaDynamicAvatar = Person.AddComponent<UMADynamicAvatar>();

        // Initialize Avatar and grab a reference to it's data component
        umaDynamicAvatar.Initialize();
        umaData = umaDynamicAvatar.umaData;

        // Attach our generator
        umaDynamicAvatar.umaGenerator = generator;
        umaData.umaGenerator = generator;

        // Set up slot Array
        umaData.umaRecipe.slotDataList = new SlotData[numberOfSlots];

        // Set up our Morph references
        umaDna = new UMADnaHumanoid();
        umaTutorialDNA = new UMADnaTutorial();
        umaData.umaRecipe.AddDna(umaDna);
        umaData.umaRecipe.AddDna(umaTutorialDNA);

        if (Random.value < 0.5f)
        {
            CreateMale();
            umaDna.height = Random.Range(0.4f, 0.7f);
            SetBodyMass(Random.Range(0.1f, 0.8f));
        }
        else
        {
            CreateFemale();
            umaDna.height = Random.Range(0.4f, 0.7f);
            SetBodyMass(Random.Range(0.1f, 0.8f));
        }

        umaDynamicAvatar.animationController = animController;

        // Generate our UMA
        umaDynamicAvatar.UpdateNewRace();

    }

    void CreateMale()
    {
        UMAData.UMARecipe umaRecipe = umaDynamicAvatar.umaData.umaRecipe;
        umaRecipe.SetRace(raceLibrary.GetRace("HumanMale"));

        SetSlot(0, "MaleEyes");
        AddOverlay(0, "EyeOverlay");

        SetSlot(1, "MaleInnerMouth");
        AddOverlay(1, "InnerMouth");

        SetSlot(2, "MaleFace");
        AddOverlay(2, "MaleHead_PigNose");
        //if (Random.value < 0.5f)
        //{
        //    AddOverlay(2, "MaleHead01");
        //}
        //else
        //{
        //    AddOverlay(2, "MaleHead02");
        //}

        Color color = Color.Lerp(new Color(0.168f, 0.118f, 0.0f), new Color(0.522f, 0.306f, 0.0f), Random.value);
        if (Random.value < 0.1f)
        {
        }
        else
        {
            if (Random.value < 0.3f)
            {
                AddOverlay(2, "MaleHair01", color);
            }
            else
            {
                AddOverlay(2, "MaleHair02", color);
            }
        }

        switch ((int)Random.Range(0.0f, 4.0f - float.Epsilon))
        {
            case 0:
                break;
            case 1:
                AddOverlay(2, "MaleBeard01", color);
                break;
            case 2:
                AddOverlay(2, "MaleBeard02", color);
                break;
            case 3:
                AddOverlay(2, "MaleBeard03", color);
                break;
        }

        if (Random.value < 0.5f)
        {
            AddOverlay(2, "MaleEyebrow01", color);
        }
        else
        {
            AddOverlay(2, "MaleEyebrow02", color);
        }

        SetSlot(3, "MaleTorso");
        AddOverlay(3, "MaleBody02");
        AddOverlay(3, "MaleUnderwear01");
        AddOverlay(3, "MaleShirt01", Random.ColorHSV());

        SetSlot(4, "MaleHands");
        LinkOverlay(4, 3);

        SetSlot(5, "MaleJeans01");
        AddOverlay(5, "MaleJeans01", Random.ColorHSV());

        SetSlot(6, "MaleFeet");
        LinkOverlay(6, 3);

        umaData.isAtlasDirty = true;
        umaData.isMeshDirty = true;
        umaData.isShapeDirty = true;
        umaData.isTextureDirty = true;
        umaData.Dirty();
    }

    void CreateFemale()
    {
        UMAData.UMARecipe umaRecipe = umaDynamicAvatar.umaData.umaRecipe;
        umaRecipe.SetRace(raceLibrary.GetRace("HumanFemale"));

        SetSlot(0, "FemaleEyes");
        AddOverlay(0, "EyeOverlay");

        SetSlot(1, "FemaleInnerMouth");
        AddOverlay(1, "InnerMouth");


        Color color = Color.Lerp(new Color(0.168f, 0.118f, 0.184f), new Color(1.0f, 0.796f, 0.184f), Random.value);
        SetSlot(2, "FemaleFace");
        AddOverlay(2, "F_H_Head");
        AddOverlay(2, "FemaleEyebrow01", color);
        if (Random.value < 0.8f)
        {
            AddOverlay(2, "FemaleLipstick01", Color.Lerp(new Color(0.878f, 0.639f, 0.502f), new Color(0.878f, 0.0f, 0.502f), Random.value));
        }

        SetSlot(3, "FemaleTorso");
        AddOverlay(3, "FemaleBody01");
        AddOverlay(3, "FemaleUnderwear01", Random.ColorHSV());
        AddOverlay(3, "FemaleJeans01", Random.ColorHSV());


        switch ((int)Random.Range(0.0f, 3.0f - float.Epsilon))
        {
            case 0:
                AddOverlay(3, "FemaleShirt01", Random.ColorHSV());
                break;
            case 1:
                AddOverlay(3, "FemaleShirt02", Random.ColorHSV());
                break;
            case 2:
                SetSlot(9, "FemaleTshirt01");
                AddOverlay(9, "FemaleTshirt01", Random.ColorHSV());
                break;
        }

        SetSlot(4, "FemaleHands");
        LinkOverlay(4, 3);

        SetSlot(5, "FemaleLegs");
        LinkOverlay(5, 3);

        SetSlot(6, "FemaleFeet");
        LinkOverlay(6, 3);

        switch ((int)Random.Range(0.0f, 3.0f - float.Epsilon))
        {
            case 0:
                SetSlot(7, "FemaleLongHair01");
                AddOverlay(7, "FemaleLongHair01", color);
                break;
            case 1:
                SetSlot(7, "FemaleLongHair01");
                AddOverlay(7, "FemaleLongHair01", color);
                SetSlot(8, "FemaleLongHair01_Module");
                AddOverlay(8, "FemaleLongHair01_Module", color);
                break;
            case 2:
                SetSlot(7, "FemaleShortHair01");
                AddOverlay(7, "FemaleShortHair01", color);
                break;
        }

        umaData.isAtlasDirty = true;
        umaData.isMeshDirty = true;
        umaData.isShapeDirty = true;
        umaData.isTextureDirty = true;
        umaData.Dirty();
    }

    ////////// Overlay Helpers //////////

    void AddOverlay(int slot, string overlayName)
    {
        umaData.umaRecipe.slotDataList[slot].AddOverlay(overlayLibrary.InstantiateOverlay(overlayName));
    }

    void AddOverlay(int slot, string overlayName, Color color)
    {
        umaData.umaRecipe.slotDataList[slot].AddOverlay(overlayLibrary.InstantiateOverlay(overlayName, color));
    }

    void LinkOverlay(int slotNumber, int slotToLink)
    {
        umaData.umaRecipe.slotDataList[slotNumber].SetOverlayList(umaData.umaRecipe.slotDataList[slotToLink].GetOverlayList());
    }

    void RemoveOverlay(int slotNumber, string overlayName)
    {
        umaData.umaRecipe.slotDataList[slotNumber].RemoveOverlay(overlayName);
    }

    void ColorOverlay(int slotNumber, string overlayName, Color color)
    {
        umaData.umaRecipe.slotDataList[slotNumber].SetOverlayColor(color, overlayName);
    }

    ////////// Overlay Helpers //////////

    void SetSlot(int slotNumber, string SlotName)
    {
        umaData.umaRecipe.slotDataList[slotNumber] = slotLibrarry.InstantiateSlot(SlotName);
    }

    void RemoveSlot(int slotNumber)
    {
        umaData.umaRecipe.slotDataList[slotNumber] = null;
    }

    ////////// DNA Helpers //////////

    void SetBodyMass(float mass)
    {
        umaDna.upperMuscle = mass;
        umaDna.upperWeight = mass;
        umaDna.lowerMuscle = mass;
        umaDna.lowerWeight = mass;
        umaDna.armWidth = mass;
        umaDna.forearmWidth = mass;
    }

    ////////// Gizmos Editor //////////

    void OnDrawGizmos()
    {
        Vector3 v1 = (transform.forward * transform.localScale.z - transform.right * transform.localScale.x) / 2 + transform.position;
        Vector3 v2 = (transform.forward * transform.localScale.z + transform.right * transform.localScale.x) / 2 + transform.position;
        Vector3 v3 = (-transform.forward * transform.localScale.z + transform.right * transform.localScale.x) / 2 + transform.position;
        Vector3 v4 = (-transform.forward * transform.localScale.z - transform.right * transform.localScale.x) / 2 + transform.position;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(v1, v2);
        Gizmos.DrawLine(v2, v3);
        Gizmos.DrawLine(v3, v4);
        Gizmos.DrawLine(v4, v1);

        Gizmos.color = Color.blue;
        for (int i = 0; i < targetList.Count; ++i)
        {
            if (targetList[i].target != null)
            {
                v1 = (targetList[i].target.forward - targetList[i].target.right) / 2 + targetList[i].target.position;
                v2 = (targetList[i].target.forward + targetList[i].target.right) / 2 + targetList[i].target.position;
                v3 = (-targetList[i].target.forward + targetList[i].target.right) / 2 + targetList[i].target.position;
                v4 = (-targetList[i].target.forward - targetList[i].target.right) / 2 + targetList[i].target.position;

                Gizmos.DrawLine(v1, v2);
                Gizmos.DrawLine(v2, v3);
                Gizmos.DrawLine(v3, v4);
                Gizmos.DrawLine(v4, v1);
            }
        }
    }
}

[System.Serializable]
public class TargetType
{
    public Transform target;
    public float probability;
    public bool destroyAtDestination;
}
