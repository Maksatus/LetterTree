using System.Collections.Generic;
using System.IO;
using Runtime.Components;
using Scellecs.Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

namespace Runtime.Systems
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public class ReadExelSystem : IInitializer
    {
        public World World { get; set; }
        private Filter _filter;

        public void OnAwake()
        {
            ref var dataComponent = ref World.Filter.With<GeneralGameDataComponent>().Build().First()
                .GetComponent<GeneralGameDataComponent>();
            Init(ref dataComponent);
        }

        private void Init(ref GeneralGameDataComponent dataComponent)
        {
            var filePath = Path.Combine(Application.dataPath, "..", dataComponent.RelativePath);
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

        private static void ReadCsv(string filePath, ICollection<List<string>> rows)
        {
            using var sr = new StreamReader(filePath);
            while (sr.ReadLine() is { } line)
            {
                var fields = line.Split(';',',');
                if (fields.Length < 2)
                {
                    Debug.LogWarning($"Неверный формат строки: {line}");
                }

                rows.Add(new List<string> { fields[0].Trim(), fields[1].Trim() });
            }
        }
        
        public void Dispose()
        {
        }
    }
}