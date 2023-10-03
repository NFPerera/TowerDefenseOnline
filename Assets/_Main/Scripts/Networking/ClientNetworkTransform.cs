using Unity.Netcode.Components;

namespace _Main.Scripts.Networking
{
    public class ClientNetworkTransform : NetworkTransform
    {
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}