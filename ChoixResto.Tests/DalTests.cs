using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChoixResto.Models;
using System.Data.Entity;
using System.Linq;

namespace ChoixResto.Tests
{
    /// <summary>
    /// Description résumée pour DalTests
    /// </summary>
    [TestClass]
    public class DalTests
    {
        private IDal dal;

        [TestInitialize]
        public void Init_AvantChaqueTest()
        {
            IDatabaseInitializer<BDDContext> init = new DropCreateDatabaseAlways<BDDContext>();
            Database.SetInitializer(init);
            init.InitializeDatabase(new BDDContext());

            dal = new Dal();
        }

        [TestCleanup]
        public void ApresChaqueTest()
        {
            dal.Dispose();
        }

        [TestMethod]
        public void CreerRestaurant_AvecUnNouveauRestaurant_ObtientTousLesRestaurantsRenvoitBienLeRestaurant()
        {
            dal.CreerRestaurant("La bonne fourchette", "0102030405");
            List<Resto> restos = dal.ObtientTousLesRestaurants();

            Assert.IsNotNull(restos);
            Assert.AreEqual(1, restos.Count);
            Assert.AreEqual("La bonne fourchette", restos[0].Nom);
            Assert.AreEqual("0102030405", restos[0].Telephone);
        }

        [TestMethod]
        public void ModifierRestaurant_CreationDUnNouveauRestaurantEtChangementNomEtTelephone_LaModificationEstCorrecteApresRechargement()
        {
            dal.CreerRestaurant("La bonne fourchette", "0102030405");
            dal.ModifierRestaurant(1, "La bonne cuillère", null);

            List<Resto> restos = dal.ObtientTousLesRestaurants();
            Assert.IsNotNull(restos);
            Assert.AreEqual(1, restos.Count);
            Assert.AreEqual("La bonne cuillère", restos[0].Nom);
            Assert.IsNull(restos[0].Telephone);
        }

        [TestMethod]
        public void RestaurantExiste_AvecCreationDunRestauraunt_RenvoiQuilExiste()
        {
            dal.CreerRestaurant("La bonne fourchette", "0102030405");

            bool existe = dal.RestaurantExiste("La bonne fourchette");

            Assert.IsTrue(existe);
        }

        [TestMethod]
        public void RestaurantExiste_AvecRestaurauntInexistant_RenvoiQuilExiste()
        {
            bool existe = dal.RestaurantExiste("La bonne fourchette");

            Assert.IsFalse(existe);
        }

        [TestMethod]
        public void ObtenirUtilisateur_UtilisateurInexistant_RetourneNull()
        {
            Utilisateur utilisateur = dal.ObtenirUtilisateur(1);
            Assert.IsNull(utilisateur);
        }

        [TestMethod]
        public void ObtenirUtilisateur_IdNonNumerique_RetourneNull()
        {
            Utilisateur utilisateur = dal.ObtenirUtilisateur("abc");
            Assert.IsNull(utilisateur);
        }

        [TestMethod]
        public void AjouterUtilisateur_NouvelUtilisateurEtRecuperation_LutilisateurEstBienRecupere()
        {
            dal.AjouterUtilisateur("Nouvel utilisateur", "12345");

            Utilisateur utilisateur = dal.ObtenirUtilisateur(1);

            Assert.IsNotNull(utilisateur);
            Assert.AreEqual("Nouvel utilisateur", utilisateur.Prenom);

            utilisateur = dal.ObtenirUtilisateur("1");

            Assert.IsNotNull(utilisateur);
            Assert.AreEqual("Nouvel utilisateur", utilisateur.Prenom);
        }

        [TestMethod]
        public void Authentifier_LoginMdpOk_AuthentificationOK()
        {
            dal.AjouterUtilisateur("Nouvel utilisateur", "12345");

            Utilisateur utilisateur = dal.Authentifier("Nouvel utilisateur", "12345");

            Assert.IsNotNull(utilisateur);
            Assert.AreEqual("Nouvel utilisateur", utilisateur.Prenom);
        }

        [TestMethod]
        public void Authentifier_LoginOkMdpKo_AuthentificationKO()
        {
            dal.AjouterUtilisateur("Nouvel utilisateur", "12345");
            Utilisateur utilisateur = dal.Authentifier("Nouvel utilisateur", "0");

            Assert.IsNull(utilisateur);
        }

        [TestMethod]
        public void Authentifier_LoginKoMdpOk_AuthentificationKO()
        {
            dal.AjouterUtilisateur("Nouvel utilisateur", "12345");
            Utilisateur utilisateur = dal.Authentifier("Nouvel", "12345");

            Assert.IsNull(utilisateur);
        }

        [TestMethod]
        public void Authentifier_LoginMdpKo_AuthentificationKO()
        {
            Utilisateur utilisateur = dal.Authentifier("Nouvel utilisateur", "12345");

            Assert.IsNull(utilisateur);
        }

        [TestMethod]
        public void ADejaVote_AvecIdNonNumerique_RetourneFalse()
        {
            bool pasVote = dal.ADejaVote(1, "abc");

            Assert.IsFalse(pasVote);
        }

        [TestMethod]
        public void ADejaVote_UtilisateurNAPasVote_RetourneFalse()
        {
            int idSondage = dal.CreerUnSondage();
            int idUtilisateur = dal.AjouterUtilisateur("Nouvel utilisateur", "12345");

            bool pasVote = dal.ADejaVote(idSondage, idUtilisateur.ToString());

            Assert.IsFalse(pasVote);
        }

