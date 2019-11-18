using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class TokenEntryScript : MonoBehaviour, IPointerDownHandler {
    public TokenListScript tokenListScript;
    public string tokenName;

    public void OnPointerDown(PointerEventData dt) {
		tokenListScript.dragToken = tokenName;
        tokenListScript.md = true;
        tokenListScript.addCursor(tokenName);
	}
}
