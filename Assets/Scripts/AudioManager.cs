using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
	public AudioClip[] Footsteps;
	private int _footstepIndex = 0;

	public AudioClip[] Jumps;

	public AudioClip[] Gunshots;

	public AudioClip[] Impacts;

	public AudioClip[] BodySplats;
	
	private AudioSource _player;

	private AudioReverbFilter _reverb;

    // Start is called before the first frame update
    void Start() {
		_player = GetComponent<AudioSource>();
		_reverb = GetComponent<AudioReverbFilter>();
    }

    // Update is called once per frame
    void Update() {
		// debug/testing
	
    }

	public void PlayFootstep() {
		_player.PlayOneShot(Footsteps[_footstepIndex]);
		_footstepIndex = (_footstepIndex + 1) % Footsteps.Length;
	}

	public void PlayGunshot() {
		// plays 2 random gunshot sounds in tandum to add uniqueness
		_player.PlayOneShot(Gunshots[Random.Range(0, Gunshots.Length)]);
		_player.PlayOneShot(Gunshots[Random.Range(0, Gunshots.Length)]);
	}

	public void PlayImpact() {
		_player.PlayOneShot(Impacts[Random.Range(0, Impacts.Length)]);
	}

	public void PlayBodySplat() {
		// plays 2 splat sounds in tandum to add uniqueness
		_player.PlayOneShot(BodySplats[Random.Range(0, BodySplats.Length)]);
		_player.PlayOneShot(BodySplats[Random.Range(0, BodySplats.Length)]);
	}

	public void PlayJump() {
		_player.PlayOneShot(Jumps[0]);
	}
}
