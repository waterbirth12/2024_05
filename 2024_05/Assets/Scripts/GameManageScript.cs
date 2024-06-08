using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManageScript : MonoBehaviour
{
	enum enState
	{
		enStateWaitStart,		// 開始前
		enStateGame,			// ゲーム中
	}

	// UI管理
	public TextMeshProUGUI	tex_score;
	public TextMeshProUGUI	tex_result_score;
	public TextMeshProUGUI	tex_play_time;
	public TextMeshProUGUI	tex_wait_time;
	public GameObject		obj_wait_ui;
	public GameObject		obj_start;
	public GameObject		obj_result;

	// ゲーム本体の管理
	public int			i_game_mode = 1;				// ゲームモード(0：撃破数/1：エンドレス)
	public int			i_purpose_count = 10;			// 撃破目的数
	enState				en_game_state = enState.enStateWaitStart;
	public float		f_play_time = 0.0f;				// 経過時間
	float				f_wait_time = 0.0f;				// 開始前待機時間
	int					i_wait_time_count = 0;			// 開始前カウンタ
	public bool			b_attack_enable = false;		// 攻撃可否
	public int			i_break_count = 0;
	public AudioClip[]	audio_clip = new AudioClip[3];
	AudioSource			audio_source;

	private void Awake()
	{
		audio_source = GetComponent<AudioSource>();
	}

	// 
	private void UpdateStateWaitTime()
	{
		f_wait_time += Time.deltaTime;

		if(4 == i_wait_time_count)
		{
			tex_wait_time.text = string.Format("");
		}

		if(1.0f <= f_wait_time)
		{
			i_wait_time_count--;
			f_wait_time = 0.0f;

			switch(i_wait_time_count)
			{
				case 3:
				case 2:
				case 1:
					// 表示テキスト変更
					tex_wait_time.text = string.Format("{0:0}", i_wait_time_count);
					// カウンタ音鳴動
					audio_source.PlayOneShot(audio_clip[0]);
					break;
				case 0:
					// 表示テキスト変更
					tex_wait_time.text = string.Format("Start!");
					// カウンタ音鳴動
					audio_source.PlayOneShot(audio_clip[1]);
					break;
				default:
					break;
			}
		}

		if(-1 == i_wait_time_count)
		{
			en_game_state = enState.enStateGame;
			b_attack_enable = true;

			obj_wait_ui.SetActive(false);
			obj_start.SetActive(false);
			obj_result.SetActive(false);

			// 音楽再生
			audio_source.loop = true;
			audio_source.volume = 0.1f;
			audio_source.PlayOneShot(audio_clip[2]);
		}
	}

	private void UpdateStateGame()
	{
		// 経過時間追加
		f_play_time += Time.deltaTime;

		tex_score.text = string.Format("{0}", i_break_count);
		tex_play_time.text = string.Format("{0:f1}", f_play_time);
	}


    // Start is called before the first frame update
    void Start()
    {
		en_game_state = enState.enStateWaitStart;
		f_play_time = 0.0f;				// 経過時間
		f_wait_time = 0.0f;				// 開始前待機時間
		i_wait_time_count = 4;
		b_attack_enable = false;		// 攻撃可否
		i_break_count = 0;
		audio_source.volume = 0.75f;
		audio_source.loop = false;

		obj_wait_ui.SetActive(true);
		obj_start.SetActive(true);
		obj_result.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
		switch(en_game_state)
		{
			case enState.enStateWaitStart:
				UpdateStateWaitTime();
				break;
			case enState.enStateGame:
				UpdateStateGame();
				break;
			default:
				break;
		}
    }

	public void BreakEnemy()
	{
		audio_source.PlayOneShot(audio_clip[3]);

		if(enState.enStateGame == en_game_state)
		{
			i_break_count++;

			// 撃破モード
			if(0 == i_game_mode)
			{
				// 目的数分倒した
				if(i_purpose_count <= i_break_count)
				{
					i_break_count = i_purpose_count;
					b_attack_enable = false;

					// 結果画面に遷移
					TransResult();
				}
			}
		}
	}

	public void BreakPlayer()
	{
		b_attack_enable = false;

		// 結果画面に遷移
		TransResult();
	}

	private void TransResult()
	{
		SceneManager.sceneLoaded += GameSceneLoad;

		// SampleSceneに切り替える
		SceneManager.LoadScene("Result");
	}

	private void GameSceneLoad(Scene next, LoadSceneMode mode)
	{
		// シーン切り替え後のスクリプトを取得
		var gameManager = GameObject.FindWithTag("GameManager").GetComponent<ResultScript>();
    
		// ゲームモードと目的数を渡す
		gameManager.i_mode = i_game_mode;
		gameManager.i_purpose_count = i_purpose_count;
		gameManager.i_break_count = i_break_count;
		gameManager.f_play_time = f_play_time;

		// イベントから削除
		SceneManager.sceneLoaded -= GameSceneLoad;
	}
}
