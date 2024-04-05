// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Blazored.LocalStorage;

namespace MudBlazor.Docs.Services.UserPreferences
{
    public interface IUserPreferencesService
    {
        /// <summary>
        /// Saves UserPreferences in local storage
        /// </summary>
        /// <param name="userPreferences">The userPreferences to save in the local storage</param>
        public Task SaveUserPreferences(UserPreferences userPreferences);

        /// <summary>
        /// Loads UserPreferences in local storage
        /// </summary>
        /// <returns>UserPreferences object. Null when no settings were found.</returns>
        public Task<UserPreferences> LoadUserPreferences();
    }

    public class UserPreferencesService : IUserPreferencesService
    {
        private readonly ILocalStorageService _localStorage;
        private const string Key = "userPreferences";

        public UserPreferencesService(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task SaveUserPreferences(UserPreferences userPreferences)
        {
            await _localStorage.SetItemAsync(Key, userPreferences);
        }

        public async Task<UserPreferences> LoadUserPreferences()
        {
            return await _localStorage.GetItemAsync<UserPreferences>(Key);
        }
    }
}
