using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine.SceneManagement;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager instance;

    DatabaseReference mDatabaseRef, _counterRef;
    private FirebaseAuth auth;
    public Text Resultado,PlayerName;
    public static Text PlayerNameFinal;
    public InputField EmailInput, SenhaInput,NomeInput;
    public Button RegistrarUsuario, LoginUsuario;
    string IdUsuario;
    public static string PrefabName,PrefabID;
    public int pontosA;
    public string jsonData;
    // Start is called before the first frame update

     void Awake()
    {
        instance = this;

    }

    void Start()
    {

        auth = FirebaseAuth.DefaultInstance;

        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://vhhikingfestival.firebaseio.com/");
        mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;

        RegistrarUsuario.onClick.AddListener(() => Registrar(EmailInput.text,SenhaInput.text,NomeInput.text));
        LoginUsuario.onClick.AddListener(() => Login(EmailInput.text, SenhaInput.text));

        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp, i.e.
                //   app = Firebase.FirebaseApp.DefaultInstance;
                // where app is a Firebase.FirebaseApp property of your application class.

                // Set a flag here indicating that Firebase is ready to use by your
                // application.
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }


     void Registrar(string Email,string Senha,string Nome)
    {
        auth.CreateUserWithEmailAndPasswordAsync(Email, Senha).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            // Firebase user has been created.
            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);

            User user = new User(Email,Senha,Nome);
            string json = JsonUtility.ToJson(user);
            mDatabaseRef.Child("Usuario").Child(newUser.UserId).SetRawJsonValueAsync(json);
            ConfirmarEmail();
        });
    }
   

    void Login(string Email, string Senha)
    {
      
        auth.SignInWithEmailAndPasswordAsync(Email, Senha).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
            StartCoroutine(coroutineA());
            IdUsuario = newUser.UserId;
            if (task.IsCompleted)
            {
               GetFirebaseName(PlayerNameFinal);

            }
        });

    }

    void ConfirmarEmail()
    {
        Firebase.Auth.FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            user.SendEmailVerificationAsync().ContinueWith(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("SendEmailVerificationAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SendEmailVerificationAsync encountered an error: " + task.Exception);
                    return;
                }

                Debug.Log("Email sent successfully.");
            });
        }
    }
    void RedefinirSenha()
    {
        Firebase.Auth.FirebaseUser user = auth.CurrentUser;
        string emailAddress = user.DisplayName;
        if (user != null)
        {
            auth.SendPasswordResetEmailAsync(emailAddress).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("SendPasswordResetEmailAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SendPasswordResetEmailAsync encountered an error: " + task.Exception);
                    return;
                }

                Debug.Log("Password reset email sent successfully.");
            });
        }
    }

    IEnumerator coroutineA()
    {
         yield return new WaitForSeconds(8.0f);
         SceneManager.LoadScene("Tela2", LoadSceneMode.Single);
         

    }
    //public void CadastrarPontos(int pontos)
    //{
    //    NewBehaviourScript.Instance.LerPontos(); 
    //    _counterRef = FirebaseDatabase.DefaultInstance.GetReference("Usuario" + "/" + IdUsuario + "/" + "Pontos");
    //    _counterRef.RunTransaction(data => {
    //        data.Value = NewBehaviourScript.gols + pontos;
    //        return TransactionResult.Success(data);
    //    }).ContinueWith(task => {
    //        if (task.Exception != null)
    //            Debug.Log(task.Exception.ToString());
    //    });
    //}
    public void ReadNome(Text PerfilText = null)
    {
        FirebaseDatabase.DefaultInstance.GetReference("Usuario").Child(IdUsuario).Child("nome").GetValueAsync().ContinueWith(task => {
                if (task.IsFaulted)
                {
                    Debug.Log("Read Error!");
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    jsonData = snapshot.GetRawJsonValue();

                    PerfilText.text = jsonData;
                }
            });
    }
    public void GetFirebaseName(Text nameText)
    {
        FirebaseDatabase.DefaultInstance.GetReference("Usuario").Child(IdUsuario).Child("nome").GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {
                Debug.Log("Read Error!");
            }
            else if (task.IsCompleted)
            {

                DataSnapshot snapshot = task.Result;
                jsonData = snapshot.GetRawJsonValue();
                PlayerName.text = jsonData;
                PrefabName = PlayerPrefs.GetString("Name ", PlayerName.text);
                PrefabID = PlayerPrefs.GetString("ID" , IdUsuario);
                
            }
        });

        if (nameText != null)
        {
            nameText.text = PlayerPrefs.GetString("nome");
        }
    }

}
public class User
{
    public string senha;
    public string email;
    public string nome;

    public User(string email,string senha, string nome)
    {
        this.email = email;
        this.senha = senha;
        this.nome = nome;
    }


}
