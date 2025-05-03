using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using EnemyPack;
using GameLoaderPack;
using MapPack;
using ProjectilePack;
using UnityEngine;

namespace DebugPack
{
    public class MetricsLogger : MonoBehaviour
    {
        public float sampleInterval = 0.1f;
        private float timer = 0f;

        private readonly List<string> lines = new();

        private float elapsedTime = 0f;

        private void Start()
        {
            lines.Add("time,fps,enemy_count,bullet_count");
        }

        private void Update()
        {
            if (!EnemyManager.Ready) return;
            
            timer += Time.unscaledDeltaTime;
            elapsedTime += Time.unscaledDeltaTime;

            if (timer < sampleInterval) return;
            
            var fps = (int)(1f / Time.unscaledDeltaTime);
            var enemies = EnemyManager.GetActiveEnemies().Count;
            var bullets = ProjectileManager.CountActive;

            lines.Add($"{(int)(elapsedTime * 1000)},{fps},{enemies},{bullets}");

            timer = 0f;
        }

        private void OnDestroy()
        {
            SaveToFile();
        }

        private void SaveToFile()
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            var fileName = $"metrics_{timestamp}.csv";
            var path = Path.Combine(Application.persistentDataPath, fileName);
            File.WriteAllLines(path, lines, Encoding.UTF8);
            Debug.Log($"Metrics saved to: {path}");
        }
    }
}