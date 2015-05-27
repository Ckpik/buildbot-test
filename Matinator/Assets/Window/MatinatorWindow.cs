using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.Linq;
//using System.Drawing;
//using Assets.Window;

public class MatinatorWindow : EditorWindow
{
    private List<Key> keyList;
    Component[] compoArray;
    private GameObject oldObject;
	private Save data;

    /* Component of Selected Object */
    private bool transformOk;
    private bool cameraOk;
    private bool lightOk;
    private bool rigidbodyOk;
    private bool meshRdrOk;
    private bool particleSystOk;

    /* Dopesheet */
    private float time = 0.00f;
    private float save = 0.00f;
    private Texture2D keyTexture;
    private Key currentKey;
    private bool onMouseDown;
    private int dsPos;

    /* Curves */
    private AnimationCurve[] anim;

    /* Save */
    Matinator scriptAnimation;

    [MenuItem("Window/Matinator")]
    public static void OpenWindow()
    {
        MatinatorWindow window = CreateInstance<MatinatorWindow>();
        window.title = "Matinator";
        window.Show();
    }

    void OnEnable()
    {
        keyTexture = Resources.Load("key", typeof(Texture2D)) as Texture2D;
        anim = new AnimationCurve[9];
        if (Selection.activeGameObject != null)
            SetParametersForNewObject();
        dsPos = 0;
		if (data == null)
			data = ScriptableObject.CreateInstance("Save") as Save;
    }

    void SetParametersForNewObject()
    {
        scriptAnimation = Selection.activeGameObject.GetComponent<Matinator>();
        if (scriptAnimation == null)
            scriptAnimation = Selection.activeGameObject.AddComponent("Matinator") as Matinator;
        scriptAnimation.SetParameters();
        keyList = new List<Key>();
        for (int i = 0; i < 9; ++i)
            anim[i] = AnimationCurve.Linear(0.0f, 0.0f, 10.0f, 0.0f);
        currentKey = null;
        onMouseDown = false;
        CheckComponent();
        SetCurvesTransform();
    }

    void SetCurvesTransform()
    {
        anim[0] = AnimationCurve.Linear(0.0f, Selection.activeGameObject.transform.position.x, 1.0f, Selection.activeGameObject.transform.position.x);
        anim[1] = AnimationCurve.Linear(0.0f, Selection.activeGameObject.transform.position.y, 1.0f, Selection.activeGameObject.transform.position.y);
        anim[2] = AnimationCurve.Linear(0.0f, Selection.activeGameObject.transform.position.z, 1.0f, Selection.activeGameObject.transform.position.z);
        anim[3] = AnimationCurve.Linear(0.0f, Selection.activeGameObject.transform.rotation.eulerAngles.x, 1.0f, Selection.activeGameObject.transform.rotation.eulerAngles.x);
        anim[4] = AnimationCurve.Linear(0.0f, Selection.activeGameObject.transform.rotation.eulerAngles.y, 1.0f, Selection.activeGameObject.transform.rotation.eulerAngles.y);
        anim[5] = AnimationCurve.Linear(0.0f, Selection.activeGameObject.transform.rotation.eulerAngles.z, 1.0f, Selection.activeGameObject.transform.rotation.eulerAngles.z);
        anim[6] = AnimationCurve.Linear(0.0f, Selection.activeGameObject.transform.localScale.x, 1.0f, Selection.activeGameObject.transform.localScale.x);
        anim[7] = AnimationCurve.Linear(0.0f, Selection.activeGameObject.transform.localScale.y, 1.0f, Selection.activeGameObject.transform.localScale.y);
        anim[8] = AnimationCurve.Linear(0.0f, Selection.activeGameObject.transform.localScale.z, 1.0f, Selection.activeGameObject.transform.localScale.z);
    }

