using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GeneralUiManager : MonoBehaviour
{
    [SerializeField] private Text collisionText;

    public void SetVisible(bool a)
    {
        gameObject.SetActive(a);
    }

    public void DislayCollisionTextText(bool a)
    {
        collisionText.gameObject.SetActive(true);
    }

    // Start is called before the first frame update
}
