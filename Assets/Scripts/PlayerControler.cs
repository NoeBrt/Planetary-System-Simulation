using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
public class PlayerControler : MonoBehaviour
{
    Vector3 cameraDefaultPosition = new Vector3(0, 41, -174);
    Quaternion cameraDefaultRotation = Quaternion.Euler(13.83f, 0, 0);
    Transform selectedObject;
    [SerializeField] SolarSystemManager solarSystem;
    [SerializeField] UiObjectManager uiSpaceObjectManager;
    [SerializeField] float mouseScroolSpeed = 50.0f;
    float zoom;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        Camera.SetupCurrent(Camera.main);
        solarSystem = GameObject.Find("SolarSystemPlanets").GetComponent<SolarSystemManager>();
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        zoom += Input.mouseScrollDelta.y * Time.unscaledDeltaTime * mouseScroolSpeed;
        if (selectedObject != null)
        {
            focusMovement();
        }
        else
        {
            defaultPos();
        }
    }

    void focusMovement()
    {
        Vector3 relativePos = selectedObject.GetComponent<Orbit>().ObritTo.transform.position - transform.position;
        transform.position = new Vector3(selectedObject.position.x + cameraDefaultPosition.x, selectedObject.position.y, zoom + cameraDefaultPosition.z);
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        transform.rotation = rotation;
    }
    void defaultPos()
    {
        transform.position = cameraDefaultPosition + transform.forward * zoom;
        transform.rotation = cameraDefaultRotation;
    }


    void Update()
    {
        Debug.Log(Orbit.isFreeze);
        reloadScene();
        freezeScene();
        Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {

                if (Physics.Raycast(ray, out RaycastHit rayHit, Mathf.Infinity, LayerMask.GetMask("Object")))
                {
                    Select(rayHit.transform);

                }
                else
                {
                    quitOrbitEditor();
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }



    void quitOrbitEditor()
    {
        if (selectedObject != null)
        {
            selectedObject.GetComponent<Orbit>().showTrajectoryLine(false);
            uiSpaceObjectManager.SetVisible(false);
            Debug.Log(uiSpaceObjectManager.CurrentSpaceObject.name);
            if (uiSpaceObjectManager.CurrentSpaceObject.gameObject.GetComponent<Camera>() != null)
            {
                uiSpaceObjectManager.CurrentSpaceObject.gameObject.SetActive(false);
                Camera.SetupCurrent(Camera.main);
                uiSpaceObjectManager.CurrentSpaceObject=null;
            }

            selectedObject = null;


        }
    }

    void Select(Transform element)
    {
        if (selectedObject != null)
        selectedObject.GetComponent<Orbit>().showTrajectoryLine(false);
        selectedObject = element;
        selectedObject.GetComponent<Orbit>().showTrajectoryLine(true);
        uiSpaceObjectManager.CurrentSpaceObject = selectedObject.GetComponent<Orbit>();
        uiSpaceObjectManager.setStartValue(selectedObject.GetComponent<Orbit>());
        uiSpaceObjectManager.SetVisible(true);
    }

    void reloadScene()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Orbit.isFreeze = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        }
    }
    void freezeScene()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !Orbit.isFreeze)
        {
            Time.timeScale = (Time.timeScale == 0) ? 1 : 0;
        }

    }

}