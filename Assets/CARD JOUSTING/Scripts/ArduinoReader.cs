using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using LibLabGames.NewGame;
using LibLabSystem;
using UnityEngine;

public class ArduinoReader : MonoBehaviour
{
    public static ArduinoReader instance;

    public string port_00;
    public string port_01;
    public int timeout = 1;
    SerialPort serial_00;
    SerialPort serial_01;

    Thread thread;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(this);
            return;
        }
    }

    void Start()
    {
        List<string> ports = SerialPort.GetPortNames().ToList();

        if (ports.Find(x => x.Contains(port_00)) == null ||
            ports.Find(x => x.Contains(port_01)) == null)
        {
            LLLog.LogW("ArduinoReader", "Arduino NFC disable!");
            gameObject.SetActive(false);
            return;
        }

        serial_00 = new SerialPort(port_00, 9600);
        serial_00.ReadTimeout = timeout;
        serial_00.Open();

        serial_01 = new SerialPort(port_01, 9600);
        serial_01.ReadTimeout = timeout;
        serial_01.Open();
    }

    void Update()
    {
        ReadSerial(serial_00);
        ReadSerial(serial_01);
    }

    void ReadSerial(SerialPort serial)
    {
        if (serial.IsOpen)
        {
            string msg = "";
            string cardID = "";
            string captorID = "";
            try
            {
                msg = serial.ReadLine();
                serial.BaseStream.Flush();
                serial.DiscardInBuffer();

                cardID = msg.Substring(0, msg.Length - 1);
                captorID = msg.Substring(msg.Length - 1);

                switch (serial.PortName)
                {
                case "COM3":
                    if (msg.Length == 9)
                    {
                        if (captorID == "0")
                        {
                            // Line Middle P0
                            GameManager.instance.ReadCardNFC_GameBoard(cardID, 0, 1);
                        }
                        else if (captorID == "1")
                        {
                            // Line Right P0
                            GameManager.instance.ReadCardNFC_GameBoard(cardID, 0, 2);
                        }
                        else if (captorID == "2")
                        {
                            // Line Left P0
                            GameManager.instance.ReadCardNFC_GameBoard(cardID, 0, 0);
                        }
                    }
                    if (captorID == "3")
                    {
                        // Evolution P0
                        GameManager.instance.ReadCardNFC_Training(cardID, 0);
                    }
                    break;

                case "COM4":
                    if (msg.Length == 9)
                    {
                        if (captorID == "0")
                        {
                            // Line Right P1
                            GameManager.instance.ReadCardNFC_GameBoard(cardID, 1, 0);
                        }
                        else if (captorID == "1")
                        {
                            // Line Middle P1
                            GameManager.instance.ReadCardNFC_GameBoard(cardID, 1, 1);
                        }
                        else if (captorID == "2")
                        {
                            // Line Left P1
                            GameManager.instance.ReadCardNFC_GameBoard(cardID, 1, 2);
                        }
                    }
                    if (captorID == "3")
                    {
                        // Evolution P1
                        GameManager.instance.ReadCardNFC_Training(cardID, 1);
                    }
                    break;
                }
            }
            catch (System.Exception) { }
        }
    }
}