using ServiceLocator.Utilities;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Blackboard : GenericMonoSingleton<Blackboard>
{
    public TextMeshProUGUI clock;
    public float timeOfDay;
    public Stack<GalleryEnjoyer> patreon = new Stack<GalleryEnjoyer>();
    public float openingTime = 7;
    public float closingTime = 18;

    void Start()
    {
        timeOfDay = 0;
        StartCoroutine(UpdateClock());
    }

    IEnumerator UpdateClock()
    {
        while (true)
        {
            timeOfDay++;
            if (timeOfDay > 23) timeOfDay = 0;
            clock.text = timeOfDay + ":00";
            if(timeOfDay == closingTime)
            {
                patreon.Clear();
            }
            yield return new WaitForSeconds(5);
        }
    }

    public bool RegisterPatron(GalleryEnjoyer patreon)
    {
        this.patreon.Push(patreon);

        return true;
    }

}
