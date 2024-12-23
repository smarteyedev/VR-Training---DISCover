using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TeleportationSystem : MonoBehaviour
{
    [Header("Teleportation Settings")]
    public Transform teleportationStartPoint; // Titik teleportasi untuk pemain
    public GameObject player; // Referensi ke player (rig atau kamera utama VR)

    [Header("Teleportation Areas")]
    public Transform officeTeleportPoint; // Titik teleportasi ruang kantor
    public Transform testRoomTeleportPoint; // Titik teleportasi ruang tes
    public GameObject teleportBackButton; // Tombol untuk teleport kembali ke ruangan teleportasi

    private float defaultScaleY = 1.4f; // Skala Y awal pemain
    private float teleportScaleY = 1.9f; // Skala Y saat teleportasi ke ruang kantor atau tes

    [Header("Fade Settings")]
    public Image fadeImage; // Image overlay untuk efek fade
    public float fadeDuration = 1f; // Durasi fade-in dan fade-out

    [Header("Height Offset")]
    public float heightOffset = 0.1f; // Offset untuk memastikan pemain berada di atas teleportation point

    void Start()
    {
        // Menyembunyikan tombol teleport kembali jika belum berada di ruang kantor
        if (teleportBackButton != null)
        {
            teleportBackButton.SetActive(false);
        }

        // Pastikan fadeImage dalam kondisi transparan di awal
        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = 0f;
            fadeImage.color = color;
        }
    }

    public void TeleportToOffice()
    {
        StartCoroutine(TeleportWithFade(officeTeleportPoint, teleportScaleY, true));
    }

    public void TeleportToTestRoom()
    {
        StartCoroutine(TeleportWithFade(testRoomTeleportPoint, teleportScaleY, false));
    }

    public void TeleportBackToStartPoint()
    {
        StartCoroutine(TeleportWithFade(teleportationStartPoint, defaultScaleY, false));
    }

    private IEnumerator TeleportWithFade(Transform teleportPoint, float newScaleY, bool showBackButton)
    {
        // Fade out (to black)
        yield return StartCoroutine(Fade(1f));

        // Teleport player
        TeleportPlayer(teleportPoint);
        if (player != null)
        {
            player.transform.localScale = new Vector3(player.transform.localScale.x, newScaleY, player.transform.localScale.z);
        }

        // Update back button visibility
        if (teleportBackButton != null)
        {
            teleportBackButton.SetActive(showBackButton);
        }

        // Fade in (to transparent)
        yield return StartCoroutine(Fade(0f));
    }

    private void TeleportPlayer(Transform teleportPoint)
    {
        if (teleportPoint != null && player != null)
        {
            // Pastikan pemain ditempatkan tepat di atas permukaan teleportation point
            Vector3 newPosition = teleportPoint.position;
            newPosition.y += heightOffset; // Tambahkan offset vertikal
            player.transform.position = newPosition;

            // Jika perlu, ikutkan rotasi
            player.transform.rotation = teleportPoint.rotation;
        }
        else
        {
            Debug.LogError("Teleportation point or player not assigned.");
        }
    }

    private IEnumerator Fade(float targetAlpha)
    {
        if (fadeImage == null) yield break;

        Color color = fadeImage.color;
        float startAlpha = color.a;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            color.a = alpha;
            fadeImage.color = color;
            yield return null;
        }

        // Pastikan alpha sesuai dengan targetAlpha di akhir
        color.a = targetAlpha;
        fadeImage.color = color;
    }
}
