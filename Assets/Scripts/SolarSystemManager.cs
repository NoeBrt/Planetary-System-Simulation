using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class SolarSystemManager : MonoBehaviour
{
    [SerializeField] GameObject planetPrefabs;
    [SerializeField] Orbit sun;
    [Header("Space Object Range")]
    [SerializeField] Vector2 rangeSpeed = new Vector2(-80f, 80f);
    [SerializeField] Vector2 rangeSizePlanet = new Vector2(10f, 20f);

    [Header("Space Object Material")]
    [SerializeField] Material moonMat;
    [SerializeField] Material planetMat;

    static int NameNumber = 0;
    public Dictionary<Orbit, List<Orbit>> SolarSytemDictionary { get; set; } = new Dictionary<Orbit, List<Orbit>>();
    //capsule champ
    public Material MoonMat { get => moonMat; set => moonMat = value; }
    public Material PlanetMat { get => planetMat; set => planetMat = value; }
    public Orbit Sun { get => sun; set => sun = value; }
    public Vector2 RangeSizePlanet { get => rangeSizePlanet; set => rangeSizePlanet = value; }
    //
    public Orbit AddSpaceObject(Orbit orbitTo, Vector2 sizeRange, Material mat)
    {
        float RanSpeed = Random.Range(rangeSpeed.x - 10f, rangeSpeed.y - 10f) + 10f;
        float RanStartAngle = Random.Range(0f, 360f);
        Vector3 orbitOrientation = new Vector3(Random.Range(-180f, 180f), Random.Range(-180f, 180f), Random.Range(-180f, 180f));
        Orbit SpaceObject = Instantiate(planetPrefabs,getMaxRadiusRotatingAround(orbitTo)*Vector3.up*100,Quaternion.identity,transform).GetComponent<Orbit>();
        
        SpaceObject.gameObject.SetActive(false);
        
        SpaceObject.gameObject.GetComponent<Renderer>().material = mat;
        
        SpaceObject.name = "[Object " + NameNumber + "]->" + orbitTo.gameObject.name;
        NameNumber++;

        SpaceObject.Size = Random.Range(sizeRange.x, sizeRange.y);
        
        float orbitRadiusGap = SpaceObject.Size * Random.Range(1.5f, 3f);
        SpaceObject.UpdateOrbit(orbitTo.transform, getMaxRadiusRotatingAround(orbitTo) + orbitRadiusGap, RanSpeed, RanStartAngle, orbitOrientation, Vector3.zero);
        SpaceObject.gameObject.SetActive(true);
        return SpaceObject;
    }

    public float getMaxRadiusRotatingAround(Orbit Object)
    {
        if (SolarSytemDictionary.Keys.Count > 0)
        {
            if (Object.gameObject.CompareTag("Sun"))
            {
                return SolarSytemDictionary.Keys.ToList().Max(maxOrbit => maxOrbit.OrbitRadius);
            }
            else if (SolarSytemDictionary.ContainsKey(Object) && SolarSytemDictionary[Object].Count > 0)
            {
                return SolarSytemDictionary[Object].Max(maxOrbit => maxOrbit.OrbitRadius);
            }
        }
        return Object.Size / 2f;
    }

    public void remove(Orbit spaceObject)
    {
        if (SolarSytemDictionary != null && SolarSytemDictionary.Keys.Count > 0)
        {
            SolarSytemDictionary[spaceObject].ForEach(moon => Destroy(moon.gameObject));
            SolarSytemDictionary[spaceObject].Clear();
            SolarSytemDictionary.Remove(spaceObject);
            Destroy(spaceObject.gameObject);

        }
    }
    public void RemoveLastPlanet()
    {
        if (SolarSytemDictionary.Count() > 0)
        {
            remove(SolarSytemDictionary.Last().Key);
        }
    }

    public void addInDictionary(Orbit Planet)
    {
        if (!SolarSytemDictionary.ContainsKey(Planet))
        {
            SolarSytemDictionary.Add(Planet, new List<Orbit>());
        }
    }
    public void addInDictionary(Orbit Planet, Orbit Moon)
    {
        if (!SolarSytemDictionary.ContainsKey(Planet))
        {
            SolarSytemDictionary.Add(Planet, new List<Orbit>());
        }
        if (Moon != null)
        {
            SolarSytemDictionary[Planet].Add(Moon);
            Debug.Log(SolarSytemDictionary[Planet].Count);
        }

    }

}
