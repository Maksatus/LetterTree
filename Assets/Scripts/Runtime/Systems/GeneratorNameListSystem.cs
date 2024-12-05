using Runtime.Components;
using Runtime.Providers;
using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace Runtime.Systems
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public class GeneratorNameListSystem : IInitializer
    {
        public World World { get; set; }
        private Filter _filterUser;
        private Entity _generalGameDataComponent;
        private AsyncInstantiateOperation<UserProvider> _users;
        private AsyncInstantiateOperation<StarProvider> _stars;
        private GeneralGameDataComponent _data;
        public void OnAwake() 
        {
            _filterUser = World.Filter.With<UserComponent>().Build();
            _generalGameDataComponent = World.Filter.With<GeneralGameDataComponent>().Build().First();
            SpawnUsers();
        }

        private void SpawnUsers()
        {
            _data = _generalGameDataComponent.GetComponent<GeneralGameDataComponent>();
            var rootUsers = _data.RootUsers;
            var userProvider = _data.RefUserProvider;

            var length = _filterUser.GetLengthSlow();
            _users = Object.InstantiateAsync(userProvider, length, rootUsers);
            _users.completed += FillUsers;
        }

        private void FillUsers(AsyncOperation obj)
        {
            var usersResult = _users.Result;
            var i = 0;
            var totalLetter = 0;
            ref var data = ref _generalGameDataComponent.GetComponent<GeneralGameDataComponent>();
            foreach (var entityUser in _filterUser)
            {
                ref var userComponent = ref entityUser.GetComponent<UserComponent>();
                var entityProvider = usersResult[i];
                ref var dataUserCell = ref entityProvider.GetData();
                entityProvider.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                dataUserCell.Name.text = userComponent.Name;
                totalLetter += userComponent.TotalLetter;
                entityUser.MigrateTo(entityProvider.Entity);
                i++;
            }
          
            _stars = Object.InstantiateAsync(data.RefStar, totalLetter, _data.RootUsers);
            _stars.completed += FillStarsUsers;
        }

        private void FillStarsUsers(AsyncOperation obj)
        {
            var filterUser = World.Filter
                .With<UserComponent>()
                .With<UserCell>()
                .Build();

            var starsResult = _stars.Result;
            var starIndex = 0;

            foreach (var entityUser in filterUser)
            {
                ref var userComponent = ref entityUser.GetComponent<UserComponent>();
                ref var userCell = ref entityUser.GetComponent<UserCell>();

                for (var index = 0; index < userComponent.TotalLetter; index++)
                {
                    var starEntity = starsResult[starIndex++];
                    ref var data = ref starEntity.GetData();
                    starEntity.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                    var indexChar = Random.Range(0, _data.GetLengthChars());
                    var letter = _data.GetChar(indexChar);
                    data.RootTransform.SetParent(userCell.RootStarsTransform);
                    data.LetterGameObject.SetActive(false);
                    data.indexChar = indexChar;
                    data.LetterText.text = letter.ToString();
                    userCell.Stars.Add(data);
                }
            }
            
            var writeExelSystem = new WriteExelSystem();
            var systemsGroup = World.CreateSystemsGroup();
            systemsGroup.AddInitializer(writeExelSystem);
            systemsGroup.Initialize();
            systemsGroup.RemoveInitializer(writeExelSystem);
        }

        public void Dispose()
        {
            _filterUser = null;
            _generalGameDataComponent = null;
            _users = null;
            _stars = null;
        }
    }
}