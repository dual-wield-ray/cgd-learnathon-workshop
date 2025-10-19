using System;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleSystem;

    [SerializeField] private StringEvent messageReceived;

    private void OnEnable()
    {
        messageReceived.OnEventRaised += OnMessageReceived;
    }
    
    private void OnDisable()
    {
        messageReceived.OnEventRaised -= OnMessageReceived;
    }

    public void OnMessageReceived(string message)
    {
        // Try to extract a color from the message
        // Supported colors: red, cyan, blue, darkblue, lightblue, purple, yellow, lime, fuchsia, white, silver, grey, black, orange, brown, maroon, green, olive, navy, teal, aqua, magenta
        // Hex colors can also be used, see here: https://www.w3schools.com/colors/colors_names.asp. For example, to send 'AliceBlue', the message would need to be '#F0F8FF'
        if (ColorUtility.TryParseHtmlString(message, out Color result))
        {
            // We received a good color, assign it to the particle system
            var main = particleSystem.main;
            main.startColor = result;
        }
        
        particleSystem.Emit(1);
    }
}
