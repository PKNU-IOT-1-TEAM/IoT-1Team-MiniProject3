import paho.mqtt.client as mqtt

def on_connect(client, userdata, flags, rc) :
	print(f'Connected with result code > {rc}')
	client.subscribe('TEAM_ONE/parking/s_data/')

def on_message(client, userdata, msg) :
	print(f'msg.topic {msg.payload}')

client = mqtt.Client()
client.on_connect = on_connect
client.on_message =  on_message # on_message callback set

client.connect("210.119.12.83", 1883, 60) # MQTT server connect
client.loop_forever()