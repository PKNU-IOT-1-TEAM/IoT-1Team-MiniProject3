using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Text;
using System;

public class MQTT_Subscribe : MonoBehaviour
{
    private MqttClient mqttClient;
    private string brokerAddress = "210.119.12.112"; // "192.168.0.25"; // MQTT BROKER IP
    private int brokerPort = 11000; // 1883;
    private string s_topic = "TEAM_ONE/parking/Sensor_data/"; // ������ ����(���������1)
	private string c_topic = "TEAM_ONE/parking/Car_Number_data/"; // ������ ����(���������2)
																  
	void Start()
    {
        mqttClient = new MqttClient(brokerAddress, brokerPort, false, null, null, MqttSslProtocols.None);
        mqttClient.MqttMsgPublishReceived += MqttClient_MqttMsgPublishReceived;
        mqttClient.Connect(Guid.NewGuid().ToString());
        mqttClient.Subscribe(new string[] { s_topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
		mqttClient.Subscribe(new string[] { c_topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
	}

    private void OnApplicationQuit()
    {
        if (mqttClient != null)
        {
            if( mqttClient.IsConnected )
                mqttClient.Disconnect();
        }
    }

    private void MqttClient_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        try
        {
            // MQTT �޽��� ���� �̺�Ʈ ó��
            string message = Encoding.UTF8.GetString(e.Message);
			// message �����ؼ� SensorData ������.
			EventManager.Instance.SetSensorData(message);
        }
        catch (Exception ex)
        {
            Debug.Log($"{ex}");
        }
    }
}
