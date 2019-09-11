using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using LibLabSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LibLabGames.NewGame
{
    public class DialogManager : MonoBehaviour
    {
        public static DialogManager instance;

        public float charPerSeconds;
        public bool characterNameIsDisplayed;

        public SettingDialogs settingDialogs;

        public CanvasGroup canvasGroup;
        public Transform panelTransform;
        public Transform nextArrow;
        public TextMeshProUGUI dialogText;
        public TextMeshProUGUI characterNameText;

        public CanvasGroup choicesCanvasGroup;
        public Transform choiceArrow;
        public Button[] choicesButtons;
        public TextMeshProUGUI[] choicesButtonsTexts;

        private WaitForSeconds waitForChar;

        private void Awake()
        {
            canvasGroup.alpha = 0;

            if (instance == null)
                instance = this;

            waitForChar = new WaitForSeconds(1f / charPerSeconds);

            choicesCanvasGroup.alpha = 0;
            choicesCanvasGroup.interactable = false;
            for (int i = 0; i < choicesButtons.Length; ++i)
            {
                choicesButtons[i].gameObject.SetActive(false);
            }
        }

        private List<List<string>> currentTexts;
        private List<List<string>> currentChoices;
        private int txtID1;
        private int txtID2;
        private int choiceID;
        public void EnableBubbleText(Dialog dialog)
        {
            dialogText.text = string.Empty;

            currentTexts = dialog.dialogs;
            currentChoices = dialog.choices;
            characterNameText.text = dialog.characterName;
            txtID1 = 0;
            txtID2 = 0;

            nextArrow.localScale = Vector3.zero;
            panelTransform.localScale = Vector3.zero;
            panelTransform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
            canvasGroup.DOFade(1f, 0.5f)
                .OnComplete(() => DisplayBubbleText(currentTexts));
        }

        private void DisplayBubbleText(List<List<string>> texts)
        {
            if (DisplayBubbleTextCoroutine != null)
            {
                StopCoroutine(DisplayBubbleTextCoroutine);
            }

            if (txtID2 < texts[txtID1].Count)
            {
                DisplayBubbleTextCoroutine = CODisplayBubbleText(texts[txtID1][txtID2]);
                StartCoroutine(DisplayBubbleTextCoroutine);
            }
            else
            {
                DisableBubbleText();
            }
        }

        private IEnumerator DisplayBubbleTextCoroutine;
        private string nextWord;
        private string wordToCheckColor;
        private string nextText;
        private bool colorWord;
        private bool skipDialog;
        private bool choiceDialog;
        private bool doWaittingToChoice;
        private List<string> choicesTexts;
        public int choiceSelectedID;
        private int lastChoiceSelectedID;
        private bool choiceArrowMoving;
        private IEnumerator CODisplayBubbleText(string text)
        {
            dialogText.text = string.Empty;

            skipDialog = false;
            choiceDialog = false;

            if (text[0] == '[')
            {
                if (text[2] == ']')
                {
                    choiceDialog = true;
                    choiceID = int.Parse(text[1].ToString());
                    text = text.Remove(0, 3);
                    choicesTexts = currentChoices[choiceID];
                    choiceSelectedID = 0;
                    choiceArrow.Y(choicesButtons[0].transform.position.y);
                }
                else
                {
                    txtID1 = int.Parse(text[1].ToString());
                    txtID2 = int.Parse(text[3].ToString());
                    text = text.Remove(0, 5);
                }
            }
            else
            {
                txtID2++;
            }

            if (text == null)
                yield break;
            if (DisplayBubbleTextCoroutine == null)
                yield break;

            for (int i = 0; i < text.Length; ++i)
            {
                // Break if bubble text is desabled
                if (DisplayBubbleTextCoroutine == null)
                    yield break;

                // Check if return line is necessary
                if (text[i] == ' ')
                {
                    for (int j = i + 1; j < text.Length; ++j)
                    {
                        if (text[j] != ' ')
                            nextWord += text[j];
                        else
                            j = text.Length;
                    }

                    // Check color word
                    wordToCheckColor = nextWord;
                    if (wordToCheckColor.Contains(".") || wordToCheckColor.Contains(","))
                        wordToCheckColor = wordToCheckColor.Remove(nextWord.Length - 1);

                    if (settingDialogs.colorWords.Contains(wordToCheckColor))
                    {
                        colorWord = true;
                        dialogText.text += "<b><color=#" +
                            ColorUtility.ToHtmlStringRGB(settingDialogs.GetColorValue(wordToCheckColor)) + ">";
                    }
                    else
                    {
                        colorWord = false;
                    }

                    nextText = dialogText.text + " " + nextWord;
                    dialogText.text += text[i] + (colorWord ? "</color></b>" : "");
                    nextWord = string.Empty;
                }
                else
                {
                    if (colorWord)
                    {
                        dialogText.text = dialogText.text.Remove(dialogText.text.Length - 12, 12);
                    }
                    dialogText.text += text[i] + ((colorWord) ? "</color></b>" : "");

                    if (colorWord && i + 1 < text.Length)
                    {
                        if (text[i + 1] == '.' || text[i + 1] == ',')
                        {
                            colorWord = false;
                        }
                    }
                }

                if (text[i] != ' ' && !skipDialog)
                {
                    float startTime = Time.time;
                    while (Time.time - startTime < 1f / charPerSeconds && !skipDialog)
                    {
                        float t = Time.time - startTime;

                        if (Input.GetButtonDown("Fire1"))
                            skipDialog = true;

                        yield return null;
                    }
                }
            }

            yield return new WaitForSeconds(0.3f);

            // Dialogue sans choix de réponse
            if (!choiceDialog)
            {
                nextArrow.DOScale(1, 0.3f);

                while (!Input.GetButtonDown("Fire1"))
                    yield return null;

                nextArrow.DOScale(0, 0.3f);
            }
            // Dialogue avec choix de réponse
            else
            {
                for (int i = 0; i < choicesButtons.Length; ++i)
                {
                    if (i < choicesTexts.Count)
                    {
                        choicesButtons[i].gameObject.SetActive(true);
                        choicesButtonsTexts[i].text = choicesTexts[i].Remove(0, 5);
                    }
                    else
                    {
                        choicesButtons[i].gameObject.SetActive(false);
                    }
                }

                choicesCanvasGroup.DOFade(1, 0.5f);
                choicesCanvasGroup.interactable = true;

                doWaittingToChoice = true;

                while (doWaittingToChoice)
                {
                    if (lastChoiceSelectedID == choiceSelectedID)
                    {
                        choiceSelectedID += (Input.GetAxis("Vertical") > 0) ? 1 : (Input.GetAxis("Vertical") < 0) ? -1 : 0;
                        choiceSelectedID = Mathf.Clamp(choiceSelectedID, 0, choicesTexts.Count - 1);
                    }
                    else if (!choiceArrowMoving)
                    {
                        choiceArrowMoving = true;

                        choiceArrow.DOKill();
                        choiceArrow.DOMoveY(choicesButtons[choiceSelectedID].transform.position.y, 0.1f).SetEase(Ease.Linear)
                            .OnComplete(() => { choiceArrowMoving = false; lastChoiceSelectedID = choiceSelectedID; });
                    }

                    if (Input.GetButtonDown("Fire1"))
                    {
                        doWaittingToChoice = false;
                        txtID1 = int.Parse(choicesTexts[choiceSelectedID][1].ToString());
                        txtID2 = int.Parse(choicesTexts[choiceSelectedID][3].ToString());
                    }

                    yield return true;
                }

                choicesCanvasGroup.DOKill();
                choicesCanvasGroup.DOFade(0, 0.3f);
                choicesCanvasGroup.interactable = false;
            }

            yield return null;

            DisplayBubbleText(currentTexts);
        }

        public void DisableBubbleText()
        {
            panelTransform.DOScale(0f, 0.5f).SetEase(Ease.OutQuad);
            canvasGroup.DOFade(0f, 0.5f);

            if (DisplayBubbleTextCoroutine != null)
            {
                StopCoroutine(DisplayBubbleTextCoroutine);
            }
        }
    }
}