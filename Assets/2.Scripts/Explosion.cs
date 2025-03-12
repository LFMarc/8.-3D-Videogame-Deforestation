using UnityEngine;

public class Explosion : MonoBehaviour
{
    #region Fields
    [SerializeField] private float _explosionArea;
    [SerializeField] private float _explosionForce = 1000;
    [SerializeField] private GameObject _effect;
    [SerializeField] private Camera _cameraExplosion;
    [SerializeField] private float _cameraDuration = 2f;
    [SerializeField] private Transform _player;
    [SerializeField] private Rigidbody _playerRB;

    private bool _isCameraActive = false;
    #endregion

    #region Unity Callbacks

    private void OnTriggerEnter(Collider other)
    {
        _effect.SetActive(true);
        _cameraExplosion.gameObject.SetActive(true);
        _isCameraActive = true;

        Animator playerAnim = other.GetComponentInParent<Animator>();
        if (playerAnim != null)
            playerAnim.enabled = false;

        ExplosionForce();
    }

    private void Update()
    {
        _cameraExplosion.transform.LookAt(_player.position);

        if (_player != null && Vector3.Distance(_cameraExplosion.transform.position, _player.transform.position) > 15)
        {
            _cameraExplosion.transform.LookAt(_player.transform.position);
            _cameraExplosion.transform.Translate(_cameraExplosion.transform.forward * Time.deltaTime * 2, Space.Self);

            if (_playerRB.velocity.magnitude < 3)
            {
                _cameraExplosion.enabled = false;
                Vector3 currentPos = _player.transform.position;
                _player.transform.localPosition = Vector3.zero;
                _player.transform.parent.GetComponent<CharacterController>().enabled = false;
                _player.transform.parent.position = currentPos;
                _player.transform.parent.GetComponent<CharacterController>().enabled = true;
                _player.transform.parent.GetComponent<Animator>().enabled = true;

                Destroy(gameObject);
            }
        }


    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _explosionArea);
    }
    #endregion

    #region Private Methods
    private void ExplosionForce()
    {
        Collider[] objects = Physics.OverlapSphere(transform.position, _explosionArea);
        for (int i = 0; i < objects.Length; i++)
        {
            Rigidbody objectRB = objects[i].GetComponent<Rigidbody>();
            if (objectRB != null)
            {
                objectRB.AddExplosionForce(_explosionForce, transform.position, _explosionArea);
            }
        }
    }
    #endregion
}