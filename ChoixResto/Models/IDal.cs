using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoixResto.Models
{
    public interface IDal : IDisposable
    {
        void CreerRestaurant(string nom, string telephone);
        void ModifierRestaurant(int id, string nom, string telephone);
        int AjouterUtilisateur(string nom, string mdp);
        Boolean RestaurantExiste(string nomRestaurant);

        Boolean ADejaVote(int idSondage, string prenom);

        int CreerUnSondage();
        void AjouterVote(int idSondage, int choix, int idUtilisateur);
        List<Resultats> ObtenirLesResultats(int idSondage);

        Utilisateur ObtenirUtilisateur(int id);
        Utilisateur ObtenirUtilisateur(string idStr);
        Utilisateur Authentifier(string prenom, string mdp);

        List<Resto> ObtientTousLesRestaurants();
    }
}
