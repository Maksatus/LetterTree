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
    public class GeneratorNameListSystem : ISystem 
    {
        public World World { get; set; }
        private Filter _filterUser;
        private AsyncInstantiateOperation<UserProvider> _users;
        public void OnAwake() 
        {
            _filterUser = World.Filter.With<UserComponent>().Build();
            var filterData = World.Filter.With<DataComponent>().Build();
            SpawnUsers(filterData);
        }

        private void SpawnUsers(Filter filterData)
        {
            Transform rootUsers = null;
            UserProvider userProvider = null;
            
            foreach (var entity in filterData)
            {
                ref var dataComponent = ref entity.GetComponent<DataComponent>();
                rootUsers = dataComponent.RootUsers;
                userProvider = dataComponent.UserProvider;
                break;
            }

            var length = _filterUser.GetLengthSlow();
            _users = Object.InstantiateAsync(userProvider, length, rootUsers);
            _users.completed += Fill;
        }

        private void Fill(AsyncOperation obj)
        {
            var usersResult = _users.Result;
            var i = 0;
            foreach (var entityUser in _filterUser)
            {
                ref var userComponent = ref entityUser.GetComponent<UserComponent>();
                var entityProvider = usersResult[i].GetComponent<UserProvider>();
                ref var data = ref entityProvider.GetData();
                data.Name.text = userComponent.Name;
                i++;
            }
        }

        public void OnUpdate(float deltaTime) 
        {
            // foreach (var entity in _filter) 
            // {
            //     ref var userComponent = ref entity.GetComponent<UserComponent>();
            //     Debug.LogWarning(userComponent.Name);
            // }
        }

        public void Dispose()
        {
            
        }
    }
}