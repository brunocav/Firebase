using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Firebase.Auth;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using Firebase.Analytics;
using UnityEngine.SceneManagement;
public class NewBehaviourScript : MonoBehaviour
{
    public static NewBehaviourScript Instance;

    private FirebaseAuth auth;
    string NomePlayer, IdUsuario;
    public Text NomePlayerTxt,pontuacaotxt,resultado;
    DatabaseReference mDatabaseRef, _counterRef;
    public string jsonData;
    public static int gols;

    int pontosTotal;

    private void Awake()
    {
                StartCoroutine(RecuperandoDados());
        Instance = this;

    }
    void Start()
    {
      
          FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://vhhikingfestival.firebaseio.com/");
        mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
   
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
        NomePlayer = FirebaseManager.PrefabName;
        IdUsuario = FirebaseManager.PrefabID;
        auth = FirebaseAuth.DefaultInstance;
        resultado.text = IdUsuario.ToString();
        NomePlayerTxt.text = NomePlayer.ToString();
    }
    public void CadastrarPontos(int pontos)
    {
        LerPontos();

        _counterRef = FirebaseDatabase.DefaultInstance.GetReference("Usuario" + "/" + IdUsuario + "/" + "Pontos");
        _counterRef.RunTransaction(data => {
            data.Value = pontos + pontosTotal;
            return TransactionResult.Success(data);
        }).ContinueWith(task => {
            if (task.Exception != null)
                Debug.Log(task.Exception.ToString());
        });
    }
    public void LerDados()
    {
        FirebaseDatabase.DefaultInstance.GetReference("Usuario").Child(IdUsuario).Child("nome").GetValueAsync().ContinueWith(task => {
       if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
           jsonData = snapshot.GetRawJsonValue();

           NomePlayerTxt.text = jsonData;
                LerPontos();


       }
   });
    }
    public void LerPontos()
    {
        FirebaseDatabase.DefaultInstance.GetReference("Usuario").Child(IdUsuario).Child("Pontos").GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                jsonData = snapshot.GetRawJsonValue();
                pontuacaotxt.text = jsonData;
                pontosTotal = int.Parse(jsonData);




            }
        });
    }
    public void AtivarReinicio()
    {
        //pontosTotal = pontosTotal + gols;
        gols = 2;
        CadastrarPontos(gols);
       // PrimeiroAnalitics();
        LerPontos();

        //  reinicio.SetActive(true);
    }
   public void PrimeiroAnalitics()
    {

        Firebase.Analytics.FirebaseAnalytics.LogEvent("Comida", "Lazanha", 0.8f);
        string json = JsonUtility.ToJson(gols);
        mDatabaseRef.Child("Usuario").Child(IdUsuario).Child("Analitcs").SetRawJsonValueAsync(json);
        resultado.text = "foi";
        Firebase.Analytics.FirebaseAnalytics.LogEvent("Evento Teste", "nivel de difuculdade", 0.4f);

    }
    IEnumerator RecuperandoDados() {
        yield return new WaitForSeconds(0.000001f);
        LerDados();
        StopCoroutine(RecuperandoDados());
    }
}
