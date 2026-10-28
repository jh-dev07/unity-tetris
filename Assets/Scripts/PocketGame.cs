using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace PocketBlocks {
public sealed class PocketGame : MonoBehaviour {
    BlocksEngine engine; BoardGraphic board,hold;BoardGraphic[] previews=new BoardGraphic[3];
    Text score,stats,bestText,message,soundText,vibeText; GameObject overlay;RectTransform safe;
    Font font;AudioSource audioSource;AudioClip click,clear;bool playing,paused,sound,vibration;int best;
    float fall,lockTime;int lockResets;Rect previousSafe;Vector2 previousScreen;
    Color bg=new Color32(9,16,29,255),panel=new Color32(23,35,54,255),accent=new Color32(107,224,169,255);
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]static void Boot(){if(FindFirstObjectByType<PocketGame>()==null)new GameObject("Pocket Blocks").AddComponent<PocketGame>();}
    void Awake(){Application.targetFrameRate=60;Screen.sleepTimeout=SleepTimeout.SystemSetting;font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");best=PlayerPrefs.GetInt("best");sound=PlayerPrefs.GetInt("sound",1)==1;vibration=PlayerPrefs.GetInt("vibration",0)==1;
        if(FindFirstObjectByType<Camera>()==null){var cameraObject=new GameObject("Background camera",typeof(Camera));cameraObject.transform.SetParent(transform);var cam=cameraObject.GetComponent<Camera>();cam.clearFlags=CameraClearFlags.SolidColor;cam.backgroundColor=bg;cam.cullingMask=0;}
        if(FindFirstObjectByType<AudioListener>()==null)gameObject.AddComponent<AudioListener>();
        audioSource=gameObject.AddComponent<AudioSource>();audioSource.playOnAwake=false;audioSource.spatialBlend=0;click=Tone(440,.055f);clear=Tone(880,.18f);BuildUI();LayoutSafe();ShowMenu("POCKET BLOCKS","A little focus. A new high score.");}
    AudioClip Tone(float hz,float duration){int n=(int)(22050*duration);float[] data=new float[n];for(int i=0;i<n;i++)data[i]=Mathf.Sin(2*Mathf.PI*hz*i/22050)*.12f*(1f-(float)i/n);var c=AudioClip.Create("Local tone",n,1,22050,false);c.SetData(data,0);return c;}
    void Beep(bool line=false){if(sound)audioSource.PlayOneShot(line?clear:click);}
    RectTransform Box(string name,Transform parent,float x,float y,float w,float h,Color? color=null){var go=new GameObject(name,typeof(RectTransform));go.transform.SetParent(parent,false);var rt=go.GetComponent<RectTransform>();rt.anchorMin=rt.anchorMax=new Vector2(.5f,1);rt.pivot=new Vector2(.5f,1);rt.anchoredPosition=new Vector2(x,-y);rt.sizeDelta=new Vector2(w,h);if(color.HasValue)go.AddComponent<Image>().color=color.Value;return rt;}
    Text Label(string text,Transform parent,float x,float y,float w,float h,int size,Color? color=null){var rt=Box(text,parent,x,y,w,h);var t=rt.gameObject.AddComponent<Text>();t.font=font;t.text=text;t.fontSize=size;t.color=color??Color.white;t.alignment=TextAnchor.MiddleCenter;t.raycastTarget=false;return t;}
    Text Button(string text,Transform parent,float x,float y,float w,float h,Action action,bool repeat=false,Color? color=null){var rt=Box(text,parent,x,y,w,h,color??panel);var b=rt.gameObject.AddComponent<RepeatButton>();b.Press=action;b.Repeat=repeat;return Label(text,rt,0,0,w,h,24);}
    BoardGraphic Preview(Transform parent,float x,float y){var rt=Box("Piece",parent,x,y,114,76);var p=rt.gameObject.AddComponent<BoardGraphic>();p.IsPreview=true;p.raycastTarget=false;return p;}
    void BuildUI(){var canvasGO=new GameObject("Canvas",typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));canvasGO.transform.SetParent(transform);var canvas=canvasGO.GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;var scaler=canvasGO.GetComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(720,1280);scaler.matchWidthOrHeight=0;
        var background=Box("Background",canvas.transform,0,0,720,4000,bg);
        if(FindFirstObjectByType<EventSystem>()==null)new GameObject("Events",typeof(EventSystem),typeof(StandaloneInputModule));
        safe=Box("Safe area",canvas.transform,0,0,720,1280);safe.anchorMin=Vector2.zero;safe.anchorMax=Vector2.one;safe.pivot=new Vector2(.5f,.5f);safe.offsetMin=safe.offsetMax=Vector2.zero;
        // Entire composition is scaled inside the physical safe area.
        var content=Box("Content",safe,0,0,720,1240);content.anchorMin=content.anchorMax=new Vector2(.5f,.5f);content.pivot=new Vector2(.5f,.5f);content.anchoredPosition=Vector2.zero;
        Label("P O C K E T  /  B L O C K S",content,0,12,600,44,23,accent);
        score=Label("0",content,0,56,300,70,52);stats=Label("LEVEL 1   /   0 LINES",content,0,126,600,36,19,new Color32(159,178,201,255));
        Button("II",content,288,60,72,66,TogglePause);
        Label("HOLD",content,-278,200,126,32,18);hold=Preview(content,-278,240);
        Label("NEXT",content,278,200,126,32,18);for(int i=0;i<3;i++)previews[i]=Preview(content,278,240+i*100);
        var br=Box("Board",content,0,190,420,840);board=br.gameObject.AddComponent<BoardGraphic>();var gesture=br.gameObject.AddComponent<BoardGesture>();gesture.Move=dx=>Move(dx);gesture.Turn=Rotate;gesture.Down=Soft;gesture.Drop=Drop;
        message=Label("TAP TO ROTATE  /  SWIPE TO MOVE",content,0,1038,690,34,17,new Color32(159,178,201,255));
        Button("HOLD",content,-278,870,126,70,Swap);
        Button("<",content,-252,1090,108,88,()=>Move(-1),true);Button(">",content,-126,1090,108,88,()=>Move(1),true);
        Button("TURN",content,12,1090,138,88,Rotate);Button("DOWN",content,168,1090,138,88,Soft,true);Button("DROP",content,300,1090,102,88,Drop,false,new Color32(39,104,87,255));
        bestText=Label("BEST  "+best.ToString("N0")+"    •    OFFLINE",content,0,1194,650,30,18,new Color32(159,178,201,255));
        overlay=Box("Menu",content,0,0,720,1240,new Color(0.035f,.06f,.105f,.98f)).gameObject;
    }
    Text menuTitle,menuSubtitle;Text resumeLabel;
    void ShowMenu(string title,string subtitle){overlay.SetActive(true);if(menuTitle==null){menuTitle=Label("",overlay.transform,0,260,680,80,43,accent);menuSubtitle=Label("",overlay.transform,0,354,640,100,23);resumeLabel=Button("CONTINUE",overlay.transform,0,500,510,88,Continue,false,new Color32(39,104,87,255));Button("NEW GAME",overlay.transform,0,606,510,88,NewGame);soundText=Button("",overlay.transform,-133,720,244,72,()=>{sound=!sound;PlayerPrefs.SetInt("sound",sound?1:0);Preferences();});vibeText=Button("",overlay.transform,133,720,244,72,()=>{vibration=!vibration;PlayerPrefs.SetInt("vibration",vibration?1:0);Preferences();});Label("Move with buttons or swipe the board.\nTap the board or TURN to rotate.\nDROP lands instantly. HOLD saves a piece.\nComplete rows to score and level up.",overlay.transform,0,840,620,170,23);Label("NO ADS  /  NO ACCOUNT  /  NO CONNECTION",overlay.transform,0,1070,660,40,17,new Color32(159,178,201,255));}
        menuTitle.text=title;menuSubtitle.text=subtitle;resumeLabel.transform.parent.gameObject.SetActive((playing&&!engine.Over)||PlayerPrefs.HasKey("run"));Preferences();}
    void Preferences(){soundText.text="SOUND  "+(sound?"ON":"OFF");vibeText.text="VIBRATE  "+(vibration?"ON":"OFF");PlayerPrefs.Save();}
    void NewGame(){engine=new BlocksEngine(Environment.TickCount);playing=true;paused=false;overlay.SetActive(false);fall=lockTime=0;lockResets=0;Screen.sleepTimeout=SleepTimeout.NeverSleep;Refresh();Save();}
    void Continue(){if(!playing||engine.Over){try{engine=BlocksEngine.Restore(JsonUtility.FromJson<BlocksEngine.Snapshot>(PlayerPrefs.GetString("run")));playing=true;}catch{PlayerPrefs.DeleteKey("run");NewGame();return;}}paused=false;overlay.SetActive(false);Screen.sleepTimeout=SleepTimeout.NeverSleep;Refresh();}
    bool Active {get{return playing&&!paused&&!engine.Over;}}
    void ResetLock(){if(lockResets<15){lockTime=0;lockResets++;}}
    void Move(int dx){if(!Active)return;if(engine.Move(dx,0)){ResetLock();Refresh();}}
    void Rotate(){if(!Active)return;if(engine.Rotate()){ResetLock();Beep();Refresh();}}
    void Soft(){if(!Active)return;if(engine.Move(0,-1)){engine.Score++;Refresh();}}
    void Swap(){if(!Active)return;engine.SwapHold();fall=lockTime=0;lockResets=0;After(0);}
    void Drop(){if(!Active)return;int n=engine.Drop();After(n);}
    void After(int lines){fall=lockTime=0;lockResets=0;Beep(lines>0);if(vibration&&Application.platform==RuntimePlatform.Android)Handheld.Vibrate();message.text=lines>0?(lines==4?"FOUR LINES!":"+ "+lines+" LINE"+(lines>1?"S":"")):"TAP TO ROTATE  /  SWIPE TO MOVE";Refresh();if(engine.Over){PlayerPrefs.DeleteKey("run");PlayerPrefs.Save();paused=true;Screen.sleepTimeout=SleepTimeout.SystemSetting;ShowMenu("NICE RUN",engine.Score.ToString("N0")+" points  /  "+engine.Lines+" lines\nReady for another round?");}else Save();}
    void Refresh(){board.Engine=engine;board.SetVerticesDirty();hold.Preview=engine.Hold;hold.SetVerticesDirty();var next=engine.Next;for(int i=0;i<3;i++){previews[i].Preview=next[i];previews[i].SetVerticesDirty();}score.text=engine.Score.ToString("N0");stats.text="LEVEL "+engine.Level+"   /   "+engine.Lines+" LINES";if(engine.Score>best){best=engine.Score;PlayerPrefs.SetInt("best",best);}bestText.text="BEST  "+best.ToString("N0")+"    •    OFFLINE";}
    void TogglePause(){if(!playing||engine.Over)return;if(paused){Continue();return;}paused=true;Save();Screen.sleepTimeout=SleepTimeout.SystemSetting;ShowMenu("TAKE A BREATH","Your game is saved on this device.");}
    void Save(){if(playing&&!engine.Over){PlayerPrefs.SetString("run",JsonUtility.ToJson(engine.Save()));PlayerPrefs.Save();}}
    void OnApplicationPause(bool value){if(value&&Active)TogglePause();}void OnApplicationFocus(bool value){if(!value&&Active)TogglePause();}void OnApplicationQuit(){Save();}
    void LayoutSafe(){Rect a=Screen.safeArea;if(Screen.width==0||Screen.height==0)return;safe.anchorMin=new Vector2(a.xMin/Screen.width,a.yMin/Screen.height);safe.anchorMax=new Vector2(a.xMax/Screen.width,a.yMax/Screen.height);var content=(RectTransform)safe.GetChild(0);float width=a.width*720/Screen.width,height=a.height*720/Screen.width;content.localScale=Vector3.one*Mathf.Min(width/720,height/1240);previousSafe=a;previousScreen=new Vector2(Screen.width,Screen.height);}
    void Update(){if(previousSafe!=Screen.safeArea||previousScreen!=new Vector2(Screen.width,Screen.height))LayoutSafe();if(Input.GetKeyDown(KeyCode.Escape)){TogglePause();return;}if(!Active)return;
        if(Input.GetKeyDown(KeyCode.LeftArrow))Move(-1);if(Input.GetKeyDown(KeyCode.RightArrow))Move(1);if(Input.GetKeyDown(KeyCode.UpArrow)||Input.GetKeyDown(KeyCode.X))Rotate();if(Input.GetKeyDown(KeyCode.C))Swap();if(Input.GetKeyDown(KeyCode.Space))Drop();if(Input.GetKey(KeyCode.DownArrow))Soft();if(!Active)return;
        float dt=Mathf.Min(Time.unscaledDeltaTime,.1f);fall+=dt;float interval=Mathf.Max(.075f,.8f*Mathf.Pow(.83f,engine.Level-1));if(fall>=interval){fall=0;if(engine.Move(0,-1))Refresh();}
        if(!engine.Fits(engine.Kind,engine.Rotation,engine.X,engine.Y-1)){lockTime+=dt;if(lockTime>=.5f)After(engine.Lock());}else lockTime=0;
    }
}}
