using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tramplin : MonoBehaviour
{
    [SerializeField]
    private float force = 3000f;
    [SerializeField]
    private float duration = 1f;

    private void OnTriggerEnter(Collider _other)
    {
        PlayerController _player = _other.gameObject.GetComponent<PlayerController>();

        if (_player != null)
        {
            if (!_player.isJumping)
            {
                StartCoroutine(StartJumpCoroutine(_player));
            }
        }
    }

    private IEnumerator StartJumpCoroutine(PlayerController _player)
    {
        _player.isJumping = true;
        var coroutine = StartCoroutine(JumpCoroutine(_player));

        yield return new WaitForSeconds(duration);

        StopCoroutine(coroutine);
        _player.isJumping = false;
    }

    private IEnumerator JumpCoroutine(PlayerController _player)
    {
        while (true)
        {
            _player.Jump(force);
            yield return null;
        }
    }
}
