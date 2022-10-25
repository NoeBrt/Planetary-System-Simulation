using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UiRandomAdderManager : MonoBehaviour
{
    [SerializeField] SolarSystemManager solarSystem;
    [SerializeField] Slider s;
    private bool isMoon;

    public int NumberMoon { get; set; }
    public void OnNumberMoonChange(float value)
    {
        NumberMoon = (int)value;
    }

    public void AddPlanetAutoController()
    {
        Orbit CurrentPlanet = solarSystem.AddSpaceObject(solarSystem.Sun, solarSystem.RangeSizePlanet, solarSystem.PlanetMat);
        if (isMoon)
        {
            for (int i = 0; i < NumberMoon; i++)
            {
                Orbit currentMoon = solarSystem.AddSpaceObject(CurrentPlanet, new Vector2(CurrentPlanet.Size / 5f, CurrentPlanet.Size / 2f), solarSystem.MoonMat);
                solarSystem.addInDictionary(CurrentPlanet, currentMoon);
            }
            CurrentPlanet.OrbitRadius += solarSystem.getMaxRadiusRotatingAround(solarSystem.SolarSytemDictionary.Last().Key) + solarSystem.getMaxRadiusRotatingAround(CurrentPlanet);
        }
        else
        {
            solarSystem.addInDictionary(CurrentPlanet);

        }
    }

    public void toggleIsMoon()
    {
        isMoon = !isMoon;
    }
    public void RemoveButton()
    {
        solarSystem.RemoveLastPlanet();
    }
}
