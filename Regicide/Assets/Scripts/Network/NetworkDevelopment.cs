using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Regicide.Networking
{
    public class NetworkDevelopment : NetworkRoomManager
    {
        public override void Awake()
        {
            base.Awake();
        }

        public override void Start()
        {
            if (!NetworkClient.active)
            {
                Debug.Log("Switching to Lobby Scene...");
                SceneManager.LoadScene(RoomScene);
            }
            base.Start();
        }
    }
}