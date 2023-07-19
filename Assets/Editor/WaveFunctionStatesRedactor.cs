using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class WaveFunctionStatesRedactor : EditorWindow
{
    WaveFunctionView waveView;
    // [MenuItem("Window/WaveFunctionStatesRedactor")]
    public static void RedactWaveFuction(WaveFunctionCollapse waveFunction)
    {
        WaveFunctionStatesRedactor wnd = GetWindow<WaveFunctionStatesRedactor>();
        wnd.titleContent = new GUIContent("WaveFunctionStatesRedactor");
        // wnd.target = waveFunction;
        // Debug.Log(wnd.target);

        WaveFunctionView waveView = wnd.waveView;
        waveView.target = waveFunction;
        waveView.GenerateStateNodes();
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        // Debug.Log(target);
        waveView = new WaveFunctionView();
        waveView.StretchToParentSize();
        root.Add(waveView);
    }
}