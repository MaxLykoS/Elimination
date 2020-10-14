using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class ServerTcp
{
	//  主定时器
	System.Timers.Timer timer = new System.Timers.Timer(1000);  //  1秒执行一次
	//  心跳时间
	public static readonly long hearBeatTime = 6;

	private ServerMessageDispatch dispatch = new ServerMessageDispatch();
	private ConnsPool connsPool = new ConnsPool();
	private Socket serverSocket;

	public ServerTcp()
	{

	}
	
	public void StartServer()
    {
		try
		{
			//  心跳检测定时器
			timer.Elapsed += new System.Timers.ElapsedEventHandler(HandleMainTimer);
			timer.AutoReset = false;
			timer.Enabled = true;

			IPAddress ip = IPAddress.Parse("127.0.0.1");

			serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

			serverSocket.Bind(new IPEndPoint(ip, 1234));  //绑定IP地址：端口  
			serverSocket.Listen(1024);    //设定最多1024个排队连接请求  

			//通过Clientsoket发送数据  
			serverSocket.BeginAccept(AcceptCB, null);
			Debug.Log("启动监听" + serverSocket.LocalEndPoint.ToString() + "成功");

		}
		catch (Exception ex)
		{
			Debug.Log("服务器启动失败:" + ex.Message);
		}
	}

	private void HandleMainTimer(object sender, System.Timers.ElapsedEventArgs e)
	{
		//处理心跳
		connsPool.HeartBeat();
		timer.Start();
	}

	private void AcceptCB(IAsyncResult ar)
	{
		try
		{
			Socket socket = serverSocket.EndAccept(ar);
			Conn conn = connsPool.YieldConn(socket);
			if (conn == null)
			{
				socket.Close();
				Debug.Log("警告，链接已满");
			}
			else
			{
				Debug.Log("客户端连接 [" + conn.GetAdress() + "] conn池ID：" + conn.index);
				conn.socket.BeginReceive(conn.readBuff, conn.bufferCount, conn.BufferRemain(), SocketFlags.None, ReceiveCB, conn);
				serverSocket.BeginAccept(AcceptCB, null);
			}
		}
		catch (Exception ex)
		{
			Debug.Log("AcceptCB失败:" + ex.Message);
		}
	}

	private void ReceiveCB(IAsyncResult ar)
	{
		Conn conn = (Conn)ar.AsyncState;
		lock (conn)
		{
			try
			{
				//  收到的字节数
				int count = conn.socket.EndReceive(ar);

				//  关闭信号
				if (count <= 0)
				{
					Debug.Log("收到[" + conn.GetAdress() + "]断开连接");
					conn.Close();
					connsPool.Recycle(conn);
				}
				else
				{
					//  数据处理
					conn.bufferCount += count;
					ProcessData(conn);

					//  继续接收
					conn.socket.BeginReceive(conn.readBuff, conn.bufferCount, conn.BufferRemain(), SocketFlags.None, ReceiveCB, conn);
				}
			}
			catch (Exception ex)
			{
				Debug.Log("收到[" + conn.GetAdress() + "]断开连接" + ex.Message);
				conn.Close();
			}
		}
	}

    private void ProcessData(Conn conn)
    {
		if (conn.ProcessMsgLen())
		{
			HandleProtocol(conn,conn.Translate());
			conn.ClearLastMsg();

			if (conn.bufferCount > 0)
				ProcessData(conn);
		}
    }

	public void HandleProtocol(Conn conn,Protocol proto)
	{
		dispatch.Dispatch(conn,proto);
	}

	public void ShutDown()
	{
		connsPool.CloseAll();
	}
}
