using UnityEngine;
using PlayerMsg;
using Google.Protobuf;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    public void Onlogin()
    {
        C2S_Login c2S_Login = new C2S_Login()
        {
            UserName = "gaoyang",
            Password = "123456"
        };
        byte[] msgBytes = c2S_Login.ToByteArray();
        NetManager.Send(MessageId.MessageId.C2SLogin, msgBytes);
        SceneManager.LoadScene("move");
    }
}