    void CheckComponent()
    {
        transformOk = false;
        cameraOk = false;
        lightOk = false;
        rigidbodyOk = false;
        meshRdrOk = false;
        particleSystOk = false;

        compoArray = Selection.activeGameObject.GetComponents<Component>();
        foreach (Component c in compoArray)
        {
            System.Type type = c.GetType();
            if (type == typeof(UnityEngine.Transform))
                transformOk = true;
            else if (type == typeof(UnityEngine.Camera))
                cameraOk = true;
            else if (type == typeof(UnityEngine.Light))
                lightOk = true;
            else if (type == typeof(UnityEngine.Rigidbody))
                rigidbodyOk = true;
            else if (type == typeof(UnityEngine.MeshRenderer))
                meshRdrOk = true;
            else if (type == typeof(UnityEngine.ParticleSystem))
                particleSystOk = true;
        }
    }

    void Update()
    {
        Repaint();
    }

    void OnGUI()
    {
        if (CheckSelectionObject())
        {

            DisplayBasicGUI();
            CheckEvent();
            DisplayDopesheetGUI();
            DisplayDynamicGUI();
            Dopesheet();

            if (scriptAnimation != null)
                scriptAnimation.Update();
        }
        oldObject = Selection.activeGameObject;
		if (GUI.Button(new Rect(25, 23, 50, 14), "save"))
		{
			data.Init(keyList);
			data.CreateAsset<Save>(data);
		}
		if (GUI.Button(new Rect(90, 23, 50, 14), "load"))
		{
			data.LoadAsset();
		}
    }

    private bool CheckSelectionObject()
    {
        if (Selection.activeGameObject == null)
            return false;
        if (oldObject != Selection.activeGameObject)
            SetParametersForNewObject();
        return true;
    }

    #region GUI

    private void DisplayBasicGUI()
    {
        if (GUI.Button(new Rect(20, 5, 60, 15), "Preview"))
            Save();
        if (GUI.Button(new Rect(85, 5, 60, 15), "Stop"))
            scriptAnimation.StopTime();
        if (GUI.Button(new Rect(150, 5, 60, 15), "Sync."))
            ReorganizeDopesheet(Key.KeyType.E_POS_X);
        DisplayTransformParameters();
        DisplayCameraParameters();
        DisplayLightParameters();
        DisplayRigidbodyParameters();
        DisplayMeshRendererParameters();
        DisplayParticleSystemParameters();
    }

    private void DisplayTransformParameters()
    {
        if (transformOk)
        {
            GUI.Label(new Rect(18, 35, 100, 15), "Transform");

            CreateOneLineParameterGUI("Position X", 55, Key.KeyType.E_POS_X);
            CreateOneLineParameterGUI("Position Y", 70, Key.KeyType.E_POS_Y);
            CreateOneLineParameterGUI("Position Z", 85, Key.KeyType.E_POS_Z);

            CreateOneLineParameterGUI("Rotation X", 105, Key.KeyType.E_ROT_X);
            CreateOneLineParameterGUI("Rotation Y", 120, Key.KeyType.E_ROT_Y);
            CreateOneLineParameterGUI("Rotation Z", 135, Key.KeyType.E_ROT_Z);

            CreateOneLineParameterGUI("Scale X", 155, Key.KeyType.E_SCALE_X);
            CreateOneLineParameterGUI("Scale Y", 170, Key.KeyType.E_SCALE_Y);
            CreateOneLineParameterGUI("Scale Z", 185, Key.KeyType.E_SCALE_Z);
        }
    }

    private void DisplayCameraParameters()
    {
        if (cameraOk)
        {
            GUI.Label(new Rect(18, 205, 100, 15), "Camera");
        }
    }

    private void DisplayLightParameters()
    {
        if (lightOk)
        {
            GUI.Label(new Rect(18, 225, 100, 15), "Light");
        }
    }

    private void DisplayRigidbodyParameters()
    {
        if (rigidbodyOk)
        {
            GUI.Label(new Rect(18, 245, 100, 15), "Rigidbody");
        }
    }

