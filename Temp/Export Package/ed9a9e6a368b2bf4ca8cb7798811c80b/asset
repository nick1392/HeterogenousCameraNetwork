using UnityEngine;
using System.Collections;

public class SourceController : MonoBehaviour {

    private bool active;


	// Use this for initialization
	void Start () {
        active = true;
	}

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RectangularSource[] pr = GetComponentsInChildren<RectangularSource>();
            if (active)
            {
                foreach (RectangularSource comp in pr)
                {
                    comp.enabled = false;
                }
                active = false;
            }
            else
            {
                foreach (RectangularSource comp in pr)
                {
                    comp.enabled = true;
                }
                active = true;
            }

        }
    }
}
