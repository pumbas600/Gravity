using System.Text;
using TMPro;
using UnityEngine;

public class CelestialBodyInformation : MonoBehaviour
{
    #region Singleton

    private static CelestialBodyInformation instance;

    private void Awake()
    {
        informationObject.gameObject.SetActive(false);
        instance = this;
    }

    #endregion;

    [SerializeField] private TMP_Text informationObject;
    [SerializeField] private bool updateInformation = true;

    private static CelestialBody currentBody;

    private void Update()
    {
        if (updateInformation && currentBody != null)
        {
            informationObject.text = GetInformation(currentBody);
        }
    }

    public static void DisplayInformation(CelestialBody body)
    {
        string information = GetInformation(body);

        instance.informationObject.text = information;
        instance.informationObject.gameObject.SetActive(true);
    }

    private static string GetInformation(CelestialBody body)
    {
        StringBuilder information = new StringBuilder();

        information.Append($"<size=25>{body.name}</size>")
            .AppendLine().AppendLine()
            .Append($"Mass: {body.Mass.ToString("0.")}").AppendLine()
            .Append($"Radius: {body.Radius.ToString("0.00")}").AppendLine()
            .Append($"Gravity: {body.SurfaceGravity.ToString("0.00")}").AppendLine()
            .Append($"Velocity: {body.VelocityMagnitude.ToString("0.00")}");

        if (body.ShowDistanceToNearestBody)
        {
            information.AppendLine()
                .Append($"Distance To Nearest Body: {body.DistanceToNearestBody.ToString("0.")}");
        }

        return information.ToString();
    }

    public static void HideInformation()
    {
        instance.informationObject.gameObject.SetActive(false);
    }

    public static void ToggleInformation(CelestialBody body)
    {
        if (currentBody == body)
        {
            instance.informationObject.gameObject.SetActive(false);
            currentBody = null;
        }
        else
        {
            DisplayInformation(body);
            currentBody = body;
        }
    }
}
