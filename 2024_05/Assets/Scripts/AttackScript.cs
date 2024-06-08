using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
	public float		f_speed = 8.0f;		// 通常移動速度
	int					i_rotate;
	bool				b_attack = false;
	CapsuleCollider		cap_col;
	MeshRenderer		mesh_rend;

    // Start is called before the first frame update
    void Start()
    {
		cap_col = GetComponent<CapsuleCollider>();
		mesh_rend = GetComponent<MeshRenderer>();

        // オブジェクト非表示
		cap_col.enabled = false;
		mesh_rend.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
		// 攻撃ON
		if(true == b_attack)
		{
			// 移動
			switch(i_rotate)
			{
				case 8:		// 上向き
					this.transform.position += new Vector3(0.0f, 0.0f, f_speed * Time.deltaTime);
					break;
				case 6:		// 右向き
					this.transform.position += new Vector3(f_speed * Time.deltaTime, 0.0f, 0.0f);
					break;
				case 2:		// 下向き
					this.transform.position += new Vector3(0.0f, 0.0f, -f_speed * Time.deltaTime);
					break;
				case 4:		// 左向き
					this.transform.position += new Vector3(-f_speed * Time.deltaTime, 0.0f, 0.0f);
					break;
				default:
					break;
			}

		}
    }

	// 位置更新
	public void SetPosition(Vector3 vec3Floor, int iRotate)
	{
		i_rotate = iRotate;
		if(false == b_attack)
		{
			switch(iRotate)
			{
				case 8:		// 上向き
					this.transform.position = new Vector3(
						vec3Floor.x,
						0.0f,
						vec3Floor.z - 0.25f);

					this.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 90.0f);		
					break;
				case 6:		// 右向き
					this.transform.position = new Vector3(
						vec3Floor.x - 0.25f,
						0.0f,
						vec3Floor.z);

					this.transform.localEulerAngles = new Vector3(0.0f, 90.0f, 90.0f);
					break;
				case 2:		// 下向き
					this.transform.position = new Vector3(
						vec3Floor.x,
						0.0f,
						vec3Floor.z + 0.25f);

					this.transform.localEulerAngles = new Vector3(0.0f, 180.0f, 90.0f);
					break;
				case 4:		// 左向き
					this.transform.position = new Vector3(
						vec3Floor.x + 0.25f,
						0.0f,
						vec3Floor.z);

					this.transform.localEulerAngles = new Vector3(0.0f, 270.0f, 90.0f);
					break;
				default:
					break;
			}
		}
	}

	// 攻撃ON
	public void Attack()
	{
		// オブジェクト表示
		cap_col.enabled = true;
		mesh_rend.enabled = true;

		// 攻撃開始
		b_attack = true;
	}

	// 衝突判定
	private void OnTriggerEnter(Collider other)
	{
		// 壁にぶつかったとき
		if(other.gameObject.CompareTag("Wall"))
		{
			// オブジェクト非表示
			cap_col.enabled = false;
			mesh_rend.enabled = false;

			// 攻撃OFF
			b_attack = false;
		}
		// 床にぶつかったとき
		else if(other.gameObject.CompareTag("Floor"))
		{
			// 床を破壊状態にする
			other.gameObject.GetComponent<FloorScript>().AttackFloor();
		}
	}
}
