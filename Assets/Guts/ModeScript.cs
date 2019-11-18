using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeScript : MonoBehaviour
{

    // ================================================ Mode Variables
    public int mode = 0;

    int playMode = 0;
    int mapMode = 1;
    int tokenMode = 2;
    int actionMode = 3;
    int effectMode = 4;
    int scriptMode = 5;
    int overlayMode = 6;
    int optionsMode = 7;
    int hamburgerMode = 8;

    int lastMode = 0;

    // ================================================ Object Refs
    public GameObject editorPanel;

    public PlayControlsScript playControls;
    public MapEditorScript mapEditor;
    public TokenEditorScript tokenEditor;
    public EditActionScript actionEditor;
    public EditEffectScript effectEditor;
    public RunScriptScript scriptEditor;
    public OverlayEditorScript overlayEditor;
    public OptionsScript optionsEditor;
    public HamburgerScript hamburgerControls;

    // ================================================
    // ================================================ Change Mode Functions
    // ================================================
    public void exitCurrentMode() {
        switch(mode) {
            case 0:
                exitPlayMode();
                break;
            case 1:
                exitMapMode();
                break;
            case 2:
                exitTokenMode();
                break;
            case 3:
                exitActionMode();
                break;
            case 4:
                exitEffectMode();
                break;
            case 5:
                exitScriptMode();
                break;
            case 6:
                exitOverlayMode();
                break;
            case 7:
                exitOptionsMode();
                break;
            case 8:
                exitHamburgerMode();
                break;
        }
    }

    public void startLastMode() {
        switch(lastMode) {
            case 0:
                startPlayMode();
                break;
            case 1:
                startMapMode();
                break;
            case 2:
                startTokenMode();
                break;
            case 3:
                startActionMode();
                break;
            case 4:
                startEffectMode();
                break;
            case 5:
                startScriptMode();
                break;
            case 6:
                startOverlayMode();
                break;
            case 7:
                startOptionsMode();
                break;
            case 8:
                startHamburgerMode();
                break;
        }
    }

    // ================================================ Play Mode
    public void startPlayMode() {
        playControls.gameObject.SetActive(true);
        playControls.clearInputs();
        mode = playMode;
    }

    public void exitPlayMode() {
        playControls.gameObject.SetActive(false);
        lastMode = playMode;
    }

    // ================================================ Map Mode
    public void startMapMode() {
        mapEditor.gameObject.SetActive(true);
        mapEditor.clearInput();
        mapEditor.changeActiveLayer();
        mode = mapMode;
    }

    public void exitMapMode() {
        mapEditor.gameObject.SetActive(false);
        lastMode = mapMode;
    }

    public void ToggleMapEditor() {
        if (mode == mapMode) {
            exitMapMode();
            startPlayMode();
        } else {
            exitCurrentMode();
            startMapMode();
        }
    }

    // ================================================ Token Mode
    public void startTokenMode() {
        tokenEditor.gameObject.SetActive(true);
        tokenEditor.clearInput();
        // tokenEditor.changeActiveLayer();
        mode = tokenMode;
    }

    public void exitTokenMode() {
        tokenEditor.gameObject.SetActive(false);
        lastMode = tokenMode;
    }

    public void ToggleTokenMode() {
        if (mode == tokenMode) {
            exitTokenMode();
            startPlayMode();
        } else {
            exitCurrentMode();
            startTokenMode();
        }
    }

    // ================================================ Action Mode
    public void startActionMode() {
        actionEditor.gameObject.SetActive(true);
        mode = actionMode;
    }

    public void exitActionMode() {
        actionEditor.gameObject.SetActive(false);
        lastMode = actionMode;
    }

    public void ToggleActionMode() {
        if (mode == actionMode) {
            exitActionMode();
            startPlayMode();
        } else {
            exitCurrentMode();
            startActionMode();
        }
    }

    // ================================================ Effect Mode
    public void startEffectMode() {
        effectEditor.gameObject.SetActive(true);
        mode = effectMode;
    }

    public void exitEffectMode() {
        effectEditor.gameObject.SetActive(false);
        lastMode = effectMode;
    }

    public void ToggleEffectMode() {
        if (mode == effectMode) {
            exitEffectMode();
            startPlayMode();
        } else {
            exitCurrentMode();
            startEffectMode();
        }
    }

    // ================================================ Script Mode
    public void startScriptMode() {
        scriptEditor.gameObject.SetActive(true);
        mode = scriptMode;
    }

    public void exitScriptMode() {
        scriptEditor.gameObject.SetActive(false);
        lastMode = scriptMode;
    }

    public void ToggleScriptMode() {
        if (mode == scriptMode) {
            exitScriptMode();
            startPlayMode();
        } else {
            exitCurrentMode();
            startScriptMode();
        }
    }

    // ================================================ Overlay Mode
    public void startOverlayMode() {
        overlayEditor.gameObject.SetActive(true);
        editorPanel.SetActive(false);
        mode = overlayMode;
    }

    public void exitOverlayMode() {
        overlayEditor.gameObject.SetActive(false);
        lastMode = overlayMode;
    }

    public void ToggleOverlayMode() {
        if (mode == overlayMode) {
            exitOverlayMode();
            startPlayMode();
        } else {
            exitCurrentMode();
            startOverlayMode();
        }
    }

    // ================================================ Options Mode
    public void startOptionsMode() {
        optionsEditor.gameObject.SetActive(true);
        editorPanel.SetActive(false);
        mode = optionsMode;
    }

    public void exitOptionsMode() {
        optionsEditor.gameObject.SetActive(false);
    }

    public void ToggleOptionsMode() {
        if (mode == optionsMode) {
            exitOptionsMode();
            startLastMode();
        } else {
            exitCurrentMode();
            startOptionsMode();
        }
    }

    // ================================================ Hamburger Mode
    public void startHamburgerMode() {
        hamburgerControls.gameObject.SetActive(true);
        editorPanel.SetActive(false);
        mode = hamburgerMode;
    }

    public void exitHamburgerMode() {
        hamburgerControls.gameObject.SetActive(false);
    }

    public void ToggleHamburgerMode() {
        if (mode == hamburgerMode) {
            exitHamburgerMode();
            startLastMode();
        } else {
            exitCurrentMode();
            startHamburgerMode();
        }
    }

    // ================================================ Editor Panel
    public void ToggleEditorPabel() {
        if (editorPanel.activeSelf) {
            editorPanel.SetActive(false);
        } else {
            editorPanel.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