        [TestMethod]
        public void ADejaVote_UtilisateurAVote_RetourneTrue()
        {
            int idSondage = dal.CreerUnSondage();
            int idUtilisateur = dal.AjouterUtilisateur("Nouvel utilisateur", "12345");
            dal.CreerRestaurant("La bonne fourchette", "0102030405");
            dal.AjouterVote(idSondage, 1, idUtilisateur);

            bool aVote = dal.ADejaVote(idSondage, idUtilisateur.ToString());

            Assert.IsTrue(aVote);
        }

        [TestMethod]
        public void ObtenirLesResultats_AvecQuelquesChoix_RetourneBienLesResultats()
        {
            int idSondage = dal.CreerUnSondage();
            int idUtilisateur1 = dal.AjouterUtilisateur("Utilisateur1", "12345");
            int idUtilisateur2 = dal.AjouterUtilisateur("Utilisateur2", "12345");
            int idUtilisateur3 = dal.AjouterUtilisateur("Utilisateur3", "12345");

            dal.CreerRestaurant("Resto pinière", "0102030405");
            dal.CreerRestaurant("Resto pinambour", "0102030405");
            dal.CreerRestaurant("Resto mate", "0102030405");
            dal.CreerRestaurant("Resto ride", "0102030405");

            dal.AjouterVote(idSondage, 1, idUtilisateur1);
            dal.AjouterVote(idSondage, 3, idUtilisateur1);
            dal.AjouterVote(idSondage, 4, idUtilisateur1);
            dal.AjouterVote(idSondage, 1, idUtilisateur2);
            dal.AjouterVote(idSondage, 1, idUtilisateur3);
            dal.AjouterVote(idSondage, 3, idUtilisateur3);

            List<Resultats> resultats = dal.ObtenirLesResultats(idSondage);

            Assert.AreEqual(3, resultats[0].NombreDeVotes);
            Assert.AreEqual("Resto pinière", resultats[0].Nom);
            Assert.AreEqual("0102030405", resultats[0].Telephone);
            Assert.AreEqual(2, resultats[1].NombreDeVotes);
            Assert.AreEqual("Resto mate", resultats[1].Nom);
            Assert.AreEqual("0102030405", resultats[1].Telephone);
            Assert.AreEqual(1, resultats[2].NombreDeVotes);
            Assert.AreEqual("Resto ride", resultats[2].Nom);
            Assert.AreEqual("0102030405", resultats[2].Telephone);
        }

        [TestMethod]
        public void ObtenirLesResultats_AvecDeuxSondages_RetourneBienLesBonsResultats()
        {
            int idSondage1 = dal.CreerUnSondage();
            int idUtilisateur1 = dal.AjouterUtilisateur("Utilisateur1", "12345");
            int idUtilisateur2 = dal.AjouterUtilisateur("Utilisateur2", "12345");
            int idUtilisateur3 = dal.AjouterUtilisateur("Utilisateur3", "12345");
            dal.CreerRestaurant("Resto pinière", "0102030405");
            dal.CreerRestaurant("Resto pinambour", "0102030405");
            dal.CreerRestaurant("Resto mate", "0102030405");
            dal.CreerRestaurant("Resto ride", "0102030405");
            dal.AjouterVote(idSondage1, 1, idUtilisateur1);
            dal.AjouterVote(idSondage1, 3, idUtilisateur1);
            dal.AjouterVote(idSondage1, 4, idUtilisateur1);
            dal.AjouterVote(idSondage1, 1, idUtilisateur2);
            dal.AjouterVote(idSondage1, 1, idUtilisateur3);
            dal.AjouterVote(idSondage1, 3, idUtilisateur3);

            int idSondage2 = dal.CreerUnSondage();
            dal.AjouterVote(idSondage2, 2, idUtilisateur1);
            dal.AjouterVote(idSondage2, 3, idUtilisateur1);
            dal.AjouterVote(idSondage2, 1, idUtilisateur2);
            dal.AjouterVote(idSondage2, 4, idUtilisateur3);
            dal.AjouterVote(idSondage2, 3, idUtilisateur3);

            List<Resultats> resultats1 = dal.ObtenirLesResultats(idSondage1);
            List<Resultats> resultats2 = dal.ObtenirLesResultats(idSondage2);

            Assert.AreEqual(3, resultats1[0].NombreDeVotes);
            Assert.AreEqual("Resto pinière", resultats1[0].Nom);
            Assert.AreEqual("0102030405", resultats1[0].Telephone);
            Assert.AreEqual(2, resultats1[1].NombreDeVotes);
            Assert.AreEqual("Resto mate", resultats1[1].Nom);
            Assert.AreEqual("0102030405", resultats1[1].Telephone);
            Assert.AreEqual(1, resultats1[2].NombreDeVotes);
            Assert.AreEqual("Resto ride", resultats1[2].Nom);
            Assert.AreEqual("0102030405", resultats1[2].Telephone);

            Assert.AreEqual(1, resultats2[0].NombreDeVotes);
            Assert.AreEqual("Resto pinambour", resultats2[0].Nom);
            Assert.AreEqual("0102030405", resultats2[0].Telephone);
            Assert.AreEqual(2, resultats2[1].NombreDeVotes);
            Assert.AreEqual("Resto mate", resultats2[1].Nom);
            Assert.AreEqual("0102030405", resultats2[1].Telephone);
            Assert.AreEqual(1, resultats2[2].NombreDeVotes);
            Assert.AreEqual("Resto pinière", resultats2[2].Nom);
            Assert.AreEqual("0102030405", resultats2[2].Telephone);
        }
    }
}
