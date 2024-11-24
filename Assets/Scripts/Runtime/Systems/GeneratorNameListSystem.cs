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
                var entityProvider = usersResult[i].GetComponent<UserProvider>();
                ref var dataUserCell = ref entityProvider.GetData();
                dataUserCell.Name.text = userComponent.Name;
                totalLetter += userComponent.TotalLetter;
                entityUser.MigrateTo(entityProvider.Entity);
                i++;
            }
            var rootUsers = _data.RootUsers;
            _stars = Object.InstantiateAsync(data.RefStar, totalLetter, rootUsers);
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
                
                userCell.Stars = new StarComponent[userComponent.TotalLetter];

                for (var index = 0; index < userComponent.TotalLetter; index++)
                {
                    var starProvider = starsResult[starIndex++];
                    userCell.Stars[index] = starProvider.GetData();
                    userCell.Stars[index].RootTransform.SetParent(userCell.RootStarsTransform);
                    //userCell.Stars[index].LetterGameObject.SetActive(false);
                    userCell.Stars[index].LetterText.text = _data.Chars[Random.Range(0, _data.Chars.Length)].ToString();
                    
                    
                    //starProvider.Entity.MigrateTo(entityUser);
                    starProvider.Entity.Dispose();
                }
            }
        }
        
        public void Dispose() { }
    }
}