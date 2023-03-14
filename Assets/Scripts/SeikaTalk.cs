using System.Diagnostics;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;

//SeikaTalk�Ŕ�������X�N���v�g
public class SeikaTalk : MonoBehaviour
{
    public string Message;
    public string exepath;
    public string narrator;
    public string AudioDevice;
    public AudioSource source;
    public Process SeikaSay2;

    public void Awake()
    {
        GameObject Game_system = GameObject.FindGameObjectWithTag("Game_system");
        SystemSetting SystemSetting = Game_system.GetComponent<SystemSetting>();
        exepath = SystemSetting.SeikaSay2_exe;
        narrator = "\"" + SystemSetting.AssistantSeika_narrator + "\"";
        AudioDevice = SystemSetting.AudioDevice;
    }
    public void SeikaTalkStart()
    {
        Message = "\"" + EditorRunTerminal.Message + "\"";
        UImanager.talking = true;
        UImanager.thinking = false;
        source = this.GetComponent<AudioSource>();
        source.clip = Microphone.Start(AudioDevice, false, EditorRunTerminal.Message.Length, 44100);
        Invoke("wait", 0.5f);
    }
    public void wait()
    {
        source.Play();
        var _ = SeikaSay2Run();
    }
    private async Task SeikaSay2Run()
    {
        UnityEngine.Debug.Log("�J�n");
        await Task.Run(() =>
        {
            SeikaSay2 = new Process();
            SeikaSay2.StartInfo.FileName = exepath;
            SeikaSay2.StartInfo.Arguments = "-async -cid " + narrator + " -t " + Message;
            //WindowStyle��Minimized���w�肵�āA�ŏ������ꂽ��ԂŋN�������悤�ɂ���
            SeikaSay2.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;
            SeikaSay2.Start();
            var emote_time = EditorRunTerminal.Message.Length * 300;
            Thread.Sleep(emote_time);
        });
        UnityEngine.Debug.Log("�I��");
        UImanager.talking = false;
        source.Stop();
    }
}