    private void DisplayMeshRendererParameters()
    {
        if (meshRdrOk)
        {
            GUI.Label(new Rect(18, 265, 100, 15), "Mesh Renderer");
        }
    }

    private void DisplayParticleSystemParameters()
    {
        if (particleSystOk)
        {
            GUI.Label(new Rect(18, 285, 100, 15), "Particle System");
        }
    }

    private void CreateOneLineParameterGUI(string name, int y, Key.KeyType type)
    {
        GUI.Label(new Rect(22, y, 70, 30), name);
        AnimationCurve tmp = anim[(int)type];
        tmp = EditorGUI.CurveField(new Rect(90, y, 80, 15), tmp);
        anim[(int)type] = tmp;
        if (currentKey != null && currentKey.type == type)
        {
            float.TryParse(GUI.TextArea(new Rect(180, y, 40, 15), currentKey.value.ToString()), out currentKey.value);
            int index = FindKeyframe(currentKey);
            anim[(int)currentKey.type].MoveKey(index, new Keyframe(currentKey.time, currentKey.value));
        }
        else
            GUI.TextField(new Rect(180, y, 40, 15), "--");
    }

    private void DisplayDopesheetGUI()
    {
        float windowWidth = position.width;

        DisplayOneBoxDopesheet(windowWidth, 25, Key.KeyType.E_EVENT);

        DisplayOneBoxDopesheet(windowWidth, 55, Key.KeyType.E_POS_X);
        DisplayOneBoxDopesheet(windowWidth, 70, Key.KeyType.E_POS_Y);
        DisplayOneBoxDopesheet(windowWidth, 85, Key.KeyType.E_POS_Z);

        DisplayOneBoxDopesheet(windowWidth, 105, Key.KeyType.E_ROT_X);
        DisplayOneBoxDopesheet(windowWidth, 120, Key.KeyType.E_ROT_Y);
        DisplayOneBoxDopesheet(windowWidth, 135, Key.KeyType.E_ROT_Z);

        DisplayOneBoxDopesheet(windowWidth, 155, Key.KeyType.E_SCALE_X);
        DisplayOneBoxDopesheet(windowWidth, 170, Key.KeyType.E_SCALE_Y);
        DisplayOneBoxDopesheet(windowWidth, 185, Key.KeyType.E_SCALE_Z);
    }

    private void DisplayOneBoxDopesheet(float windowWidth, int y, Key.KeyType type)
    {
        if (GUI.Button(new Rect(240, y, windowWidth - 242, 15), "") && currentKey == null)
        {
            Vector2 pos = Event.current.mousePosition;
            pos.y = y;
            Key key = new Key();
            key.SetValues(pos, 0.0f, 0.0f, type, -1, 0.0f, 0);
            keyList.Add(key);
            SetValueForKey(key);
            currentKey = key;
            if (type != Key.KeyType.E_EVENT)
               anim[(int)type].AddKey(key.time, key.value);
        }
    }

    private void DisplayDynamicGUI()
    {
        int nbKey = keyList.Count;
        for (int i = 0; i < nbKey; ++i)
        {
                GUI.DrawTexture(new Rect(keyList[i].pos.x, keyList[i].pos.y, 15, 15), keyTexture, ScaleMode.StretchToFill, true, 10.0f);
        }
    }

