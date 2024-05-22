using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Managers
{
    //todo: @Kamil ogarnianianie zapisu achievementow
    //todo: @Filip dodanie metod do odczytu, update statystyk
    public class BackendService : MonoBehaviour
    {
        private static string baseUrl = "http://localhost:3000/api";
        private static string authToken = "";

        public void RegisterUser(string username, string password)
        {
            StartCoroutine(RegisterUserCoroutine(username, password));
        }

        private IEnumerator RegisterUserCoroutine(string username, string password)
        {
            var user = new Dictionary<string, string>
            {
                { "username", username },
                { "password", password }
            };

        
            var jsonData = JsonUtility.ToJson(user); //JsonConvert.SerializeObject(user);

            using var request = new UnityWebRequest(baseUrl + "/register", "POST");
            var bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("User registered successfully!");
            }
            else
            {
                Debug.LogError("Error: " + request.error);
            }
        }

        public void LoginUser(string username, string password)
        {
            StartCoroutine(LoginUserCoroutine(username, password));
        }

        private IEnumerator LoginUserCoroutine(string username, string password)
        {
            var user = new Dictionary<string, string>
            {
                { "username", username },
                { "password", password }
            };

            var jsonData = JsonUtility.ToJson(user); //JsonConvert.SerializeObject(user);

            using var request = new UnityWebRequest(baseUrl + "/login", "POST");
        
            var bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonUtility.FromJson<Dictionary<string, object>>(request.downloadHandler.text); //JsonConvert.DeserializeObject<Dictionary<string, object>>(request.downloadHandler.text);
                authToken = response["token"].ToString();
                Debug.Log("User logged in successfully!");
            }
            else
            {
                Debug.LogError("Error: " + request.error);
            }
        }

        public void GetAchievements()
        {
            StartCoroutine(GetAchievementsCoroutine());
        }

        private IEnumerator GetAchievementsCoroutine()
        {
            using var request = UnityWebRequest.Get(baseUrl + "/achievements");
        
            request.SetRequestHeader("x-access-token", authToken);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var achievements = JsonUtility.FromJson<List<Dictionary<string, object>>>(request.downloadHandler.text); // JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error: " + request.error);
            }
        }

        public void UpdateAchievement(int achievementId, int value)
        {
            StartCoroutine(UpdateAchievementCoroutine(achievementId, value));
        }

        private IEnumerator UpdateAchievementCoroutine(int achievementId, int value)
        {
            var achievement = new Dictionary<string, int>
            {
                { "achievementId", achievementId },
                { "value", value }
            };

            var jsonData = JsonUtility.ToJson(achievement);

            using var request = new UnityWebRequest(baseUrl + "/achievements", "PUT");
        
            var bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("x-access-token", authToken);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Achievement updated successfully!");
            }
            else
            {
                Debug.LogError("Error: " + request.error);
            }
        }

        public void GetAllAchievements()
        {
            StartCoroutine(GetAllAchievementsCoroutine());
        }

        private IEnumerator GetAllAchievementsCoroutine()
        {
            using var request = UnityWebRequest.Get(baseUrl + "/all-achievements");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var achievements = JsonUtility.FromJson<List<Dictionary<string, object>>>(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error: " + request.error);
            }
        }
    }
}
