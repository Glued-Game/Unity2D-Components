﻿using DG.Tweening;
using Matcha.Dreadful;
using UnityEngine;
using UnityEngine.Assertions;

public class DisplayStashedLeft : BaseBehaviour
{
	private Vector3 slideTweenOrigin;
	private new Transform transform;
	private Camera mainCamera;
	private SpriteRenderer spriteRenderer;
	private Sequence fadeInHUD;
	private Sequence fadeOutHUD;
	private Sequence fadeToTransparency;
	private Sequence fadeOutInstant;
	private Tween slideItem;

	void Awake()
	{
		transform = GetComponent<Transform>();
		Assert.IsNotNull(transform);

		spriteRenderer = GetComponent<SpriteRenderer>();
		Assert.IsNotNull(spriteRenderer);
	}

	void Start()
	{
		mainCamera = Camera.main.GetComponent<Camera>();
		Assert.IsNotNull(mainCamera);

		// cache & pause tween sequences.
		(fadeInHUD = MFX.Fade(spriteRenderer, STASHED_ITEM_TRANSPARENCY, HUD_FADE_IN_AFTER, HUD_INITIAL_TIME_TO_FADE)).Pause();
		(fadeOutHUD = MFX.Fade(spriteRenderer, 0, HUD_FADE_OUT_AFTER, HUD_INITIAL_TIME_TO_FADE)).Pause();
		(fadeToTransparency = MFX.Fade(spriteRenderer, STASHED_ITEM_TRANSPARENCY, 0f, .05f)).Pause();
		(fadeOutInstant = MFX.Fade(spriteRenderer, 0, 0, 0)).Pause();

		Invoke("PositionHUDElements", .001f);
	}

	void PositionHUDElements()
	{
		// shift item to the left
		transform.position = mainCamera.ScreenToWorldPoint(new Vector3(
			Screen.width / 2 - STASHED_ITEM_OFFSET,
			Screen.height - INVENTORY_Y_POS,
			HUD_Z)
		);
	}

	void InitStashedWeapon(GameObject weapon)
	{
		spriteRenderer.sprite = weapon.GetComponent<Weapon>().iconSprite;
		Assert.IsNotNull(spriteRenderer.sprite);

		fadeOutInstant.Restart();
		fadeInHUD.Restart();
	}

	void ChangeStashedWeapon(GameObject weapon)
	{
		spriteRenderer.sprite = weapon.GetComponent<Weapon>().iconSprite;
		Assert.IsNotNull(spriteRenderer.sprite);

		fadeOutInstant.Restart();
		fadeToTransparency.Restart();
	}

	void OnFadeHud(bool status)
	{
		fadeOutHUD.Restart();
	}

	void OnScreenSizeChanged(float vExtent, float hExtent)
	{
		PositionHUDElements();
	}

	void OnInitStashedWeapon(GameObject weapon, int weaponSide)
	{
		if (weaponSide == LEFT) {
			InitStashedWeapon(weapon);
		}
	}

	void OnChangeStashedWeapon(GameObject weapon, int weaponSide)
	{
		if (weaponSide == LEFT) {
			ChangeStashedWeapon(weapon);
		}
	}

	void OnEnable()
	{
		EventKit.Subscribe<GameObject, int>("init stashed weapon", OnInitStashedWeapon);
		EventKit.Subscribe<GameObject, int>("change stashed weapon", OnChangeStashedWeapon);
		EventKit.Subscribe<bool>("fade hud", OnFadeHud);
		EventKit.Subscribe<float, float>("screen size changed", OnScreenSizeChanged);
	}

	void OnDestroy()
	{
		EventKit.Unsubscribe<GameObject, int>("init stashed weapon", OnInitStashedWeapon);
		EventKit.Unsubscribe<GameObject, int>("change stashed weapon", OnChangeStashedWeapon);
		EventKit.Unsubscribe<bool>("fade hud", OnFadeHud);
		EventKit.Unsubscribe<float, float>("screen size changed", OnScreenSizeChanged);
	}
}