    #region Dope Sheet
    private void Dopesheet()
    {
        /*Pen blackPen = new Pen(Color.black, 3);
        Point	p1 = new Point(220, 5);
        Point	p2 = new Point(220, 500);
        Graphics.Drawline();*/

        float tot;
        int nbKey = keyList.Count;
        time = (float)(Math.Round((double)save, 2));

        // button left
        if (GUI.Button(new Rect(240, position.height - 20, 42, 20), "left"))
        {
            if (time > 0.09f)
            {
                for (int n = 0; n < nbKey; ++n)
                    keyList[n].nbCase++;
                save -= 0.1f;
                time = (float)(Math.Round((double)save, 2));
             //   moved = true;
                dsPos--;
            }
        }

        // button right
        if (GUI.Button(new Rect(position.width - 40, position.height - 20, 42, 20), "right"))
        {
            for (int n = 0; n < nbKey; ++n)
            {
                if (keyList[n].pos.x < 240 + position.width / 13)
                    keyList[n].pos.x = -16;
                keyList[n].nbCase--;
            }
            save += 0.1f;
            time = (float)(Math.Round((double)save, 2));
        //    moved = true;
            dsPos++;
        }

        // display timeline
        for (int i = 0; i < 13; i++)
        {
            float posX = 240 + (i * (position.width / 13));
            string timeName = System.Convert.ToString(time);
            GUI.Button(new Rect(posX, 5, (position.width / 13), 20), timeName);
            time = (float)(Math.Round((double)(time + 0.1f), 2));
        }

        for (int n = 0; n < nbKey; ++n)
        {
            if (keyList[n].nbCase > -1)
            {
                tot = (position.width / 13);
                keyList[n].pos.x = ((tot / 100) * keyList[n].pourcentage) + 240 + (keyList[n].nbCase * (position.width / 13));
            }
        }
    }

    public void SetValueForKey(Key key)
    {
        float tot = (position.width / 13);
        float cur;

        for (int i = 0; i < 13; i++)
        {
            float posX = 240 + (i * (position.width / 13));
            if (i > 0 && key.pos.x > posX && key.pos.x < posX + (i * tot))
            {
                cur = key.pos.x - posX;
                key.pourcentage = (cur / tot) * 100;
                key.nbTtlCase = i + dsPos;
                key.nbCase = i;
                key.nbTime = (i + dsPos) / 10.0f;
                key.time = (key.nbTime + (0.001f * key.pourcentage));
            }
            else if (i == 0 && key.pos.x > posX && key.pos.x < posX + (1 * tot))
            {
                cur = key.pos.x - posX;
                key.pourcentage = (cur / tot) * 100;
                key.nbTtlCase = i + dsPos;
                key.nbCase = i;
                key.nbTime = (i + dsPos) / 10.0f;
                key.time = (key.nbTime + (0.001f * key.pourcentage));
            }
        }
        //   Debug.Log("Key % = " + key.pourcentage + " Key Real Pos = " + key.nbTtlCase + " Key false pos = " + key.nbCase +  "Key nb time = " + key.nbTime + " Key time = " + key.time);
    }

    #endregion
    #endregion

    #region Event functions
    private void CheckEvent()
    {
        if (currentKey != null && onMouseDown)
        {
            int index = 0;
            if (currentKey.type != Key.KeyType.E_EVENT)
              index = FindKeyframe(currentKey);
            Vector2 pos = Event.current.mousePosition;
            if (pos.x < 240)
                currentKey.pos.x = 240;
            else if (pos.x >= position.width - 18)
                currentKey.pos.x = position.width - 18;
            else
                currentKey.pos.x = pos.x;
            SetValueForKey(currentKey);
            if (currentKey.type != Key.KeyType.E_EVENT)
                anim[(int)currentKey.type].MoveKey(index, new Keyframe(currentKey.time, currentKey.value));
        }
        else if (Event.current.rawType == EventType.MouseDown)
        {
            Vector2 pos = Event.current.mousePosition;
            int nbKey = keyList.Count;
            for (int i = 0; i < nbKey; ++i)
            {
                if ((pos.x >= keyList[i].pos.x && pos.x <= keyList[i].pos.x + 15) &&
                    (pos.y >= keyList[i].pos.y && pos.y <= keyList[i].pos.y + 15))
                {
                    currentKey = keyList[i];
                    onMouseDown = true;
                    if (currentKey.type == Key.KeyType.E_EVENT)
                    {
                        if (Event.current.button == 1)
                            EventWindow.CreateWizard(currentKey);
                    }
                    GUI.FocusControl(null);
                    break;
                }
                currentKey = null;
            }
        }
        if (Event.current.rawType == EventType.MouseUp)
            onMouseDown = false;
        if (currentKey != null && Event.current.rawType == EventType.KeyDown && Event.current.keyCode == KeyCode.Delete)
        {
            if (currentKey.type != Key.KeyType.E_EVENT)
            {
                int index = FindKeyframe(currentKey);
                anim[(int)currentKey.type].RemoveKey(index);
            }
            keyList.Remove(currentKey);
            currentKey = null;
        }
    }

