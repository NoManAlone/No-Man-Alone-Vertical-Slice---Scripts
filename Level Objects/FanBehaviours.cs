using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FanBehaviours : MonoBehaviour 
{	
	/*---------------------------------------------------- DECLARATIONS ----------------------------------------------------*/
	
	PowerManager powerManager;

    int powerCost;

    public Sprite consoleView_Powered, consoleView_Unpowered;
    SpriteRenderer consoleViewImage;

    BoxCollider2D effectorCollider1, effectorCollider2, effectorCollider3;

    Animator anim;

	/*---------------------------------------------------- AWAKE ----------------------------------------------------*/

    void Awake()
    {
        powerManager = GameObject.Find("GameManager").GetComponent<PowerManager>();

        anim = transform.FindChild("Animation").GetComponent<Animator>();

        effectorCollider1 = transform.FindChild("AreaEffectors").GetChild(0).GetComponent<BoxCollider2D>();
        effectorCollider2 = transform.FindChild("AreaEffectors").GetChild(1).GetComponent<BoxCollider2D>();
        effectorCollider3 = transform.FindChild("AreaEffectors").GetChild(2).GetComponent<BoxCollider2D>();

        consoleViewImage = transform.FindChild("ConsoleView").GetComponent<SpriteRenderer>();

        powerCost = GetComponent<Power>().powerCost;

        GameObject fanDistance = transform.FindChild("FanDistance").gameObject;

        float difference = Vector2.Distance(transform.position, fanDistance.transform.position);

        effectorCollider1.size = new Vector2(difference-1, effectorCollider1.size.y);
        effectorCollider1.offset = new Vector2(difference / 2, 0);

        effectorCollider2.size = new Vector2(difference - 1, effectorCollider2.size.y);
        effectorCollider2.offset = new Vector2(difference / 2, 1.7f);

        effectorCollider3.size = new Vector2(difference - 1, effectorCollider3.size.y);
        effectorCollider3.offset = new Vector2(difference / 2, -1.7f);
    }

	/*---------------------------------------------------- SET POWERED ----------------------------------------------------*/

    public void SetPowered(bool powered)
    {
        if (powered)
            FanOn();

        else
            FanOff();
    }

	/*---------------------------------------------------- FAN ON ----------------------------------------------------*/

    void FanOn()
    {
        effectorCollider1.GetComponent<AreaEffector2D>().enabled = true;
        effectorCollider2.GetComponent<AreaEffector2D>().enabled = true;
        effectorCollider3.GetComponent<AreaEffector2D>().enabled = true;
        transform.FindChild("Particles").GetComponent<ParticleSystem>().enableEmission = true;
        anim.SetBool("Powered", true);
        GetComponent<AudioSource>().PlayDelayed(0f);

        powerManager.AlterMaxPower(-powerCost);

        consoleViewImage.sprite = consoleView_Powered;
    }

	/*---------------------------------------------------- FAN OFF ----------------------------------------------------*/

    void FanOff()
    {
        effectorCollider1.GetComponent<AreaEffector2D>().enabled = false;
        effectorCollider2.GetComponent<AreaEffector2D>().enabled = false;
        effectorCollider3.GetComponent<AreaEffector2D>().enabled = false;
        transform.FindChild("Particles").GetComponent<ParticleSystem>().enableEmission = false;
        anim.SetBool("Powered", false);
        GetComponent<AudioSource>().Stop();

        powerManager.AlterMaxPower(powerCost);

        consoleViewImage.sprite = consoleView_Unpowered;
    }
}
