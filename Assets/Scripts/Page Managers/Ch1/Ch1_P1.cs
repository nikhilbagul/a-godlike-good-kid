using UnityEngine;
using System.Collections;
using System;

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

    bool isAcceptingInput = false;
    bool nextStepRequested = false;

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
        StartCoroutine(BeginInputCooldown(3.0f));
    }

	void Update()
	{
		//numbOfFails = (State)cutscenePlayer.GetIntegerVariable("State");
	}

    public void	fail()
	{
		numbOfFails+=1;		
		SetSolved (advance: false);
	}

	public void win()
    {
		SetSolved (advance: true);
	}

	IEnumerator PlayStorySequence()
	{
		if (numbOfFails == 0)
		{            
            SetSpriteRenderersVisibility(false);            

            yield return new WaitForSeconds(2);

			faderUtility.FadeIn(emptyRestaurant);
            Camera.main.GetComponent<CameraSaturation>().Desaturate();
            
            yield return new WaitUntil(() => nextStepRequested == true);
            nextStepRequested = false;

            faderUtility.FadeIn(fayeErnestRestaurant);
            faderUtility.FadeIn(propCandle);
            faderUtility.FadeIn(propCup);
            faderUtility.FadeIn(propSalt);

            yield return new WaitUntil(() => nextStepRequested == true);
            nextStepRequested = false;

			faderUtility.FadeIn(caption1);

            yield return new WaitUntil(() => nextStepRequested == true);
            nextStepRequested = false;

            faderUtility.FadeIn(speechBubble1);

            yield return new WaitUntil(() => nextStepRequested == true);
            nextStepRequested = false;

            faderUtility.FadeIn(speechBubble2);

            yield return new WaitUntil(() => nextStepRequested == true);
            nextStepRequested = false;

            faderUtility.FadeOut(speechBubble1, null, 0.5f);
            faderUtility.FadeIn(speechBubble3, null, 2);

            yield return new WaitUntil(() => nextStepRequested == true);
            nextStepRequested = false;

            faderUtility.FadeOut(speechBubble2, null, 0.5f);
            faderUtility.FadeIn(speechBubble4, null, 2);

            yield return new WaitUntil(() => nextStepRequested == true);
            nextStepRequested = false;

            faderUtility.FadeOut(caption1, null, 1);
            faderUtility.FadeOut(speechBubble3, null, 1);
            faderUtility.FadeOut(speechBubble4, null, 1);

            yield return new WaitForSeconds(2.0f);            

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

    public void OnScreenClicked()
    {        
        if (!isAcceptingInput)
            return;
        
        nextStepRequested = true;
        StartCoroutine(BeginInputCooldown());
    }

    IEnumerator BeginInputCooldown(float clickCooldownTime = 2.0f)
    {
        isAcceptingInput = false;
        yield return new WaitForSeconds(clickCooldownTime);
        isAcceptingInput = true;
    }
}
