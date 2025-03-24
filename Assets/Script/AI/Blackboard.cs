using ServiceLocator.Utilities;
using System.Collections;
using TMPro;
using UnityEngine;

public class Blackboard : GenericMonoSingleton<Blackboard>
{
    public TextMeshProUGUI clock;
    public float timeOfDay;
    public GalleryEnjoyer patreon;

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
            yield return new WaitForSeconds(5);
        }
    }

    public GalleryEnjoyer RegisterPatron(GalleryEnjoyer patreon)
    {
        if( this.patreon == null)
        {
            this.patreon = patreon;
        }

        return this.patreon;
    }

    public void DeregisterPatreon()
    {
        this.patreon = null;
    }
}
