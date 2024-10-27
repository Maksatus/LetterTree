using System.Collections.Generic;
using System.IO;
using Runtime.Components;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

namespace Runtime.Systems
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Systems/" + nameof(ReadExelSystem))]
    public sealed class ReadExelSystem : UpdateSystem
    {
        private Filter _filter;
        
        public override void OnAwake() 
        {
            _filter = World.Filter.With<DataComponent>().Build();
            foreach (var entity in _filter)
            {
                ref var dataComponent = ref entity.GetComponent<DataComponent>();
                Init(dataComponent.relativePath);
                return;
            }
        }

        private void Init(string relativePath)
        {
            var filePath = Path.Combine(Application.dataPath, "..", relativePath);
            var rows = new List<List<string>>();

            if (File.Exists(filePath))
            {
                ReadCsv(filePath, rows);
                var totalUsers = rows.Count;
                var user = new UserComponent[totalUsers];

                for (var i = 0; i < rows.Count; i++)
                {
                    var row = rows[i];
                    int.TryParse(row[1], out var totalLetter);
                    
                    var userEntity = World.CreateEntity();
                    ref var addUserComponentReference = ref userEntity.AddComponent<UserComponent>();
                    addUserComponentReference.Name = row[0];
                    addUserComponentReference.TotalLetter = totalLetter;
                    
                    user[i] = addUserComponentReference;
                }
            }
            else
            {
                Debug.LogError("Файл не найден: " + filePath);
            }
        }
        
        private void ReadCsv(string filePath, ICollection<List<string>> rows)
        {
            using var sr = new StreamReader(filePath);
            while (sr.ReadLine() is { } line)
            {
                var fields = line.Split(',');
                rows.Add(new List<string>(fields));
            }
        }
        
        public override void OnUpdate(float deltaTime) { }
    }
}