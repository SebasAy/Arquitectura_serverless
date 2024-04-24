using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class FirebaseUsers : MonoBehaviour
{
    public InputField emailInput;
    public InputField passwordInput;
    public InputField usernameInput;
    public InputField additionalDataInput;
    public Text statusText;

    private FirebaseAuth auth;
    private FirebaseFirestore db;

    private async void Start()
    {
        // Inicializar Firebase Auth y Firestore
        await FirebaseApp.CheckAndFixDependenciesAsync();
        FirebaseApp app = FirebaseApp.DefaultInstance;
        auth = FirebaseAuth.GetAuth(app);
        db = FirebaseFirestore.GetInstance(app);
    }

    public async void RegisterUser()
    {
        string email = emailInput.text;
        string password = passwordInput.text;
        string username = usernameInput.text;
        string additionalData = additionalDataInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(username))
        {
            statusText.text = "Please fill in all required fields.";
            return;
        }

        try
        {
            // Registrar un nuevo usuario con correo electrónico y contraseña
            var userCredential = await auth.CreateUserWithEmailAndPasswordAsync(email, password);

            // Obtener el usuario actualmente autenticado
            FirebaseUser user = userCredential.User;

            // Guardar el nombre de usuario y los datos adicionales en Firestore
            DocumentReference userRef = db.Collection("users").Document(user.UserId);
            Dictionary<string, object> userData = new Dictionary<string, object>
            {
                { "Username", username },
                { "AdditionalData", additionalData }
            };

            await userRef.SetAsync(userData);

            // Registro exitoso
            statusText.text = "Registration successful! Welcome, " + username + "!";
        }
        catch (Exception ex)
        {
            // Manejar el error
            statusText.text = "Failed to register user: " + ex.Message;
        }
    }

    public async void SignIn()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            statusText.text = "Please fill in both email and password.";
            return;
        }

        try
        {
            // Iniciar sesión con correo electrónico y contraseña
            var userCredential = await auth.SignInWithEmailAndPasswordAsync(email, password);

            // Inicio de sesión exitoso
            FirebaseUser user = userCredential.User;
            statusText.text = "Welcome back, " + user.DisplayName + "!";
        }
        catch (Exception ex)
        {
            // Manejar el error
            statusText.text = "Failed to sign in: " + ex.Message;
        }
    }

    public void SignOut()
    {
        auth.SignOut();
        statusText.text = "Signed out successfully.";
    }

    public async void ResetPassword()
    {
        string email = emailInput.text;

        if (string.IsNullOrEmpty(email))
        {
            statusText.text = "Please enter your email address.";
            return;
        }

        try
        {
            // Enviar correo electrónico de restablecimiento de contraseña
            await auth.SendPasswordResetEmailAsync(email);

            // Correo electrónico de restablecimiento de contraseña enviado exitosamente
            statusText.text = "Password reset email sent. Please check your inbox.";
        }
        catch (Exception ex)
        {
            // Manejar el error
            statusText.text = "Failed to send password reset email: " + ex.Message;
        }
    }
}
