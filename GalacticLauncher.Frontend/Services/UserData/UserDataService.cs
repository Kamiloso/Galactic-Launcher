using System;
using System.Collections.Generic;
using System.Linq;
using GalacticLauncher.Frontend.Repositories;

namespace GalacticLauncher.Frontend.Services.UserData
{
    public interface IUserDataService
    {
        void ToggleFavorite(long gameId);
        bool IsFavorite(long gameId);
        void AddToLibrary(long gameId);
        void RemoveFromLibrary(long gameId);
        IEnumerable<long> GetFavoriteIds();
        IEnumerable<long> GetLibraryIds();

        void ClearFavourites();
        void ClearLibrary();


    }
    internal class UserDataService(IDataRepository dataRepository) : IUserDataService
    {
        private const string FAVORITES_KEY = "favorites";
        private const string LIBRARY_KEY = "library";

        public void AddToLibrary(long gameId)
        {
            dataRepository.Add(LIBRARY_KEY, gameId);
        }

        public IEnumerable<long> GetFavoriteIds()
        {
            return dataRepository.GetAll(FAVORITES_KEY);
        }

        public IEnumerable<long> GetLibraryIds()
        {
            return dataRepository.GetAll(LIBRARY_KEY);
        }

        public bool IsFavorite(long gameId)
        {
            IEnumerable<long> favourites = dataRepository.GetAll(FAVORITES_KEY);
            return favourites.Contains(gameId);
        }

        public void ClearFavourites()
        {
            dataRepository.Clear(FAVORITES_KEY);
        }

        public void ClearLibrary()
        {
            dataRepository.Clear(LIBRARY_KEY);
            ClearFavourites();
        }
        public void RemoveFromLibrary(long gameId)
        {
            dataRepository.Remove(LIBRARY_KEY, gameId);
        }

        public void ToggleFavorite(long gameId)
        {
            var favs = dataRepository.GetAll(FAVORITES_KEY).ToList();
            if (favs.Contains(gameId))
                dataRepository.Remove(FAVORITES_KEY, gameId);
            else
                dataRepository.Add(FAVORITES_KEY, gameId);
        }
    }
}
