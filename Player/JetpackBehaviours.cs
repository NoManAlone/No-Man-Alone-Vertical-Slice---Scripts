using UnityEngine;
using System.Collections;

public class JetpackBehaviours : MonoBehaviour
{
    ParticleSystem jetpackParticles;
	PlayerAudio playerAudio;

	PhotonView playerAnimPhotonView;

    void Awake()
    {
		playerAudio = transform.parent.GetComponent<PlayerAudio>();
        jetpackParticles = transform.FindChild("Particle Emitter").GetComponent<ParticleSystem>();

		playerAnimPhotonView = GetComponent<PhotonView>();
    }

	public void CallSwitchJetpack(bool jetpacking)
	{
		playerAnimPhotonView.RPC("SwitchJetpack", PhotonTargets.AllBuffered, jetpacking);
	}

    [PunRPC]
    void SwitchJetpack(bool jetpacking)
    {
        if (jetpacking)
			jetpackParticles.Play();
        else
			jetpackParticles.Stop();

		playerAudio.Boost(jetpacking);
    }
}
