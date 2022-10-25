using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

public class UiObjectManager : MonoBehaviour
{
    public Orbit CurrentSpaceObject { get; set; }
    [Header("Solar system")]
    [SerializeField] SolarSystemManager solarSystem;
    [Header("Orbit Ui element")]
    [SerializeField] Slider StartAngleSlide;
    [SerializeField] Slider OrbitRadiusSlide;
    [SerializeField] Slider OrbitSpeedSlide;
    [SerializeField] Slider OrbitOrientationSlideX;
    [SerializeField] Slider OrbitOrientationSlideY;
    [SerializeField] Slider OrbitOrientationSlideZ;
    [Header("Object Ui element")]
    [SerializeField] Slider ObjectSizeSlide;

    [Header("Eliptict orbit Ui element")]
    [SerializeField] Toggle ElipticCheck;
    [SerializeField] Slider ElipticSizeX;
    [SerializeField] Slider ElipticSizeY;
    [SerializeField] Slider ElipticOffsetAlongFocil;

    [Header("Object Rotation Ui element")]
    [SerializeField] Toggle GravityLockCheck;
    [SerializeField] Toggle SelfRotationCheck;
    [SerializeField] Slider SelfRotationSpeedSlide;
    [Header("moon button")]

    [SerializeField] Button addMoonButton;
    bool buttonPressed = false;
    [Header("orbit Camera")]
    [SerializeField] Button putCameraInOrbit;
    [Header("destroy")]

    [SerializeField] Button DestroyButton;
    private Orbit _previousObject;



    public void SetVisible(bool isVisible)
    {
        gameObject.SetActive(isVisible);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(CurrentSpaceObject);
        if (CurrentSpaceObject != null)
        {
            updateSpaceObjectValue();
            updateSlideRange();
        }
        else
        {
            SetVisible(false);
        }
    }
    private void updateSlideRange()
    {
        ElipticOffsetAlongFocil.minValue = -CurrentSpaceObject.ElipticOffsetBound;
        ElipticOffsetAlongFocil.maxValue = CurrentSpaceObject.ElipticOffsetBound;
        ElipticSizeX.minValue = 1 / (CurrentSpaceObject.ObritTo.localScale.z / 2f);
        ElipticSizeY.minValue = 1 / (CurrentSpaceObject.ObritTo.localScale.z / 2f);
    }

    private void updateSpaceObjectValue()
    {
        Vector3 orientation = new Vector3(OrbitOrientationSlideX.value, OrbitOrientationSlideY.value, OrbitOrientationSlideZ.value);
        CurrentSpaceObject.Size = ObjectSizeSlide.value;
        CurrentSpaceObject.UpdateOrbit(OrbitRadiusSlide.value, OrbitSpeedSlide.value, StartAngleSlide.value, orientation, CurrentSpaceObject.OrbitOffset);
        CurrentSpaceObject.UpdateOjectRotation(GravityLockCheck.isOn, SelfRotationCheck.isOn, SelfRotationSpeedSlide.value);
        CurrentSpaceObject.UpdateEllipsis(ElipticSizeX.value, ElipticSizeY.value, ElipticOffsetAlongFocil.value);
    }



    public void setStartValue(Orbit spaceObject)
    {
        CurrentSpaceObject = spaceObject;
        StartAngleSlide.value = spaceObject.OrbitStartAngle;
        OrbitRadiusSlide.value = spaceObject.OrbitRadius;
        OrbitSpeedSlide.value = spaceObject.OrbitSpeed;
        OrbitOrientationSlideX.value = spaceObject.OrbitOrientation.x;
        OrbitOrientationSlideY.value = spaceObject.OrbitOrientation.y;
        OrbitOrientationSlideZ.value = spaceObject.OrbitOrientation.z;
        ObjectSizeSlide.value = spaceObject.Size;
        ElipticCheck.isOn = (spaceObject.XElipticLength != spaceObject.ZElipticLength);
        ElipticSizeX.value = spaceObject.XElipticLength;
        ElipticSizeY.value = spaceObject.ZElipticLength;
        ElipticOffsetAlongFocil.value = spaceObject.ElipticOffset;
        GravityLockCheck.isOn = spaceObject.IsGravitLocked;
        SelfRotationCheck.isOn = spaceObject.IsRotateSelf;
        SelfRotationSpeedSlide.value = spaceObject.RotationSpeed;
        addMoonButton.interactable = spaceObject.ObritTo.CompareTag("Sun");
    }

    public void CameraInOrbit(Camera camera)
    {
        buttonPressed = !buttonPressed;
        if (buttonPressed)
        {
            _previousObject = CurrentSpaceObject;
            CurrentSpaceObject.showTrajectoryLine(false);
            DestroyButton.interactable = false;
            camera.gameObject.SetActive(true);
            putCameraInOrbit.GetComponentInChildren<Text>().text = "Return to Main View";
            Camera.SetupCurrent(camera);
            camera.GetComponent<Orbit>().ObritTo = CurrentSpaceObject.transform;
            camera.GetComponent<Orbit>().OrbitRadius = 20f;
            camera.GetComponent<Orbit>().OrbitSpeed = 10f;
            CurrentSpaceObject = camera.GetComponent<Orbit>();
        }
        else
        {
            DestroyButton.interactable = true;
            CurrentSpaceObject = _previousObject;
            putCameraInOrbit.GetComponentInChildren<Text>().text = "Put Camera in Orbit";
            CurrentSpaceObject.showTrajectoryLine(true);
            Camera.SetupCurrent(Camera.main);
            camera.gameObject.SetActive(false);
        }
        setStartValue(CurrentSpaceObject);
    }

    public void AddMoonController()
    {
        Orbit currentMoon = solarSystem.AddSpaceObject(CurrentSpaceObject, new Vector2(CurrentSpaceObject.Size / 5f, CurrentSpaceObject.Size / 2f), solarSystem.MoonMat);
        solarSystem.addInDictionary(CurrentSpaceObject, currentMoon);
    }
    public void RemoveButton()
    {
        if (CurrentSpaceObject != null)
        {
            solarSystem.remove(CurrentSpaceObject);
            if (solarSystem.SolarSytemDictionary.Count() > 0)
            {
                CurrentSpaceObject = solarSystem.SolarSytemDictionary.Keys.Last();
            }
        }
    }
}
