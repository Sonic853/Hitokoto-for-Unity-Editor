// Sonic853编写了此代码。
// 禁止在Assetstore售卖该代码！
// 否则我将向Unity提出DMCA诉讼要求！
// Sonic853 wrote this code.
// It is forbidden to sell this code in Assetstore!
// Otherwise I will file a DMCA lawsuit with Unity!
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using Unity.EditorCoroutines.Editor;
using System;
using System.Collections;

public class EditorHitokoto
{
    static EditorCoroutine m_HitokotoCoroutine;
    static bool m_force = false;
    static HitokotoSetting hitokotoSetting;
    [InitializeOnLoad]
    public class Startup {
        static Startup()
        {
            if(EditorPrefs.GetString("HitokotoJ_853lab")==""){
                hitokotoSetting = new HitokotoSetting{disable = 0};
                EditorPrefs.SetString("HitokotoJ_853lab",JsonUtility.ToJson(hitokotoSetting));
                Debug.Log("从现在起，这里会显示一言哦！\n一言项目：https://github.com/Sonic853/Unity-Editor-Hitokoto\n这条信息只会显示一次_(:з」∠)_");
            }else{
                hitokotoSetting = JsonUtility.FromJson<HitokotoSetting>(EditorPrefs.GetString("HitokotoJ_853lab"));
            }
            GetHitokoto();
        }
    }
    [InitializeOnLoadAttribute]
    public static class PlayModeChangedShowHitokoto
    {
        static PlayModeChangedShowHitokoto()
        {
            EditorApplication.playModeStateChanged += LogPlayModeState;
        }

        private static void LogPlayModeState(PlayModeStateChange state)
        {
            if(state == PlayModeStateChange.EnteredEditMode){
                GetHitokoto();
            }
        }
    }
    // [InitializeOnLoadMethod]
    // static void InitializeOnLoadMethod()
    // {
    //     EditorApplication.wantsToQuit -= Quit;
    //     EditorApplication.wantsToQuit += Quit;
    // }
    // static bool Quit(){
    //     GetHitokoto(true);
    //     return true;
    // }
    [MenuItem("Hitokoto/显示一言 _F1")]
    private static void MenuItemHitokoto()
    {
        GetHitokoto(true);
    }
    [MenuItem("Hitokoto/禁用自动显示")]
    private static void DisableHitokoto()
    {
        hitokotoSetting.disable = 1;
        EditorPrefs.SetString("HitokotoJ_853lab",JsonUtility.ToJson(hitokotoSetting));
    }
    [MenuItem("Hitokoto/禁用自动显示", true)]
    private static bool CheckDisableHitokoto()
    {
        if(hitokotoSetting.disable != 1)
        {
            return true;
        }
        return false;
    }
    [MenuItem("Hitokoto/启用自动显示")]
    private static void EnableHitokoto()
    {
        hitokotoSetting.disable = 0;
        EditorPrefs.SetString("HitokotoJ_853lab",JsonUtility.ToJson(hitokotoSetting));
    }
    [MenuItem("Hitokoto/启用自动显示", true)]
    private static bool CheckEnableHitokoto()
    {
        if(hitokotoSetting.disable != 0)
        {
            return true;
        }
        return false;
    }
    private static void GetHitokoto(){
        m_HitokotoCoroutine = EditorCoroutineUtility.StartCoroutineOwnerless(MakeCoroutine());
    }
    private static void GetHitokoto(bool force){
        m_force = force;
        m_HitokotoCoroutine = EditorCoroutineUtility.StartCoroutineOwnerless(MakeCoroutine());
    }
    static IEnumerator MakeCoroutine(){
        if((hitokotoSetting.disable != 1&&!EditorApplication.isPlaying)||m_force){
            m_force = false;
            UnityWebRequest response = UnityWebRequest.Get("https://v1.hitokoto.cn/?c=c");
            response.SendWebRequest();
            while(!response.isDone){
            }
            if(response.downloadHandler.text!=""){
                Hitokoto hitokoto = JsonUtility.FromJson<Hitokoto>(response.downloadHandler.text);
                Debug.Log(hitokoto.hitokoto+"\n出自：「"+hitokoto.from+"」\n<a>https://hitokoto.cn?id="+hitokoto.id+"</a>");
            }else{
                Debug.Log("一言加载失败");
            }
        }
        yield return null;
        EditorCoroutineUtility.StopCoroutine(m_HitokotoCoroutine);
    }
    [Serializable]
    public class Hitokoto{
        [SerializeField]
        public int id;

        [SerializeField]
        public string hitokoto;

        [SerializeField]
        public string from;
    }
    [Serializable]
    public class HitokotoSetting{
        [SerializeField]
        public int disable = 0;
    }
}
