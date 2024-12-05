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
    public class WriteExelSystem : IInitializer
    {
        public World World { get; set; }
        private Filter _filter;

        public void OnAwake()
        {
            ref var dataComponent = ref World.Filter.With<GeneralGameDataComponent>().Build().First()
                .GetComponent<GeneralGameDataComponent>();

            var rows = new List<List<string>>();

            var filterUser = World.Filter
                .With<UserComponent>()
                .With<UserCell>()
                .Build();

            foreach (var entityUser in filterUser)
            {
                ref var userComponent = ref entityUser.GetComponent<UserComponent>();
                ref var userCell = ref entityUser.GetComponent<UserCell>();
                var list = new List<string>(){userComponent.Name};
                for (var i = 0; i < userComponent.TotalLetter; i++)
                {
                    list.Add(dataComponent.GetChar(userCell.Stars[i].indexChar).ToString());
                }

                rows.Add(list);
            }


            var outputFilePath = Path.Combine(Application.dataPath, "..", "Output_" + dataComponent.RelativePath);
            dataComponent.OutputPathFile = outputFilePath;
            WriteCsv(outputFilePath, rows);
        }


        private static void WriteCsv(string filePath, IEnumerable<List<string>> rows)
        {
            using var sw = new StreamWriter(filePath);
            foreach (var row in rows)
            {
                var line = string.Join(";", row);
                sw.WriteLine(line);
            }
        }


        public void Dispose()
        {
        }
    }
}