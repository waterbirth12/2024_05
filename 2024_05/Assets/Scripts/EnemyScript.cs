using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class EnemyScript : MonoBehaviour
{
	// 敵の状態
	enum enEnemyState{
		enActive,		// アクティブ
		enBreak,		// 撃破状態
		enRespone,		// リスポーン
	};

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
	public GameObject	gameobj_cursor;				// 攻撃位置のカーソルオブジェクト
	public GameObject	gameobj_attack;				// 攻撃オブジェクト
	public FloorScript	gameobj_floor;				// 初期の床オブジェ
	bool				b_hide = false;
	AudioSource			audio_source;
	MeshRenderer		mesh_rend_cursor;
	public GameObject	gameobj_break_particle;				// 撃破時のエフェクトオブジェクト
	GameObject			gameobj_break_particle_instant;		// 撃破時のエフェクトオブジェクト(一時作成用)
	public GameObject	gameobj_respone_particle;			// リスポーン時のエフェクトオブジェクト
	GameObject			gameobj_respone_particle_instant;	// リスポーン時のエフェクトオブジェクト(一時作成用)
	enEnemyState		en_enemy_state;				// 敵の状態
	float				f_state_time;				// ステータス時間
	public GameObject	gameobj_mesh;				// 本体メッシュ
	public AudioClip[]	audio_clip = new AudioClip[2];	// 音楽データ

	// 入力関係
	float				f_input_attack = 0.0f;
	float				f_input_horizonal = 0.0f;
	float				f_input_vertical = 0.0f;
	float				f_wait_time_attack = 0.0f;
	float				f_wait_time_horizonal = 0.0f;
	float				f_wait_time_vertical = 0.0f;
	float				f_time_attack = 0.0f;
	float				f_time_horizonal = 0.0f;
	float				f_time_vertical = 0.0f;

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

		mesh_rend_cursor = gameobj_cursor.GetComponent<MeshRenderer>();
		mesh_rend_cursor.enabled = true;
		en_enemy_state = enEnemyState.enActive;

		// 攻撃オブジェクト非表示
		//gameobj_attack.SetActive(false);

		vec3_startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
		switch(en_enemy_state)
		{
			case enEnemyState.enActive:
				UpdateStateActive();
				break;
			case enEnemyState.enBreak:
				UpdateStateBreak();
				break;
			case enEnemyState.enRespone:
				UpdateStateRespone();
				break;
			default:
				break;
		}
        
    }

	private void UpdateStateActive()
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

		f_time_attack += Time.deltaTime;
		f_time_horizonal += Time.deltaTime;
		f_time_vertical += Time.deltaTime;

		// 移動入力更新
		if(f_wait_time_horizonal <= f_time_horizonal)
		{
			f_input_horizonal = Random.value;
			if(0.33f > f_input_horizonal)
			{
				f_input_horizonal = -1.0f;
			}
			else if(0.66f < f_input_horizonal)
			{
				f_input_horizonal = 1.0f;
			}
			else
			{
				f_input_horizonal = 0;
			}

			// 入力の待ち時間設定
			f_time_horizonal = 0.0f;
			f_wait_time_horizonal = Random.Range(0.0f, 1.0f);
		}

		if(f_wait_time_vertical <= f_time_vertical)
		{
			f_input_vertical = Random.value;
			if(0.33f > f_input_vertical)
			{
				f_input_vertical = -1.0f;
			}
			else if(0.66f < f_input_vertical)
			{
				f_input_vertical = 1.0f;
			}
			else
			{
				f_input_vertical = 0;
			}

			// 入力の待ち時間設定
			f_time_vertical = 0.0f;
			f_wait_time_vertical = Random.Range(0.0f, 1.0f);
		}

		// 攻撃アニメ再生中か判定
		if(false == GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Attack"))
		{
			// 非再生中
			// 攻撃入力判定
			if(f_wait_time_attack <= f_time_attack)
			{
				f_input_attack = Random.value;

				// 入力の待ち時間設定
				f_time_attack = 0.0f;
				f_wait_time_attack = Random.Range(0.0f, 3.0f);
			}

			// 攻撃ON
			if((true == game_manage.b_attack_enable) &&
				(-0.5f <= transform.position.y) &&
				(0.5f <= f_input_attack))
			{
				f_input_attack = 0.0f;
				// 移動停止
				c_con.velocity.Set(0, c_con.velocity.y, 0);

				// 攻撃アニメON
				anim.SetBool("Attack", true);

				// 攻撃ON
				gameobj_attack.GetComponent<AttackScript>().Attack();

				// 攻撃音鳴動
				audio_source.PlayOneShot(audio_clip[0]);
			}
			else
			{
				// 攻撃ボタンを押した時点で入力無効
				if(false == anim.GetBool("Attack"))
				{
					// カメラの向き基準の正面ベクトル取得
					Vector3	vec3_cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;

					// 前後入力
					Vector3	vec3_moveZ = vec3_cameraForward * f_input_vertical * f_speed;

					// 左右入力
					Vector3	vec3_moveX = Camera.main.transform.right * f_input_horizonal * f_speed;

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

		if(-0.5f >= transform.position.y)
		{
			mesh_rend_cursor.enabled = false;
		}
		else
		{
			mesh_rend_cursor.enabled = true;
		}
	}

	private void UpdateStateBreak()
	{
		f_state_time += Time.deltaTime;

		if(0.75f <= f_state_time)
		{
			// リスポーン中に状態更新
			en_enemy_state = enEnemyState.enRespone;

			// タイマクリア
			f_state_time = 0.0f;
		}		
	}

	private void UpdateStateRespone()
	{
		f_state_time += Time.deltaTime;

		if(1.5f <= f_state_time)
		{
			// メッシュ再表示
			gameobj_mesh.SetActive(true);

			//vec3_moveDirection = Vector3.zero;
			transform.position = new Vector3(Random.Range(-3.5f, 3.5f), 1.0f, Random.Range(-3.5f, 3.5f));
			transform.rotation = Quaternion.identity;
			// 元々乗ってたオブジェクトの乗ってる判定を戻す
			gameobj_on_floor.GetComponent<FloorScript>().OnEnemyFloor(false);
			gameobj_on_floor = GameObject.Find("Cube_None");

			// ランダム操作タイマクリア
			f_wait_time_horizonal = 0.0f;
			f_wait_time_vertical = 0.0f;

			anim.SetBool("Attack", false);

			c_con.enabled = true;

			// アクティブに状態更新
			en_enemy_state = enEnemyState.enActive;

			// リスポーン時のSE発生
			audio_source.PlayOneShot(audio_clip[1]);

			// タイマクリア
			f_state_time = 0.0f;

			// リスポーンエフェクト作成
			gameobj_respone_particle_instant = Instantiate(
				gameobj_respone_particle,
				new Vector3(transform.position.x, transform.position.y, transform.position.z),
				Quaternion.Euler(0, 0, 0));
		}
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		// 乗っているオブジェクト変更
		if( (0 <= hit.gameObject.name.IndexOf("Cube")) &&
			(gameobj_on_floor.gameObject.name != hit.gameObject.name))
		{
			// 離れたオブジェクトを元に戻す
			gameobj_on_floor.GetComponent<FloorScript>().OnEnemyFloor(false);

			// 乗っているオブジェクト更新
			gameobj_on_floor = hit.gameObject;

			// 乗ったオブジェクト更新
			gameobj_on_floor.GetComponent<FloorScript>().OnEnemyFloor(true);
		}
		// 壁に衝突
		else if(hit.collider.CompareTag("Wall"))
		{
			// ランダム操作タイマクリア
			f_wait_time_horizonal = 0.0f;
			f_wait_time_vertical = 0.0f;
		}
	}

	// 強制リスポーン
	public void MoveStartPos()
	{
		c_con.enabled = false;

		// メッシュ非表示
		gameobj_mesh.SetActive(false);

		// 撃破状態に更新
		en_enemy_state = enEnemyState.enBreak;

		// 撃破エフェクト作成
		gameobj_break_particle_instant = Instantiate(
			gameobj_break_particle,
			new Vector3(transform.position.x, transform.position.y, transform.position.z),
			Quaternion.Euler(0, 0, 0));
	}
}
