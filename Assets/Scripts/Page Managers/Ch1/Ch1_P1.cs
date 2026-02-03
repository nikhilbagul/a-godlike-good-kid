using UnityEngine;
using System.Collections;

public class Ch1_P1 : PageManager 
{
	public static int numbOfFails;
    public GameObject allSprites;
    public SpriteRenderer propCandle;
    public SpriteRenderer propSalt;
    public SpriteRenderer propCup;	
	public SpriteRenderer emptyRestaurant;
    public SpriteRenderer fayeErnestRestaurant;
    public SpriteRenderer caption1;
    public SpriteRenderer caption2;
    public SpriteRenderer speechBubble1;
    public SpriteRenderer speechBubble2;
    public SpriteRenderer speechBubble3;
    public SpriteRenderer speechBubble4;
    public SpriteRenderer speechBubble5;
    public SpriteRenderer speechBubble6;
    public SpriteRenderer speechBubble7;
    public SpriteRenderer speechBubble8;
    CutscenePlayer cutscenePlayer;
	Fader faderUtility;
	SpriteRenderer[] spriteRenderers;

    public void	fail()
	{
		numbOfFails+=1;		
		SetSolved (advance: false);
	}

	void Awake()
	{
		cutscenePlayer = FindFirstObjectByType<CutscenePlayer>();
        faderUtility = FindFirstObjectByType<Fader>();
		spriteRenderers = allSprites.GetComponentsInChildren<SpriteRenderer>();
    }

	protected override void Start()
	{
		base.Start();        
        Debug.Log ("numbOfFails: " + numbOfFails);
        StartCoroutine(PlayStorySequence());
    }

	void Update()
	{
		//numbOfFails = (State)cutscenePlayer.GetIntegerVariable("State");
	}

	public void win(){
		SetSolved (advance: true);
	}

	IEnumerator PlayStorySequence()
	{
		if (numbOfFails == 0)
		{            
            SetSpriteRenderersVisibility(false);            

            yield return new WaitForSeconds(1);

			faderUtility.FadeIn(emptyRestaurant);
            Camera.main.GetComponent<CameraSaturation>().Desaturate();			

            yield return new WaitForSeconds(2);

            faderUtility.FadeIn(fayeErnestRestaurant);
            faderUtility.FadeIn(propCandle);
            faderUtility.FadeIn(propCup);
            faderUtility.FadeIn(propSalt);

            yield return new WaitForSeconds(2);

			faderUtility.FadeIn(caption1);

            yield return new WaitForSeconds(2);

            faderUtility.FadeIn(speechBubble1);

            yield return new WaitForSeconds(2);

            faderUtility.FadeIn(speechBubble2);

            yield return new WaitForSeconds(2);

            faderUtility.FadeOut(speechBubble1, null, 1);
            faderUtility.FadeIn(speechBubble3, null, 2);

            yield return new WaitForSeconds(2);

            faderUtility.FadeOut(speechBubble2, null, 1);
            faderUtility.FadeIn(speechBubble4, null, 2);

            yield return new WaitForSeconds(2);

            faderUtility.FadeOut(caption1, null, 1);
            faderUtility.FadeOut(speechBubble3, null, 1);
            faderUtility.FadeOut(speechBubble4, null, 1);

            yield return new WaitForSeconds(2);
            Camera.main.GetComponent<CameraSaturation>().Saturate();


        }
		else if (numbOfFails >= 1)
		{            
            SetSpriteRenderersVisibility(false);            
			faderUtility.FadeIn(fayeErnestRestaurant);
            faderUtility.FadeIn(propCandle);
            faderUtility.FadeIn(propCup);
            faderUtility.FadeIn(propSalt);			
        }

		yield return null;
	}
	void SetSpriteRenderersVisibility(bool isVisible)
	{
		foreach (SpriteRenderer sprite in spriteRenderers)
		{
			if (isVisible)
				sprite.color = new Color(1, 1, 1, 1);
			else
                sprite.color = new Color(1, 1, 1, 0);            
		}
	}
}
