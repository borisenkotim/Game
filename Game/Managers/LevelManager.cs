using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace CPTS487_Game
{
    public class LevelManager
    {
        // List of the waypoint lists, each vector in the list coresponds to a waypoint
        public Dictionary<int, Queue<Vector2>> pathWaypoints = new Dictionary<int, Queue<Vector2>>();
        // Dictionary holding the wave number (key) and the enemy queue (type and initial frame size)
        public Dictionary<int, Queue<EnemyInfo>> waveInfo = new Dictionary<int, Queue<EnemyInfo>>();
        // Queue of the enemy types that spawn during the wave

        public struct EnemyInfo
        {
            public int type;
            public Rectangle frameSize;
            public Queue<Vector2> path;
        }
       
        public EnemyInfo generateEnemyInfo(int type, Rectangle frameSize, Queue<Vector2> enemyPath)
        {
            EnemyInfo generatedEnemy = new EnemyInfo();
            var path = new Queue<Vector2>(enemyPath);
            generatedEnemy.type = type;
            generatedEnemy.frameSize = frameSize;
            generatedEnemy.path = path;
            return generatedEnemy;
        }

        public void serializeData(Object obj)
        {
            string output = JsonConvert.SerializeObject(obj);
            Console.WriteLine(output);
        }

        public void readWaypointsJson(string filePath)
        {
            string jsonFromFile;

            using (var reader = new StreamReader(filePath))
            {
                jsonFromFile = reader.ReadToEnd();
            }
            pathWaypoints = JsonConvert.DeserializeObject<Dictionary<int, Queue<Vector2>>>(jsonFromFile);
        }

        public void readWaveInfoJson(string filePath)
        {
            string jsonFromFile;

            using (var reader = new StreamReader(filePath))
            {
                jsonFromFile = reader.ReadToEnd();
            }
            waveInfo = JsonConvert.DeserializeObject<Dictionary<int, Queue<EnemyInfo>>>(jsonFromFile);
        }
    }
}
