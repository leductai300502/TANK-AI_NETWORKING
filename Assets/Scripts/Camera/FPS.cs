using UnityEngine;

public class FPS : MonoBehaviour
{
    public Transform target; // Tham chiếu đến transform của tank
    public Vector3 offset; // Set giá trị mặc định cho offset
    public Quaternion offset2;
    public NetTankMove tankMovement;
    public Vector2 xminmax;


    private void LateUpdate()
    {
        // Nếu không có tham chiếu đến transform của tank hoặc nó không được gán
        if (!target)
        {
            Debug.LogWarning("Không có đối tượng tank để theo dõi.");
            return;
        }

        if (tankMovement == null)
        {
            tankMovement = FindObjectOfType<NetTankMove>();
        }

        Transform tankTransform = tankMovement.GetTankTransform();
        // Debug thông tin của target
        Debug.Log("Position of the target: " + tankTransform.rotation);



        // Xác định vị trí mới của camera dựa trên vị trí và hướng của tank cộng với offset
        Vector3 newPosition = tankTransform.position + offset;
        transform.rotation = tankTransform.rotation;
        // Đặt vị trí mới cho camera
        transform.position = newPosition;

        // Quay camera để nhìn về phía tank
        //transform.LookAt(tankTransform);
    }
}
