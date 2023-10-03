using Unity.Netcode.Components;

namespace _Main.Scripts.Networking
{
    public class ClientNetworkAnimator : NetworkAnimator
    {
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}