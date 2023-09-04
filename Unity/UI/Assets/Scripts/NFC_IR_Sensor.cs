using System;
using System.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using MySql.Data.MySqlClient;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class NFC_IR_Sensor : MonoBehaviour
{
	private string db_Address = "210.119.12.112";
	private string db_Port = "10000";
	private string db_Id = "pi";
	private string db_Pw = "12345";
	private string db_Name = "team1_iot";
	public static string conn_string;

	private bool ready = false;

	private void Awake()
	{
		conn_string = "Server=" + db_Address + ";Port=" + db_Port + ";Database=" + db_Name + ";User=" + db_Id + ";Password=" + db_Pw;
	}

	// Update is called once per frame
	void Update()
	{
		
		try
		{
			string irSensorValue = EventManager.Instance.Sensor_Data.AD1_RCV_IR_Sensor;
			string nfcValue = EventManager.Instance.Sensor_Data.AD4_RCV_NFC;
			string checknfc = null;


			if (nfcValue != "None")
			{
				Debug.Log("nfcvalue : " + nfcValue);
			}

			if (irSensorValue == "0")
			{
				checknfc = Connect(); // nfc 값 가져옴

				if (checknfc == nfcValue)
				{
					// 동일한 nfc !
					Debug.Log("동일!");
					Modify(1);
					Inserthistory(Idreturn(nfcValue), true);
					ready = true;
				}
				else
				{
					Debug.Log("틀림!");
					Modify(2);
				}
			}
			else
			{
				if (!Check())
				{
					Inserthistory(Idreturn(nfcValue), false);
					Modify(3);
				}
			}
		}
		catch (NullReferenceException)
		{
		}
	}

	string Connect()
	{
		try
		{
			using (MySqlConnection conn = new MySqlConnection(conn_string))
			{
				if (conn.State == ConnectionState.Closed) conn.Open();

				// Debug.Log("Mysql state: " + conn.State);

				string sql = "SELECT NFC FROM parking_status WHERE id_x = 6;";
				MySqlCommand cmd = new MySqlCommand(sql, conn);
				using (MySqlDataReader reader = cmd.ExecuteReader())
				{
					if (reader.HasRows)
					{
						while (reader.Read() == true)
						{
							return reader[0].ToString();
						}
					}
				}
			}

		}
		catch (Exception e)
		{
			Debug.Log(e.Message);
		}
		return null;
	}

	void Modify(int reserve)
	{
		string reserve_status = "";

		if (reserve == 1)
		{
			reserve_status = "0";
		}
		else if (reserve == 2)
		{
			reserve_status = "3";
		}
		else if (reserve == 3)
		{
			reserve_status = "1";
		}

		try
		{
			using (MySqlConnection conn = new MySqlConnection(conn_string))
			{
				if (conn.State == ConnectionState.Closed) conn.Open();

				// Debug.Log("Mysql state: " + conn.State);
				string sql = "";

				if (reserve == 3)
				{
					if (ready == true)
					{
						sql = string.Format("UPDATE parking_status SET reservation_status = '{0}', NFC = NULL WHERE id_x = 6;", reserve_status);
						Debug.Log("빠져나감");
						ready = false;
					}
					
				}
				else
				{
					sql = string.Format("UPDATE parking_status SET reservation_status = '{0}' WHERE id_x = 6;", reserve_status);
				}
				MySqlCommand cmd = new MySqlCommand(sql, conn);
				cmd.ExecuteNonQuery();
			}

		}
		catch (Exception e)
		{
			Debug.Log(e.Message);
		}
	}

	string Idreturn(string nfc)
	{
		try
		{
			using (MySqlConnection conn = new MySqlConnection(conn_string))
			{
				if (conn.State == ConnectionState.Closed) conn.Open();

				// Debug.Log("Mysql state: " + conn.State);

				string sql = string.Format("SELECT id FROM account_parking WHERE nfc_registered = '{0}';", nfc);
				MySqlCommand cmd = new MySqlCommand(sql, conn);
				using (MySqlDataReader reader = cmd.ExecuteReader())
				{
					if (reader.HasRows)
					{
						while (reader.Read() == true)
						{
							return reader[0].ToString();
						}
					}
				}
			}

		}
		catch (Exception e)
		{
			Debug.Log(e.Message);
		}

		return null;
	}

	void Inserthistory(string id, bool inout)
	{
		if (inout)
		{
			try
			{
				using (MySqlConnection conn = new MySqlConnection(conn_string))
				{
					if (conn.State == ConnectionState.Closed) conn.Open();

					string sql = string.Format("SELECT * FROM parking_history WHERE id = '{0}' AND deparure_time IS NULL;", id);
					MySqlCommand cmd = new MySqlCommand(sql, conn);

					using (MySqlDataReader reader = cmd.ExecuteReader())
					{
						if (reader.HasRows == false)
						{
							reader.Close(); // 첫 번째 데이터 리더 닫기

							sql = string.Format("INSERT INTO parking_history(id, entrance_time) VALUES('{0}', NOW());", id);
							cmd.CommandText = sql;
							cmd.ExecuteNonQuery();
						}
					}
				}
			}
			catch (Exception e)
			{
				Debug.Log(e.Message);
			}
			return;
		}
		else
		{
			try
			{
				using (MySqlConnection conn = new MySqlConnection(conn_string))
				{
					if (conn.State == ConnectionState.Closed) conn.Open();

					string sql = string.Format("SELECT * FROM parking_history WHERE id = '{0}' AND deparure_time IS NULL;", id);
					MySqlCommand cmd = new MySqlCommand(sql, conn);

			
					using (MySqlDataReader reader = cmd.ExecuteReader())
					{
						if (reader.HasRows)
						{
							reader.Close(); // 첫 번째 데이터 리더 닫기

							sql = string.Format("UPDATE parking_history SET deparure_time = NOW() WHERE id = '{0}' ORDER BY id_x DESC LIMIT 1", id);
							cmd.CommandText = sql;
							cmd.ExecuteNonQuery();
						}
					}
					
					
				}
			}
			catch (Exception e)
			{
				Debug.Log(e.Message);
			}
			return;
		}

	}

	bool Check()
	{
		try
		{
			using (MySqlConnection conn = new MySqlConnection(conn_string))
			{
				if (conn.State == ConnectionState.Closed) conn.Open();
				string sql = string.Format("SELECT reservation_status FROM parking_status WHERE id_x = 6;");
				MySqlCommand cmd = new MySqlCommand(sql, conn);
				using (MySqlDataReader reader = cmd.ExecuteReader())
				{
					if (reader.HasRows)
					{
						while (reader.Read() == true)
						{
							if (reader[0].ToString() == "2")
							{
								return true;
							}
							return false;
						}
					}
				}
			}

		}
		catch (Exception e)
		{
			Debug.Log(e.Message);
		}
		return false;
	}
}
