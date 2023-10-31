using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ChatManager : NetworkBehaviour
{
    public TextMeshProUGUI content;
    public ScrollRect scrollRect;
    public TMP_InputField inputFieldMessage;
    public TMP_InputField inputFieldChannel;
    private Dictionary<ulong, string> _nicknames = new Dictionary<ulong, string>();
    private Dictionary<string, ulong> _nicknamesInverse = new Dictionary<string, ulong>();
    private Dictionary<string, Action<ulong, string>> _commands = new Dictionary<string, Action<ulong, string>>();
    private Dictionary<string, HashSet<ulong>> _channels = new Dictionary<string, HashSet<ulong>>();
    private ulong _localId;

    private void Awake()
    {
        _localId = NetworkManager.Singleton.LocalClientId;
        GenerateCommands();
    }

    void GenerateCommands()
    {
        _commands["r/"] = DMCommand;
    }

    [ServerRpc(RequireOwnership = false)]
    public void RegisterUserServerRpc(ulong id, string nickname)
    {
        _nicknames[id] = nickname;
        _nicknamesInverse[nickname] = id;
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddToChannelServerRpc(ulong id, string channel)
    {
        if (!_channels.ContainsKey(channel))
        {
            _channels[channel] = new HashSet<ulong>();
        }
        _channels[channel].Add(id);
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void RemoveFromChannelServerRpc(ulong id, string channel)
    {
        if (_channels.ContainsKey(channel))
        {
            if (_channels[channel].Contains(id))
            {
                _channels[channel].Remove(id);
                if (_channels[channel].Count == 0)
                {
                    _channels.Remove(channel);
                }
            }
        }
    }
    
    public void SendChatMessage(string message)
    {
        if (string.IsNullOrEmpty(inputFieldMessage.text) || string.IsNullOrWhiteSpace(inputFieldMessage.text)) return;
        inputFieldMessage.text = "";
        MessageServerRpc(_localId, message, inputFieldChannel.text);
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void MessageServerRpc(ulong id, string message, string channel = "")
    {
        // Nuestro comando para mensaje privado es /r
        string[] split = message.Split(' ');
        var nickname = _nicknames[id];

        if (_commands.ContainsKey(split[0]))
        {
            var join = string.Join(" ", split, 1, split.Length - 1);
            _commands[split[0]](id, join);
        }
        else
        {
            if (channel != "" && _channels.ContainsKey(channel) && _channels[channel].Contains(id))
            {
                var ids = _channels[channel];
                List<ulong> listIds = new List<ulong>();
                foreach (ulong currId in ids)
                {
                    listIds.Add(currId);
                }

                ClientRpcParams p = new ClientRpcParams();
                p.Send.TargetClientIds = listIds;
                UpdateChatClientRpc(id, nickname, message, channel, p);
            }
            else
            {
                UpdateChatClientRpc(id, nickname, message, channel);
            }
            
        }
    }

    [ClientRpc]
    public void UpdateChatClientRpc(ulong userId, string userNickname, string message, string channel, ClientRpcParams p = default)
    {
        string commandColor;
        if (userId == _localId)
        {
            commandColor = "<color=blue>";
        }
        else
        {
            commandColor = "<color=red>";
        }
        content.text += commandColor + userNickname + ": " + "</color>" + message + "\n";
        if (scrollRect.verticalNormalizedPosition < 0.1f)
            StartCoroutine(WaitToScroll());
    }
    [ClientRpc]
    public void DMClientRpc(ulong userId, string userNickname, string message, ClientRpcParams p = default)
    {
        string commandColor = "<color=purple>";
        
        content.text += commandColor + userNickname + ": " + message + "</color>" + "\n";
        if (scrollRect.verticalNormalizedPosition < 0.1f)
            StartCoroutine(WaitToScroll());
    }

    IEnumerator WaitToScroll()
    {
        yield return new WaitForEndOfFrame();
        scrollRect.verticalNormalizedPosition = 0;
    }

    void DMCommand(ulong id, string message)
    {
        var split = message.Split(' ');
        if (!_nicknamesInverse.ContainsKey(split[0])) return;
        
        var p = new ClientRpcParams();
        p.Send.TargetClientIds = new ulong[] { id, _nicknamesInverse[split[0]] };
        var join = string.Join(" ", split, 1, split.Length - 1);
        DMClientRpc(id, _nicknames[id], join, p);
    }
}
