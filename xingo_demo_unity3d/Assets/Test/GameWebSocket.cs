using System;
using UnityEngine;
using BestHTTP;
using BestHTTP.WebSocket;

public class GameWebSocket
{
    WebSocket webSocket;
    public bool Connected;
    public Action<string> OnRceiveMsg;

    public void Connect(string address)
    {
        Debug.Log(address);
        webSocket = new WebSocket(new Uri("ws://" + address));

        if (HTTPManager.Proxy != null)
            webSocket.InternalRequest.Proxy = new HTTPProxy(HTTPManager.Proxy.Address, HTTPManager.Proxy.Credentials, false);

        // Subscribe to the WS events  
        webSocket.OnOpen += OnOpen;
        webSocket.OnMessage += OnMessageReceived;
        webSocket.OnClosed += OnClosed;
        webSocket.OnError += OnError;

        // Start connecting to the server  
        webSocket.Open();
    }

    public void Request(string msgRoute,string msgContent)
    {
        webSocket.Send(msgRoute + "*" + msgContent);
    }

    public void Close()
    {
        webSocket.Close(1000, "Bye!");
        Connected = false;
    }


    #region WebSocket Event Handlers  

    /// <summary>  
    /// Called when the web socket is open, and we are ready to send and receive data  
    /// </summary>  
    void OnOpen(WebSocket ws)
    {
        Debug.Log("connected");
        Connected = true;
    }

    /// <summary>  
    /// Called when we received a text message from the server  
    /// </summary>  
    void OnMessageReceived(WebSocket ws, string message)
    {
        if (OnRceiveMsg != null) OnRceiveMsg(message);
    }

    /// <summary>  
    /// Called when the web socket closed  
    /// </summary>  
    void OnClosed(WebSocket ws, UInt16 code, string message)
    {
        webSocket = null;
        Connected = false;
    }

    /// <summary>  
    /// Called when an error occured on client side  
    /// </summary>  
    void OnError(WebSocket ws, Exception ex)
    {
        string errorMsg = string.Empty;
        if (ws.InternalRequest.Response != null)
            errorMsg = string.Format("Status Code from Server: {0} and Message: {1}", ws.InternalRequest.Response.StatusCode, ws.InternalRequest.Response.Message);
        webSocket = null;
        Connected = false;
    }

    #endregion
}