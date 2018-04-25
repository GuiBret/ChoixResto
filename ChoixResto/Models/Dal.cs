using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace ChoixResto.Models
{
    public class Dal : IDal
    {

        private BDDContext bdd;

        public Dal()
        {
            bdd = new BDDContext();
        }

        public List<Resto> ObtientTousLesRestaurants()
        {
            return bdd.Restos.ToList();
        }

        public void CreerRestaurant(string nom, string telephone)
        {
            bdd.Restos.Add(new Resto { Nom = nom, Telephone = telephone });
            bdd.SaveChanges();
        }

        public void ModifierRestaurant(int id, string nom, string telephone)
        {
            Resto restoTrouve = bdd.Restos.FirstOrDefault(Resto => Resto.Id == id);
            if(restoTrouve != null)
            {
                restoTrouve.Nom = nom;
                restoTrouve.Telephone = telephone;
                bdd.SaveChanges();
            }
        }

        public Boolean RestaurantExiste(string nomRestaurant)
        {
            Resto restoTrouve = bdd.Restos.FirstOrDefault(resto => resto.Nom == nomRestaurant);
            return (restoTrouve != null);
        }

        public void Dispose()
        {
            bdd.Dispose();
        }

        public Utilisateur ObtenirUtilisateur(int id)
        {
            return bdd.Utilisateurs.FirstOrDefault(util => util.Id == id);
        }
        
        public Utilisateur ObtenirUtilisateur(string idStr)
        {
            int id;
            if(Int32.TryParse(idStr, out id))
            {
                return ObtenirUtilisateur(id);
            } else
            {
                return null;
            }
        }

        public int AjouterUtilisateur(string prenom, string mdp)
        {
            bdd.Utilisateurs.Add(new Utilisateur { Prenom = prenom, MotDePasse = EncodeMD5(mdp)});
            bdd.SaveChanges();

            int idNouvelUtilisateur = bdd.Utilisateurs.FirstOrDefault(util => util.Prenom == prenom).Id;

            return idNouvelUtilisateur;
        }

        private string EncodeMD5(string motDePasse)
        {
            string motDePasseSel = "ChoixResto" + motDePasse + "ASP.NET MVC";
            return BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(ASCIIEncoding.Default.GetBytes(motDePasseSel)));
        }

        public Utilisateur Authentifier(string prenom, string mdp)
        {
            Utilisateur utilTrouve = bdd.Utilisateurs.FirstOrDefault(util => util.Prenom == prenom);

            if(utilTrouve != null && EncodeMD5(mdp) == utilTrouve.MotDePasse)
            {
                return utilTrouve;
            }

            return null;
        }

        public Boolean ADejaVote(int idSondage, string idUtilisateur)
        {
            Sondage sondageActuel = bdd.Sondages.FirstOrDefault(sond => sond.Id == idSondage);
            

            if(sondageActuel != null)
            {
                foreach(Vote v in sondageActuel.Votes)
                {
                    
                    if (v.Utilisateur.Id.ToString() == idUtilisateur)
                        return true;
                }
            }
            return false;
        }

        public int CreerUnSondage()
        {
            Sondage s = new Sondage { Date = DateTime.Now, Votes = new List<Vote>() };
            bdd.Sondages.Add(s);

            bdd.SaveChanges();

            return s.Id;
        }

        public void AjouterVote(int idSondage, int choix, int idUtilisateur)
        {
            Utilisateur u = ObtenirUtilisateur(idUtilisateur);
            Resto r = bdd.Restos.FirstOrDefault(resto => resto.Id == choix);

            if(r != null)
            {
                

                Sondage sondageActuel = bdd.Sondages.FirstOrDefault(sond => sond.Id == idSondage);

                if(sondageActuel != null)
                {
                    Vote v = new Vote { Resto = r, Utilisateur = u };
                    sondageActuel.Votes.Add(v);
                }
            }
            


        }

        public List<Resultats> ObtenirLesResultats(int idSondage)
        {
            List<Resultats> res = new List<Resultats>();
            Sondage sondageActuel = bdd.Sondages.FirstOrDefault(sond => sond.Id == idSondage);

            if(sondageActuel != null)
            {
                foreach (Vote v in sondageActuel.Votes)
                {
                    // We check if the result has already been defined
                    Resultats r = res.Find(result => result.Nom == v.Resto.Nom);

                    if(r != null)
                    {
                        r.NombreDeVotes += 1;
                    } else
                    {
                        r = new Resultats { Nom = v.Resto.Nom, Telephone = v.Resto.Telephone, NombreDeVotes = 1 };
                        res.Add(r);
                    }
                    
                }
            }

            return res;
        }
    }
}