using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Seville
{
    public class PopupController : MonoBehaviour
    {
        private PopupData popupDataTemp;

        [Header("Component Dependencies")]
        [SerializeField] private PopupConfiguration popupConfig;

        [Space(4f)]
        [SerializeField] private GameObject panelMain;
        [SerializeField] private TextMeshProUGUI textTitle;
        [SerializeField] private GameObject imageSection;
        [SerializeField] private Image imageContent;
        [SerializeField] private GameObject textSection;
        [SerializeField] private TextMeshProUGUI textDescription;
        [SerializeField] private Button buttonOpenPopup;
        [SerializeField] private Button buttonClosePopup;


        private void Start()
        {
            buttonOpenPopup.onClick.AddListener(() => OpenOrClosePopupPanel(true));
            buttonClosePopup.onClick.AddListener(() => OpenOrClosePopupPanel(false));

            if (popupConfig.isOpenOnStart) OpenOrClosePopupPanel(true);
        }

        public void OpenOrClosePopupPanel(bool isOpen)
        {
            if (isOpen)
            {
                if (popupDataTemp == null)
                {
                    if (string.IsNullOrEmpty(popupConfig.GetPopupData().descriptionText) &&
                        popupConfig.GetPopupData().contentSprite == null)
                    {
                        Debug.LogWarning($"Seville Popup Controller: there is no content data in popup configuration");
                        return;
                    }
                    else popupDataTemp = popupConfig.GetPopupData();
                }

                UIAnimator.ScaleInObject(
                    panelMain,
                    () =>
                    {
                        buttonOpenPopup.gameObject.SetActive(false);

                        textTitle.text = !string.IsNullOrEmpty(popupDataTemp.titleText) ? popupDataTemp.titleText : "Popup Information";
                        textSection.SetActive(!string.IsNullOrEmpty(popupDataTemp.descriptionText) ? true : false);
                        imageSection.SetActive(popupDataTemp.contentSprite != null ? true : false);
                        textDescription.text = popupDataTemp.descriptionText;
                        imageContent.sprite = popupDataTemp.contentSprite;
                    },
                    () => { }
                );
            }
            else
            {
                UIAnimator.ScaleOutObject(
                    panelMain,
                    () => { },
                    () =>
                    {
                        buttonOpenPopup.gameObject.SetActive(true);
                    }
                );
            }
        }
    }
}