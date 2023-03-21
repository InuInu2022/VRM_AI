using System.Text.RegularExpressions;
using UnityEngine;

//ボイスアプリに連携するスクリプト
public class CallVoice : MonoBehaviour
{
    public string VoiceApp;
    public void Awake()
    {
        GameObject Game_system = GameObject.FindGameObjectWithTag("Game_system");
        SystemSetting SystemSetting = Game_system.GetComponent<SystemSetting>();
        VoiceApp = SystemSetting.VoiceApp;
    }
    public void Speak()
    {
        if (VoiceApp == "VoiceVox")
        {
            VoiceVox VoiceVox = this.gameObject.GetComponent<VoiceVox>();
            VoiceVox.VoiceVoxStart();
        }
        else if (VoiceApp == "VoicePeak")
        {
            VoicePeak VoicePeak = this.gameObject.GetComponent<VoicePeak>();
            VoicePeak.VoicePeakStart();
        }
        else if (VoiceApp == "COEIROINK")
        {
            COEIROINK COEIROINK = this.gameObject.GetComponent<COEIROINK>();
            COEIROINK.COEIROINKStart();
        }
        else if (VoiceApp == "AssistantSeika")
        {
            SeikaTalk SeikaTalk = this.gameObject.GetComponent<SeikaTalk>();
            SeikaTalk.SeikaTalkStart();
        }
        else if (Regex.IsMatch(
            VoiceApp,
            @"CeVIO",
            RegexOptions.IgnoreCase
        )){
			var voiceApp = this.gameObject
				.GetComponent<CeVIO>();
			UnityEngine.Debug.Log($"voiceApp:{voiceApp}");
    		voiceApp.AppStart();
		}
    }
}