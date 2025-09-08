using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UIElements;

/* Manages the window in which the DialogueGraphView is displayed. 
 * Uses DialogueGraphView class
 */

public class InheritanceGraph : EditorWindow
{
    private InhertanceGraphView _graphView;
    private String _fileName = "New Narrative";

    [MenuItem("Graph/Dialogue Graph")] //makes method available as top bar menu option. Method must be static to do this. 
    public static void OpenDialogieGraphViewWindow()
        //this method will open a window containting my GraphView
    {
        //opens the window defined by this class. puts it in 'window'
        var window = GetWindow<InheritanceGraph>();
        //sets title
        window.titleContent = new GUIContent("Dialogue Graph");
    }


    private void OnEnable() //called when window is opened
    {
        ConstructGraphView();
        GenerateToolbar();
    }

    private void ConstructGraphView()
    {
        //create a DialogueGraphView object
        _graphView = new InhertanceGraphView
        {
            name = "HoogaBooga" //in contructor of GraphView (i think)
        };

        _graphView.StretchToParentSize(); //Adjust GraphView size to fit window. 

        //Actually adds the DialogueGraphView to the window.
        rootVisualElement.Add(_graphView);
    }


    private void GenerateToolbar()
    {
        var toolbar = new Toolbar();// included in Unity

        //creat editable text field for file name. 
        var fileNameTextField = new TextField("File Name:");
        fileNameTextField.SetValueWithoutNotify(_fileName);
        fileNameTextField.MarkDirtyRepaint(); //Triggers a repaint of the VisualElement on the next frame.
        fileNameTextField.RegisterValueChangedCallback(evt => _fileName = evt.newValue); //triggers event when text value is changed. 
        toolbar.Add(fileNameTextField); //add textfield to toolbar

        toolbar.Add(new Button( () => RequestDataOperation(true)) { text = "SaveData" });        
        toolbar.Add(new Button( () => RequestDataOperation(false)) { text = "LoadData" });

        //create a button object
        var nodeCreateButton = new Button(
            () => { _graphView.CreateNode("Dialogue Node"); } //the funtion called by the button
        );
        nodeCreateButton.text = "Create Node";

        toolbar.Add(nodeCreateButton); //add button to toolbar
        rootVisualElement.Add(toolbar); //add toolbar to window
    }

    private void RequestDataOperation(bool save) // Save or Load Data
    {
        if (string.IsNullOrEmpty(_fileName))
        {
            EditorUtility.DisplayDialog("Invalid file name!", "Please enter valid file name.", "OK");
            return;
        }
        var saveUtility = InheritGraphSaveUtility.GetInstance(_graphView);
        if (save)
        {
            saveUtility.SaveGraph(_fileName);
        }
        else
        {
            saveUtility.LoadGraph(_fileName);
        }
    }

    private void OnDisable() //called when window is closed
    {
        //removes the DialogueGraphView from the window.
        rootVisualElement.Remove(_graphView);
    }
}
