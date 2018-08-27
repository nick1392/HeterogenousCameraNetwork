using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SimController : MonoBehaviour
{

    public GameObject newWallButton;
    //public GameObject startSimulationButton;
    //public GameObject endSimulationButton;
    public GameObject people;
    //public GameObject item;
    public Camera cam1;
    public Camera cam2;
    public Camera cam3;
    public Camera cam4;

    private Person[] person;
    private Animator[] animator;
    private GameObject[] environment;

    private bool next;
    private bool odd;


    void Start()
    {
        SetControllerReferenceOnPeople();
        SetPeopleActive(false);
        //endSimulationButton.SetActive(false);
        cam1.enabled = true;
        cam2.enabled = false;
        cam3.enabled = false;
        cam4.enabled = false;

        next = false;
        odd = false;
    }

    private void SetControllerReferenceOnPeople()
    {
        person = people.GetComponentsInChildren<Person>();
        animator = people.GetComponentsInChildren<Animator>();
    }

    private void SetPeopleActive(bool toggle)
    {
        foreach (Person item in person)
        {
            item.enabled = toggle;
        }

        foreach (Animator item in animator)
        {
            item.enabled = toggle;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            cam1.enabled = true;
            cam2.enabled = false;
            cam3.enabled = false;
            cam4.enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            cam1.enabled = false;
            cam2.enabled = true;
            cam3.enabled = false;
            cam4.enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            cam1.enabled = false;
            cam2.enabled = false;
            cam3.enabled = true;
            cam4.enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            cam1.enabled = false;
            cam2.enabled = false;
            cam3.enabled = false;
            cam4.enabled = true;
        }

    }

    private void LateUpdate()
    {
        if(next)
        {
            person = people.GetComponentsInChildren<Person>();
            foreach (Person item in person)
            {
                if (!item.justClicked)
                {
                    item.clickMe = false;
                    item.selection.gameObject.SetActive(false);
                }

            }
            next = false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if(odd)
            {
                next = true;
                odd = false;
            }
            else
            {
                odd = true;
            }
            
        }

    }


    public void StartSimulation()
    {
        people.SetActive(true);
        //startSimulationButton.SetActive(false);
        newWallButton.SetActive(false);
        SetPeopleActive(true);
        //endSimulationButton.SetActive(true);
    }

    public void EndSimulation()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void CreateItem(GameObject item)
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;
        Physics.Raycast(ray, out hit);
        Vector3 initPosition = hit.point,cent;
        initPosition.y = 0.1f;
        Quaternion initRotation = new Quaternion(-0.707f, 0, 0, 0.707f);
        Instantiate(item, initPosition, initRotation);
    }

    public void DestroyItem()
    {
        //Destroy
    }

    public void ViewInteraction()
    {

    }

}