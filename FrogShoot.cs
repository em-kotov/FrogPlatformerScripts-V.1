using UnityEngine;

public class FrogShoot : MonoBehaviour
{
    [SerializeField] private Transform _shootingPoint;
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private float _shootDistance;

    private BulletSpawner _bulletSpawner;
    private FrogMovement _frogMovement;
    private bool _isShooting = false;

    private void Start()
    {
        _bulletSpawner = GetComponent<BulletSpawner>();
        _frogMovement = GetComponent<FrogMovement>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            _isShooting = true;
    }

    private void FixedUpdate()
    {
        if (_isShooting)
        {
            Shoot(GetTargetDirection(DetectEnemy()));
            _isShooting = false;
        }
    }

    private EnemyBehaviour DetectEnemy()
    {
        float distance = 0;
        float closestDistance = Mathf.Infinity;
        EnemyBehaviour closestEnemy = null;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(_shootingPoint.position, _shootDistance);
        Debug.DrawRay(_shootingPoint.position, new Vector3(1, 1) * _shootDistance, Color.green);

        foreach (Collider2D collider in colliders)
        {
            if (collider.TryGetComponent(out EnemyBehaviour enemy))
            {
                distance = Vector2.Distance(_shootingPoint.position, enemy.transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }
        }

        return closestEnemy;
    }

    private Vector2 GetTargetDirection(EnemyBehaviour closestEnemy)
    {
        Vector2 targetDirection;
        Vector2 defaultDirection = new Vector2(0.8f, 0.8f);

        if (_frogMovement.IsFacingRight)
            targetDirection = defaultDirection;
        else
            targetDirection = defaultDirection * -1;

        if (closestEnemy != null)
        {
            Vector3 targetPosition = closestEnemy.transform.position;
            targetDirection = (targetPosition - _shootingPoint.position).normalized;
        }

        return targetDirection;
    }

    private void Shoot(Vector2 targetDirection)
    {
        Bullet bullet = _bulletSpawner.Spawn();

        if (bullet.TryGetComponent(out Rigidbody2D bulletRigidbody))
        {
            bulletRigidbody.position = _shootingPoint.position;
            bulletRigidbody.AddForce(targetDirection * _bulletSpeed, ForceMode2D.Impulse);
        }

        StartCoroutine(_bulletSpawner.StartDeactivation(bullet));
    }
}
