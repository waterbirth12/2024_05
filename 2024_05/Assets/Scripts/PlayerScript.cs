using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerScript : MonoBehaviour
{
	GameManageScript	game_manage;
	CharacterController c_con;
	Animator			anim;

	public float		f_normalSpeed = 3.0f;		// 通常移動速度
	public int			i_rotate = 8;				// 向き
	float				f_sprintSpeed = 5.0f;		// ダッシュ移動速度
	float				f_jump_pow = 8.0f;			// ジャンプ力
	float				f_gravity = 10.0f;			// 重力

	Vector3				vec3_moveDirection = Vector3.zero;
	Vector3				vec3_startPos;
	GameObject			gameobj_on_floor;			// 現在乗っている床のオブジェクト
	GameObject			gameobj_cursor;				// 攻撃位置のカーソルオブジェクト
	public GameObject	gameobj_attack;				// 攻撃オブジェクト
	AudioSource			audio_source;

	private void Awake()
	{
		audio_source = GetComponent<AudioSource>();
	}

    // Start is called before the first frame update
    void Start()
    {
		game_manage = GameObject.Find("GameManager").GetComponent<GameManageScript>();
        c_con = GetComponent<CharacterController>();
		anim = GetComponent<Animator>();

		gameobj_on_floor = GameObject.Find("Cube_None");
		gameobj_cursor = transform.Find("PlayerCursor").gameObject;

		// 攻撃オブジェクト非表示
		//gameobj_attack.SetActive(false);

		vec3_startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // 移動速度取得
		float	f_speed = 0.0f;

		// シフトダッシュは削除
#if false
		if(true == Input.GetKey(KeyCode.LeftShift))
		{
			f_speed = f_sprintSpeed;
		}
		else
#endif
		{
			f_speed = f_normalSpeed;
		}

// 地上判定のみなので、移動だけ計算
#if false
		// 地上判定
		if(true == c_con.isGrounded)
		{
			vec3_moveDirection = vec3_moveZ + vec3_moveX;

			// ジャンプも削除
			if(true == Input.GetButtonDown("Jump"))
			{
				vec3_moveDirection.y = f_jump_pow;
			}
		}
		else
		{
			// 重力を加える
			vec3_moveDirection = vec3_moveZ+ vec3_moveX + new Vector3(0, vec3_moveDirection.y, 0);
			vec3_moveDirection.y -= f_gravity * Time.deltaTime;
		}
#else
		// 攻撃アニメ再生中か判定
		if(false == GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Attack"))
		{
			// 非再生中
			// スペース入力判定
			if((true == game_manage.b_attack_enable) &&
				(true == Input.GetButtonDown("Jump")))
			{
				// 移動停止
				c_con.velocity.Set(0, c_con.velocity.y, 0);

				// 攻撃アニメON
				anim.SetBool("Attack", true);

				// 攻撃ON
				gameobj_attack.GetComponent<AttackScript>().Attack();

				// 攻撃音鳴動
				audio_source.PlayOneShot(audio_source.clip);
			}
			else
			{
				// 攻撃ボタンを押した時点で入力無効
				if(false == anim.GetBool("Attack"))
				{
					// カメラの向き基準の正面ベクトル取得
					Vector3	vec3_cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;

					// 前後入力
					Vector3	vec3_moveZ = vec3_cameraForward * Input.GetAxis("Vertical") * f_speed;

					// 左右入力
					Vector3	vec3_moveX = Camera.main.transform.right * Input.GetAxis("Horizontal") * f_speed;

					// 移動可能
					vec3_moveDirection = vec3_moveZ + vec3_moveX;

					// 移動アニメ
					anim.SetFloat("MoveSpeed", (vec3_moveZ + vec3_moveX).magnitude);

					// ジャンプアニメ
					//anim.SetBool("Jump", !c_con.isGrounded);

					// キャラの向きを入力方向に更新
					transform.LookAt(transform.position + vec3_moveZ + vec3_moveX);

					// 移動
					c_con.Move(vec3_moveDirection * Time.deltaTime);
				}
			}
		}
		else
		{
			// 攻撃アニメOFF
			anim.SetBool("Attack", false);
		}
#endif
		// 向きとカーソル位置更新(テンキーの値で設定)
		// カーソルはrotation固定
		gameobj_cursor.transform.rotation = 
			new Quaternion(
				0.0f,
				-this.transform.localRotation.y,
				0.0f,
				0.0f);

		if( (-45 < this.transform.localEulerAngles.y) &&
			(45 >= this.transform.localEulerAngles.y))
		{
			// 上向き
			i_rotate = 8;

			// カーソル位置更新
			gameobj_cursor.transform.position = new Vector3(
					gameobj_on_floor.transform.position.x,
					0.01f,
					gameobj_on_floor.transform.position.z + 1);
		}
		else if((45 < this.transform.localEulerAngles.y) &&
				(135 >= this.transform.localEulerAngles.y))
		{
			// 右向き
			i_rotate = 6;

			// カーソル位置更新
			gameobj_cursor.transform.position = new Vector3(
					gameobj_on_floor.transform.position.x + 1,
					0.01f,
					gameobj_on_floor.transform.position.z);
		}
		else if((135 < this.transform.localEulerAngles.y) &&
				(225 >= this.transform.localEulerAngles.y))
		{
			// 下向き
			i_rotate = 2;

			// カーソル位置更新
			gameobj_cursor.transform.position = new Vector3(
					gameobj_on_floor.transform.position.x,
					0.01f,
					gameobj_on_floor.transform.position.z - 1);		
		}
		else
		{
			// 左向き
			i_rotate = 4;
			// カーソル位置更新
			gameobj_cursor.transform.position = new Vector3(
					gameobj_on_floor.transform.position.x - 1,
					0.01f,
					gameobj_on_floor.transform.position.z);
		}

		// 攻撃オブジェクトの位置更新
		gameobj_attack.GetComponent<AttackScript>().SetPosition(gameobj_on_floor.transform.position, i_rotate);
    }

	// コライダーの当たり判定
	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		// 乗っているオブジェクト変更
		if( (0 <= hit.gameObject.name.IndexOf("Cube")) &&
			(gameobj_on_floor.gameObject.name != hit.gameObject.name))
		{
			// 離れたオブジェクトを元に戻す
			gameobj_on_floor.GetComponent<FloorScript>().OnPlayerFloor(false);

			// 乗っているオブジェクト更新
			gameobj_on_floor = hit.gameObject;

			// 乗ったオブジェクト更新
			gameobj_on_floor.GetComponent<FloorScript>().OnPlayerFloor(true);
		}
	}

	// 強制リスポーン
	public void MoveStartPos()
	{
		c_con.enabled = false;

		vec3_moveDirection = Vector3.zero;
		transform.position = vec3_startPos + Vector3.up * 10.0f;
		transform.rotation = Quaternion.identity;
		// 元々乗ってたオブジェクトの乗ってる判定を戻す
		gameobj_on_floor.GetComponent<FloorScript>().OnPlayerFloor(false);
		gameobj_on_floor = GameObject.Find("Cube_None");

		c_con.enabled = true;
	}
}
