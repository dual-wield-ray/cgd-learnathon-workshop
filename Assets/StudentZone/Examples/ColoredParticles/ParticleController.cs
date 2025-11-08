using UnityEngine;

public class ParticleController : MonoBehaviour
{
    // To control the particles
    [SerializeField] private ParticleSystem particleSys;

    // To bind to our message event
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
            Debug.Log($"Received color: {result}");
            
            // We received a good color, assign it to the particle system
            var main = particleSys.main;
            main.startColor = result;
            
        }
        // Emit a single particle (one per message)
        particleSys.Emit(1);
    }
}
