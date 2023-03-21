using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FluentCeVIOWrapper.Common;

using UnityEngine;
using UnityEngine.Networking;

//CeVIO で発声するスクリプト
public class CeVIO : MonoBehaviour
{
	public string Message;
	public string exepath;
	public string outpath;
	public string wavpath;
	public string narrator;
	public string product;

	//ChatGPTに教えた感情に対応するトークソフトのパラメータ
	public List<string> VoiceEmotes;
	public List<List<(string, double)>> VoiceEmoteWeights
        = new();
	public List<List<(string, double)>> VoiceEmoteOptions;

	//ChatGPTの表現する感情名と比率
	private string emoName;
	private double emoWeight;

	//トークソフトに渡すパラメータリスト
	public Dictionary<string, double> emotions
		= new();
	public Dictionary<string, double> options
		= new();

	public float emote_time;
	public Process exProcess;

	private FluentCeVIO fcw;

	public void Awake()
	{
		GameObject Game_system = GameObject.FindGameObjectWithTag("Game_system");
		SystemSetting SystemSetting = Game_system.GetComponent<SystemSetting>();
		exepath = SystemSetting.CeVIO_exe;
		//outpath = "\"" + Application.temporaryCachePath + "/output.wav" + "\"";
		narrator = SystemSetting.CeVIO_narrator;
		wavpath = Application.temporaryCachePath + $"/{Guid.NewGuid().ToString()}.wav";
		product = SystemSetting.CeVIO_product;

		//ChatGPTに教えた感情のリスト
		VoiceEmotes = SystemSetting.VoiceEmotes;
		//ChatGPTの感情リストに対するトークソフトの感情パラメータリスト
		VoiceEmoteWeights = SystemSetting.VoiceEmoteWeights;
		//ChatGPTの感情リストに対するトークソフトの発声オプションリスト
		VoiceEmoteOptions = SystemSetting.VoiceEmoteOptions;

		var _ = AwakeAsync();
	}

	public void AppStart()
	{
        UnityEngine.Debug.Log($"AppStart");

		Message = EditorRunTerminal.Message;
		//ChatGPTの表現する感情名と比率
		emoName = EditorRunTerminal.Emo;
		emoWeight = EditorRunTerminal.Emo_Weight;

        if(!string.IsNullOrEmpty(emoName)){
            UnityEngine.Debug.Log($"emoName: {emoName}");
            var emoIndex = VoiceEmotes.FindIndex(v => v ==  emoName);
            UnityEngine.Debug.Log($"emoIndex: {emoIndex}");

            if(emoIndex >= 0){
                emotions = VoiceEmoteWeights
                    .ElementAt(emoIndex)
                    .Select(v => (v.Item1, v.Item2 * emoWeight))
                    .ToDictionary(v => v.Item1, v => v.Item2)
                    ;
				emotions.ToList().ForEach(v =>
				{
					UnityEngine.Debug.Log($"    emotions:{v.Key}, {v.Value}");
				});
				options = VoiceEmoteOptions
                    .ElementAt(emoIndex)
                    .Select(v => (v.Item1, v.Item2))
                    .ToDictionary(v => v.Item1, v => v.Item2)
                    ;
                options.ToList().ForEach(v =>
				{
					UnityEngine.Debug.Log($"    options:{v.Key}, {v.Value}");
				});
            }
        }
		var _ = SpeakAsync();
	}

	void OnApplicationQuit()
    {
		//if(fcw is not null)fcw.Dispose();
	}

    private async Task AwakeAsync()
    {
		//lib
		var p = product.Replace("\"","").Trim() switch
		{
			"CeVIO_AI" => Product.CeVIO_AI,
			"CeVIO_CS" => Product.CeVIO_CS,
			_ => Product.CeVIO_AI
		};
		UnityEngine.Debug.Log($"product: {product}, p: {p}");
		fcw = await FluentCeVIO.FactoryAsync(product:p);
        var _ = await fcw.StartAsync();
		await fcw.SetCastAsync(narrator);
	}

    private async Task SpeakAsync()
    {
        UnityEngine.Debug.Log("開始");

        //set params
		var sendParams = fcw
			.CreateParam();

        if(emotions is not null){
			Dictionary<string, uint> es = emotions
				.ToDictionary(
					v => v.Key,
					v => Convert.ToUInt32(v.Value)
				);
            es.ToList().ForEach(v =>
            {
                UnityEngine.Debug.Log($"    es:{v.Key}, {v.Value}");
            });
			sendParams
				.Emotions(es);
		}
        if(options is not null){
			Dictionary<string, uint> ops = options
				.ToDictionary(
					v => v.Key,
					v => Convert.ToUInt32(v.Value)
				);
            if(ops.ContainsKey("Alpha")){
				sendParams.Alpha(ops["Alpha"]);
			}
            if(ops.ContainsKey("Tone")){
				sendParams.Tone(ops["Tone"]);
			}
            if(ops.ContainsKey("ToneScale")){
				sendParams.ToneScale(ops["ToneScale"]);
			}
            if(ops.ContainsKey("Speed")){
				sendParams.Speed(ops["Speed"]);
			}
            if(ops.ContainsKey("Volume")){
				sendParams.Volume(ops["Volume"]);
			}
		}

        await sendParams.SendAsync();
		await Task.Delay(100);


		await fcw.OutputWaveToFileAsync(
            Message,
            wavpath
        );
		UnityEngine.Debug.Log("終了");
        StartCoroutine("Play");

        //reset
		var current = await fcw.GetComponentsAsync();
		current.ToList().ForEach(c => { c.Value = 0; });
		await fcw.CreateParam()
            .Components(current)
            .Alpha(50)
			.Speed(50)
            .Tone(50)
            .ToneScale(50)
            .Volume(50)
			.SendAsync();
        UnityEngine.Debug.Log("RESET");
    }

    IEnumerator Play()
    {
        var source = this.GetComponent<AudioSource>();
        using (UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip("file://" + wavpath, AudioType.WAV))
        {
            ((DownloadHandlerAudioClip)req.downloadHandler).streamAudio = true;
            req.SendWebRequest();
            while (!req.isDone)
            {
                yield return null;
            }
            var outwav = DownloadHandlerAudioClip.GetContent(req);
            source.clip = outwav;
            emote_time = source.clip.length;
            source.Play();
            UImanager.talking = true;
            UImanager.thinking = false;
            StartCoroutine("Talking_Off");
        }
    }

    IEnumerator Talking_Off()
    {
        yield return new WaitForSeconds(emote_time);
        UImanager.talking = false;
        if(System.IO.File.Exists(wavpath)){
			System.IO.File.Delete(wavpath);
		}
    }

}