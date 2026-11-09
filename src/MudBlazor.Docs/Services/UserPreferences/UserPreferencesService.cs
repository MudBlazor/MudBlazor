// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Docs.Services.UserPreferences
{
    public interface IUserPreferencesService
    {
        /// <summary>
        /// Saves UserPreferences in local storage
        /// </summary>
        /// <param name="userPreferences">The userPreferences to save in the local storage</param>
        Task SaveUserPreferences(UserPreferences userPreferences);

        /// <summary>
        /// Loads UserPreferences in local storage
        /// </summary>
        /// <returns>UserPreferences object. Null when no settings were found.</returns>
        Task<UserPreferences> LoadUserPreferences();
    }

    public class UserPreferencesService : IUserPreferencesService
    {
        private readonly IBrowserStorage _browserStorage;
        private const string Key = "userPreferences";

        public UserPreferencesService(IBrowserStorage browserStorage)
        {
            _browserStorage = browserStorage;
        }

        public async Task SaveUserPreferences(UserPreferences userPreferences)
        {
            await _browserStorage.SetAsync(Key, userPreferences);
        }

        public async Task<UserPreferences> LoadUserPreferences()
        {
            return await _browserStorage.GetAsync<UserPreferences>(Key);
        }
    }
}
