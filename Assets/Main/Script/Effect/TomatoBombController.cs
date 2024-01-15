using UnityEngine;

public class TomatoBombController : MonoBehaviour
{
    public GameObject ExplosionEffect;
    public GameObject ExplosionSpark;
    public GameObject sun;
    private Vector3 originalPosition;
    private Quaternion originalRotation; // 元の回転の値を保存
    private bool hasExploded = false;

    public Vector3 OriginalPosition
    {
        get { return originalPosition; }
    }

    public Vector3 TargetEnemy
    {
        get { return originalPosition; }
    }

    void Start()
    {
        // sun.SetActive(false);
    }

    public void BombThrow()
    {
        // sun.SetActive(true);
        originalPosition = transform.position;
        originalRotation = transform.rotation; // インスタンス化されたときに元の回転を保存する
        // クリック時にhasExplodedをリセット
        hasExploded = false;

        // TomatoBombの位置からRayを飛ばす
        Ray ray = new Ray(transform.position, transform.forward); // transform.forwardはTomatoBombの前方を表すベクトル
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // 目標地点を元の位置から引いて、方向ベクトルを得る
            Vector3 shootDirection = hit.point - originalPosition;

            // Enemyの位置に応じて力を自動調整する
            AdjustForce(hit.collider, shootDirection);
        }
        // sun.SetActive(false);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (!hasExploded)
            {
                ContactPoint contact = collision.contacts[0];
                Vector3 enemyCenter = contact.point;
                Instantiate(ExplosionEffect, enemyCenter, Quaternion.identity);
                Instantiate(ExplosionSpark, enemyCenter, Quaternion.identity);

                if (!collision.gameObject.CompareTag("TomatoBomb"))
                {
                    GetComponent<Rigidbody>().isKinematic = true;
                    transform.position = originalPosition;
                    // Reset the rotation to the original state
                    transform.rotation = originalRotation;
                    // originalPositionを更新
                    originalPosition = transform.position;
                }

                hasExploded = true;
            }
        }
    }

    void Update()
    {
        // if (Input.GetMouseButtonDown(0))
        // {
        //     // クリック時にhasExplodedをリセット
        //     hasExploded = false;

        //     // TomatoBombの位置からRayを飛ばす
        //     Ray ray = new Ray(transform.position, transform.forward); // transform.forwardはTomatoBombの前方を表すベクトル
        //     RaycastHit hit;

        //     if (Physics.Raycast(ray, out hit))
        //     {
        //         // 目標地点を元の位置から引いて、方向ベクトルを得る
        //         Vector3 shootDirection = hit.point - originalPosition;

        //         // Enemyの位置に応じて力を自動調整する
        //         AdjustForce(hit.collider, shootDirection);
        //     }
        // }
    }

    void AdjustForce(Collider enemyCollider, Vector3 shootDirection)
    {
        // Enemyの位置に応じて力を計算する
        float distanceToEnemy = Vector3.Distance(originalPosition, enemyCollider.bounds.center);
        float upwardForce = CalculateUpwardForce(distanceToEnemy);
        float rotationSpeed = CalculateRotationSpeed(distanceToEnemy);

        // Shootメソッドを呼び出して物体を飛ばす
        Shoot(shootDirection.normalized * 3000f, upwardForce, rotationSpeed);
        // hasExploded = false;
    }

    public float CalculateUpwardForce(float distanceToEnemy)
    {
        // 距離が近いほど上向きの力を増やす
        return Mathf.Max(300f, 900f - distanceToEnemy * 10f);
    }

    public float CalculateRotationSpeed(float distanceToEnemy)
    {
        // 距離が遠いほど回転速度を増やす
        return Mathf.Min(300f, distanceToEnemy * 5f);
    }

    public void Shoot(Vector3 dir, float upwardForce, float rotationSpeed)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;

        rb.rotation = Quaternion.identity;
        rb.AddTorque(new Vector3(1, 1, 1) * rotationSpeed, ForceMode.Impulse);
        rb.AddForce(Vector3.up * upwardForce);
        rb.AddForce(dir);
    }
}