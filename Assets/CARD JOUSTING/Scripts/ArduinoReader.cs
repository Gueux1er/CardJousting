using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using System.Linq;
using LibLabGames.NewGame;
using UnityEngine;
using LibLabSystem;

public class ArduinoReader : MonoBehaviour
{
    public string port_00;
    public string port_01;
    public string port_02;
    public int timeout = 1;
    SerialPort serial_00;
    SerialPort serial_01;
    SerialPort serial_02;

    Thread thread;

    void Start()
    {
        List<string> ports = SerialPort.GetPortNames().ToList();

        if (ports.Find(x => x.Contains(port_00)) == null ||
            ports.Find(x => x.Contains(port_01)) == null ||
            ports.Find(x => x.Contains(port_02)) == null)
        {
            LLLog.LogW("ArduinoReader","Can't found the arduino's ports.");
            gameObject.SetActive(false);
            return;
        }

        serial_00 = new SerialPort(port_00, 9600);
        serial_00.ReadTimeout = timeout;
        serial_00.Open();

        serial_01 = new SerialPort(port_01, 9600);
        serial_01.ReadTimeout = timeout;
        serial_01.Open();

        serial_02 = new SerialPort(port_02, 9600);
        serial_02.ReadTimeout = timeout;
        serial_02.Open();
    }

    void Update()
    {
        ReadSerial(serial_00);
        ReadSerial(serial_01);
        ReadSerial(serial_02);
    }

    void ReadSerial(SerialPort serial)
    {
        if (serial.IsOpen)
        {
            string msg = "";
            string cardID = "";
            try
            {
                msg = serial.ReadLine();
                serial.BaseStream.Flush();
                serial.DiscardInBuffer();

                print(msg);
                cardID = msg.Split(' ') [1];

                switch (serial.PortName)
                {
                case "COM1":
                    if (msg[0] == 0)
                    {
                        GameManager.instance.ReadCardNFC_GameBoard(cardID, 0, 0);
                    }
                    else if (msg[0] == 1)
                    {
                        GameManager.instance.ReadCardNFC_GameBoard(cardID, 0, 1);
                    }
                    else if (msg[0] == 2)
                    {
                        GameManager.instance.ReadCardNFC_GameBoard(cardID, 0, 2);
                    }
                    break;

                case "COM2":
                    if (msg[0] == 0)
                    {
                        GameManager.instance.ReadCardNFC_GameBoard(cardID, 1, 0);
                    }
                    else if (msg[0] == 1)
                    {
                        GameManager.instance.ReadCardNFC_GameBoard(cardID, 1, 1);
                    }
                    else if (msg[0] == 2)
                    {
                        GameManager.instance.ReadCardNFC_GameBoard(cardID, 1, 2);
                    }
                    break;

                case "COM3":
                    if (msg[0] == 0)
                    {
                        GameManager.instance.ReadCardNFC_Training(cardID, 0);
                    }
                    else if (msg[0] == 1)
                    {
                        GameManager.instance.ReadCardNFC_Training(cardID, 1);
                    }
                    break;
                }
            }
            catch (System.Exception) { }
        }
    }
}