    private void Save()
    {
        scriptAnimation.SetCurves(anim[0], (int)Key.KeyType.E_POS_X);
        scriptAnimation.SetCurves(anim[1], (int)Key.KeyType.E_POS_Y);
        scriptAnimation.SetCurves(anim[2], (int)Key.KeyType.E_POS_Z);
        scriptAnimation.SetCurves(anim[3], (int)Key.KeyType.E_ROT_X);
        scriptAnimation.SetCurves(anim[4], (int)Key.KeyType.E_ROT_Y);
        scriptAnimation.SetCurves(anim[5], (int)Key.KeyType.E_ROT_Z);
        scriptAnimation.SetCurves(anim[6], (int)Key.KeyType.E_SCALE_X);
        scriptAnimation.SetCurves(anim[7], (int)Key.KeyType.E_SCALE_Y);
        scriptAnimation.SetCurves(anim[8], (int)Key.KeyType.E_SCALE_Z);
        scriptAnimation.SetEvent(SaveEvent());
        scriptAnimation.StartTime();
    }

    private List<Key> SaveEvent()
    {
        List<Key> tmp = new List<Key>();

        foreach (Key k in keyList)
        {
            if (k.type == Key.KeyType.E_EVENT && k.function != null)
                tmp.Add(k);
        }
        return tmp;
    }

    private int FindKeyframe(Key toFind)
    {
        int index = 0;
        int type = (int)toFind.type;

        for (int i = 0; i < anim[type].length; ++i)
        {
            if (currentKey.time == anim[type].keys[i].time)
                index = i;
        }
        return index;
    }

    private void ReorganizeDopesheet(Key.KeyType type)
    {
        List<Key> tmp = new List<Key>();

        foreach (Key k in keyList)
        {
            if (k.type == type)
                tmp.Add(k);
        }
        foreach (Key k in tmp)
            keyList.Remove(k);
        foreach (Keyframe k in anim[(int)type].keys)
        {
           Key newKey = new Key();
            float round;
           round = (float)System.Math.Round(k.time, 1);
           if (round > k.time)
            round = (float)System.Math.Round(k.time - 0.1f, 1);

           newKey.SetValues(new Vector2(-100, 55), k.time, k.value, Key.KeyType.E_POS_X, -1, 0.0f, 0);
           newKey.nbTtlCase = (int)System.Math.Round(round / 0.1f, 0);
           if (newKey.nbTtlCase >= dsPos & newKey.nbTtlCase <= dsPos + 10)
               newKey.nbCase = newKey.nbTtlCase - dsPos;
            if (Math.Abs(k.time - round) < 0.00001)
                newKey.pourcentage = 0.0f;
            else
                newKey.pourcentage = Math.Abs(((k.time - round) / 0.1f) * 100);
            newKey.nbTime = round;
            Debug.Log("Value = " + newKey.value + " Time = " + newKey.time + " Percent = " + newKey.pourcentage + " Nbcase = " + newKey.nbCase + " nbtime = " + newKey.nbTime + " Nbttlecase = " + newKey.nbTtlCase);
           keyList.Add(newKey);
        }
    }
    #endregion
}
