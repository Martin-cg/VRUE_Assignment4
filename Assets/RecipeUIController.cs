using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeUIController : MonoBehaviour {

    [Range(0.0F, 1.0F)]
    public float Progress = 1.0F;

    public string RecipeName = "Bun Only Preset";

    [SerializeField]
    private RectTransform ProgressBarTransform;

    [SerializeField]
    private Image RecipeImage;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (Progress >= 1.0F || Progress <= 0.0F) {
            ProgressBarTransform?.transform?.parent?.gameObject?.SetActive(false);
            RecipeImage?.gameObject?.SetActive(false);
        } else {
            ProgressBarTransform?.transform?.parent?.gameObject?.SetActive(true);
            RecipeImage?.gameObject?.SetActive(true);
        }

        if (ProgressBarTransform) {
            Rect r = ProgressBarTransform.rect;

            ProgressBarTransform.sizeDelta = new Vector2(Progress, ProgressBarTransform.sizeDelta.y);
        }
        if (RecipeName != null && RecipeImage) {
            RecipeImage.sprite = Resources.Load<Sprite>("Textures/Recipe/" + RecipeName);
        }
    }
}
