﻿using habilitations2024.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace habilitations2024.dal
{
    /// <summary>
    /// Classe permettant de gérer les demandes concernant les developpeurs
    /// </summary>
    public class DeveloppeurAccess
    {
        /// <summary>
        /// Instance unique de l'accès aux données
        /// </summary>
        private readonly Access access = null;

        /// <summary>
        /// Constructeur pour créer l'accès aux données
        /// </summary>
        public DeveloppeurAccess()
        {
            access = Access.GetInstance();
        }

        /// <summary>
        /// Controle si l'utillisateur a le droit de se connecter (nom, prénom, pwd et profil "admin")
        /// </summary>
        /// <param name="nom"></param>
        /// <param name="prenom"></param>
        /// <param name="pwd"></param>
        /// <returns>vrai si l'utilisateur a le profil "admin"</returns>
        public Boolean ControleAuthentification(Admin admin)
        {
            if (access.Manager != null)
            {
                string req = "select * from developpeur d join profil p on d.idprofil=p.idprofil ";
                req += "where d.nom=@nom and d.prenom=@prenom and pwd=SHA2(@pwd, 256) and p.nom='admin';";
                Dictionary<string, object> parameters = new Dictionary<string, object> {
                    { "@nom", admin.Nom },
                    { "@prenom", admin.Prenom },
                    { "@pwd", admin.Pwd }
                };
                try
                {
                    List<Object[]> records = access.Manager.ReqSelect(req, parameters);
                    if (records != null)
                    {
                        return (records.Count > 0);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Log.Error("DeveloppeurAccess.ControleAuthentification catch req={0} erreur={1}", req, e.Message);
                    Environment.Exit(0);
                }
            }
            return false;
        }

        /// <summary>
        /// Récupère et retourne les développeurs
        /// </summary>
        /// <returns>liste des développeurs</returns>
        public List<Developpeur> GetLesDeveloppeurs()
        {
            List<Developpeur> lesDeveloppeurs = new List<Developpeur>();
            if (access.Manager != null)
            {
                string req = "select d.iddeveloppeur as iddeveloppeur, d.nom as nom, d.prenom as prenom, d.tel as tel, d.mail as mail, p.idprofil as idprofil, p.nom as profil ";
                req += "from developpeur d join profil p on (d.idprofil = p.idprofil) ";
                req += "order by nom, prenom;";
                try
                {
                    List<Object[]> records = access.Manager.ReqSelect(req);
                    if (records != null)
                    {
                        Log.Debug("DeveloppeurAccess.GetLesDeveloppeurs nb records = {0}", records.Count);
                        foreach (Object[] record in records)
                        {
                            Log.Debug("DeveloppeurAccess.GetLesDeveloppeurs Profil : id={0} nom={1}", record[5], record[6]);
                            Log.Debug("DeveloppeurAccess.GetLesDeveloppeurs Developpeur : id={0} nom={1} prenom={2} tel={3} mail={4} ", record[0], record[1], record[2], record[3], record[4]);
                            Profil profil = new Profil((int)record[5], (string)record[6]);
                            Developpeur developpeur = new Developpeur((int)record[0], (string)record[1], (string)record[2],
                                (string)record[3], (string)record[4], profil);
                            lesDeveloppeurs.Add(developpeur);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Log.Error("DeveloppeurAccess.GetLesDeveloppeurs catch req={0} erreur={1}", req, e.Message);
                    Environment.Exit(0);
                }
            }
            return lesDeveloppeurs;
        }

        /// <summary>
        /// Demande de suppression d'un développeur
        /// </summary>
        /// <param name="developpeur">objet developpeur à supprimer</param>
        public void DelDepveloppeur(Developpeur developpeur)
        {
            if (access.Manager != null)
            {
                string req = "delete from developpeur where iddeveloppeur = @iddeveloppeur;";
                Dictionary<string, object> parameters = new Dictionary<string, object> {
                    {"@iddeveloppeur", developpeur.Iddeveloppeur }
                };
                try
                {
                    access.Manager.ReqUpdate(req, parameters);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Log.Error("DeveloppeurAccess.DelDepveloppeur catch req={0} erreur={1}", req, e.Message);
                    Environment.Exit(0);
                }
            }
        }

        /// <summary>
        /// Demande d'ajout un développeur
        /// </summary>
        /// <param name="developpeur">objet developpeur à ajouter</param>
        public void AddDeveloppeur(Developpeur developpeur)
        {
            if (access.Manager != null)
            {
                string req = "insert into developpeur(nom, prenom, tel, mail, pwd, idprofil) ";
                req += "values (@nom, @prenom, @tel, @mail, SHA2(@pwd, 256), @idprofil);";
                Dictionary<string, object> parameters = new Dictionary<string, object> {
                    { "@nom", developpeur.Nom },
                    { "@prenom", developpeur.Prenom },
                    { "@tel", developpeur.Tel },
                    { "@mail", developpeur.Mail },
                    { "@pwd", developpeur.Nom },
                    { "@idprofil", developpeur.Profil.Idprofil }
                };
                try
                {
                    access.Manager.ReqUpdate(req, parameters);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Log.Error("DeveloppeurAccess.AddDeveloppeur catch req={0} erreur={1}", req, e.Message);
                    Environment.Exit(0);
                }
            }
        }

        /// <summary>
        /// Demande de modification d'un développeur
        /// </summary>
        /// <param name="developpeur">objet developpeur à modifier</param>
        public void UpdateDeveloppeur(Developpeur developpeur)
        {
            if (access.Manager != null)
            {
                string req = "update developpeur set nom = @nom, prenom = @prenom, tel = @tel, mail = @mail, idprofil = @idprofil ";
                req += "where iddeveloppeur = @iddeveloppeur;";
                Dictionary<string, object> parameters = new Dictionary<string, object> {
                    { "@idDeveloppeur", developpeur.Iddeveloppeur },
                    { "@nom", developpeur.Nom },
                    { "@prenom", developpeur.Prenom },
                    { "@tel", developpeur.Tel },
                    { "@mail", developpeur.Mail },
                    { "idprofil", developpeur.Profil.Idprofil }
                };
                try
                {
                    access.Manager.ReqUpdate(req, parameters);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Log.Error("DeveloppeurAccess.UpdateDeveloppeur catch req={0} erreur={1}", req, e.Message);
                    Environment.Exit(0);
                }
            }
        }

        /// <summary>
        /// Demande de modification du pwd
        /// </summary>
        /// <param name="developpeur">objet developpeur avec nouveau pwd</param>
        public void UpdatePwd(Developpeur developpeur)
        {
            if (access.Manager != null)
            {
                string req = "update developpeur set pwd = SHA2(@pwd, 256) ";
                req += "where iddeveloppeur = @iddeveloppeur;";
                Dictionary<string, object> parameters = new Dictionary<string, object> {
                    { "@idDeveloppeur", developpeur.Iddeveloppeur },
                    { "@pwd", developpeur.Pwd }
                };
                try
                {
                    access.Manager.ReqUpdate(req, parameters);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Log.Error("DeveloppeurAccess.UpdatePwd catch req={0} erreur={1}", req, e.Message);
                    Environment.Exit(0);
                }
            }
        }

    }
}
