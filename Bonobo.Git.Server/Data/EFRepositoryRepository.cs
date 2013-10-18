using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bonobo.Git.Server.Models;
using System.Data;
using Bonobo.Git.Server.Data;

namespace Bonobo.Git.Server.Data
{
    public class EFRepositoryRepository : IRepositoryRepository
    {
        public IList<Models.RepositoryModel> GetAllRepositories()
        {
            using (var db = new BonoboGitServerContext())
            {
                var result = new List<RepositoryModel>();
                foreach (var item in db.Repositories)
                {
                    result.Add(ConvertToModel(item));
                }
                return result;
            }
        }

        public IList<RepositoryModel> GetPermittedRepositories(string username, string[] teams)
        {
            if (username == null) throw new ArgumentException("username");

            username = username.ToLowerInvariant();
            return GetAllRepositories().Where(i => i.Administrators.Contains(username)
                || i.Users.Contains(username)
                || i.Teams.FirstOrDefault(teams.Contains) != null
                || i.AnonymousAccess).ToList();
        }

        public IList<RepositoryModel> GetAdministratedRepositories(string username)
        {
            if (username == null) throw new ArgumentException("username");
            
            username = username.ToLowerInvariant();
            return GetAllRepositories().Where(i => i.Administrators.Contains(username)).ToList();
        }

        public RepositoryModel GetRepository(string name)
        {
            if (name == null) throw new ArgumentException("name");

            using (var db = new BonoboGitServerContext())
            {
                return ConvertToModel(db.Repositories.FirstOrDefault(i => i.Name == name));
            }
        }

        public void Delete(string name)
        {
            if (name == null) throw new ArgumentException("name");

            using (var db = new BonoboGitServerContext())
            {
                var repo = db.Repositories.FirstOrDefault(i => i.Name == name);
                if (repo != null)
                {
                    repo.Administrators.Clear();
                    repo.Teams.Clear();
                    db.Repositories.Remove(repo);
                    db.SaveChanges();
                }
            }
        }

        public bool Create(RepositoryModel model)
        {
            if (model == null) throw new ArgumentException("model");
            if (model.Name == null) throw new ArgumentException("name");

            using (var database = new BonoboGitServerContext())
            {
                var repository = new Repository
                {
                    Name = model.Name,
                    Description = model.Description,
                    Anonymous = model.AnonymousAccess,
                };
                database.Repositories.Add(repository);
                try
                {
                    database.SaveChanges();
                }
                catch (UpdateException)
                {
                    return false;
                }
            }

            AddUsersToRepositories(model.Name, model.Users);

            return true;
        }

        public void Update(RepositoryModel model)
        {
            if (model == null) throw new ArgumentException("model");
            if (model.Name == null) throw new ArgumentException("name");

            using (var db = new BonoboGitServerContext())
            {
                var repo = db.Repositories.FirstOrDefault(i => i.Name == model.Name);
                if (repo != null)
                {
                    repo.Description = model.Description;
                    repo.Anonymous = model.AnonymousAccess;

                    repo.Teams.Clear();
                    repo.Administrators.Clear();

                    AddMembers(model.Administrators, model.Teams, repo, db);

                    db.SaveChanges();
                }
            }

            AddUsersToRepositories(model.Name, model.Users);
        }

        private RepositoryModel ConvertToModel(Repository item)
        {
            if (item != null)
            {
                var rm = new RepositoryModel
                        {
                            Name = item.Name,
                            Description = item.Description,
                            AnonymousAccess = item.Anonymous,
                            Teams = item.Teams.Select(i => i.Name).ToArray(),
                            Administrators = item.Administrators.Select(i => i.Username).ToArray(),
                            Users = GetUserRepositoriesForRepository(item.Name).Select(ur => ur.User_Username).ToArray(),
                        };
                rm.Users = GetUserRepositoriesForRepository(item.Name).Select(u => u.User_Username).ToArray();

                return rm;
            }

            return null;
        }

        private void AddMembers(string[] admins, string[] teams, Repository repo, BonoboGitServerContext database)
        {
            if (admins != null)
            {
                var administrators = database.Users.Where(i => admins.Contains(i.Username));
                foreach (var item in administrators)
                {
                    repo.Administrators.Add(item);
                }
            }

            if (teams != null)
            {
                var permittedTeams = database.Teams.Where(i => teams.Contains(i.Name));
                foreach (var item in permittedTeams)
                {
                    repo.Teams.Add(item);
                }
            }
        }

        public void ClearAllUserRepositoryPermissionsForRepository(string repository)
        {
            using (var db = new BonoboGitServerContext())
            {
                var users = db.UserRepositoryPermissions.Where(u => u.Repository_Name == repository).ToList();
                foreach (var userRepositoryPermission in users)
                {
                    db.UserRepositoryPermissions.Remove(userRepositoryPermission);
                }
                db.SaveChanges();
            }
        }

        public IList<UserRepositoryPermission> GetUserRepositoriesForUser(string username)
        {
            using (var db = new BonoboGitServerContext())
            {
                return db.UserRepositoryPermissions.Where(u => u.User_Username == username).ToList();
            }
        }

        public IList<UserRepositoryPermission> GetUserRepositoriesForRepository(string repository)
        {
            using (var db = new BonoboGitServerContext())
            {
                return db.UserRepositoryPermissions.Where(u => u.Repository_Name == repository).ToList();
            }
        } 

        public void AddUsersToRepositories(string repository, string[] users)
        {
            ClearAllUserRepositoryPermissionsForRepository(repository);
            using (var db = new BonoboGitServerContext())
            {
                foreach (var user in users)
                {
                    var urp = new UserRepositoryPermission();
                    urp.User_Username = user;
                    urp.Repository_Name = repository;
                    db.UserRepositoryPermissions.Add(urp);
                }
                db.SaveChanges();
            }
        }
    }
}