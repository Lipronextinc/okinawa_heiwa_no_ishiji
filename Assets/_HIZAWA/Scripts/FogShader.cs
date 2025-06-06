using SpatialSys.UnitySDK;
using UnityEngine;

public class FogShader : MonoBehaviour
{

    void Update()
    {
        //if (player != null)
        {
            IAvatar localAvatar = SpatialBridge.actorService.localActor.avatar;
            Shader.SetGlobalVector("_PlayerPos", localAvatar.position);
        }
    }
}