using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleScript : MonoBehaviour
{
    public Text displayText;
    public InputField inputText;
    public GameEnv gameEnv;

    public bool consoleLog = true;
    public bool combatLog = true;
    public bool chatLog = true;

    List<Message> messages = new List<Message>();

    public void FilterConsole(int type) {
        if (type == 0) {
            consoleLog = !consoleLog;
        } else if (type == 1) {
            combatLog = !combatLog;
        } else if (type == 2) {
            chatLog = !chatLog;
        }
        Refresh();
    }

    public void Refresh() {
        displayText.text = "";
        foreach (Message message in messages) {
            PrintMessage(message);
        }
    }

    public void PrintMessage(Message message) {
        if (message.type == 0 && consoleLog) {
            displayText.text += string.Concat("\n ", string.Format("{0:00}", message.hour), ":", string.Format("{0:00}", message.minute), ":", string.Format("{0:00}", message.second), " |console| ", message.text);
        } else if (message.type == 1 && combatLog) {
            displayText.text += string.Concat("\n ", string.Format("{0:00}", message.hour), ":", string.Format("{0:00}", message.minute), ":", string.Format("{0:00}", message.second), " |game| ", message.text);
        } else if (message.type == 2 && chatLog) {
            displayText.text += string.Concat("\n ", string.Format("{0:00}", message.hour), ":", string.Format("{0:00}", message.minute), ":", string.Format("{0:00}", message.second), " |chat| ", message.text);
        }
    }

    public void ConsoleLog(string message) {
        Message message_object = new Message(message, 0);
        messages.Add(message_object);
        PrintMessage(message_object);
    } 

    public void CombatLog(string message) {
        Message message_object = new Message(message, 1);
        messages.Add(message_object);
        PrintMessage(message_object);
    }    

    public void ChatLog(string message) {
        Message message_object = new Message(message, 2);
        messages.Add(message_object);
        PrintMessage(message_object);
    }

    public void Enter() {
        string input = inputText.text;
        if (input.Length > 3 && input.Substring(0,3) == "\\s ") {
            try {
                Value result = DaedScript.evaluate(input.Substring(3), ref gameEnv);
                ConsoleLog(Utils.ValueToString(result));
            } catch {
                ConsoleLog("Unable To Parse Script: \"" + input.Substring(3) + "\"");
            }
        } else {
            ChatLog(input);
        }
        inputText.Select();
        inputText.text = "";
    }

    public void CheckFocusAndEnter() {
        if (inputText.isFocused) {
            Enter();
        }
    }

    // void Start() {
    //     inputText.OnSubmit(Enter());
    //     inputText.onSubmit.AddListener(delegate { inputSubmitCallBack(); });
    //     if(inputText.isFocused && inputText.text != "" && Input.GetKey(KeyCode.Return)) {
    //         Enter();
    //     }
    // }
}
