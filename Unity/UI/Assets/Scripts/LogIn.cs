using System;
using System.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using MySql.Data.MySqlClient;
using UnityEngine.UI;

public class LogIn : MonoBehaviour
{
    // 데이터베이스 연결에 필요한 정보를 설정합니다.
    private string db_Address = "210.119.12.112";  // 데이터베이스 서버 IP 주소
    private string db_Port = "10000";        // 데이터베이스 포트 번호
    private string db_Id = "pi";          // 데이터베이스 접속 ID
    private string db_Pw = "12345";         // 데이터베이스 접속 비밀번호
    private string db_Name = "team1_iot";    // 사용할 데이터베이스 이름

        // 데이터베이스 연결 문자열을 설정
    private string conn_string;
    public TMP_InputField Id_Text;
    public TMP_InputField PW_Text;
    public GameObject error_Login;

    private void Awake()
    {
        conn_string = "Server=" + db_Address + ";Port=" + db_Port + ";Database=" + db_Name + ";User=" + db_Id + ";Password=" + db_Pw;
        error_Login.SetActive(false);
        
    }        
    

    // 이 메서드는 LOGINButton이 클릭될 때 호출
    public void OnLoginButtonClick()
    {
        try
        {
            // 데이터베이스 연결을 시도
            using (MySqlConnection conn = new MySqlConnection(conn_string))
            {
                if(conn.State == ConnectionState.Closed) conn.Open();

                Debug.Log("Mysql state: " + conn.State);

                string sql = string.Format("SELECT id, password, admin FROM account_parking WHERE id = '{0}' AND password = '{1}' AND admin = '0';", Id_Text.text, PW_Text.text);
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    // 데이터베이스에서 데이터를 가져올 SQL 쿼리를 작성

                    if (reader.HasRows)
                    {
                        // 쿼리 결과를 읽어와서 Unity 콘솔에 로그로 출력
                        // READ하면 다음 행으로 넘어가버리기 때문에 값이 있는지 없는지 체크하는게 가장 좋은 방법
                        while (reader.Read() == true)
                        {
                            //Debug.Log($"rdr[0].ToString(): '{rdr[0]}'");
                            //Debug.Log($"Id_Text.text: '{Id_Text.text}'");

                            // 문자열 내 숨겨진 문자가 포함되어서 나오기 때문에 Replace 해줘야한다.
                            string Login_ID_Text = Id_Text.text.Replace("​", "");
                            string Login_PW_Text = PW_Text.text.Replace("​", "");

                            if (reader[0].ToString() == Login_ID_Text &&
                                reader[1].ToString() == Login_PW_Text &&
                                reader[2].ToString() == "0")
                            {
                                // 로그인 성공 시 처리
                                SceneManager.LoadScene("UI_Scene", LoadSceneMode.Single);
                                SceneManager.LoadScene("Inside_Scene", LoadSceneMode.Additive);
                                // SceneManager.UnloadScene("LogIn_Scene");                                
                                break; // 로그인 성공했으므로 더 이상 확인할 필요가 없으므로 반복문을 종료
                            }
                        }
                    }
                    else
                    {
                        // 로그인 실패 시 처리
                        PW_Text.text = "";
                        error_Login.SetActive(true);

                    }
                }
            }

        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }
}